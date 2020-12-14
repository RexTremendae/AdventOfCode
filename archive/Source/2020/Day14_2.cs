using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace _2020_Day14
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Program().Run();
        }

        public void Run()
        {
            var mask = "";
            var addr = 0L;
            var arg = 0L;
            var memoryData = new Dictionary<long, long>();
            var maskData = new List<Dictionary<long, bool>>();

            foreach (var line in System.IO.File.ReadAllLines("Day14.txt"))
            {
                if (line.StartsWith("mask"))
                {
                    mask = line[7..];
                    //WriteLine();
                    //WriteLine($"New mask: {mask}");
                    maskData = GetCombinations(mask);
                }
                else
                {
                    var parts = line.Split('=').Select(p => p.Trim()).ToArray();
                    addr = int.Parse(parts[0][4..^1]);
                    arg = int.Parse(parts[1]);

                    //WriteLine();
                    //WriteLine($"Apply [{addr}]: {arg}");
                    Apply(mask, maskData, memoryData, addr, arg);
                }
            }

            WriteLine(memoryData.Sum(m => m.Value));
        }

        public List<Dictionary<long, bool>> GetCombinations(string mask)
        {
            var debug = false;

            var bVal = 1L;
            var bVals = new List<Dictionary<long, bool>>();
            for (var idx = mask.Length-1; idx >= 0; idx--, bVal <<= 1)
            {
                if (mask[idx] != 'X') continue;

                if (!bVals.Any())
                {
                    bVals.Add(new Dictionary<long, bool>{{bVal, true}});
                    bVals.Add(new Dictionary<long, bool>{{bVal, false}});
                }
                else
                {
                    var copy = bVals.ToList();
                    foreach (var dict in copy)
                    {
                        bVals.Remove(dict);

                        var clone1 = dict.Clone();
                        clone1.Add(bVal, true);

                        var clone2 = dict.Clone();
                        clone2.Add(bVal, false);

                        bVals.Add(clone1);
                        bVals.Add(clone2);
                    }
                }
            }

            if (debug)
            {
                foreach (var dict in bVals)
                {
                    foreach ((var key, var value) in dict)
                    {
                        Write($"([{key}]: {(value ? 1 : 0).ToString()}) ");
                    }
                    WriteLine();
                }
            }

            return bVals;
        }

        public void Apply(string mask, List<Dictionary<long, bool>> maskData, Dictionary<long, long> memoryData, long addr, long arg)
        {
            bool debug = false;

            foreach (var maskDict in maskData)
            {
                var address = 0L;
                var bVal = 1L;
                for (int i = 35; i >= 0; i--, bVal <<= 1)
                {
                    if (mask[i] == 'X')
                    {
                        var maskBit = maskDict[bVal];
                        if (debug) WriteLine($"{bVal}: mask X ({(maskBit ? 1 : 0).ToString()})");
                        if (maskBit) address += bVal;
                    }
                    else if (mask[i] == '1')
                    {
                        if (debug) WriteLine($"{bVal}: mask 1");
                        address += bVal;
                    }
                    else
                    {
                        if (debug) WriteLine($"{bVal}: address ({((addr & bVal) > 0 ? 1 : 0).ToString()})");
                        address += addr & bVal;
                    }
                }

                if (debug)
                {
                    foreach ((var key, var value) in maskDict)
                    {
                        Write($"([{key}]: {(value ? 1 : 0).ToString()}) ");
                    }
                    WriteLine(" => " + address);
                }

                memoryData[address] = arg;
            }
        }
    }

    public static class Extensions
    {
        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            var clone = new Dictionary<TKey, TValue>();
            foreach ((var key, var value) in source)
            {
                clone.Add(key, value);
            }

            return clone;
        }
    }
}
