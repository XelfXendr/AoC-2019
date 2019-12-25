using System;
using System.Collections.Generic;
using System.Text;

namespace Day_13
{
    public class IntcodeComputer
    {
        long[] memory;
        Dictionary<long, long> extraMemory;
        long relativeBase = 0;

        public void RunCode(long[] code, Func<long> onInput, Action<long> onOutput)
        {
            memory = code;
            extraMemory = new Dictionary<long, long>();
            relativeBase = 0;

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
                        setRegister(upcode[2], memory[i + 1], onInput());
                        i += 2;
                        break;
                    case "04": //output
                        onOutput(getValue(upcode[2], memory[i + 1]));
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
                        return;
                    default:
                        Console.WriteLine($"Halt on upcode {upcode.Substring(3, 2)}, position {i}.");
                        return;
                }
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
