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
        static LinkedList<PathNode> pathNodes = new LinkedList<PathNode>();
        static LinkedList<PathNode> keyNodes = new LinkedList<PathNode>();
        static PathNode playerPos;
        static long best = long.MaxValue;
        static Dictionary<(PathNode a, PathNode b), (int distance, char[] gates, char[] keys)> distances = new Dictionary<(PathNode a, PathNode b), (int distance, char[] gates, char[] keys)>();
        static void Main()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            string[] input = Resources.Input.Split("\r\n");
            for (int j = 0; j < input.Length; j++)
                for (int i = 0; i < input[j].Length; i++)
                    if (input[j][i] == '.')
                    {
                        pathNodes.AddLast(new PathNode(i, j));
                    }
                    else if (input[j][i] == '@')
                    {
                        var node = new PathNode(i, j);
                        playerPos = node;
                        pathNodes.AddLast(node);
                    }
                    else if (char.IsLetter(input[j][i]))
                    {
                        var node = new PathNode(i, j);
                        pathNodes.AddLast(node);
                        if (char.IsUpper(input[j][i]))
                        {
                            node.isGate = true;
                            node.type = char.ToLower(input[j][i]);
                        }
                        else
                        {
                            node.isKey = true;
                            node.type = input[j][i];
                        }
                    }
            foreach (var p1 in pathNodes)
                foreach (var p2 in pathNodes.Where(n => Math.Abs(p1.x - n.x) + Math.Abs(p1.y - n.y) == 1))
                    p1.neighbours.AddLast(p2);

            keyNodes = new LinkedList<PathNode>(pathNodes.Where(p => p.isKey));
            keyNodes.AddLast(playerPos);
            foreach (var a in keyNodes)
                foreach (var b in keyNodes)
                    distances.Add((a, b), FindPath(a, b));
            keyNodes.RemoveLast();
            Console.WriteLine("Precomputing done.");
            getKeys(new LinkedList<char>(), keyNodes, 0, playerPos);
            timer.Stop();
            Console.WriteLine($"Shortest path is {best}. {timer.Elapsed.ToString()} elapsed."); //4830;
            File.WriteAllText("output.txt", $"Shortest path is {best}. {timer.Elapsed.ToString()} elapsed.");
        }

        public static void getKeys(LinkedList<char> keysOwned, LinkedList<PathNode> keysWanted, int stepsSoFar, PathNode current)
        {
            if(keysWanted.Count == 0)
            {
                best = stepsSoFar;
                Console.WriteLine(best);
                return;
            }
            var k = keysWanted.First;
            while (true)
            {
                var info = distances[(current, k.Value)];
                if (stepsSoFar + info.distance >= best)
                {
                    if (k == keysWanted.Last)
                        break;
                    k = k.Next;
                    continue;
                }
                if ((info.gates.Length > 0 && info.gates.Any(x => !keysOwned.Contains(x))) || (info.keys.Length > 0 && info.keys.Any(x => !keysOwned.Contains(x))))
                {
                    if (k == keysWanted.Last)
                        break;
                    k = k.Next;
                    continue;
                }
                keysOwned.AddLast(k.Value.type);

                bool last = k == keysWanted.Last;
                PathNode anchor = null;
                if (!last)
                    anchor = k.Next.Value;
                keysWanted.Remove(k); //TODO improve to not create a new list
                getKeys(keysOwned, keysWanted, stepsSoFar + info.distance, k.Value);
                keysOwned.Remove(k.Value.type);

                if (last)
                {
                    keysWanted.AddLast(k);
                    break;
                }
                var anchorNode = keysWanted.Find(anchor);
                keysWanted.AddBefore(anchorNode, k.Value);
                k = anchorNode;
            }
        }
        
        static (int distance, char[] gates, char[] keys) FindPath(PathNode from, PathNode to)
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
            LinkedList<char> gates = new LinkedList<char>();
            LinkedList<char> keys = new LinkedList<char>();
            while (!(current is null))
            {
                dist++;
                if(current.isGate)
                    gates.AddLast(current.type);
                else if(current.isKey)
                    keys.AddLast(current.type);
                current = current.pathParent;
            }

            return (dist, gates.ToArray(), keys.ToArray());
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

        public bool isGate = false;
        public bool isKey = false;
        public char type;

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
