using System;
using Day_13;
using Day_17.Properties;
using System.Collections.Generic;
using System.Linq;

namespace Day_17
{
    class Program
    {
        static void Main()
        {
            long[] code = Resources.Input.Split(',').Select(x => long.Parse(x)).ToArray();
            IntcodeComputer robot = new IntcodeComputer();

            //PEPARATION
            List<List<int>> rows = new List<List<int>>();
            rows.Add(new List<int>());
            robot.RunCode((long[])code.Clone(), () => 0,
            o => //35 => #; 45 => .; 10 => \n
            {
                switch (o)
                {
                    case 10:
                        rows.Add(new List<int>());
                        break;
                    default:
                        rows.Last().Add((int)o);
                        break;
                }
                Console.Write((char)o);
            });
            rows.RemoveAll(x => x.Count == 0);

            int width = rows[0].Count, height = rows.Count;
            int[,] map = new int[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    map[i, j] = rows[j][i];

            //PART 1
            int sum = 0;
            for (int i = 1; i < width - 1; i++)
                for (int j = 1; j < height - 1; j++)
                    if (map[i, j] == 35 &&
                        map[i + 1, j] == 35 &&
                        map[i - 1, j] == 35 &&
                        map[i, j + 1] == 35 &&
                        map[i, j - 1] == 35)
                        sum += i * j;
            Console.WriteLine($"Sum of alignment parameters is {sum}");

            //PART 2
            char[] playerPoses = new char[] { '^', '<', '>', 'v' };
            (int x, int y) start = (0, 0);
            (int x, int y) end = (0, 0);
            LinkedList<(int x, int y)> turns = new LinkedList<(int x, int y)>();
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    switch ((char)map[i, j])
                    {
                        case '^':
                        case '<':
                        case '>':
                        case 'v':
                            start = (i, j);
                            break;
                        case '#':
                            if (i == 0 || i == width - 1)
                            {
                                if (map[i, j - 1] != map[i, j + 1])
                                    turns.AddLast((i, j));
                                break;
                            }
                            if (j == 0 || j == height - 1)
                            {
                                if (map[i - 1, j] != map[i + 1, j])
                                    turns.AddLast((i, j));
                                break;
                            }
                            if (map[i - 1, j] != map[i + 1, j] && map[i, j - 1] != map[i, j + 1])
                            {
                                turns.AddLast((i, j));
                                break;
                            }
                            if ((map[i - 1, j] == map[i + 1, j] ^ map[i, j - 1] == map[i, j + 1]) && !(new int[] { map[i - 1, j], map[i + 1, j], map[i, j - 1], map[i, j + 1] }).Any(x => playerPoses.Contains((char)x)))
                                end = (i, j);
                            break;
                    }
                }

            //PATH FINDING
            LinkedList<(int x, int y)> path = new LinkedList<(int x, int y)>();
            path.AddFirst(start);
            while (turns.Count > 0)
            {
                var current = path.Last.Value;
                var possible = turns.Where(p => p.x == current.x);
                foreach (var p in possible)
                {
                    bool right = true;
                    for (int i = current.y; i != p.y; i += Math.Sign(p.y - current.y))
                    {
                        if (map[p.x, i] == 46)
                        {
                            right = false;
                            break;
                        }
                    }
                    if (right)
                    {
                        path.AddLast(p);
                        turns.Remove(p);
                        break;
                    }
                }
                possible = turns.Where(p => p.y == current.y);
                foreach (var p in possible)
                {
                    bool right = true;
                    for (int i = current.x; i != p.x; i += Math.Sign(p.x - current.x))
                    {
                        if (map[i, p.y] == 46)
                        {
                            right = false;
                            break;
                        }
                    }
                    if (right)
                    {
                        path.AddLast(p);
                        turns.Remove(p);
                        break;
                    }
                }
            }
            path.AddLast(end);
            // y-- up, y++ down, x-- left, x++ right
            string crudeCommand = "";
            (int x, int y) dir = (0, -1);
            var currentP = path.First;
            while(currentP != path.Last)
            {
                (int cx, int cy) = currentP.Value;
                (int nx, int ny) = currentP.Next.Value;
                (int x, int y) nDir = (Math.Sign(nx - cx), Math.Sign(ny - cy));
                crudeCommand += dir.x == nDir.y && dir.y == -nDir.x ? "R" : "L";
                crudeCommand += $",{Math.Abs(nx - cx + ny - cy)},";
                dir = nDir;
                currentP = currentP.Next;
            }
            Console.WriteLine(crudeCommand);

            /*
            L,6,R,8,R,12,L,6,L,8,L,10,L,8,R,12,L,6,R,8,R,12,L,6,L,8,L,8,L,10,L,6,L,6,L,10,L,8,R,12,L,8,L,10,L,6,L,6,L,10,L,8,R,12,L,6,R,8,R,12,L,6,L,8,L,8,L,10,L,6,L,6,L,10,L,8,R,12
            [L,6,R,8,R,12,L,6,L,8]  <L,10,L,8,R,12>  [L,6,R,8,R,12,L,6,L,8]  {L,8,L,10,L,6,L,6}  <L,10,L,8,R,12>  {L,8,L,10,L,6,L,6}  <L,10,L,8,R,12>  [L,6,R,8,R,12,L,6,L,8]  {L,8,L,10,L,6,L,6}  <L,10,L,8,R,12>
            A = L,6,R,8,R,12,L,6,L,8
            B = L,10,L,8,R,12
            C = L,8,L,10,L,6,L,6
            A,B,A,C,B,C,B,A,C,B
            */

            char[] input = ("A,B,A,C,B,C,B,A,C,B" + ((char)10) + "L,6,R,8,R,12,L,6,L,8" + ((char)10) + "L,10,L,8,R,12" + ((char)10) + "L,8,L,10,L,6,L,6" + ((char)10) + 'n' + ((char)10)).ToCharArray();
            int index = 0;

            code[0] = 2;
            robot.RunCode((long[])code.Clone(), () =>
            {
                char ret = input[index];
                index++;
                return ret;
            },
            o =>
            {
                if (o < 126)
                    return;
                Console.WriteLine($"{o} drones have been cleaned.");
            });
        }
    }
}
