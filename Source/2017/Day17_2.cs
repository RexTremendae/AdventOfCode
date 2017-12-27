using System;
using System.Collections.Generic;
using System.Linq;

using static System.Console;
using static System.Environment;

namespace Day17
{
    public class Program
    {
        static bool Debug = false;

        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            // Small example
            //var goal = 25;
            //var sizes = new[] {20,15,10,5,5};
            
            // Puzzle input
            var goal = 150;
            var sizes = new[] {11,30,47,31,32,36,3,1,5,3,32,36,15,11,46,26,28,1,19,3};

            var solutions = FindSolutions(goal, sizes).ToList();
            int min = solutions.Min(x => x.Length);

            WriteLine($"A total of {solutions.Count(x => x.Length == min)} min solutions was found.");
        }

        private IEnumerable<int[]> FindSolutions(int goal, int[] sizes)
        {
            var q = new Queue<(int[], int[])>();

            var left = Clone(sizes);

            foreach(var s in sizes)
            {
                var tried = new[] {left[0]};
                left = CloneWithout(left, 0);

                DbgEnqueue(tried, left);
                q.Enqueue((tried, left));
            }

            while (q.Any())
            {
                var dq = q.Dequeue();
                var sum = dq.Item1.Sum();

                string s = "";
                foreach (var n in dq.Item1)
                {
                    s += n + ", ";
                }

                s = s.Substring(0, s.Length-2);
                if (Debug) WriteLine($"Testing [{s}]: sum = {sum}");

                if (sum == goal)
                {
                    yield return dq.Item1;
                    continue;
                }

                if (sum > goal) continue;
                if (!dq.Item2.Any()) continue;

                Enqueue(q, dq);
            }
        }

        private void Enqueue(Queue<(int[], int[])> q, (int[], int[]) itm)
        {
            var tried = Clone(itm.Item1);
            var left = Clone(itm.Item2);

            foreach(var s in itm.Item2)
            {
                var clone = CloneWithAdd(tried, left[0]);
                left = CloneWithout(left, 0);

                DbgEnqueue(clone, left);
                q.Enqueue((clone, left));
            }
        }

        private void DbgEnqueue(int[] tried, int[] left)
        {
            if (!Debug) return;

            string s1 = "";
            foreach (var n in tried)
            {
                s1 += n + ", ";
            }
            s1 = s1.Length < 1 ? s1 : s1.Substring(0, s1.Length-2);

            string s2 = "";
            foreach (var n in left)
            {
                s2 += n + ", ";
            }
            s2 = s2.Length < 1 ? s2 : s2.Substring(0, s2.Length-2);

            WriteLine($"Enqueueing [{s1}], [{s2}]");
        }

        private int[] Clone(int[] input)
        {
            var output = new int[input.Length];

            int idx = 0;
            foreach (var n in input)
            {
                output[idx] = input[idx];
                idx++;
            }

            return output;            
        }

        private int[] CloneWithAdd(int[] input, int itemToAdd)
        {
            var output = new int[input.Length+1];

            int idx = 0;
            foreach (var n in input)
            {
                output[idx] = input[idx];
                idx++;
            }

            output[idx] = itemToAdd;
            return output;            
        }

        private int[] CloneWithout(int[] input, int indexToRemove)
        {
            var output = new int[input.Length-1];

            int idxIn = 0, idxOut = 0;
            foreach (var n in input)
            {
                if (idxIn == indexToRemove)
                {
                    idxIn++;
                    continue;
                }

                if (idxOut >= output.Length)
                {
                    WriteLine($"{idxOut} is too large, size: {output.Length}, indexToRemove: {indexToRemove}");
                    Exit(1);
                }

                output[idxOut] = input[idxIn];
                idxIn++;
                idxOut++;
            }

            return output;
        }
    }
}
