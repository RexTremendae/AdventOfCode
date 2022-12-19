using static System.Console;

public class Day9
{
    public async Task Part1()
    {
        WriteLine("Reading indata...");
        WriteLine();

        var data = new List<string>();
        var head = (x: 0, y: 0);
        var tail = (x: 0, y: 0);

        var tailMap = new HashSet<(int, int)>();
        tailMap.Add(tail);

        await foreach(var line in System.IO.File.ReadLinesAsync("Day9.txt"))
        {
            if (string.IsNullOrEmpty(line)) continue;
            var parts = line.Split(" ");
            var delta = parts[0] switch
            {
                "R" => (x:  1, y:  0),
                "L" => (x: -1, y:  0),
                "U" => (x:  0, y:  1),
                "D" => (x:  0, y: -1),
                _ => throw new InvalidOperationException("Invalid input: " + line)
            };
            var multiplier = int.Parse(parts[1]);

            foreach (var i in 0.To(multiplier))
            {
                var lastHead = head;
                head = (x: head.x + delta.x, y: head.y + delta.y);
                var dist = (x: Math.Abs(head.x - tail.x), y: Math.Abs(head.y - tail.y));
                if (dist.x > 1 || dist.y > 1)
                {
                    tail = lastHead;
                    tailMap.Add(tail);
                }
            }
        }

        WriteLine(tailMap.Count);
    }

    public async Task Part2()
    {
        WriteLine("Reading indata...");
        WriteLine();

        var data = new List<string>();
        var knots = new (int x, int y)[10];

        foreach (var i in 0.To(10))
        {
            knots[i] = (0, 0);
        }

        var tailMap = new HashSet<(int, int)>();
        tailMap.Add(knots[0]);

        await foreach(var line in System.IO.File.ReadLinesAsync("Day9.txt"))
        {
            if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;
            var parts = line.Split(" ");
            var delta = parts[0] switch
            {
                "R" => (x:  1, y:  0),
                "L" => (x: -1, y:  0),
                "U" => (x:  0, y:  1),
                "D" => (x:  0, y: -1),
                _ => throw new InvalidOperationException("Invalid input: " + line)
            };
            var multiplier = int.Parse(parts[1]);

            var debug = parts[0] == "L";
debug = false;
            if (debug) WriteLine(line);
            foreach (var x in 0.To(multiplier))
            {
                var from = knots[0];
                var to = (x: from.x + delta.x, y: from.y + delta.y);
                knots[0] = to;
                if (debug) Write($"({from.x},{from.y})-({to.x},{to.y}):");

                foreach (var i in 1.To(10))
                {
                    from = knots[i];
                    var dist = (x: Math.Abs(from.x - to.x), y: Math.Abs(from.y - to.y));
                    if (dist.x <= 1 && dist.y <= 1)
                        break;

                    var dx = Math.Abs(to.x - from.x);
                    var dy = Math.Abs(to.y - from.y);

                    if (debug) Write($" ({from.x},{from.y})-({to.x},{to.y}):");

                    if (dx == 0 && dy != 0)
                    {
                        to = to.y > from.y
                            ? (from.x, from.y+1)
                            : (from.x, from.y-1);
                        if (debug) Write("A");
                    }
                    else if (dy == 0 && dx != 0)
                    {
                        to = to.x > from.x
                            ? (from.x+1, from.y)
                            : (from.x-1, from.y);
                        if (debug) Write("B");
                    }
                    else if (dx > dy)
                    {
                        var toX = to.x > from.x
                            ? from.x+1
                            : from.x-1;
   
                        to = (toX, to.y);
                        if (debug) Write("C");
                    }
                    else if (dy > dx)
                    {
                        var toY = to.y > from.y
                            ? from.y+1
                            : from.y-1;
   
                        to = (to.x, toY);
                        if (debug) Write("D");
                    }
                    else // dx == dy
                    {
                        var toX = to.x > from.x
                            ? from.x+1
                            : from.x-1;

                        var toY = to.y > from.y
                            ? from.y+1
                            : from.y-1;

                        to = (toX, toY);
                        if (debug) Write("E");
                    }

                    knots[i] = to;

                    if (i == 9)
                    {
                        tailMap.Add(to);
                    }
                }
                if (debug)
                {
                    WriteLine();
                    Output(knots);
                }
            }
        }

        WriteLine(tailMap.Count);
    }

    private void Output((int x, int y)[] knots)
    {
        var height = 20;
        var width = 40;

        var lines = new char[height][];
        foreach (var l in 0.To(height))
        {
            lines[l] = new char[width];
            foreach (var m in 0.To(width)) lines[l][m] = '.';
        }

        var offsX = 20;
        var offsY = 10;
        lines[offsY][offsX] = 's';
        for (int a = 9; a >= 0; a--)
        {
            lines[height-(knots[a].y+offsY)][knots[a].x+offsX] =
                a == 0 ? 'H' : (char)('0' + a);
        }

        foreach (var l in 0.To(height))
        {
            WriteLine(lines[l]);
        }
        WriteLine();
    }
}
