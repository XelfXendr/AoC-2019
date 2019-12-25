using System;
using Day_7.Properties;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
namespace Day_7
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] code = Resources.Input.Split(',').Select(x => int.Parse(x)).ToArray();

            int[] toPermutate = new int[] { 5, 6, 7, 8, 9 };
            int[,] permutations = new int[120, 5];

            int[] c = new int[5];
            for (int o = 0; o < 5; o++)
                permutations[0, o] = toPermutate[o];
            int permIndex = 1;

            for (int i = 0; i < 5;)
            {
                if (c[i] < i)
                {
                    if ((i & 1) == 0)
                    {
                        int h = toPermutate[0];
                        toPermutate[0] = toPermutate[i];
                        toPermutate[i] = h;
                    }
                    else
                    {
                        int h = toPermutate[c[i]];
                        toPermutate[c[i]] = toPermutate[i];
                        toPermutate[i] = h;
                    }
                    for(int o = 0; o < 5; o++)
                        permutations[permIndex, o] = toPermutate[o];
                    permIndex++;
                    c[i]++;
                    i = 0;
                }
                else
                {
                    c[i] = 0;
                    i++;
                }
            }

            int[] signals = new int[120];

            for(int i = 0; i < 120; i++)
            {
                LinkedList<int> inA = new LinkedList<int>(new int[] { permutations[i, 0], 0 });
                LinkedList<int> inB = new LinkedList<int>(new int[] { permutations[i, 1] });
                LinkedList<int> inC = new LinkedList<int>(new int[] { permutations[i, 2] });
                LinkedList<int> inD = new LinkedList<int>(new int[] { permutations[i, 3] });
                LinkedList<int> inE = new LinkedList<int>(new int[] { permutations[i, 4] });

                Amplifier A = new Amplifier(inA, inB);
                Amplifier B = new Amplifier(inB, inC);
                Amplifier C = new Amplifier(inC, inD);
                Amplifier D = new Amplifier(inD, inE);
                Amplifier E = new Amplifier(inE, inA);

                Thread tA = new Thread(new ThreadStart(() => A.RunCode(code)));
                Thread tB = new Thread(new ThreadStart(() => B.RunCode(code)));
                Thread tC = new Thread(new ThreadStart(() => C.RunCode(code)));
                Thread tD = new Thread(new ThreadStart(() => D.RunCode(code)));
                Thread tE = new Thread(new ThreadStart(() => signals[i] = E.RunCode(code)));

                Console.WriteLine($"Starting threads {i}.");

                tA.Start();
                tB.Start();
                tC.Start();
                tD.Start();
                tE.Start();

                while (tA.IsAlive || tB.IsAlive || tC.IsAlive || tD.IsAlive || tE.IsAlive)
                    Thread.Sleep(1);
            }
            Console.WriteLine($"Maximum is {signals.Max()}");
        }
    }

    public class Amplifier
    {
        public LinkedList<int> inputList;
        public LinkedList<int> outputList;

        public Amplifier(LinkedList<int> inputList, LinkedList<int> outputList)
        {
            this.inputList = inputList;
            this.outputList = outputList;
        }

        public int RunCode(int[] code)
        {
            int[] memory = (int[])code.Clone();
            bool halt = false;
            int output = 0;
            for (int i = 0; i < memory.Length;)
            {
                string upcode = memory[i].ToString("D5");
                switch (upcode.Substring(3, 2))
                {
                    case "01":
                        memory[memory[i + 3]] = (upcode[2] == '0' ? memory[memory[i + 1]] : memory[i + 1]) + (upcode[1] == '0' ? memory[memory[i + 2]] : memory[i + 2]);
                        i += 4;
                        break;
                    case "02":
                        memory[memory[i + 3]] = (upcode[2] == '0' ? memory[memory[i + 1]] : memory[i + 1]) * (upcode[1] == '0' ? memory[memory[i + 2]] : memory[i + 2]);
                        i += 4;
                        break;

                    case "03":
                        while (inputList.Count <= 0) 
                            Thread.Sleep(1);
                        memory[memory[i + 1]] = inputList.First.Value;
                        inputList.RemoveFirst();
                        i += 2;
                        break;
                    case "04":
                        output = upcode[2] == '0' ? memory[memory[i + 1]] : memory[i + 1];
                        outputList.AddLast(output);
                        i += 2;
                        break;
                    case "05":
                        if (upcode[2] == '0' ? memory[memory[i + 1]] != 0 : memory[i + 1] != 0)
                            i = upcode[1] == '0' ? memory[memory[i + 2]] : memory[i + 2];
                        else
                            i += 3;
                        break;
                    case "06":
                        if (upcode[2] == '0' ? memory[memory[i + 1]] == 0 : memory[i + 1] == 0)
                            i = upcode[1] == '0' ? memory[memory[i + 2]] : memory[i + 2];
                        else
                            i += 3;
                        break;
                    case "07":
                        memory[memory[i + 3]] = ((upcode[2] == '0' ? memory[memory[i + 1]] : memory[i + 1]) < (upcode[1] == '0' ? memory[memory[i + 2]] : memory[i + 2])) ? 1 : 0;
                        i += 4;
                        break;
                    case "08":
                        memory[memory[i + 3]] = ((upcode[2] == '0' ? memory[memory[i + 1]] : memory[i + 1]) == (upcode[1] == '0' ? memory[memory[i + 2]] : memory[i + 2])) ? 1 : 0;
                        i += 4;
                        break;

                    case "99":
                        halt = true;
                        break;
                    default:
                        Console.WriteLine($"Halt on upcode {upcode.Substring(3, 2)}, position {i}.");
                        halt = true;
                        break;
                }
                if (halt)
                    break;
            }
            return output;
        }
    }
}
