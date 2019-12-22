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
            var data = Load("Day16.txt");

            foreach (var d in data) Write(d);
            WriteLine();
            WriteLine();

            for (int i = 0; i < 100; i++)
            {
                data = ExecutePhase(data);
            }

            var str = "";
            foreach (var d in data)
            {
                str += d.ToString();
                Write(d);
            }
            WriteLine();
            WriteLine();

            WriteLine($"First 8: {str.Substring(0, 8)}");
        }

        public List<long> ExecutePhase(List<long> input)
        {
            var output = new List<long>(input.Count);
            var patternIdx = 0;
            var dataIdx = 0;
            var posCount = 0;
            var pattern = new[] { 0, 1, 0, -1 };

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
                output.Add(str[str.Length-1] - '0');

                dataIdx++;
            }

            return output;
        }

        public List<long> Load(string filename)
        {
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
