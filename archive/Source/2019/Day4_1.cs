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

            for (int i = 1; i < s.Length; i++)
            {
                if (s[i] < s[i-1]) return false;
                if (s[i] == s[i-1]) twoConsecutive = true;
            }

            return twoConsecutive;
        }
    }
}