using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace Day4
{
    public class Program
    {
        const int Start = 156218;
        const int End = 652527;

        public static void Main(string[] args)
        {
            new Program().Run();
        }

        public void Run()
        {
            int count = 0;
            for (int n = Start; n <= End; n++)
            {
                if (IsCandidate(n)) count++;
            }

            WriteLine(count);
        }

        public bool IsCandidate(int n)
        {
            var s = n.ToString();
            bool twoConsecutive = false;
            var counter = new Dictionary<char, int>();
            for (int i = 0; i < 10; i ++)
                counter[(char)('0' + i)] = 0;

            counter[s[0]]++;
            for (int i = 1; i < s.Length; i++)
            {
                counter[s[i]]++;
                if (s[i] < s[i-1]) return false;
            }

            return counter.Any(x => x.Value == 2);
        }
    }
}