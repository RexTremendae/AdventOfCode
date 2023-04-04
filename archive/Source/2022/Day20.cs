public class Day20
{
    private bool _debug = false;

    public async Task Part1()
    {
        checked
        {
        var (node, len) = await ReadData("Day20.txt");

        var orderedNodes = new List<Node>();
        var zeroNode = Node.NullNode;
        foreach (var _ in Enumerable.Range(0, len))
        {
            orderedNodes.Add(node);
            if (node == 0) zeroNode = node;
            node = node.Next;
        }

        if (zeroNode == Node.NullNode) throw new InvalidOperationException();

        PrintData(node, len);

        foreach (var current in orderedNodes)
        {
            WriteLine($"{current}: ");
            var sign = long.Sign(current);
            var steps = sign * (long.Abs(current) % (len-1));
            if (steps == 0)
            {
                PrintData(node, len);
                continue;
            }

            var newPos = current;
            for (var l = 0L; l < long.Abs(steps); l++)
            {
                if (sign > 0)
                {
                    newPos = newPos.Next;
                }
                else
                {
                    newPos = newPos.Prev;
                }
            }

            var oldBefore = current.Prev;
            var oldAfter = current.Next;

            var before = sign > 0 ? newPos : newPos.Prev;
            var after = before.Next;

            WriteLine($"  oldBefore: {oldBefore}");
            WriteLine($"  oldAfter: {oldAfter}");
            WriteLine($"  before: {before}");
            WriteLine($"  after: {after}");

            before.Next = current;
            after.Prev = current;
            current.Next = after;
            current.Prev = before;

            oldBefore.Next = oldAfter;
            oldAfter.Prev = oldBefore;

            PrintData(node, len);
        }

        var sum = 0L;
        {
        node = zeroNode.Next;
        var count = 1;
        while (node != zeroNode)
        {
            node = node.Next;
            count++;
        }

        if (len != count)
        {
            // Reordering has changed length of list...
            throw new InvalidOperationException();
        }

        var steps = 1000 % len;
        foreach (var _ in Enumerable.Range(0, 3))
        {
            foreach (var __ in Enumerable.Range(0, steps))
            {
                node = node.Next;
            }
            WriteLine(node);
            sum += node;
        }
        }
        WriteLine("======");
        Console.WriteLine(sum);
        }
    }

    public async Task Part2()
    {
        checked
        {
        var (node, len) = await ReadData("Day20.txt", 811589153);

        var orderedNodes = new List<Node>();
        var zeroNode = Node.NullNode;
        foreach (var _ in Enumerable.Range(0, len))
        {
            orderedNodes.Add(node);
            if (node == 0) zeroNode = node;
            node = node.Next;
        }

        if (zeroNode == Node.NullNode) throw new InvalidOperationException();

        PrintData(node, len);

        foreach (var _ in Enumerable.Range(0, 10))
        foreach (var current in orderedNodes)
        {
            WriteLine($"{current}: ");
            var sign = long.Sign(current);
            var steps = long.Abs(current) % (len-1);
            if (steps == 0)
            {
                PrintData(node, len);
                continue;
            }

            if (steps > len/2)
            {
                steps = len - steps - 1;
                sign = sign * -1;
            }

            var newPos = current;
            for (var l = 0L; l < steps; l++)
            {
                if (sign > 0)
                {
                    newPos = newPos.Next;
                }
                else
                {
                    newPos = newPos.Prev;
                }
            }

            var oldBefore = current.Prev;
            var oldAfter = current.Next;

            var before = sign > 0 ? newPos : newPos.Prev;
            var after = before.Next;

            WriteLine($"  oldBefore: {oldBefore}");
            WriteLine($"  oldAfter: {oldAfter}");
            WriteLine($"  before: {before}");
            WriteLine($"  after: {after}");

            before.Next = current;
            after.Prev = current;
            current.Next = after;
            current.Prev = before;

            oldBefore.Next = oldAfter;
            oldAfter.Prev = oldBefore;

            PrintData(node, len);
        }

        var sum = 0L;
        {
        node = zeroNode.Next;
        var count = 1;
        while (node != zeroNode)
        {
            node = node.Next;
            count++;
        }

        if (len != count)
        {
            // Reordering has changed length of list...
            throw new InvalidOperationException();
        }

        var steps = 1000 % len;
        foreach (var _ in Enumerable.Range(0, 3))
        {
            foreach (var __ in Enumerable.Range(0, steps))
            {
                node = node.Next;
            }
            WriteLine(node);
            sum += node;
        }
        }
        WriteLine("======");
        Console.WriteLine(sum);
        }
    }

    public async Task<(Node node, int length)> ReadData(string filePath, long multiplicator = 1)
    {
        var dataNode = Node.NullNode;
        var length = 0;

        foreach (var line in await File.ReadAllLinesAsync(filePath))
        {
            if (string.IsNullOrEmpty(line)) continue;
            var data = long.Parse(line) * multiplicator;
            if (dataNode == Node.NullNode)
            {
                dataNode = new Node(data);
                dataNode.Next = dataNode;
                dataNode.Prev = dataNode;
            }
            else
            {
                var newNode = new Node(data);
                var afterNewNode = dataNode.Next;

                newNode.Next = afterNewNode;
                afterNewNode.Prev = newNode;
                dataNode.Next = newNode;
                newNode.Prev = dataNode;
                dataNode = newNode;
            }
            length++;
        }

        return (dataNode.Next, length);
    }

    public void PrintData(Node node, int len)
    {
        foreach (var _ in Enumerable.Range(0, len))
        {
            Write($"{node} ");
            node = node.Next;
        }

        WriteLine();
    }

    public void Write(object? data = null)
    {
        if (_debug) Console.Write(data);
    }

    public void WriteLine(object? data = null)
    {
        if (_debug) Console.WriteLine(data);
    }
}

public class Node
{
    public long Data { get; }

    public Node Prev { get; set; } = NullNode;
    public Node Next { get; set; } = NullNode;

    public static readonly Node NullNode = new Node(0);

    public Node(long data)
    {
        Data = data;
        Next = NullNode;
        Prev = NullNode;
    }

    public static implicit operator long(Node n) => n.Data;
    public override string ToString() => Data.ToString();
}
