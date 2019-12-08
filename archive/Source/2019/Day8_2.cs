using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;

using static System.Console;

namespace Day8
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            var width = 25;
            var height = 6;
            var offset = 0;
            var size = width*height;

            var data = File.ReadAllText("Day8.txt");

            int layerIndex = 0;
            var layers = new List<List<char>>();
            while (offset+size <= data.Length)
            {
                var span = data.Slice(offset, size);
                layers.Add(span);
                offset += size;
                layerIndex ++;
            }

            WriteLine();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pos = y*width + x;
                    var chr = '2';

                    //layers.Reverse();
                    foreach (var l in layers)
                    {
                        if (l[pos] != '2')
                        {
                            chr = l[pos];
                            break;
                        }
                    }
                    
                    if (chr == '2') throw new InvalidOperationException("Encountered transparant pixel");

                    System.Console.ForegroundColor = chr == '1' ? ConsoleColor.Yellow : ConsoleColor.DarkGray;
                    Write('█');
                }

                WriteLine();
            }

            WriteLine();
            Console.ResetColor();
        }
    }

    public static class Extensions
    {
        public static List<char> Slice(this string data, int start, int length)
        {
            return data.Skip(start).Take(length).ToList();
        }
    }
}