using static System.Console;

public class Node
{
    public Node(Node? parent = null, int? data = null, int? id = null)
    {
        Parent = parent;
        Data = data;
        Id = id;
    }

    public int? Id { get; }
    public int? Data { get; }
    public List<Node> Children { get; } = new();
    public Node? Parent { get; }

    public override string ToString()
    {
        var idString = Id.HasValue ? $"{Id}: " : "";

        if (Data != null)
        {
            return idString + Data.ToString()!;
        }
        else
        {
            var str = $"{string.Join(",", Children.Select(_ => _.ToString()))}";
            return idString + (Parent == null ? str : $"[{str}]");
        }
    }

    public bool Compare(Node other)
    {
        return CompareInner(other) ?? true;
    }

    private bool? CompareInner(Node other)
    {
        if (Data == null && other.Data == null)
        {
            int i = 0;
            for (; i < Children.Count; i++)
            {
                if (i >= other.Children.Count) return false;
                var compare = Children[i].CompareInner(other.Children[i]);
                if (compare.HasValue) return compare.Value;
            }

            if (i < other.Children.Count) return true;
            if (Parent == null) return true;
            return null;
        }
        else if (Data != null && other.Data != null)
        {
            if (Data.Value == other.Data.Value) return null;
            return Data.Value < other.Data.Value;
        }
        else
        {
            var left = (Data == null) ? Children : new List<Node> { new Node(data: Data) };
            var right = (other.Data == null) ? other.Children : new List<Node> { new Node(data: other.Data) };

            int i = 0;
            for (; i < left.Count; i++)
            {
                if (i >= right.Count) return false;
                var compare = left[i].CompareInner(right[i]);
                if (compare.HasValue) return compare.Value;
            }

            if (i < right.Count) return true;
            if (Parent == null) return true;
            return null;
        }
    }
}

public class Day13
{
    public async Task Part1()
    {
        var idx = 0;
        var sum = 0;
        await foreach (var (first, second) in ReadInput("Day13.txt"))
        {
            idx++;

            var firstNode = Parse(first);
            var secondNode = Parse(second);
            if (firstNode.Compare(secondNode)) sum += idx;
        }

        WriteLine(sum);
    }

    public async Task Part2()
    {
        List<Node> sorted = new List<Node>
        {
            Parse("[[2]]", 2),
            Parse("[[6]]", 6)
        };

        var idx = 0;
        await foreach (var (first, second) in ReadInput("Day13.txt"))
        {
            var firstNode = Parse(first);
            var secondNode = Parse(second);

            idx = 0;
            while (idx < sorted.Count && sorted[idx].Compare(firstNode)) idx++;
            sorted.Insert(idx, firstNode);

            idx = 0;
            while (idx < sorted.Count && sorted[idx].Compare(secondNode)) idx++;
            sorted.Insert(idx, secondNode);
        }

        var sln = 1;
        idx = 0;
        foreach (var node in sorted)
        {
            idx++;
            if (node.Id.HasValue) sln *= idx;
            //WriteLine(node.ToString());
        }

        WriteLine(sln);
    }

    string _currentNumber = string.Empty;

    private Node Parse(string input, int? id = null)
    {
        var node = new Node(id: id);

        var idx = 0;

        while (idx < input.Length)
        {
            var currentToken = input[idx];
            switch (currentToken)
            {
                case '[':
                    var parent = node;
                    node = new Node(parent);
                    parent.Children.Add(node);
                    break;

                case ']':
                    AddCurrent(node);
                    node = node?.Parent ?? throw new InvalidOperationException();
                    break;

                case ',':
                    AddCurrent(node);
                    break;

                case var num when char.IsNumber(num):
                    _currentNumber += currentToken;
                    break;

                default:
                    throw new InvalidOperationException();
            }

            idx++;
        }

        return node;
    }

    private void AddCurrent(Node node)
    {
        if (!string.IsNullOrEmpty(_currentNumber))
        {
            node.Children.Add(new (parent: node, data: int.Parse(_currentNumber)));
        }

        _currentNumber = string.Empty;
    }

    private async IAsyncEnumerable<(string, string)> ReadInput(string filename)
    {
        var first = string.Empty;
        var second = string.Empty;

        await foreach(var line in System.IO.File.ReadLinesAsync(filename))
        {
            if (string.IsNullOrEmpty(line))
            {
                yield return (first, second);
                first = second = string.Empty;
            }

            if (string.IsNullOrEmpty(first))
            {
                first = line;
            }
            else
            {
                second = line;
            }
        }

        if (!string.IsNullOrEmpty(first)) yield return (first, second);
    }
}