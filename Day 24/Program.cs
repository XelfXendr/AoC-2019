using System;
using System.Collections.Generic;
namespace Day_24
{
    class Program
    {
        static void Main()
        {
            string[] input = "...#. #.##. #..## #.### ##...".Split(' ');
            string[] test = "....# #..#. #..## ..#.. #....".Split(' ');
            bool[,] eris = new bool[5,5];
            for (int j = 0; j < 5; j++)
                for (int i = 0; i < 5; i++)
                    eris[i, j] = input[j][i] == '#';

            //PART1
            LinkedList<int> seen = new LinkedList<int>();
            bool[,] newEris = new bool[5, 5];
            while (true)
            {
                int bugs = 0;

                for (int j = 4; j >= 0; j--)
                    for(int i = 4; i >= 0; i--)
                    {
                        bugs <<= 1;
                        if (eris[i, j]) bugs++;

                        int count = 0;
                        if (i > 0 && eris[i - 1, j]) 
                            count++;
                        if (i < 4 && eris[i + 1, j])
                            count++;
                        if (j > 0 && eris[i, j - 1]) 
                            count++;
                        if (j < 4 && eris[i, j + 1])
                            count++;

                        if (eris[i, j]) 
                            newEris[i, j] = count == 1;
                        else
                            newEris[i, j] = count == 1 || count == 2;
                    }
                var h = eris;
                eris = newEris;
                newEris = h;
                if(seen.Contains(bugs))
                {
                    Console.WriteLine($"Biodiversity rating is {bugs}.");
                    break;
                }
                seen.AddLast(bugs);
            }

            bool[,,] rEris = new bool[401, 5, 5];
            for (int j = 0; j < 5; j++)
                for (int i = 0; i < 5; i++)
                    rEris[200, i, j] = input[j][i] == '#';
            bool[,,] newrEris = new bool[401, 5, 5];
            for (int m = 0; m < 200; m++)
            {
                for(int d = 0; d < 401; d++)
                    for(int i = 0; i < 5; i++)
                        for(int j = 0; j < 5; j++)
                        {
                            if (i == 2 && j == 2)
                                continue;
                            if (d == 0 || d == 400)
                                continue;
                            int count = 0;

                            //up
                            if (j == 0)
                            {
                                if (rEris[d - 1, 2, 1])
                                    count++;
                            }
                            else if (j == 3 && i == 2)
                            {
                                if (rEris[d + 1, 0, 4]) count++;
                                if (rEris[d + 1, 1, 4]) count++;
                                if (rEris[d + 1, 2, 4]) count++;
                                if (rEris[d + 1, 3, 4]) count++;
                                if (rEris[d + 1, 4, 4]) count++;
                            }
                            else if (rEris[d, i, j - 1]) count++;

                            //down
                            if (j == 4)
                            {
                                if (rEris[d - 1, 2, 3])
                                    count++;
                            }
                            else if (j == 1 && i == 2)
                            {
                                if (rEris[d + 1, 0, 0]) count++;
                                if (rEris[d + 1, 1, 0]) count++;
                                if (rEris[d + 1, 2, 0]) count++;
                                if (rEris[d + 1, 3, 0]) count++;
                                if (rEris[d + 1, 4, 0]) count++;
                            }
                            else if (rEris[d, i, j + 1]) count++;

                            //left
                            if (i == 0)
                            {
                                if (rEris[d - 1, 1, 2])
                                    count++;
                            }
                            else if (j == 2 && i == 3)
                            {
                                if (rEris[d + 1, 4, 0]) count++;
                                if (rEris[d + 1, 4, 1]) count++;
                                if (rEris[d + 1, 4, 2]) count++;
                                if (rEris[d + 1, 4, 3]) count++;
                                if (rEris[d + 1, 4, 4]) count++;
                            }
                            else if (rEris[d, i - 1, j]) count++;

                            //right
                            if (i == 4)
                            {
                                if (rEris[d - 1, 3, 2]) 
                                    count++;
                            }
                            else if (j == 2 && i == 1)
                            {
                                if (rEris[d + 1, 0, 0]) count++;
                                if (rEris[d + 1, 0, 1]) count++;
                                if (rEris[d + 1, 0, 2]) count++;
                                if (rEris[d + 1, 0, 3]) count++;
                                if (rEris[d + 1, 0, 4]) count++;
                            }
                            else if (rEris[d, i + 1, j]) count++;

                            if (rEris[d, i, j])
                                newrEris[d, i, j] = count == 1;
                            else
                                newrEris[d, i, j] = count == 1 || count == 2;
                        }
                var h = newrEris;
                newrEris = rEris;
                rEris = h;
            }
            int finalCount = 0;
            for (int d = 0; d < 401; d++)
                for (int i = 0; i < 5; i++)
                    for (int j = 0; j < 5; j++)
                        if (rEris[d, i, j]) finalCount++;
            Console.WriteLine(finalCount);
        }
    }
}
