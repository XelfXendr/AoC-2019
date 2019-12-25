using System;
using Day_13;
using Day_23.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace Day_23
{
    class Program
    {
        public static NIC[] nics = new NIC[50];
        public static long[] code = Resources.Input.Split(',').Select(x => long.Parse(x)).ToArray();
        public static (long x, long y) natIn;

        static void Main()
        {
            long lastY = 0;
            for (int i = 0; i < 50; i++)
                nics[i] = new NIC(i);
            for (int i = 0; i < 50; i++)
            {
                nics[i].Run();
            }
            new Thread(new ThreadStart(() =>
            {
                while(true)
                {
                    if(nics.All(n => n.idleFor >= 100))
                    {
                        if (natIn.y == lastY)
                        {
                            Console.WriteLine(lastY);
                            Console.ReadKey();
                            return;
                        }
                        Console.WriteLine($"{natIn.x} {natIn.y}");
                        lastY = natIn.y;
                        nics[0].input.AddLast(natIn);
                        nics[0].idleFor = 0;
                        Thread.Sleep(500);
                        continue;
                    }
                    Thread.Sleep(200);
                }
            })).Start();
        }
    }

    class NIC
    {
        public int address;
        private bool booted = false;

        public LinkedList<(long x, long y)> input = new LinkedList<(long x, long y)>();
        private int inputIndex = 0;

        private long[] output = new long[3];
        private int outputIndex = 0;

        public IntcodeComputer intcode = new IntcodeComputer();

        public long idleFor = 0;

        public NIC(int add)
        {
            address = add;
        }

        public long OnInput()
        {
            if (!booted)
            {
                booted = true;
                return address;
            }
            if (input.Count <= 0 || input.First is null)
            {
                Thread.Sleep((int)idleFor/5);
                idleFor++;
                return -1;
            }
            idleFor = 0;
            if (inputIndex == 0)
            {
                inputIndex++;
                return input.First.Value.x;
            }
            long value = input.First.Value.y;
            input.RemoveFirst();
            inputIndex = 0;
            return value;
        }

        public void OnOutput(long o)
        {
            if (outputIndex < 2)
            {
                output[outputIndex] = o;
                outputIndex++;
                return;
            }
            output[outputIndex] = o;
            outputIndex = 0;

            if (output[0] == 255)
            {
                Console.WriteLine($"{address} => {output[0]}: {output[1]} {output[2]}");
                Program.natIn = (output[1], output[2]);
                return;
            }

            Program.nics[output[0]].input.AddLast((output[1], output[2]));
            Program.nics[output[0]].idleFor = 0;
            Thread.Sleep(10);
        }

        public Thread Run()
        {
            Thread thread = new Thread(new ThreadStart(() => intcode.RunCode((long[])Program.code.Clone(), OnInput, OnOutput)));
            thread.Start();
            return thread;
        }
    }
}
