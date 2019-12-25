using System;
using Day_5;
using Day_9.Properties;
using System.Linq;
namespace Day_9
{
    class Program
    {
        static void Main(string[] args)
        {
            var pc = new IntcodeComputer();
            pc.RunCode(Resources.Input.Split(',').Select(x => long.Parse(x)).ToArray(), new long[] { 2 });
        }
    }
}
