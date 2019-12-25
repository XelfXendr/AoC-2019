using Day_1.Properties;
using System;
namespace Day_1
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] input = Resources.Input.Split("\n");
            int fuel = 0;
            int extraFuel = 0;
            foreach(string l in input)
            {
                extraFuel = (int)Math.Floor(int.Parse(l) / 3f) - 2;
                while (extraFuel > 0)
                {
                    fuel += extraFuel;
                    extraFuel = (int)Math.Floor(extraFuel / 3f) - 2;
                }
            }
            Console.WriteLine(fuel);
            Console.ReadLine();
        }
    }
}
