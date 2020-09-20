using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace Day24
{
    public class Program
    {
        private int _smallestGroupSize;
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            // Sample
            //Solve(1,2,3,4,5,7,8,9,10,11);

            // Challenge
            Solve(1,2,3,5,7,13,17,19,23,29,31,37,41,43,53,59,61,67,71,73,79,83,89,97,101,103,107,109,113);
        }

        int smallestGroupSize;
        public void Solve(params int[] packages)
        {
            var startTime = DateTime.Now;

            var target = packages.Sum() / 3;
            smallestGroupSize = int.MaxValue;
            var smallestGroup = FindSum(packages.OrderByDescending(p => p).ToArray(), new int[] {}, target)
                .GroupBy(s => s.Length)
                .OrderBy(g => g.Key)
                .First();

            var smallestQuantum = long.MaxValue;
            foreach (var list in smallestGroup)
            {
                var quantum = (long)list.First();
                foreach (var itm in list.Skip(1))
                {
                    quantum *= itm;
                }
                if (quantum < smallestQuantum)
                {
                    smallestQuantum = quantum;
                }
            }

            var duration = DateTime.Now - startTime;
            WriteLine($"{smallestQuantum}  [{duration.Minutes.ToString("00")}:{duration.Seconds.ToString("00")}]");
        }

        public IEnumerable<int[]> FindSum(int[] potential, int[] selected, int target)
        {
            var sum = selected.Sum();

            for (int i = 0; i < potential.Length; i++)
            {
                var newSum = potential[i] + sum;
                if (newSum > target)
                {
                    continue;
                }

                var newSelected = CloneAndAdd(selected, potential[i]);

                if (newSum == target)
                {
                    if (newSelected.Length < smallestGroupSize) smallestGroupSize = newSelected.Length;
                    yield return newSelected;
                    continue;
                }

                if (newSelected.Length == smallestGroupSize) continue;

                foreach (var result in FindSum(potential[..i].Union(potential[(i+1)..]).ToArray(), newSelected, target))
                {
                    yield return result;
                }
            }
        }

        public int[] CloneAndAdd(int[] array, int newItem)
        {
            var clone = new int[array.Length+1];

            for (int i = 0; i < array.Length; i++)
            {
                clone[i] = array[i];
            }

            clone[array.Length] = newItem;

            return clone;
        }
    }
}
