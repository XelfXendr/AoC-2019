using System;
using Day_18_Part_2.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace Day_18_Part_2
{
    class Program
    {
        static void Main()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            /*
             * There are 26 keys so Int32 will be enough to express each one as a specific bit
            */

            //setting up input for part 2
            char[][] input = Resources.Input.Split("\r\n").Select(x => x.ToCharArray()).ToArray();
            int maxx = input.Max(x => x.Length);
            int maxy = input.Length;
            int center = 0;

            for (int i = maxx/2 - 2; i < maxx; i++)
                if(input[i][i] == '@')
                {
                    center = i;
                    break;
                }

            input[center - 1][center - 1] = '@';
            input[center + 1][center - 1] = '@';
            input[center - 1][center + 1] = '@';
            input[center + 1][center + 1] = '@';
            input[center][center] = '#';
            input[center - 1][center] = '#';
            input[center + 1][center] = '#';
            input[center][center - 1] = '#';
            input[center][center + 1] = '#';

            //parsing input
            List<PathNode>[] keyNodes = new List<PathNode>[4];
            PathNode[] playerNodes = new PathNode[4];

            PathNode[,] map = new PathNode[maxx, maxy];

            Dictionary<char, byte> keyIndexes = new Dictionary<char, byte>();
            Dictionary<int, (int distance, int gates, int keys)> distances = new Dictionary<int, (int distance, int gates, int keys)>();
            byte currentIndex = 0;

            for(int s = 0; s < 4; s++)
            {
                keyNodes[s] = new List<PathNode>();

                int xs = s == 0 || s == 2 ? 0 : center + 1;
                int ys = s == 0 || s == 1 ? 0 : center + 1;
                int ye = s == 0 || s == 1 ? center : maxy;

                for (int j = ys; j < ye; j++)
                    for (int i = xs; i < (s == 0 || s == 2 ? center : input[j].Length); i++)
                    {
                        if (input[j][i] == '.') //normal node
                        {
                            map[i, j] = new PathNode(i, j);
                        }
                        else if (input[j][i] == '@') //player node
                        {
                            var node = new PathNode(i, j);
                            playerNodes[s] = node;
                            map[i, j] = node;
                            playerNodes[s].keyIndex = 0xFF;
                        }
                        else if (char.IsLetter(input[j][i])) //gate and key nodes
                        {
                            var node = new PathNode(i, j);
                            map[i, j] = node;
                            if (char.IsUpper(input[j][i]))
                            {
                                node.isGate = true;
                                node.type = char.ToLower(input[j][i]);
                            }
                            else
                            {
                                node.isKey = true;
                                node.type = input[j][i];
                                keyNodes[s].Add(node);
                            }

                            if (keyIndexes.ContainsKey(node.type))
                                node.keyIndex = keyIndexes[node.type];
                            else
                            {
                                node.keyIndex = currentIndex;
                                keyIndexes.Add(node.type, currentIndex);
                                currentIndex++;
                            }
                            node.keyBin = 1 << node.keyIndex;
                        }

                        if (!(map[i, j] is null)) //neighboring nodes
                        {
                            if (!(map[i - 1, j] is null))
                            {
                                map[i, j].neighbours.AddLast(map[i - 1, j]);
                                map[i - 1, j].neighbours.AddLast(map[i, j]);
                            }
                            if (!(map[i, j - 1] is null))
                            {
                                map[i, j].neighbours.AddLast(map[i, j - 1]);
                                map[i, j - 1].neighbours.AddLast(map[i, j]);
                            }
                        }
                    }
            }

            Console.WriteLine($"Parsing done after {sw.Elapsed}");

            //finding distances between key nodes
            for (int s = 0; s < 4; s++)
            {
                keyNodes[s].Add(playerNodes[s]);
                for (int i = 0; i < keyNodes[s].Count - 1; i++)
                    for (int j = i + 1; j < keyNodes[s].Count; j++)
                    {
                        var values = FindPath(keyNodes[s][i], keyNodes[s][j]);
                        distances.Add((keyNodes[s][i].keyIndex << 8) + keyNodes[s][j].keyIndex, values);
                        distances.Add((keyNodes[s][j].keyIndex << 8) + keyNodes[s][i].keyIndex, values);
                    }
                keyNodes[s].Remove(playerNodes[s]);
            }
            Console.WriteLine($"Precomputing distances done after {sw.Elapsed}");

            int allKeysOwned = 0x03FFFFFF; //26 1s
            
            //finding best path
            Queue<(int distanceSoFar, int keysOwned, PathNode[] nodes)> q = new Queue<(int, int, PathNode[])>();
            q.Enqueue((0, 0, playerNodes));

            int shortest = int.MaxValue;
            while (q.Count > 0)
            {
                var current = q.Dequeue();

                if (current.keysOwned == allKeysOwned)
                {
                    if (current.distanceSoFar < shortest)
                        shortest = current.distanceSoFar;
                    continue;
                }

                for (int s = 0; s < 4; s++)
                {
                    foreach (var k in keyNodes[s])
                    {
                        if ((current.keysOwned & k.keyBin) != 0)
                            continue;
                        var d = distances[(current.nodes[s].keyIndex << 8) + k.keyIndex];
                        if ((d.gates & current.keysOwned) != d.gates)
                            continue;
                        if ((d.keys & current.keysOwned) != d.keys)
                            continue;

                        PathNode[] newNodes = (PathNode[])current.nodes.Clone();
                        newNodes[s] = k;

                        if (q.Any(x => (x.keysOwned == (current.keysOwned | k.keyBin)) && (Same(x.nodes, newNodes)) && (x.distanceSoFar <= current.distanceSoFar + d.distance)))
                            continue;

                        q.Enqueue((current.distanceSoFar + d.distance, current.keysOwned | k.keyBin, newNodes));
                    }
                }
            }

            Console.WriteLine($"Shortest path is {shortest}. Found after {sw.Elapsed}"); //4830
            
        }

        public static bool Same(PathNode[] a, PathNode[] b)
        {
            bool ret = true;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i])
                    ret = false;
            return ret;
        }

        static (int distance, int gates, int keys) FindPath(PathNode from, PathNode to)
        {
            LinkedList<PathNode> open = new LinkedList<PathNode>();
            LinkedList<PathNode> closed = new LinkedList<PathNode>();

            PathNode current = from;
            open.AddLast(current);
            current.pathParent = null;
            current.g = 0;
            current.h = 0;

            while (open.Count > 0)
            {
                current = open.First.Value;
                foreach (var n in open)
                    if (n.F < current.F)
                        current = n;
                open.Remove(current);
                closed.AddLast(current);

                if (current == to)
                    break;

                foreach (var n in current.neighbours)
                {
                    if (closed.Contains(n))
                        continue;
                    float newG = current.g + 1;
                    float newH = PathNode.Distance(n, to);
                    if (open.Contains(n) && newG + newH >= n.F)
                        continue;
                    n.g = newG;
                    n.h = newH;
                    n.pathParent = current;
                    if (!open.Contains(n))
                        open.AddLast(n);
                }
            }

            current = to.pathParent;
            int dist = 0;
            int gates = 0;
            int keys = 0;
            while (!(current is null))
            {
                dist++;
                if (!(current.pathParent is null))
                {
                    if (current.isGate)
                        gates |= 1 << current.keyIndex;
                    else if (current.isKey)
                        keys |= 1 << current.keyIndex;
                }
                current = current.pathParent;
            }
            return (dist, gates, keys);
        }
    }

    public class PathNode
    {
        public int x;
        public int y;
        public LinkedList<PathNode> neighbours = new LinkedList<PathNode>();

        //pathfinding
        public float g;
        public float h;
        public float F { get => g + h; }
        public PathNode pathParent;

        //key stuff
        public bool isGate = false;
        public bool isKey = false;
        public char type;
        public byte keyIndex;
        public int keyBin;

        public PathNode(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static float Distance(PathNode node1, PathNode node2)
        {
            return MathF.Sqrt(MathF.Pow(node1.x - node2.x, 2) + MathF.Pow(node1.y - node2.y, 2));
        }
    }
}