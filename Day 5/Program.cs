using System;
using Day_5.Properties;
using System.Linq;

namespace Day_5
{
    class Program
    {
        static void Main(string[] args)
        {
            long[] code = Resources.Input.Split(",").Select(x => long.Parse(x)).ToArray();
            var pc = new IntcodeComputer();
            pc.RunCode(code, new long[] { 5 });
            Console.ReadKey();
        }
    }
}
