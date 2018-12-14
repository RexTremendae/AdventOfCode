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
            var input = "51589";
            if (args.Length > 0) input = args[0];
            new Program().Run(input, false);
        }

        public void Run(string input, bool debugPrint)
        {
            WriteLine($"Input: {input}");

            Node first = new Node { Value = 3, Index = 0 };
            Node last = new Node { Value = 7, Index = 1 };

            first.Next = last;
            last.Next = first;

            Node firstElf = first;
            Node secondElf = last;

            var current = first;
            if (debugPrint) Print(first, firstElf, secondElf);
            var targetStart = first;

            while (true)
            {
                var newR = (firstElf.Value + secondElf.Value).ToString();
                foreach (var c in newR)
                {
                    Node newN = new Node { Value = c - '0', Next = first, Index = last.Index + 1 };
                    last.Next = newN;
                    last = newN;
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

                bool foundTarget = false;
                while (last.Index - targetStart.Index + 1 >= input.Length)
                {
                    var targetFind = targetStart;
                    foundTarget = true;
                    for (int i = 0; i < input.Length; i++)
                    {
                        if (debugPrint) Write(targetFind.Value);
                        if ((char)targetFind.Value+'0' != input[i])
                        {
                            if (debugPrint) WriteLine(" - NO match");
                            foundTarget = false;
                            break;
                        }
                        if (!foundTarget) break;
                        targetFind = targetFind.Next;
                    }
                    if (foundTarget)
                    {
                        if (debugPrint) WriteLine(" - MATCH");
                        break;
                    }
                    targetStart = targetStart.Next;
                }
                if (foundTarget) break;
            }

            Write("Target found: [");
            var index = targetStart.Index;
            for (int i = 0; i < input.Length; i++)
            {
                Write(targetStart.Value);
                targetStart = targetStart.Next;
            }
            WriteLine($"] {index}");
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