using System;
using Day_18.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace Day_18
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

            //Parsing
            string[] input = Resources.Input.Split("\r\n");
            List<PathNode> keyNodes = new List<PathNode>();
            List<PathNode> gateNodes = new List<PathNode>();
            PathNode playerNode = null;
            PathNode[,] map = new PathNode[input.Max(x => x.Length), input.Length];
            Dictionary<char, byte> keyIndexes = new Dictionary<char, byte>();
            Dictionary<int, (int distance, int gates, int keys)> distances = new Dictionary<int, (int distance, int gates, int keys)>();
            byte currentIndex = 0;
            
            for (int j = 0; j < input.Length; j++)
                for (int i = 0; i < input[j].Length; i++)
                {
                    if (input[j][i] == '.') //normal node
                    {
                        map[i, j] = new PathNode(i, j);
                    }
                    else if (input[j][i] == '@') //player node
                    {
                        var node = new PathNode(i, j);
                        playerNode = node;
                        map[i, j] = node;
                        playerNode.keyIndex = 0xFF;
                    }
                    else if (char.IsLetter(input[j][i])) //gate and key nodes
                    {
                        var node = new PathNode(i, j);
                        map[i, j] = node;
                        if (char.IsUpper(input[j][i]))
                        {
                            node.isGate = true;
                            node.type = char.ToLower(input[j][i]);
                            gateNodes.Add(node);
                        }
                        else
                        {
                            node.isKey = true;
                            node.type = input[j][i];
                            keyNodes.Add(node);
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

                    if(!(map[i, j] is null)) //neighboring nodes
                    {
                        if(!(map[i - 1, j] is null))
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

            Console.WriteLine($"Parsing done after {sw.Elapsed}");

            //finding distances between key nodes
            keyNodes.Add(playerNode);
            for (int i = 0; i < keyNodes.Count - 1; i++)
                for (int j = i + 1; j < keyNodes.Count; j++)
                {
                    var values = FindPath(keyNodes[i], keyNodes[j]);
                    distances.Add((keyNodes[i].keyIndex << 8) + keyNodes[j].keyIndex, values);
                    distances.Add((keyNodes[j].keyIndex << 8) + keyNodes[i].keyIndex, values);
                }
            keyNodes.Remove(playerNode);

            Console.WriteLine($"Precomputing distances done after {sw.Elapsed}");

            int allKeysOwned = 0x03FFFFFF; //26 1s
            
            //finding best path
            LinkedList<(int distanceSoFar, int keysOwned, PathNode currentNode)> q = new LinkedList<(int distanceSoFar, int keysOwned, PathNode currentNode)>();
            q.AddFirst((0, 0, playerNode));

            long h = 0;
            int highest = 0;
            while(q.Count > 0)
            {
                var current = q.First.Value; //dequeueing the one with lowest distance

                q.Remove(current);
                if (current.distanceSoFar > highest)
                    highest = current.distanceSoFar;

                
                if (h % 100000 == 0)
                    Console.WriteLine(h + " " + q.Count + " " + current.distanceSoFar + " " + Convert.ToString(current.keysOwned, 2));
                h++;
                
                //Console.WriteLine(q.Count);

                if (current.keysOwned == allKeysOwned)
                {
                    Console.WriteLine($"Shortest path is {current.distanceSoFar}. Found after {sw.Elapsed}"); //4830
                    File.AppendAllText("output.txt", $"Shortest path is {current.distanceSoFar}. Found after {sw.Elapsed}");
                    return;
                }

                foreach (var k in keyNodes)
                {
                    if ((current.keysOwned & k.keyBin) != 0)
                        continue;
                    var d = distances[(current.currentNode.keyIndex << 8) + k.keyIndex];
                    if ((d.gates & current.keysOwned) != d.gates)
                        continue;
                    if ((d.keys & current.keysOwned) != d.keys)
                        continue;
                    q.AddLast((current.distanceSoFar + d.distance, current.keysOwned | k.keyBin, k));
                }
            }

            Console.WriteLine("Didn't find path :( " + highest + " " + sw.Elapsed);
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

            while(open.Count > 0)
            {
                current = open.First.Value;
                foreach(var n in open)
                    if(n.F < current.F)
                        current = n;
                open.Remove(current);
                closed.AddLast(current);

                if (current == to)
                    break;

                foreach(var n in current.neighbours)
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
