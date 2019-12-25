using System;
using System.Collections.Generic;
using System.Linq;
using Day_14.Properties;
namespace Day_14
{
    class Program
    {
        public static Dictionary<string, (long quantity, (string chemical, long quantity)[] inputs)> recipes = new Dictionary<string, (long quantity, (string chemical, long quantity)[] inputs)>();
        public static Dictionary<string, long> leftovers = new Dictionary<string, long>();
        static void Main(string[] args)
        {
            string[] input = Resources.Input.Split("\r\n");
            foreach(var line in input)
            {
                var sides = line.Split("=>");
                var output = sides[1].Trim().Split(" ");
                List<(string, long)> inputs = new List<(string, long)>();
                foreach(var o in sides[0].Split(","))
                {
                    var i = o.Trim().Split(" ");
                    inputs.Add((i[1], long.Parse(i[0])));
                }
                recipes.Add(output[1], (long.Parse(output[0]), inputs.ToArray()));
            }
            recipes.Add("ORE", (1, new (string, long)[] { ("ORE", 1) }));
            Console.WriteLine($"1 FUEL requires roughly {CountOre("FUEL", 1)} ORE");

            long starting = 0;
            for (long i = 1000000000000 / 2; true; i /= 2)
            {
                starting += DoLoop(starting, i) * i;
                if (i == 1)
                    break;
            }
            Console.WriteLine($"Maximum of {starting} FUEL can be made.");

        }

        public static long DoLoop(long starting, long delta)
        {
            for (long i = 0; true; i++)
            {
                if (CountOre("FUEL", starting + i * delta) > 1000000000000)
                    return i - 1;
            }
        }

        public static long CountOre(string chemical, long count)
        {
            if (chemical == "ORE")
                return count;

            var recipe = recipes[chemical].inputs;
            long oreNeeded = 0;
            
            foreach(var (material, matCount) in recipe)
            {
                long leftover = 0;
                if(leftovers.ContainsKey(material))
                {
                    leftover = leftovers[material];
                    leftovers[material] = 0;
                }

                long need = matCount * count - leftover;
                if(need <= 0)
                {
                    leftovers[material] = leftover - matCount * count;
                    continue;
                }

                long recCount = recipes[material].quantity;
                long k = (long)Math.Ceiling((float)need / recCount);

                if (leftovers.ContainsKey(material))
                    leftovers[material] = k * recCount - need;
                else
                    leftovers.Add(material, k * recCount - need);

                oreNeeded += CountOre(material, k);
            }
            return oreNeeded;

        }
    }
}
