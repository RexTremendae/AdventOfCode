using System;
using System.IO;
using System.Collections.Generic;

using static System.Console;

namespace Day23
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            long maxR = 0;
            (long x, long y, long z, long r) maxBot = (0, 0, 0, 0);
            var allBots =  ParseInput("Day23.txt");

            foreach (var bot in allBots)
            {
                //WriteLine($"Found bot ({bot.x}, {bot.y}, {bot.z}) with range {bot.r}");

                if (bot.r > maxR)
                {
                    maxBot = bot;
                    maxR = bot.r;
                }
            }

            //WriteLine("----------------------------");
            //WriteLine($"Using bot ({maxBot.x}, {maxBot.y}, {maxBot.z}) with range {maxBot.r} as base");

            var count = 0;
            foreach (var bot in allBots)
            {
                //Write($"Bot ({bot.x}, {bot.y}, {bot.z}): ");

                if (bot.IsWithinDistanceOf(maxBot))
                {
                    count++;
                    //WriteLine("IN range");
                }
                else
                {
                    //WriteLine("OUT OF range");
                }
            }
            WriteLine(count);
        }

        public IEnumerable<(long x, long y, long z, long r)> ParseInput(string filename)
        {
            foreach (var line in File.ReadAllLines(filename))
            {
                if (string.IsNullOrEmpty(line)) continue;

                var splitIndex = line.IndexOf("r=");
                long r = long.Parse(line.Substring(splitIndex+2));
                var coordSplit = line.Substring(5, splitIndex-8).Split(",");

                var x = long.Parse(coordSplit[0]);
                var y = long.Parse(coordSplit[1]);
                var z = long.Parse(coordSplit[2]);

                yield return (x, y, z, r);
            }
        }
    }

    public static class Extensions
    {
        public static bool IsWithinDistanceOf(this (long x, long y, long z, long r) bot, (long x, long y, long z, long r) botWithR)
        {
            var distance = Math.Abs(botWithR.x - bot.x) +
                           Math.Abs(botWithR.y - bot.y) +
                           Math.Abs(botWithR.z - bot.z);

            return distance <= botWithR.r;
        }
    }
}