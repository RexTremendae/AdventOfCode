using System;
using System.Collections.Generic;

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
            var visited = new HashSet<Tuple<int, int>>();

            char[] directions = {'U','R','D','L'};
            int dir = 0;
            var current = Tuple.Create(0, 0);
            Tuple<int, int> visitedTwice = null;

            while (visitedTwice == null)
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

                    Console.WriteLine($"Going {directions[dir]} {dist} steps");
                    int dX = 0, dY = 0;

                    switch(directions[dir])
                    {
                        case 'U':
                            dY = 1;
                            break;

                        case 'D':
                            dY = -1;
                            break;

                        case 'L':
                            dX = -1;
                            break;

                        case 'R':
                            dX = 1;
                            break;

                        default:
                            break;
                    }

                    for (int i = 0; i < dist; i++)
                    {
                        Console.WriteLine($"Adding ({current.Item1}, {current.Item2})");
                        visited.Add(current);
                        current = Tuple.Create(current.Item1 + dX, current.Item2 + dY);
                        if (visited.Contains(current))
                        {
                            visitedTwice = current;
                            break;
                        }
                    }

                    if (visitedTwice != null)
                    {
                        break;
                    }
                } // foreach

                WriteSum(current);
            } // while
            Console.WriteLine($"Twice visited distance: {visitedTwice.Item1 + visitedTwice.Item2}");
        }

        private void WriteSum(Tuple<int, int> pos)
        {
            int xDist =  pos.Item1 < 0 ? -pos.Item1 : pos.Item1;
            int yDist =  pos.Item2 < 0 ? -pos.Item2 : pos.Item2;

            Console.WriteLine(xDist + yDist);
        }
    }
}