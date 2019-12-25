using System;
using Day_10.Properties;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Day_10
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Asteroid> belt = new List<Asteroid>();
            List<Asteroid> targets = new List<Asteroid>();
            string[] lines = Resources.Input.Split("\r\n");

            for (int j = 0; j < lines.Length; j++)
                for (int i = 0; i < lines[j].Length; i++)
                    if (lines[j][i] == '#')
                        belt.Add(new Asteroid(i, j));

            int sitex = 23;
            int sitey = 20;
            Asteroid site = belt.Find(x => x.x == sitex && x.y == sitey);
            belt.Remove(site);

            int count = 0;
            while(belt.Count > 0)
            {
                foreach (var another in belt)
                {
                    bool blocked = false;
                    foreach (var blocking in belt)
                    {
                        if (another == blocking)
                            continue;
                        if (site.distanceFrom(another) < site.distanceFrom(blocking))
                            continue;
                        if (!Asteroid.equalInRange(site.vectorTo(another), site.vectorTo(blocking), 0.00001f))
                            continue;
                        blocked = true;
                        break;
                    }
                    if (blocked)
                        continue;
                    targets.Add(another);
                }

                var ordered = targets.OrderBy(t => Math.Acos(-(site.y - t.y) / Math.Sqrt(Math.Pow(site.x - t.x, 2) + Math.Pow(site.y - t.y, 2))) * Math.Sign(site.x - t.x));
                foreach(var asteroid in ordered)
                {
                    if (count == 199)
                    {
                        Console.WriteLine(asteroid.x * 100 + asteroid.y);
                        return;
                    }
                    belt.Remove(asteroid);
                    count++;
                }
            }
        }
    }

    public class Asteroid
    {
        public int x;
        public int y;
        public int view = 0;
        public Asteroid(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public float distanceFrom(Asteroid asteroid)
            => (float)Math.Sqrt(Math.Pow(x - asteroid.x, 2) + Math.Pow(y - asteroid.y, 2));
        public Vector2 vectorTo(Asteroid asteroid)
        {
            Vector2 vector = new Vector2(asteroid.x - x, asteroid.y - y);
            return vector / vector.Length();
        }   
        public static bool equalInRange(Vector2 a, Vector2 b, float range)
        {
            float xr = Math.Abs(a.X * range);
            float yr = Math.Abs(a.Y * range);

            return (a.X - xr <= b.X && b.X <= a.X + xr) && (a.Y - yr <= b.Y && b.Y <= a.Y + yr);
        }
    }
}
