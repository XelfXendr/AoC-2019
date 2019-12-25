using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
namespace Day_12
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "<x=17, y=-7, z=-11>;<x=1, y=4, z=-1>;<x=6, y=-2, z=-6>;<x=19, y=11, z=9>";
            string[] cleaned = input.Replace("<", "").Replace(">", "").Replace("x", "").Replace("y", "").Replace("z", "").Replace("=", "").Replace(",", "").Split(";");
            
            Moon[] moons = new Moon[4];
            Vector3[] initial = new Vector3[4];
            
            bool[] seen = new bool[3];
            long[] periods = new long[3];
            List<long>[] primeFactors = new List<long>[3];
            
            for (int i = 0; i < 4; i++)
            {
                string[] m = cleaned[i].Split(" ");
                initial[i] = new Vector3(int.Parse(m[0]), int.Parse(m[1]), int.Parse(m[2]));
                moons[i] = new Moon(initial[i]);
            }

            for (long i = 0; true; i++)
            {
                for (int m1 = 0; m1 < 3; m1++)
                    for (int m2 = m1 + 1; m2 < 4; m2++)
                    {
                        moons[m1].Gravity(moons[m2]);
                        moons[m2].Gravity(moons[m1]);
                    }

                for(int m = 0; m < 4; m++)
                    moons[m].Move();
                if (!seen[0] && moons[0].position.X == initial[0].X && moons[0].velocity.X == 0 && moons[1].position.X == initial[1].X && moons[1].velocity.X == 0 && moons[2].position.X == initial[2].X && moons[2].velocity.X == 0)
                {
                    Console.WriteLine($"X {i + 1}");
                    seen[0] = true;
                    periods[0] = i + 1;
                }
                if (!seen[1] && moons[0].position.Y == initial[0].Y && moons[0].velocity.Y == 0 && moons[1].position.Y == initial[1].Y && moons[1].velocity.Y == 0 && moons[2].position.Y == initial[2].Y && moons[2].velocity.Y == 0)
                {
                    Console.WriteLine($"Y {i + 1}");
                    seen[1] = true;
                    periods[1] = i + 1;
                }
                if (!seen[2] && moons[0].position.Z == initial[0].Z && moons[0].velocity.Z == 0 && moons[1].position.Z == initial[1].Z && moons[1].velocity.Z == 0 && moons[2].position.Z == initial[2].Z && moons[2].velocity.Z == 0)
                {
                    Console.WriteLine($"Z {i + 1}");
                    seen[2] = true;
                    periods[2] = i + 1;
                }

                if (seen[0] && seen[1] && seen[2])
                    break;
            }

            for(int i = 0; i<3; i++)
            {
                primeFactors[i] = new List<long>();
                while(periods[i] != 1)
                    for(int f = 2; f <= periods[i]; f++)
                        if(periods[i] % f == 0)
                        {
                            periods[i] /= f;
                            primeFactors[i].Add(f);
                            break;
                        }
            }

            long max = primeFactors.Max(x => x.Max(y => y));
            long result = 1;
            for(int i = 2; i <= max; i++)
                if (primeFactors.Any(x => x.Any(y => y == i)))
                    result *= (long)Math.Pow(i, primeFactors.Max(x => x.Count(y => y == i)));
            Console.WriteLine(result);

            float sum = 0;
            foreach(Moon m in moons)
            {
                sum += (Math.Abs(m.position.X) + Math.Abs(m.position.Y) + Math.Abs(m.position.Z)) * (Math.Abs(m.velocity.X) + Math.Abs(m.velocity.Y) + Math.Abs(m.velocity.Z));
            }
            Console.WriteLine($"Total energy is {sum}.");
        }
    }

    class Moon
    {
        public Vector3 position;
        public Vector3 velocity = new Vector3(0, 0, 0);

        public Moon(Vector3 position)
        {
            this.position = position;
        }

        public void Gravity(Moon moon)
        {
            Vector3 pos = moon.position;
            if (pos.X != position.X)
                velocity.X += pos.X > position.X ? 1 : -1;
            if (pos.Y != position.Y)
                velocity.Y += pos.Y > position.Y ? 1 : -1;
            if (pos.Z != position.Z)
                velocity.Z += pos.Z > position.Z ? 1 : -1;
        }

        public void Move()
        {
            position += velocity;
        }
    }
}
