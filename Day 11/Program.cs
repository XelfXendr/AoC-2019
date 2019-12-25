using System;
using Day_11.Properties;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
namespace Day_11
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<(int, int), bool> ship = new Dictionary<(int, int), bool>();
            LinkedList<(int, int)> coloredPanels = new LinkedList<(int, int)>();
            Robot robot = new Robot(ship, coloredPanels);
            long[] code = Resources.Input.Split(",").Select(x => long.Parse(x)).ToArray();

            ship.Add((0, 0), true);

            robot.Camera(1);
            robot.brain.RunCode(code);

            Console.WriteLine($"{coloredPanels.Distinct().Count()} squares were painted.");
            var whiteSquares = coloredPanels.Distinct().Where(x => ship[x]);

            int minx = whiteSquares.Min(x => x.Item1);
            int miny = whiteSquares.Min(x => x.Item2);
            int height = whiteSquares.Max(x => x.Item2) - miny;
            foreach ((int x, int y) in whiteSquares)
            {
                Console.SetCursorPosition(x - minx, height - y + miny + 1);
                Console.Write("█");
            }
            Console.SetCursorPosition(0, 10);
        }
    }
    
    public class Robot
    {
        public int x = 0;
        public int y = 0;

        public int dirx = 0;
        public int diry = 1;

        public IntcodeComputer brain = new IntcodeComputer();

        public Dictionary<(int, int), bool> ship;
        public LinkedList<(int, int)> coloredPanels;

        public Robot(Dictionary<(int, int), bool> ship, LinkedList<(int, int)> coloredPanels)
        {
            brain.robot = this;
            this.ship = ship;
            this.coloredPanels = coloredPanels;
        }

        public void Turn(long a)
        {
            int hx;
            int hy;
            if(a == 0)
            {
                hx = -diry;
                hy = dirx;
            }
            else
            {
                hx = diry;
                hy = -dirx;
            }
            dirx = hx;
            diry = hy;
        }

        public void Move()
        {
            x += dirx;
            y += diry;
        }

        public void Camera(long panel) //0 black, 1 white
        {
            brain.input.AddLast(panel);
        }

        public void GetOutput(long color, long turn)
        {
            coloredPanels.AddLast((x, y));
            if (ship.ContainsKey((x, y)))
                ship[(x, y)] = color == 1;
            else
                ship.Add((x, y), color == 1);
            Turn(turn);
            Move();
            if (ship.ContainsKey((x, y)))
                Camera(ship[(x, y)] ? 1 : 0);
            else
                Camera(0);
        }
    }

    public class IntcodeComputer
    {
        long[] memory;
        Dictionary<long, long> extraMemory;
        long relativeBase = 0;

        public LinkedList<long> input = new LinkedList<long>();
        public long[] outputs = new long[2];
        public int outputCount = 0;

        public Robot robot;

        public void RunCode(long[] code)
        {
            memory = (long[])code.Clone();
            extraMemory = new Dictionary<long, long>();

            bool halt = false;
            for (long i = 0; i < memory.Length;)
            {
                string upcode = memory[i].ToString("D5");
                switch (upcode.Substring(3, 2))
                {
                    case "01":
                        setRegister(upcode[0], memory[i + 3], getValue(upcode[2], memory[i + 1]) + getValue(upcode[1], memory[i + 2]));
                        i += 4;
                        break;
                    case "02":
                        setRegister(upcode[0], memory[i + 3], getValue(upcode[2], memory[i + 1]) * getValue(upcode[1], memory[i + 2]));
                        i += 4;
                        break;

                    case "03": //input
                        while (input.Count <= 0)
                            Thread.Sleep(1);
                        setRegister(upcode[2], memory[i + 1], input.First.Value);
                        input.RemoveFirst();
                        i += 2;
                        break;
                    case "04": //output
                        if(outputCount == 0)
                        {
                            outputCount++;
                            outputs[0] = getValue(upcode[2], memory[i + 1]);
                        }
                        else
                        {
                            outputCount = 0;
                            outputs[1] = getValue(upcode[2], memory[i + 1]);
                            robot.GetOutput(outputs[0], outputs[1]);
                        }
                        i += 2;
                        break;
                    case "05":
                        if (getValue(upcode[2], memory[i + 1]) != 0)
                            i = getValue(upcode[1], memory[i + 2]);
                        else
                            i += 3;
                        break;
                    case "06":
                        if (getValue(upcode[2], memory[i + 1]) == 0)
                            i = getValue(upcode[1], memory[i + 2]);
                        else
                            i += 3;
                        break;
                    case "07":
                        setRegister(upcode[0], memory[i + 3], getValue(upcode[2], memory[i + 1]) < getValue(upcode[1], memory[i + 2]) ? 1 : 0);
                        i += 4;
                        break;
                    case "08":
                        setRegister(upcode[0], memory[i + 3], getValue(upcode[2], memory[i + 1]) == getValue(upcode[1], memory[i + 2]) ? 1 : 0);
                        i += 4;
                        break;
                    case "09":
                        relativeBase += getValue(upcode[2], memory[i + 1]);
                        i += 2;
                        break;

                    case "99":
                        halt = true;
                        break;
                    default:
                        Console.WriteLine($"Halt on upcode {upcode.Substring(3, 2)}, position {i}.");
                        halt = true;
                        break;
                }
                if (halt)
                    break;
            }
        }

        public long getValue(char mode, long parameter)
        {
            return mode switch
            {
                '0' => getFromMemory(parameter),
                '1' => parameter,
                '2' => getFromMemory(parameter + relativeBase),
                _ => throw new Exception($"Wrong parameter mode {mode} on read."),
            };
        }

        public long getFromMemory(long index)
        {
            if (index < 0)
                throw new Exception($"Trying to access memory at negative index.");
            if (index < memory.Length)
                return memory[index];
            if (extraMemory.ContainsKey(index))
                return extraMemory[index];
            return 0;
        }

        public void saveToMemory(long index, long value)
        {
            if (index < 0)
                throw new Exception($"Trying to access memory at negative index.");
            if (index < memory.Length)
            {
                memory[index] = value;
                return;
            }
            if (extraMemory.ContainsKey(index))
            {
                extraMemory[index] = value;
                return;
            }
            extraMemory.Add(index, value);
        }

        public void setRegister(char mode, long parameter, long value)
        {
            switch (mode)
            {
                case '0':
                    saveToMemory(parameter, value);
                    return;
                case '2':
                    saveToMemory(parameter + relativeBase, value);
                    return;
                default:
                    throw new Exception($"Wrong parameter mode {mode} on write.");
            }
        }
    }
}
