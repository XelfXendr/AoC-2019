using System;
using Day_19.Properties;
using Day_13;
using System.Collections.Generic;
using System.Linq;
namespace Day_19
{
    class Program
    {

        static IntcodeComputer drone = new IntcodeComputer();
        static long[] code = Resources.Input.Split(',').Select(x => long.Parse(x)).ToArray();
        static void Main(string[] args)
        {

            int max = 50;
            int count = 0;

            for (int i = 0; i < max; i++)
                for (int j = 0; j < max; j++)
                {
                    if (getBeam(i, j))
                    {
                        count++;
                        Console.SetCursorPosition(i * 2, j);
                        Console.WriteLine("[]");
                    }
                }
            Console.WriteLine(count);

            int x = 0;
            int y = 0;

            for (x = 700; x < 1000; x++)
            {
                for (y = 700; y < 1000; y++)
                    if (getBeam(x, y) && getBeam(x + 99, y) && getBeam(x, y + 99) && getBeam(x + 99, y + 99))
                    {
                        Console.WriteLine($"{x * 10000 + y}");
                        return;
                    }
            }
        }

        static bool getBeam(int x, int y)
        {
            bool help = false;
            drone.RunCode((long[])code.Clone(), () =>
            {
                help = !help;
                if (help)
                    return x;
                else
                    return y;
            },
            o =>
            {
                help = o == 1;
            });
            return help;
        }
    }
}
