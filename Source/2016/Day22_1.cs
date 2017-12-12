using System;
using System.Collections.Generic;
using System.IO;

using static System.Console;

namespace Day22
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            var grid = ReadIndata();

            int viableCount = 0;
            int x = 0, y = 0;
            bool end = false;
            for (;;)
            {
                for (;;)
                {
                    if (!grid.TryGetValue((x, y), out var d))
                    {
                        if (x == 0) end = true;
                        x = 0;
                        break;
                    }

                    int ix = 0, iy = 0;
                    bool iend = false;
                    for (;;)
                    {
                        for (;;)
                        {
                            if (!grid.TryGetValue((ix, iy), out var id))
                            {
                                if (ix == 0) iend = true;
                                ix = 0;
                                break;
                            }

                            if ((x != ix || y != iy) &&
                                (d.Used != 0) &&
                                (d.Used+id.Used <= id.Size))
                            {
                                viableCount++;
                            } 

                            ix++;
                        }
                        iy++;

                        if (iend) break;
                    }

                    x++;
                }
                y++;
                if (end) break;
            }

            WriteLine(viableCount);
        }

        public Dictionary<(int X, int Y), (int Used, int Size)> ReadIndata()
        {
            var grid = new Dictionary<(int,int), (int,int)>();
            using (var reader = new StreamReader("Day22.txt"))
            {
                int count = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line)) break;
                    count++;
                    if (count <= 2) continue;

                    int startIdx = 15;
                    int endIdx = startIdx;
                    while (line[endIdx] != ' ') endIdx++;

                    var xyRaw = line.Substring(startIdx, endIdx - startIdx);
                    var xySplit = xyRaw.Split('-');
                    var x = Convert.ToInt32(xySplit[0].Substring(1));
                    var y = Convert.ToInt32(xySplit[1].Substring(1));

                    startIdx = endIdx+1;
                    while (line[startIdx] == ' ') startIdx++;
                    endIdx = startIdx;
                    while (line[endIdx] != ' ') endIdx++;
                    var sizeRaw = line.Substring(startIdx, endIdx - startIdx);
                    var size = Convert.ToInt32(sizeRaw.Substring(0, sizeRaw.Length-1));

                    startIdx = endIdx+1;
                    while (line[startIdx] == ' ') startIdx++;
                    endIdx = startIdx;
                    while (line[endIdx] != ' ') endIdx++;
                    var usedRaw = line.Substring(startIdx, endIdx - startIdx);
                    var used = Convert.ToInt32(usedRaw.Substring(0, usedRaw.Length-1));

                    grid.Add((x, y), (used, size));
                }
            }

            return grid;
        }
    }
}