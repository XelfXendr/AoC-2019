using System;
using System.Collections;
using Day_8.Properties;
using System.Linq;

namespace Day_8
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = Resources.Input;
            int width = 25;
            int height = 6;
            int numberOfLayers = input.Length / (width * height);

            int[][] layers = new int[numberOfLayers][];

            for(int i = 0; i < numberOfLayers; i++)
            {
                layers[i] = new int[width * height];
                for(int j = 0; j < width*height; j++)
                {
                    layers[i][j] = int.Parse(input[i * width * height + j].ToString());
                }
            }

            //part 1
            int[] minLayer = null;
            int count = width * height + 1;
            foreach(var l in layers)
            {
                int newCount = l.Count(x => x == 0);
                if(newCount < count)
                {
                    count = newCount;
                    minLayer = l;
                }
            }
            Console.WriteLine(minLayer.Count(x => x == 1) * minLayer.Count(x => x == 2));

            //part 2
            int[] stack = layers[0];
            for (int i = 0; i < numberOfLayers; i++)
                for (int j = 0; j < width * height; j++)
                    if (stack[j] == 2)
                        stack[j] = layers[i][j];

            string image = "";
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                    image += stack[i * width + j] == 0 ? "  " : "██";
                image += "\n";
            }
            Console.Write(image);
        }
    }
}
