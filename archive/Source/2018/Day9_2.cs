using System;
using System.Linq;

namespace Day9_2
{
    public class Marble
    {
        public Marble Next { get; set; }
        public Marble Prev { get; set; }
        public long Value { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            int playerCount = 3;
            long marbleMaxValue = 10;

            if (args.Length > 0)
            {
                int.TryParse(args[0], out playerCount);
            }
            if (args.Length > 1)
            {
                long.TryParse(args[1], out marbleMaxValue);
            }

            new Program().Run(playerCount, marbleMaxValue);
        }

        public void Run(int playerCount, long marbleMaxValue)
        {
            Console.WriteLine($"Playing marble game with {playerCount} players and {marbleMaxValue} as max marble value.");
            var nullMarble = new Marble
            {
                Value = 0
            };

            nullMarble.Next = nullMarble;
            nullMarble.Prev = nullMarble;

            Marble current = nullMarble;
            Marble newMarble = nullMarble;
            int counter = 0;
            var scores = new long[playerCount];
            var currentPlayer = 0;

            //Console.WriteLine("[-] (0)");
            while (newMarble.Value < marbleMaxValue)
            {
                newMarble = new Marble { Value = newMarble.Value + 1 };

                if (newMarble.Value % 23 == 0)
                {
                    for (int i = 0; i < 7; i++)
                        current = current.Prev;

                    scores[currentPlayer] += newMarble.Value + current.Value;

                    current.Prev.Next = current.Next;
                    current.Next.Prev = current.Prev;
                    current = current.Next;
                }
                else
                {
                    current = current.Next.Next;

                    newMarble.Next = current;
                    newMarble.Prev = current.Prev;
                    if (current.Next != current)
                    {
                        current.Prev.Next = newMarble;
                    }
                    else
                    {
                        current.Next = newMarble;
                    }
                    current.Prev = newMarble;

                    current = newMarble;
                }

                //PrintRound(currentPlayer+1, current, nullMarble);
                currentPlayer++;
                if (currentPlayer >= playerCount) currentPlayer = 0;
                counter++;
            }

            Console.WriteLine();
            Console.WriteLine($"Max score: {scores.Max()}");
        }

        private void PrintRound(int player, Marble current, Marble nullMarble)
        {
            Console.Write($"[{player}] ");
            Marble m = nullMarble;
            do
            {
                var val = m.Value.ToString();
                if (m == current) val = $"({val})";

                Console.Write(val + " ");
                m = m.Next;
            } while (m != nullMarble);

            Console.WriteLine();
        }
    }
}