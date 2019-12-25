using System;
using System.Collections.Generic;
using System.Linq;
using Day_13;
using Day_25.Properties;

namespace Day_25
{
    class Program
    {
        static void Main(string[] args)
        {
            long[] code = Resources.code.Split(',').Select(x => long.Parse(x)).ToArray();

            IntcodeComputer drone = new IntcodeComputer();

            Queue<char> inputQ = new Queue<char>();
            while (true)
            {
                Console.WriteLine("Starting drone...");
                drone.RunCode((long[])code.Clone(), () =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    if(inputQ.Count == 0)
                    {
                        string command = Console.ReadLine() + "\n";
                        foreach (char c in command)
                            inputQ.Enqueue(c);
                    }
                    return inputQ.Dequeue();
                },
                o =>
                {
                    if (o == 10)
                        Console.ForegroundColor = ConsoleColor.White;
                    if ((char)o == '=')
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    if ((char)o == '-')
                        Console.ForegroundColor = ConsoleColor.Green;

                    Console.Write((char)o);
                });
            }
        }
    }
}
