using System;
using System.Collections.Generic;
using System.Linq;
using Day_13;
using Day_15.Properties;

namespace Day_15
{
    class Program
    {
        static void Main(string[] args)
        {
            IntcodeComputer droid = new IntcodeComputer();
            long[] code = Resources.Input.Split(',').Select(x => long.Parse(x)).ToArray();
            int x = 0;
            int y = 0;

            int dirx = 0;
            int diry = 1;

            LinkedList<(int x, int y)> visited = new LinkedList<(int x, int y)>();
            visited.AddLast((0, 0));
            int distance = 0;
            bool distanceFound = false;
            int noNewEncountered = 0;

            int ox = 0;
            int oy = 0;
            droid.RunCode(code, () => //1 up, 2 down, 3 left, 4 right
            {
                if (dirx == 0)
                    return diry == 1 ? 1 : 2;
                return dirx == 1 ? 4 : 3;
            }, o => //0 wall, 1 moved, 2 found
            {
                switch(o)
                {
                    case 0:
                        int h = dirx;
                        dirx = -diry;
                        diry = h;
                        break;
                    case 1:
                    case 2:
                        if(noNewEncountered == 10000)
                        {
                            for (int i = 0; i < code.Length; i++)
                                code[i] = 99;
                            break;
                        }

                        x += dirx;
                        y += diry;

                        if(visited.Contains((x, y)))
                        {
                            distance--;
                            noNewEncountered++;
                        }
                        else
                        {
                            visited.AddLast((x, y));
                            distance++;
                            noNewEncountered = 0;
                        }
                        if(!distanceFound && o == 2)
                        {
                            distanceFound = true;
                            Console.WriteLine($"Distance is {distance}.");
                            ox = x;
                            oy = y;
                        }
                        h = dirx;
                        dirx = diry;
                        diry = -h;
                        break;
                }
            });

            LinkedList<(int x, int y)> newOxygen = new LinkedList<(int x, int y)>();
            visited.Remove((ox, oy));
            newOxygen.AddLast((ox, oy));

            for(int minutes = 1; true; minutes++)
            {
                var current = newOxygen.ToArray();
                newOxygen.Clear();
                foreach(var c in current)
                {
                    var n = visited.Where(p => (p.x == c.x && (p.y == c.y - 1 || p.y == c.y + 1)) || (p.y == c.y && (p.x == c.x - 1 || p.x == c.x + 1))).ToArray();
                    foreach(var p in n)
                    {
                        visited.Remove(p);
                        newOxygen.AddLast(p);
                    }
                }

                if(visited.Count == 0)
                {
                    Console.WriteLine($"It took {minutes} minutes to fill the place with oxygen.");
                    break;
                }
            }
        }
    }
}
