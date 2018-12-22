using System.Collections.Generic;

using static System.Console;

namespace Day22
{
    public enum ErosionLevel
    {
        Rocky,
        Wet,
        Narrow
    }

    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            // Sample
            //long depth = 510;
            //var target = (x: (long)10, y: (long)10);

            // Puzzle input
            long depth = 4002;
            var target = (x: (long)5, y: (long)746);

            var data = new List<long[]>();
            var erosionLevel = new Dictionary<ErosionLevel, char>
            {
                { ErosionLevel.Rocky,  '.' },
                { ErosionLevel.Wet,    '=' },
                { ErosionLevel.Narrow, '|' }
            };

            long risklevel = 0;

            checked {
            for (int y = 0; y <= target.y; y++)
            {
                data.Add(new long[target.x+1]);
                for (int x = 0; x <= target.x; x++)
                {
                    if ((x == 0 && y == 0))
                    {
                        Write("M");
                        continue;
                    }
                    if (x == target.x && y == target.y)
                    {
                        Write("T");
                        continue;
                    }
                    if (y == 0) data[y][x] = (x * 16807 + depth) % 20183;
                    else if (x == 0) data[y][x] = (y * 48271 + depth) % 20183;
                    else data[y][x] = (data[y-1][x] * data[y][x-1] + depth) % 20183;

                    var erosion = (ErosionLevel)(data[y][x] % 3);
                    Write(erosionLevel[erosion]);

                    risklevel += (long)erosion;
                }
                WriteLine();
            }}

            WriteLine(risklevel);
        }
    }
}