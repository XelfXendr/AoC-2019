using System;
using System.Collections.Generic;
using System.Linq;
namespace Day_4
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "138241-674034";
            int lower = int.Parse(input.Substring(0, 6));
            int upper = int.Parse(input.Substring(7, 6));

            int count = 0;

            for(int pass = lower; pass <= upper; pass++)
            {
                byte[] digits = pass.ToString().Select(c => byte.Parse(c.ToString())).ToArray();
                if (digits[0] > digits[1] || digits[1] > digits[2] || digits[2] > digits[3] || digits[3] > digits[4] || digits[4] > digits[5])
                    continue;
                bool doubles = false;
                for(int d = 0; d < 5; d++)
                    if (digits[d] == digits[d + 1] && digits.Count(x => x == digits[d]) == 2)
                    {
                        doubles = true;
                        break;
                    }
                if(doubles)
                    count++;
            }

            Console.WriteLine(count);
        }
    }
}
