using System;
using Day_6.Properties;
using System.Linq;
using System.Collections.Generic;
namespace Day_6
{
    class Program
    {
        static void Main()
        {
            string[] input = Resources.Input.Split("\r\n");
            Dictionary<string, Satellite> satellites = new Dictionary<string, Satellite>();
            foreach (string line in input)
            {
                string[] relatives = line.Split(')');
                Satellite A;
                Satellite B;

                if (satellites.ContainsKey(relatives[0]))
                    A = satellites[relatives[0]];
                else 
                {
                    A = new Satellite(relatives[0]);
                    satellites.Add(A.name, A);
                }

                if (satellites.ContainsKey(relatives[1]))
                    B = satellites[relatives[1]];
                else
                {
                    B = new Satellite(relatives[1]);
                    satellites.Add(B.name, B);
                }

                A.children.Add(B);
                B.parent = A;   
            }

            Console.WriteLine($"Checksum is {satellites["COM"].Checksum(0)}.");

            string[] YOUpath = satellites["YOU"].PathToCOM();
            string[] SANpath = satellites["SAN"].PathToCOM();
            string commonOrbit = "";
            foreach(string satellite in YOUpath)
            {
                if (SANpath.Contains(satellite))
                {
                    commonOrbit = satellite;
                    break;
                }
            }
            if (commonOrbit == "")
                throw new Exception("No common orbit!");
            int distance = Array.FindIndex(YOUpath, x => x == commonOrbit) + Array.FindIndex(SANpath, x => x == commonOrbit);
            Console.WriteLine(distance);
        }
    }

    public class Satellite
    {
        public string name;
        public Satellite parent;
        public List<Satellite> children = new List<Satellite>();

        public Satellite(string name) => this.name = name;
    }

    public static class Ext
    {
        public static int Checksum(this Satellite satellite, int position)
        {
            int sum = position;
            position++;
            foreach (Satellite child in satellite.children)
                sum += child.Checksum(position);
            return sum;
        }
        public static string[] PathToCOM(this Satellite satellite)
        {
            List<string> path = new List<string>();

            Satellite current = satellite;
            while (current.parent != null)
            {
                current = current.parent;
                path.Add(current.name);
            }
            return path.ToArray();
        }
    }
}
