using static System.Console;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

public class Day9
{
    public async Task Part1()
    {
        var disk = await ReadInput("Day9.txt");

        var backPtr = disk.Count - 1;
        var frontPtr = 0;

        while (disk[frontPtr] != null) frontPtr++;

        while (frontPtr < backPtr)
        {
            if (disk[backPtr] == null)
            {
                backPtr--;
                continue;
            }

            disk[frontPtr] = disk[backPtr];
            disk[backPtr] = null;

            while (disk[frontPtr] != null)
                frontPtr++;
        }

        var chksm = 0L;
        frontPtr = 0;
        while (disk[frontPtr] != null)
        {
            chksm += disk[frontPtr]!.Value*frontPtr;
            frontPtr++;
        }

        WriteLine(chksm);
    }

    private async Task<List<int?>> ReadInput(string filename)
    {
        var data = (await File.ReadAllLinesAsync(filename)).First();

        var disk = new List<int?>();

        var pos = 0L;
        var isFile = true;
        var fileIdx = 0;

        foreach (var ch in data)
        {
            var bytes = (ch - '0');
            for (int i = 0; i < bytes; i++)
            {
                if (isFile)
                {
                    disk.Add(fileIdx);
                }
                else
                {
                    disk.Add(null);
                }

                pos++;
            }

            if (isFile)
            {
                fileIdx ++;
            }

            isFile = !isFile;
        }

        return disk;
    }
}
