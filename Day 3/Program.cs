using System;
using System.Collections.Generic;
using Day_3.Properties;
using System.Linq;

namespace Day_3
{
    class Program
    {
        static void Main(string[] args)
        {
            Vector[] wireA = Wiring(Resources.WireA);
            Vector[] wireB = Wiring(Resources.WireB);
            List<int> distances = new List<int>();
            List<Vector> intersections = new List<Vector>();

            //1*
            for (int a = 1; a < wireA.Length; a++)
            {
                for (int b = 1; b < wireB.Length; b++)
                {
                    if ((wireA[a - 1].x == wireA[a].x) == (wireB[b - 1].x == wireB[b].x))
                        continue;
                    if (wireA[a - 1].x == wireA[a].x)
                    {
                        if ((Math.Sign(wireA[a].x - wireB[b - 1].x) != Math.Sign(wireA[a].x - wireB[b].x)) && (Math.Sign(wireB[b].y - wireA[a - 1].y) != Math.Sign(wireB[b].y - wireA[a].y)))
                        {
                            distances.Add(Math.Abs(wireB[b].y) + Math.Abs(wireA[a].x));
                            intersections.Add(new Vector(wireA[a].x, wireB[b].y));
                        }
                    }
                    else if ((Math.Sign(wireB[b].x - wireA[a - 1].x) != Math.Sign(wireB[b].x - wireA[a].x)) && (Math.Sign(wireA[a].y - wireB[b - 1].y) != Math.Sign(wireA[a].y - wireB[b].y)))
                    {
                        distances.Add(Math.Abs(wireA[a].y) + Math.Abs(wireB[b].x));
                        intersections.Add(new Vector(wireB[b].x, wireA[a].y));
                    }
                }
            }
            distances.RemoveAll(x => x == 0);
            intersections.RemoveAll(x => x.x == 0 && x.y == 0);
            Console.WriteLine("1*: " + distances.Min());

            //2*
            int xMin, xMax, yMin, yMax;
            xMin = (new int[] { wireA.Min(w => w.x), wireB.Min(w => w.x) }).Min();
            xMax = (new int[] { wireA.Max(w => w.x), wireB.Max(w => w.x) }).Max();
            yMin = (new int[] { wireA.Min(w => w.y), wireB.Min(w => w.y) }).Min();
            yMax = (new int[] { wireA.Max(w => w.y), wireB.Max(w => w.y) }).Max();
            int[,] panelA = SetPanel(xMin, xMax, yMin, yMax, wireA);
            int[,] panelB = SetPanel(xMin, xMax, yMin, yMax, wireB);

            Console.WriteLine($"2*: {intersections.Min(i => panelA[i.x - xMin, i.y - yMin] + panelB[i.x - xMin, i.y - yMin])}");
        }

        public static Vector[] Wiring(string wire)
        {
            LinkedList<Vector> wiring = new LinkedList<Vector>();
            wiring.AddLast(new Vector(0, 0));
            foreach(var w in wire.Split(","))
            {
                wiring.AddLast(w[0] switch
                {
                    'L' => new Vector(wiring.Last.Value.x - int.Parse(w.Substring(1)), wiring.Last.Value.y),
                    'R' => new Vector(wiring.Last.Value.x + int.Parse(w.Substring(1)), wiring.Last.Value.y),
                    'U' => new Vector(wiring.Last.Value.x, wiring.Last.Value.y + int.Parse(w.Substring(1))),
                    'D' => new Vector(wiring.Last.Value.x, wiring.Last.Value.y - int.Parse(w.Substring(1))),
                    _ => throw new Exception($"Incorrect input {w[0]}.")
                });
            }
            return wiring.ToArray();
        }

        public static int[,] SetPanel(int xMin, int xMax, int yMin, int yMax, Vector[] wire)
        {
            int[,] panel = new int[xMax - xMin + 1, yMax - yMin + 1];
            int distance = 0;
            for (int w = 1; w < wire.Length; w++)
            {
                var A = wire[w - 1];
                var B = wire[w];

                if (A.x == B.x) //vertikální
                {
                    int d = Math.Abs(B.y - A.y);
                    for (int i = 0; i <= d; i++)
                    {
                        int cy = (A.y * (d - i) + B.y * i) / d;
                        int x = A.x - xMin;
                        int y = cy - yMin;
                        if (panel[x, y] == 0)
                            panel[x, y] = distance;
                        if(i != d)
                            distance++;
                    }
                }
                else //horizontální
                {
                    int d = Math.Abs(B.x - A.x);
                    for (int i = 0; i <= d; i++)
                    {
                        int cx = (A.x * (d - i) + B.x * i) / d;
                        int x = cx - xMin;
                        int y = A.y - yMin;
                        if (panel[x, y] == 0)
                            panel[x, y] = distance;
                        if (i != d)
                            distance++;
                    }
                }
            }
            return panel;
        }
    }

    struct Vector
    {
        public int x; 
        public int y;
        public Vector(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public override string ToString()
        {
            return $"[{x}, {y}]";
        }
    }
}
