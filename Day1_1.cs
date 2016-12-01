using System;

namespace Day1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Program().Run();
        }

        public void Run()
        {
            char[] directions = {'U','R','D','L'};
            int dir = 0;
            int xDist = 0;
            int yDist = 0;

            while (true)
            {
                var input = Console.ReadLine();
                var split = input.Split(',');

                foreach(var s in split)
                {
                    var trimmed = s.Trim();
                    if (string.IsNullOrEmpty(trimmed)) continue;

                    char turn = trimmed[0];
                    int dist = int.Parse(trimmed.Substring(1, trimmed.Length-1));

                    if (trimmed[0] == 'L')
                    {
                        dir--;
                        if (dir < 0)
                        {
                            dir = 3;
                        }
                    }
                    else
                    {
                        dir++;
                        if (dir > 3)
                        {
                            dir = 0;
                        }
                    }

                    //Console.WriteLine($"Going {directions[dir]} {dist} steps");
                    switch(directions[dir])
                    {
                        case 'U':
                            yDist += dist;
                            break;

                        case 'D':
                            yDist -= dist;
                            break;

                        case 'L':
                            xDist -= dist;
                            break;

                        case 'R':
                            xDist += dist;
                            break;

                        default:
                            break;
                    }
                } // foreach

                WriteSum(xDist, yDist);
            } // while (true)
        }

        private void WriteSum(int xDist, int yDist)
        {
            if (xDist < 0) xDist *= -1;
            if (yDist < 0) yDist *= -1;

            Console.WriteLine(xDist + yDist);
        }
    }
}