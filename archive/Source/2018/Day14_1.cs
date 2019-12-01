using static System.Console;

namespace Day14
{
    public class Node
    {
        public int Index { get; set; }
        public int Value { get; set; }
        public Node Next { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            int input = 9;
            if (args.Length > 0) int.TryParse(args[0], out input);
            new Program().Run(input, false);
        }

        public void Run(int input, bool debugPrint)
        {
            WriteLine($"Input: {input}");

            Node first = new Node { Value = 3, Index = 0 };
            Node last = new Node { Value = 7, Index = 1 };

            first.Next = last;
            last.Next = first;

            Node firstElf = first;
            Node secondElf = last;
            Node target = null;

            var current = first;
            if (debugPrint) Print(first, firstElf, secondElf);

            while (last.Index < input+10)
            {
                var newR = (firstElf.Value + secondElf.Value).ToString();
                foreach (var c in newR)
                {
                    Node newN = new Node { Value = c - '0', Next = first, Index = last.Index + 1 };
                    last.Next = newN;
                    last = newN;

                    if (newN.Index == input) target = newN;
                }

                int end = firstElf.Value + 1;
                for (int f = 0; f < end; f++)
                {
                    firstElf = firstElf.Next;
                }

                end = secondElf.Value + 1;
                for (int f = 0; f < end; f++)
                {
                    secondElf = secondElf.Next;
                }

                if (debugPrint) Print(first, firstElf, secondElf);
            }

            for (int i = 0; i < 10; i++)
            {
                Write(target.Value);
                target = target.Next;
            }
            WriteLine();
        }

        private void Print(Node first, Node firstElf, Node secondElf)
        {
            var current = first;

            do
            {
                if (firstElf == current)
                {
                    Write($"({current.Value})");
                }
                else if (secondElf == current)
                {
                    Write($"[{current.Value}]");
                }
                else
                {
                    Write($" {current.Value} ");
                }

                current = current.Next;
            }
            while (current != first);

            WriteLine();
        }
    }
}