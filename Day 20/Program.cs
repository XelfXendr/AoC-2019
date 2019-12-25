using System;
using System.Linq;
using System.Collections.Generic;
using Day_20.Properties;

namespace Day_20 //Donut
{
    class Program
    {
        public static PathNode[,] nodes;
        public static List<PathNode> portals = new List<PathNode>();
        static void Main()
        {
            char[][] input = Resources.Input.Split("\r\n").Select(x => x.ToCharArray()).ToArray();
            int mx = input.Max(x => x.Length);
            int my = input.Length;
            nodes = new PathNode[mx, my];
            //parsing input
            for (int j = 0; j < input.Length; j++)
                for(int i = 0; i < input[j].Length; i++)
                    if(input[j][i] == '.')
                    {
                        nodes[i, j] = new PathNode();
                        if (i > 5 && j > 5 && i < mx - 5 && j < my - 5)
                            nodes[i, j].isOuter = false;

                        //add neighbours
                        if (!(nodes[i - 1, j] is null))
                        {
                            nodes[i, j].neighbours.Add(nodes[i - 1, j]);
                            nodes[i - 1, j].neighbours.Add(nodes[i, j]);
                        }
                        if (!(nodes[i, j - 1] is null))
                        {
                            nodes[i, j].neighbours.Add(nodes[i, j - 1]);
                            nodes[i, j - 1].neighbours.Add(nodes[i, j]);
                        }

                        //check for portals
                        if (char.IsLetter(input[j - 1][i]))
                        {
                            nodes[i, j].isPortal = true;
                            nodes[i, j].type = $"{input[j - 2][i]}{input[j - 1][i]}";
                            if(portals.Any(x => x.type == nodes[i,j].type))
                            {
                                var p = portals.Find(x => x.type == nodes[i, j].type);
                                p.otherEnd = nodes[i, j];
                                nodes[i, j].otherEnd = p;
                            }
                            portals.Add(nodes[i, j]);
                            continue;
                        }
                        if (char.IsLetter(input[j + 1][i]))
                        {
                            nodes[i, j].isPortal = true;
                            nodes[i, j].type = $"{input[j + 1][i]}{input[j + 2][i]}";
                            if (portals.Any(x => x.type == nodes[i, j].type))
                            {
                                var p = portals.Find(x => x.type == nodes[i, j].type);
                                p.otherEnd = nodes[i, j];
                                nodes[i, j].otherEnd = p;
                            }
                            portals.Add(nodes[i, j]);
                            continue;
                        }
                        if (char.IsLetter(input[j][i - 1]))
                        {
                            nodes[i, j].isPortal = true;
                            nodes[i, j].type = $"{input[j][i - 2]}{input[j][i - 1]}";
                            if (portals.Any(x => x.type == nodes[i, j].type))
                            {
                                var p = portals.Find(x => x.type == nodes[i, j].type);
                                p.otherEnd = nodes[i, j];
                                nodes[i, j].otherEnd = p;
                            }
                            portals.Add(nodes[i, j]);
                            continue;
                        }
                        if (char.IsLetter(input[j][i + 1]))
                        {
                            nodes[i, j].isPortal = true;
                            nodes[i, j].type = $"{input[j][i + 1]}{input[j][i + 2]}";
                            if (portals.Any(x => x.type == nodes[i, j].type))
                            {
                                var p = portals.Find(x => x.type == nodes[i, j].type);
                                p.otherEnd = nodes[i, j];
                                nodes[i, j].otherEnd = p;
                            }
                            portals.Add(nodes[i, j]);
                            continue;
                        }
                    }
            
            //precomputing connections
            foreach (var n in portals)
                n.FindConnections();

            //A* sorta
            PathNode from = portals.Find(x => x.type == "AA");
            PathNode to = portals.Find(x => x.type == "ZZ");

            LinkedList<(int depth, PathNode node)> open = new LinkedList<(int depth, PathNode node)>();
            (int depth, PathNode node) current = (0, from);
            open.AddLast(current);
            current.node.depthCost[0] = 0;
            while(open.Count > 0)
            {
                //select best path to check and close it
                current = open.First.Value;
                foreach (var n in open)
                    if (n.node.depthCost[n.depth] < current.node.depthCost[current.depth])
                        current = n;
                open.Remove(current);

                //found path
                if(current.node == to)
                {
                    Console.WriteLine(current.node.depthCost[current.depth]-1);
                    return;
                }

                //teleport
                (int depth, PathNode node) other;
                if (current.node.type == "AA")
                    other = current;
                else
                    other = (current.depth + (current.node.isOuter ? -1 : 1), current.node.otherEnd);

                //open new paths
                foreach(var n in other.node.connections)
                {
                    if (open.Contains((other.depth, n.node)) && current.node.depthCost[current.depth] + n.distance >= n.node.depthCost[other.depth])
                        continue;
                    if (other.depth == 0 && n.node.isOuter && n.node.type != "ZZ")
                        continue;
                    if (other.depth != 0 && (n.node.type == "AA" || n.node.type == "ZZ"))
                        continue;
                    n.node.depthCost[other.depth] = current.node.depthCost[current.depth] + n.distance + 1;
                    if (!open.Contains((other.depth, n.node)))
                        open.AddLast((other.depth, n.node));
                }
            }
        }
    }

    public class PathNode
    {
        public List<PathNode> neighbours = new List<PathNode>();

        public bool isPortal = false;
        public string type = "";
        public bool isOuter = true;

        public PathNode otherEnd;
        public List<(int distance, PathNode node)> connections;

        public Dictionary<int, int> depthCost = new Dictionary<int, int>();

        public void FindConnections()
            => connections = GetConnections(null, 0);

        private List<(int distance, PathNode node)> GetConnections(PathNode originNode, int distanceSoFar)
        {
            List<(int distance, PathNode node)> retList = new List<(int distance, PathNode node)>();
            if(!(originNode is null) && this.isPortal)
            {
                retList.Add((distanceSoFar, this));
                return retList;
            }
            foreach (var n in this.neighbours.Where(x => x != originNode))
                retList.AddRange(n.GetConnections(this, distanceSoFar + 1));
            return retList;
        }
    }
}
