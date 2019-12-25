using System;
using System.Collections.Generic;
using System.Linq;
using Day_22.Properties;
using System.Numerics;
namespace Day_22
{
    class Program
    {
        static void Main(string[] args)
        {
            //parsing input
            List<(byte op, BigInteger n)> temp = new List<(byte op, BigInteger n)>();
            foreach (var l in Resources.Input.Split("\r\n"))
            {
                if (l.Length == 0)
                    continue;
                var ls = l.Split(' ');
                if(ls[0] == "cut")
                {
                    temp.Add((0, BigInteger.Parse(ls[1])));
                    continue;
                }
                if(ls[1] == "into")
                {
                    temp.Add((1, 0));
                    continue;
                }
                temp.Add((2, BigInteger.Parse(ls[3])));
            }
            (byte op, BigInteger n)[] shuffles = temp.ToArray();

            //part 1
            BigInteger length = 10007;
            var part1shuffles = Simplify(((byte op, BigInteger n)[])shuffles.Clone(), length);
            BigInteger p = 2019;
            foreach (var s in part1shuffles)
            {
                Console.WriteLine(s.op + " " + s.n);
                if (s.op == 0)
                    p -= s.n;
                else if (s.op == 1)
                    p = length - p - 1;
                else
                    p *= s.n;
                p %= length;
                if (p < 0)
                    p += length;
            }
            Console.WriteLine(p);
            Console.WriteLine(Reverse(part1shuffles, p, length));
            
            //part2
            length = 119315717514047;
            BigInteger iters = 101741582076661;
            p = 2020;

            List<(BigInteger i, (byte op, BigInteger n)[] shuffles)> iterShuffles = new List<(BigInteger i, (byte op, BigInteger n)[] shuffles)>();
            iterShuffles.Add((1, Simplify(shuffles, length)));

            for(BigInteger i = 2; i <= iters; i*=2)
            {
                var s = ((byte op, BigInteger n)[])iterShuffles.Last().shuffles.Clone();
                s = s.Concat(s).ToArray();
                iterShuffles.Add((i, Simplify(s, length)));
            }
            Console.WriteLine("next step");

            (byte op, BigInteger n)[] superUltraMegaShuffle = new (byte op, BigInteger n)[0];
            for(int i = 0; i < 64; i++)
            {
                if((iters & 1) == 1)
                {
                    superUltraMegaShuffle = Simplify(superUltraMegaShuffle.Concat(iterShuffles[i].shuffles).ToArray(), length);
                }
                iters >>= 1;
            }
            Console.WriteLine("trying to reverse");
            BigInteger rev = Reverse(superUltraMegaShuffle, 2020, length);
            Console.WriteLine(rev);
            foreach (var s in superUltraMegaShuffle)
            {
                Console.WriteLine(s.op + " " + s.n);
                if (s.op == 0)
                    rev -= s.n;
                else if (s.op == 1)
                    rev = length - rev - 1;
                else
                    rev *= s.n;
                rev %= length;
                if (rev < 0)
                    rev += length;
            }
            Console.WriteLine(rev);
        }

        public static BigInteger Reverse((byte op, BigInteger n)[] shuffles, BigInteger p, BigInteger length)
        {
            for(int i = shuffles.Length - 1; i >= 0; i--)
            {
                switch(shuffles[i].op)
                {
                    case 0:
                        p += shuffles[i].n;
                        break;
                    case 1:
                        p = length - p - 1;
                        break;
                    case 2:
                        //modular inverse time

                        //find b such that (n*b)%l = 1
                        BigInteger n = shuffles[i].n;
                        if (n < 0)
                            n += length;

                        List<(BigInteger q, BigInteger r, BigInteger s, BigInteger t)> table = new List<(BigInteger q, BigInteger r, BigInteger s, BigInteger t)>();
                        table.Add((0, n, 1, 0));
                        table.Add((0, length, 0, 1));
                        for (int j = 0; true; j++)
                        {
                            BigInteger q = table[j].r / table[j + 1].r;
                            BigInteger r = table[j].r - q * table[j + 1].r;
                            BigInteger s = table[j].s - q * table[j + 1].s;
                            BigInteger t = table[j].t - q * table[j + 1].t;
                            if (r == 1)
                            {
                                p = s * p;
                                break;
                            }
                            table.Add((q, r, s, t));
                        }
                        break;
                }
                p %= length;
                if (p < 0)
                    p += length;
            }
            return p;
        }

        public static (byte op, BigInteger n)[] Simplify((byte op, BigInteger n)[] shuffles, BigInteger length)
        {
            List<(byte op, BigInteger n)> list = shuffles.ToList();

            for(int i = 1; i < list.Count; i++)
            {
                if (list[i].op != 2)
                    continue;

                for(int j = i; j > 0; j--)
                {
                    if (list[j - 1].op == 2)
                        continue;
                    if(list[j - 1].op == 0)
                    {
                        BigInteger n1 = list[j - 1].n;
                        BigInteger n2 = list[j].n;
                        list[j - 1] = (2, n2);
                        list[j] = (0, (n1 * n2) % length);
                        continue;
                    }
                    if(list[j - 1].op == 1)
                    {
                        list[j - 1] = (2, -list[j].n);
                        list[j] = (0, list[j].n);
                        continue;
                    }
                }
            }

            for(int i = 1; i < list.Count; i++)
            {
                if (list[i].op != 0)
                    continue;
                for(int j = i; j > 0; j--)
                {
                    if (list[j - 1].op == 0)
                        continue;
                    if (list[j - 1].op == 2)
                        break;
                    if (list[j - 1].op == 1)
                    {
                        list[j - 1] = (0, -list[j].n);
                        list[j] = (1, 0);
                    }
                }
            }
            
            for(int i = list.Count - 1; i > 0; i--)
            {
                if(list[i].op == list[i - 1].op)
                {
                    switch(list[i].op)
                    {
                        case 0:
                            list[i - 1] = (0, (list[i - 1].n + list[i].n) % length);
                            list.RemoveAt(i);
                            break;
                        case 1:
                            list.RemoveAt(i);
                            list.RemoveAt(i - 1);
                            i--;
                            break;
                        case 2:
                            list[i - 1] = (2, (list[i - 1].n * list[i].n) % length);
                            list.RemoveAt(i);
                            break;
                    }
                }
            }
            
            return list.ToArray();
        }
    }
}
