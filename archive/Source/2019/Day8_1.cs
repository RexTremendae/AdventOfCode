using System;
using System.Collections.Generic;
using System.IO;

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
            var digitCount = new List<Dictionary<char, int>>();
            while (offset+size <= data.Length)
            {
                var span = data.AsSpan(offset, size);
                offset += size;

                digitCount.Add(new Dictionary<char, int>());
                foreach (var c in span)
                {
                    var count = 0;
                    digitCount[layerIndex].TryGetValue(c, out count);
                    digitCount[layerIndex][c] = count+1;
                }

                layerIndex ++;
            }

            var minZero = int.MaxValue;
            var minLayerIndex = 0;

            for (int idx = 0; idx < digitCount.Count; idx++)
            {
                var current = digitCount[idx]['0'];
                if (current < minZero)
                {
                    minZero = current;
                    minLayerIndex = idx;
                }
            }

            WriteLine(digitCount[minLayerIndex]['1'] * digitCount[minLayerIndex]['2']);
        }
    }
}