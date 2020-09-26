using System;
using System.Collections.Generic;
using System.IO;

using static System.Console;

namespace Day16
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            var originalData = Load("Day16.txt");

            for (int repetitions = 10_000; repetitions < 10_010; repetitions++)
            {
                var data = Repeat(originalData, repetitions);

                Calculate(RawCalculation, 1, data.ToArray(), "Raw");
                Calculate(OptimizedCalculation, 1, data.ToArray(), "Opt");

                WriteLine();
            }
        }

        public void Calculate(Func<int, long[], long[]> calculate, int phases, long[] data, string label)
        {
            var start = DateTime.Now;
            var result = calculate(phases, data);
            var duration = (DateTime.Now - start);

            var str = "";
            for (int i = 0; i < 8; i ++)
            {
                str += result[i].ToString();
            }
            WriteLine($"{label}: {str}  ({(int)duration.TotalSeconds} s)");
        }

        public List<long> Repeat(List<long> inData, int repetitions)
        {
            var outData = new List<long>();
            var dataLen = inData.Count;

            for (int j = 0; j < repetitions; j++)
            {
                for (int k = 0; k < dataLen; k++)
                    outData.Add(inData[k]);
            }

            return outData;
        }

        public long[] OptimizedCalculation(int phases, long[] data)
        {
            for (int i = 0; i < phases; i++)
            {
                data = ExecuteOptimizedPhase(data);
            }

            return data;
        }

        public long[] RawCalculation(int phases, long[] data)
        {
            for (int i = 0; i < phases; i++)
            {
                data = ExecutePhase(data);
            }

            return data;
        }

        public long[] ExecuteOptimizedPhase(long[] input)
        {
            var output = new long[input.Length];
            var patternIdx = 0;
            var oIdx = 0;
            var posCount = 0;
            var pattern = new[] { 0, 1, 0, -1 };

            foreach (var o in input)
            {
                posCount = oIdx == 0 ? 0 : 1;
                patternIdx = oIdx == 0 ? 1 : 0;

                var current = 0L;
                var dIdx = 0;

                while (dIdx < input.Length)
                {
                    current += input[dIdx]*pattern[patternIdx];
                    if (pattern[patternIdx] != 0)
                    {
                        posCount++;
                        dIdx++;
                        if (posCount > oIdx)
                        {
                            patternIdx++;
                            if (patternIdx >= pattern.Length) patternIdx = 0;
                            posCount = 0;
                        }
                    }

                    if (pattern[patternIdx] == 0)
                    {
                        patternIdx ++;
                        dIdx += oIdx+1 - posCount;
                        posCount = 0;
                    }
                }

                var str = (current.ToString());
                output[oIdx] = str[str.Length-1] - '0';

                oIdx++;
            }

            return output;
        }

        public long[] ExecutePhase(long[] input)
        {
            var output = new long[input.Length];
            var patternIdx = 0;
            var dataIdx = 0;
            var posCount = 0;
            var pattern = new[] { 0, 1, 0, -1 };
            int oIdx = 0;

            foreach (var o in input)
            {
                posCount = dataIdx == 0 ? 0 : 1;
                patternIdx = dataIdx == 0 ? 1 : 0;

                var current = 0L;
                foreach (var d in input)
                {
                    current += d*pattern[patternIdx];
                    posCount++;
                    if (posCount > dataIdx)
                    {
                        patternIdx++;
                        if (patternIdx >= pattern.Length) patternIdx = 0;
                        posCount = 0;
                    }
                }

                var str = (current.ToString());
                output[oIdx] = str[str.Length-1] - '0';
                oIdx++;
                dataIdx++;
            }

            return output;
        }

        public List<long> Load(string filename)
        {
            return new List<long> { 1,2,3,4,5,6,7,8 };
            var data = new List<long>();

            foreach (var c in File.ReadAllText(filename))
            {
                if (c >= '0' && c <= '9')
                {
                    data.Add(c - '0');
                }
            }

            return data;
        }
    }
}
