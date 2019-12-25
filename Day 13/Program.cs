using System;
using System.Collections.Generic;
using System.Linq;
using Day_13.Properties;
using System.Threading;
namespace Day_13
{
    class Program
    {
        static void Main(string[] args)
        {
            long[] code = Resources.Input.Split(',').Select(x => long.Parse(x)).ToArray();
            code[0] = 2;
            IntcodeComputer game = new IntcodeComputer();
            int ballpos = 0;
            int paddlepos = 0;
            byte outputIndex = 0;
            long[] output = new long[3];

            game.RunCode(code, () =>
            {
                Thread.Sleep(10);
                if (ballpos == paddlepos)
                    return 0;
                return ballpos > paddlepos ? 1 : -1;
                /*if (!Console.KeyAvailable)
                    return 0;
                return Console.ReadKey().Key switch
                {
                    ConsoleKey.A => -1,
                    ConsoleKey.D => 1,
                    _ => 0
                };*/
            }, (long x) =>
            {
                if(outputIndex < 2)
                {
                    output[outputIndex] = x;
                    outputIndex++;
                    return;
                }

                output[2] = x;
                outputIndex = 0;

                if(output[0] == -1 && output[1] == 0)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.Write($"Score: {output[2]}                         ");
                    return;
                }

                Console.SetCursorPosition((int)output[0] * 2, (int)output[1] + 1);
                if (output[2] == 3)
                    paddlepos = (int)output[0];
                else if (output[2] == 4)
                    ballpos = (int)output[0];
                Console.Write(output[2] switch
                {
                    0 => "  ",
                    1 => "██",
                    2 => "▒▒",
                    3 => "==",
                    4 => "()",
                    _ => throw new Exception($"Wrong output {output[2]}")
                });
            });

        }
    }
}
