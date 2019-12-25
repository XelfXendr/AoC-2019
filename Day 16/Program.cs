using System;
using Day_16.Properties;
using System.Linq;
using System.Collections.Generic;

namespace Day_16
{
    class Program
    {
        public static byte[] signal;
        public static sbyte[] pattern;
        static void Main()
        {
            byte[] input = Resources.Input.Select(x => byte.Parse(x.ToString())).ToArray();
            byte[] signal = new byte[input.Length * 10000]; //byte[]
            for (int i = 0; i < signal.Length; i++)
                signal[i] = input[i % input.Length];
            
            int offset = signal[0] * 1000000 + signal[1] * 100000 + signal[2] * 10000 + signal[3] * 1000 + signal[4] * 100 + signal[5] * 10 + signal[6] * 1;
            
            for (int phase = 0; phase < 100; phase++)
            {
                for(int i = signal.Length-2; i >= offset; i--)
                {
                    signal[i] = (byte)((signal[i] + signal[i + 1]) % 10);
                }
            }

            for (int i = 0; i < 8; i++)
                Console.Write(signal[i + offset]);
        }

        public static int UsePattern(int length, int index)
        {
            return signal[index] * pattern[((index + 1) % (4 * length)) / length];
        }
    }
}
