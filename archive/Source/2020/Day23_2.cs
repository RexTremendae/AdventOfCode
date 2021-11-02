using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace _2020
{
    public class Day23
    {
        public static void Main()
        {
            new Day23().Run();
        }

        public void Run()
        {
            var current = new Cup(0);

            long highest = long.MinValue;
            long lowest = long.MaxValue;

            var cupCount = 0;

            foreach (var data in new long[] { 6, 4, 3, 7, 1, 9, 2, 5, 8 })
            {
                current = Add(current, data);
                cupCount++;
                highest = Math.Max(data, highest);
                lowest = Math.Min(data, lowest);
            }

            for (long l = 10; l <= 1_000_000; l++)
            {
                current = Add(current, l);
                cupCount++;
                highest = Math.Max(l, highest);
                lowest = Math.Min(l, lowest);
            }

            current = current.Next;

            var start = current.Prev;
            var cupIndex = new Cup[cupCount+1];

            while (start != current)
            {
                cupIndex[current.Data] = current;
                current = current.Next;
            }
            cupIndex[current.Data] = current;

            current = current.Next;

            //PrintSequence(current, true);
            //PrintSequence(current, false);

            //PrintSequence(current);
/*
            for (int i = 1; i < cupIndex.Length; i++)
                Write($"{cupIndex[i].Data} ");
            WriteLine();
*/
            for (long i = 0; i < 10_000_000; i++)
            {
                var pickedUpCup = current.Next;
                var next = current.Next.Next.Next.Next;
                current.Next = next;
                next.Prev = current;

                var pickedUpLabels = new long[3];
                pickedUpLabels[0] = pickedUpCup.Data;
                pickedUpLabels[1] = pickedUpCup.Next.Data;
                pickedUpLabels[2] = pickedUpCup.Next.Next.Data;

                //WriteLine($"Pick up: {pickedUpLabels[0]}, {pickedUpLabels[1]}, {pickedUpLabels[2]}");

                long destLabel = current.Data;
                do
                {
                    destLabel--;
                    if (destLabel < lowest) destLabel = highest;
                } while (pickedUpLabels.Contains(destLabel));

                //WriteLine($"Destination: {destLabel}");

                var destCup = cupIndex[destLabel];
                next = destCup.Next;
                destCup.Next = pickedUpCup;
                pickedUpCup.Prev = destCup;

                var lastPickedUpCup = pickedUpCup.Next.Next;
                lastPickedUpCup.Next = next;
                next.Prev = lastPickedUpCup;

                current = current.Next;

                //PrintSequence(current);
            }

            var _1 = cupIndex[1].Next;
            var _2 = _1.Next;

            WriteLine(_1.Data * _2.Data);
        }

        public void PrintSequence(Cup cup, bool forward = true)
        {
            var start = cup;

            do
            {
                Write($"{cup.Data} ");
                cup = forward ? cup.Next : cup.Prev;
            } while (cup != start);
            WriteLine(cup.Data);
        }

        public Cup Add(Cup current, long data)
        {
            var cup = new Cup(data);
            if (current.Data != 0)
            {
                if (current.Prev == current)
                {
                    current.Next = cup;
                    current.Prev = cup;
                    cup.Prev = current;
                    cup.Next = current;

                    current = cup;
                }
                else
                {
                    current.Next.Prev = cup;
                    var next = current.Next;

                    current.Next = cup;
                    cup.Next = next;
                    cup.Prev = current;
                }
            }
            return cup;
        }
    }

    public class Cup
    {
        public Cup(long data)
        {
            Next = this;
            Prev = this;
            Data = data;
        }

        public Cup Next { get; set; }
        public Cup Prev { get; set; }
        public long Data { get; }
    }
}