using System;
using System.Collections.Generic;
using Day_13;
using Day_21.Properties;
using System.Linq;
namespace Day_21
{
    class Program
    {
        static void Main(string[] args)
        {
            long[] code = Resources.Input.Split(",").Select(x => long.Parse(x)).ToArray();
            IntcodeComputer drone = new IntcodeComputer();

            //PART1
            string commands = "NOT T T\nAND A T\nAND B T\nAND C T\nNOT T T\nOR T J\nAND D J\nWALK\n";
            int commandIndex = 0;
            drone.RunCode((long[])code.Clone(), () =>
            {
                long i = commands[commandIndex];
                commandIndex++;
                return i;
            }, o =>
            {
                if (o < 126)
                    Console.Write((char)o);
                else
                    Console.WriteLine($"Hull damage is {o}");
            });
            //PART2
            commands = "NOT T T\nAND A T\nAND B T\nAND C T\nNOT T T\nOR E J\nOR H J\nAND D J\nAND T J\nRUN\n";
            commandIndex = 0;
            drone.RunCode((long[])code.Clone(), () =>
            {
                long i = commands[commandIndex];
                commandIndex++;
                return i;
            }, o =>
            {
                if (o < 126)
                    Console.Write((char)o);
                else
                    Console.WriteLine($"Hull damage is {o}");
            });
        }
    }
}
