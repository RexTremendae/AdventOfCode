using System.Text;
using static System.Console;

public class Day17
{
    public Task Part1()
    {
        var (_, _, maxHeight) = Solve(streams_puzzle, 2022);
        WriteLine(maxHeight);
        return Task.CompletedTask;
    }

    private (int cycleStart, int cycleLength) FindGraphicCycle(HashSet<(int x, int y)> board, int threshold)
    {
        var lines = new List<int>();

        var foundCycle = false;
        var cycleStart = 0;
        var cycleLength = 0;
        var y = 0;

        while (!foundCycle)
        {
            int line = 0;
            foreach (var x in 0.To(7))
            {
                if (board.Contains((x, y)))
                {
                    line += (1 << x);
                }
            }
            lines.Add(line);

            if (y > threshold)
            {
                //WriteLine($"---- {y} ----");
                foreach (var ry in 0.To(y))
                {
                    if (ry >= 0 && lines[ry] == lines[y])
                    {
                        var a = ry;
                        var b = y;

                        var eqCount = 0;
                        while (a >= 0 && lines[a] == lines[b])
                        {
                            eqCount++;
                            a--;
                            b--;
                        }

                        //WriteLine($"{ry}: {eqCount}");
                        if (eqCount > threshold && ry + eqCount == y)
                        {
                            cycleStart = ry;
                            cycleLength = eqCount;
                            foundCycle = true;
                            break;
                        }
                    }
                }
            }
            y++;
        }

        return (cycleStart, cycleLength);
    }

    private (HashSet<(int x, int y)> board, IEnumerable<(int rock, int height)> sequence, int maxHeight) Solve(string streams, int iterations, bool verbose = false, bool printBoardToFile = false)
    {
        var strmIdx = 0;
        var maxHeight = 0;
        var rockIdx = 0;

        var board = new HashSet<(int x, int y)>();
        var sequence = new List<(int, int)>();
        var current = Array.Empty<string>();
        var currPos = (x: 0, y: 0);

        for (int i = 0; i < iterations; i ++)
        {
            var laidToRest = false;
            while (!laidToRest)
            {
                if (!current.Any())
                {
                    if (verbose) PaintBoard(board, maxHeight);
                    current = Rocks[rockIdx];
                    rockIdx++;
                    if (rockIdx >= Rocks.Length) rockIdx = 0;
                    currPos = (2, maxHeight + 3);
                    if (verbose) WriteLine($"  {currPos}");
                }

                var nextPos = streams[strmIdx] == '<'
                    ? (x: currPos.x-1, currPos.y)
                    : (x: currPos.x+1, currPos.y);
                if (CanMove(board, current, nextPos))
                {
                    currPos = nextPos;
                }
                if (verbose) WriteLine($"{streams[strmIdx]} {currPos}");
                strmIdx++; if (strmIdx >= streams.Length) strmIdx = 0;

                nextPos = (currPos.x, currPos.y-1);
                if (CanMove(board, current, nextPos))
                {
                    currPos = nextPos;
                }
                else
                {
                    laidToRest = true;
                    foreach (var y in 0.To(current.Length))
                    {
                        var yy = currPos.y+(current.Length - y - 1);
                        maxHeight = Math.Max(maxHeight, yy+1);
                        var line = current[y];
                        foreach (var x in 0.To(line.Length))
                        {
                            if (line[x] == '#')
                                board.Add((x+currPos.x, yy));
                        }
                    }
                    current = Array.Empty<string>();
                    sequence.Add((i, maxHeight));
                }
                if (verbose) WriteLine($"🡳 {currPos}");
            }
        }

        if (verbose) PaintBoard(board, maxHeight);
        if (printBoardToFile) PaintBoard(board, maxHeight, toFile: true);

        return (board, sequence, maxHeight);
    }

    private void PaintBoard(HashSet<(int x, int y)> board, int maxHeight, bool toFile = false)
    {
        var builder = new StringBuilder();

        foreach (var y in maxHeight.To(-1))
        {
            var line = "│";
            foreach (var x in 0.To(7))
            {
                line += board.Contains((x, y)) ? '#' : '·';
            }
            line += "│";
            builder.AppendLine(line);
        }

        builder.Append("╰");
        foreach (var x in 0.To(7))
        {
            builder.Append("─");
        }
        builder.AppendLine("╯");

        if (toFile)
        {
            var filename = "output.txt";
            WriteLine("Writing file: " + filename);
            File.WriteAllText(filename, builder.ToString(), Encoding.UTF8);
        }
        else
        {
            Write(builder.ToString());
        }
    }

    private bool CanMove(HashSet<(int x, int y)> board, string[] rock, (int x, int y) pos)
    {
        foreach (var y in 0.To(rock.Length))
        {
            var line = rock[y];
            var yy = pos.y+(rock.Length - y - 1);
            if (yy < 0) return false;
            foreach (var x in 0.To(line.Length))
            {
                var xx = pos.x + x;
                if (xx < 0 || xx >= 7) return false;
                if (line[x] == '#')
                {
                    if (board.Contains((xx, yy)))
                    return false;
                }
            }
        }

        return true;
    }

    string[][] Rocks = new[]
    {
        new[]
        {
            "####"
        },

        new[]
        {
            ".#.",
            "###",
            ".#."
        },

        new[]
        {
            "..#",
            "..#",
            "###"
        },

        new[]
        {
            "#",
            "#",
            "#",
            "#"
        },

        new[]
        {
            "##",
            "##"
        },
    };

    const string streams_example = ">>><<><>><<<>><>>><<<>>><<<><<<>><>><<>>";
    const string streams_puzzle = "><<<>>>><<<>>>><>><<><<>>>><<<<>><<>><<<>>>><<<>>>><<<<>>><>>><<>>><<>>>><<>>>><>><>><<>>>><><<<<>>>><><>>><>>>><>>>><<>>>><><<<>>><<<<>><<<><<<<>>>><<>>><<<>>><><<>>><<<<>>><<<<>>><<<<>>>><<<><<<<>><>><<<>>>><><<<<><><><<>>><><>>><>>>><<<<><>>>><<>><<>>>><<>><<<>>>><<>>>><<>><<<>>>><<<<>>><<><<<<>>><<<>><><<<<>><<>><><<><<><>>>><<><<<>><<>>>><<<><<<>>><<<<>>><<<>>>><<<<><<>><<>>><>><<<>>><<<>>><<>><>><<>>>><<>>><<<<>><<<<>>><>>><>>>><<<>>><<<><>><<<>><<<>>>><<<><<<<><<>><>><<<>><>>><<>>><<<><<><><>>><<<<><<<<><<><<><<<<>><<<>>><<<>><<>><<<>><>>><<<>>><<>><<<<><<<<>><<<><>><<>><><<>><<<>><<>>><<<<><><>><<>>>><<<<><<<>><<<><<<<>>>><<<<><<<>>><<><<<<><<<<>>><<><<<>>>><>>>><<>>><><<>><<<<>><>>><<<>>>><<><<<><<<>><<>>><<<>><<<>>>><<<<>>>><>><><>>><<<>>><<<>>>><<<><<<<>>>><<><<<<>>>><<<<>>><<<>><><<><<<<><><<>>>><<<<>><>>><<<>><<<<><>>><><<>>><>>><<>>>><>><<>>><>>>><><<>>><<<>>>><<<>>><>>><<<>>>><<>>>><<<<>>><>><<<><<><>>>><<<>>><<>><<<<>>><<><<<<>>><<<>>><<<>>><>>><<><<<>><<<<>>>><>>><<<>>>><<>>><<<<><<<>><<<><<<<><<>>>><<>><>>><<>>>><<<<>>>><<<>>><<>>><<<>><<><>>>><<>>>><<>>>><>>><<>>>><<>><<><<>><<<<>><<<><<>>>><<>><<<<>>>><<<<>>>><<<>><>>><<<>>><>>>><<<<><<<>><>>><<<<><<<<><<<>><>>>><>><<<<><<>>>><<<<>>><<<<>><<<>>>><<>>>><>><<>>>><<<<>>><<>>><>>><<<<>><>>><<<<>>><<>>>><<>>><<><>>><<<<>>><<<><<<<>>>><<>>><><>>><<>>>><<>>>><<>><><>>>><<<<>>>><>><>>>><<<>>>><>>>><<>>><<<<>>><<<>><><<<><<><<<<>>><<><<>><<<<>><><><<<>>>><<<<>><><<<>>>><><<<>>>><>>>><<<<>><<<><<>>><<<<>>><<>>>><<<<>>><>><<<><<>><<>>>><>>>><<>>><<>>>><<<>><>><<<>>><<<<>>>><<>><<<<>>><<<<>><<>>><<<>>>><>>><<<<>>>><<>>>><<>>><><<<><>><<>>>><<<>>>><<<>><>><<<>><<><<<>>>><<><<>><<<<><<>><<<><><<<<>>><<>>>><<><<<><<<<>>><<<>>><<>><><<<><<>><<>><<<><>>><<<<>>>><<>>><><<<<>>><<>><<<>>>><><<<<><<<>>><>>><<<<><<<<>>><<<<>><<<<><<><<>><<<><<<>><<<><><<>>><>><<<<>>><<>><<>><<<>>>><<<<>><<<<><>>>><<><>><<<<><<<>>><<<<><<><<<>>><<<><><>>>><<>>>><>><<<<><<<>><<<><>>><<>><<<><<<<>>><>>><<<>><<<<><>><<<><<<>>><>>>><>>><<><<>>><<>><<><<<<>>>><<>><<>>>><>>>><<><<<><<<<>>>><<<>>><<<>>><<<<>><<<>>>><>>><<><<<<>>>><<<>>>><<><<<<>>><<<>><><<<<>>>><<<<>>><>>><<>>><>>>><<<<>>><<<>><<>><<<>><<<>>><>>><><<>><<<>><<><<<>><><<>>><<><<>>><<><<<<>>>><>>>><<>>>><<<<>>><<<<><>><<><<<>><>><<>><<>>><<<>><<>><>>><<<<><<<><<<<><>>>><<><<>><<>>><<<>><<<<>>><<<<><<<>>>><<><<>><<<<><<<<>><>>><<><<<>><<<>><<<>>><><<><><<<<>>>><<<>><<<><<<<>>>><<<>>>><<<<>>>><<<<>><<<>>>><>><<<><><<<><<<<><><<<<><<<<><<<<><<<<>>>><<>>><<<<>>><><<<>>><<<>><<<<>>><<<>><>>>><<<<>>>><<<>><<<>>>><<<>><>>><>>>><<<<>><<<>>><<>><<<<>>><<<<>>>><>>><<<<><>>>><<<>>>><<<>><<<>>><<><<>><<<<>>><<<<>><<<>>><<>><<>>><<<>><<>>>><<<>><<<>>><<>>><<<<>>><>>>><><<<<>>><>>>><<<>>><>><<<<>>><<<>><<<>>>><<>>>><<><<<<>>>><<<<>>>><<>>>><<<><<>>>><<><<>>>><>>><<<<><<>>>><>>>><<<<><<<>>>><<<>><>>><<<<>>><<><<<<>><>>><<<<>>>><><<<>>><<<<>><>><>><<<<>>><<<<>>>><<>>>><<<>>>><>>><<<>>><<<<>><<<><<<>><<<><>><<<>>>><<<><>>>><>><<<<>><>>><><<<<>>>><<>>><<<<>>>><<<>>><<<>><<<>>><><>><<<>><<>>><<<<><<><<<>>>><<<<><<><<<<>>><<>>>><>><<>>><<<<>>><<>>><<>>><<<<>>><>><<<>>><<>>>><<<>><<<<>><<>>>><<<><>>>><<>>><<><<<<><>>><>>><<<>>><>>>><<<<>>>><<<<>>><<<>>>><>>>><<>>>><<<>>><<<<><<>>><<>>>><<<>>>><<<<>><<<<>><<><>><<<<>><>>>><<><<<>>><>><>>><<<<>><<<><<<>>><<<<><>>><<>>>><>>><<<<>>><>><<>><>>><<>>>><<>><<<<>><<>>><<<>>>><<<><<<><<>>><<<<>>>><<>><<<><<>><<>><<<<>>><<<<>>><<<<>>><>>><><<><<<<><<<<>>><<<>>>><<<>>><>>>><<<<>><<><>>>><<<<><<<<><<<<>>><<<<>>>><<<<>>><<<<><<<<>><<<<>><>><<<<>><<<>>><<>><<<>><<<<>>><<<>>><<>>><<<>><<><<<>>><<><<<<>>>><<<<>>><<<>><>><>>>><<>>>><<<<>>><<><<<<>><<<><<<<>>>><><<<<>>>><<<<>><<>>>><>>>><<>>><<<>>>><<<<>>>><<>><<>>><<<<>>><<><><>>><>>>><<<<>>><>>>><<<<>><<<<><<<>><<<<>><<>>><><<><<>><<<<>>>><<<>><<>><<<>>>><>>><<<>>>><><>>>><<<>>>><<><<<<><<<<>>>><>>>><<<>>><>>>><<<<>><>><>><<<<><<>>><<<<>>>><<><<<>>>><<>><<>>><<<><<<><<<<><><<>>><<>><<<>>>><<<<><>>><><><><<<>>>><<<><<<<><>><><<<<><<<>>>><<><>><<>>><<<<>>>><<>><<<>><<<<>><<<<><<>>><>><<<<>>>><>>><><>><>><<>>><>><<<>>><<<<>>>><<><<<<>>>><<<<>>>><><<>>>><<>>>><>><<>><<<<>>>><>><<<<>>><<>><<<>>><<<<><<><<>>>><><>>>><<<<>>><>><<<<>><<<>><<<>>><<<<>>><<>><<<>>><<<<>>><<<<><<>>>><><<>>>><<<><<<<>>>><>><<<<>><<<<>>><<>><<<<>>>><<>>><>>><<<>>><<<<>>><<<<><<<>><>>>><><<><<><<><<>>><<<>>>><<<<>><<<<>>>><<>>>><>>><>>><>><<><<<>>>><>>><<<>><>>>><>>><>>>><<<<>><>>><<<<>>>><><<<><<<>>><<<>>>><<<<>><>><<<<>>><<<<><<>><<><<><<>>><<>>>><<<<>>>><<><<><<<>><<<>><<>>><<<><><>>>><>>><<<<>>><<>><<>>><<<<>>><<<><<>><<<>><<<<>>>><>>>><<><>>>><<<<>>>><<<<>>><<>>><<>>><><>><><<<<><<>>>><<><>><<<><<<<>><<>><<>>><><>>><<><<<<>>>><>>>><><<<><<<>><<<>>><>>>><<<<>><<>>>><>>>><>><<>>><<<><<>>>><>><>><><<<><>><<>>><<<><>>><<<>>>><<<><<>>><<<>><<<>><<>><<<<><<<<>><>><<<>>>><><<<>>>><<<<>>>><<<<>>><<<><<<<>>>><<><<<>>><<<>>><>>>><>>><<<<><<<<>>><><<>>>><<<><<>><<<<>><<>>>><<><<<<><<><<>>>><<<>>><>>><>>>><<<>>>><<<>><><<<>><<<>><<><<>>><><<<<>>>><>>><>>><<<><<>>>><<<>>>><<>><>>>><<<<>>><><><<<<><<<<>>>><<<>><>>>><>>><<<<>>><<<<>>><><<>>><<>>><<<<><<>>><>><<<>>><<<><<<<>>>><><<<><<><><<<>>><<>>><<<<><<<>>><<<><>><<><<>>><<<<>>>><>>>><<><<<>><>>>><<><<>>>><<<<><>>><>>>><<>>>><<<<>>><<<<>>><<><<><<<<>><<>>><<<<>><<<<>><<<>>>><<<<>>><<>>>><<<>>><<<><<<<>>><<<>>><>><<<>><<>>>><>>>><<<<>>><>>>><<<>>>><<<<>>>><<<>><<>><<<<><<<<><<<<><<<>>><<><<<<>>>><<><>><<<<>>><<>><><<<>>><<<><>>><<<<><<>>>><<<>>>><><><<<>><<<<>>><<>><<>>><<<>><<<>><<>>>><<<><><<><<<>><><>><<<>><<>>>><<><<<<>><<><<<<>><>>><<>>>><<>><>>>><<<>>><<>>><>>>><<>>><<<>>><><<>><>>><<<<>>>><<<>>><<<<>>>><<>>><<<<><<<>>>><>><<<><>>><<>><<<>>><<<><<<<>><<>>><<>><><<>><<<>><<<>>>><<<<>><><<<<><>>>><<>>>><<><<>><<>><<<><<>>>><<<>><<>>><<<>><>>>><<<>><<<<>><<>><<<<>>>><><<>>><<><>>><<<>>>><<>>><>>><<<<><<<>>><<>><<>><<>><<<>>>><<<>><<>>><<<<>>>><><<<>>>><<<<>><>>><<>><<<<><<<>>>><>><>><<<>><>>><<<>>><<<<>><<>>>><><<<>>>><<<>><<<>>>><<>>>><<>><<<>>>><>>>><>>><>>>><><>>>><<<<>><<<>>>><>><<<>>><<<><<<<><<<><<<><<<<>>>><<>>>><><<<>><<>>>><<><<<<>>>><>>><><<<<>>><<<>><<<<>><<<<>>>><<<>>><>><<<<>>>><<>><<><<>>><<<<>>>><<<<><<>><<<>><<<<>><<>>><><<>>><>>><<<<>>><<><<>>><<<<>><><<<>><<<<><<>><<><<<>><<<<>>><<<<>>><<<<><<>><>>><<>>><>>>><>><<<<>><<<<>><<<>><>>>><>><<<>>>><<<<>>><<<>>>><<><><><<<<>>>><<<>>><<<<>>>><<<<>><<>><<<>>>><<>><<><<>><<<<>>><<<>>><<<>>>><<<><<><>>><<<>>><<<>><<<><<>><<<<>><>><<<>><<<<>>><><<<><>><><<<<>>>><<>>>><<<>>><<<<>>><<<<><>><<>>>><>>><<<>>>><><<>>><<<>>>><<<>>>><<<<>>>><<<<>>><<<<>>>><<><<<<><<<>><>>>><<<<><><<>>><<>>>><<<<>><<<<>>><<<><><><>><<<>>><<<<><>><<<>>>><<<>><>><<<<>>><<><>>>><><<<><>><<<>>><<<<>><<>>><>>><<>><<>>><>>><<<><><<<<><<<><<><<<><<>><>>><>><<<<><<<>><>><<><<>>>><>>><>>>><>>>><<<<>>><>>>><<>>><<>>>><<<>><<<<>><<><<>><<<<>><<><<<>>><>><<>><<<<>><>><><<<>>>><<<>>><<>>>><>><<<<>><>>>><<>>><<>>><<<>><<<>>><<<<><<><<>><<<<>>>><<<<>><<>><>>>><<<<>>><<<<>>><<<><<<<>>>><<<<>><<<<>><>>><<<<>>><<<><<<<>>><<<<>><<<<>>>><<<<>>>><<<<>>>><<><>>>><<>>><<><>><<>>>><<><>><<>>>><<>>>><<>>>><<<<>><><>>>><<>>><<>>>><<<>>>><<<<>><<>><<>><<<><<<>>>><<<><>>><<<<>><<<<>>>><<<<>>>><<<><<<<><<<<>>>><>>>><>>>><<<<>><><<<><<<<>>><<><<<>><<<>>>><<<<>>>><<<>><<>>>><<>>>><<<<>><<<<>>><<<>>><<<>><<<>><>>><<<>>>><<<<>>>><<<>>>><>>><<<<>>>><<<<>>><<<>>><>><<<>><<<>><<<<>>><<<>>>><<><<><<<>>><<>>>><<<>>><<<>>><<>><<<<>>>><<><<><<>>><>>>><<>>>><<>>>><<<<>>>><<<<><<<<>>><>>>><<><>>><><<<><<<>><>><>>><<<>><<<<>>>><<<><><>>>><<>>><<>>>><<<>>>><><>>>><<<>><<<><<<>>>><<>>>><<>>>><<><<>>><>><<<>>><<<<>><<<<><><<<<>>>><<><>><<<>>>><><<<<>><>>>><<<>>><<<>><>><<<>><<><<<>>><<<>>><<><<>>><<<><<<>>>><<>>>><><<<<><<<<>>><><<<>>><<>>><<>><>><>>><<>>><<<>>>><<><<>>>><<<<>><<<><>><<<<><<<><><<>><<<<><<<>><<<<>>><<<<><<<<>>>><<<>><><>><<><<>>>><<><<<<>><<<>>><<<><<<>>>><<>><<<>>><>>><<>><<><<<<><>><<<<>><>><<<<><<<<><<<><>>>><<>>>><>><<>>>><<>><<>>><>>><<<>>>><<>>>><>><>>><<><<<>>><<><<><><<<<><>><<<>>><<>><<<>>><<<<>>><<<<>>><<<><><<<>>>><<<>>>><>><<>>>><>><>>>><<<>><<<<>>><<>>><<<>>><<<<>>><<<<><<>><><<<<>>>><<>><<<<>><<<><>>><<><>><>>>><<>><<<<>><<<>>><<<>>><<<<><<<<>><<<<>>>><<<>>><<><<<><<<<>><><<>><<<<>>><<><>>><<<<>>><<>><<>>>><<<<>>>><<<<>>>><>>>><<>>>><>>>><<><<><>>>><<><<<<>>>><<<><<<>>>><<>>>><<<<>>>><<<>>>><<<<>>><>>><<<><<<>><<>><<<<>><<>>><<>>><<>>><<<<><>>>><<>><<><<<<>>><<<<>>><<<<><<<<>>><<<>>>><>>>><<>>>><>>>><<>><<>>>><<<><<<<><>><<<<><<>>>><>>>><<<<>>><<>><<<<>>>><<>>><<>>>><<<<>>><<<<>>><<<<>><<<<>>>><<><<<<>><<<<>>>><<><><<<<><<<>>><<<<><<<>>><<<>><<<><<>>>><<<<>><<<<><<<<><<<<>>>><<><<>>>><<>><<<>>><<<>>>><<<>><<<>>><<>><>><<<<><<<<>><<<<><<>>>><<<<>><<<<>>>><<><<<<>>>><<><<<<><<>>><<><<<>>><<><<<<>>><<>><<<<><<<<><<<<><<><<<<>>>><<<<><<>>>><<<<>>>><>><<<<>>><>>><<<>>>><>>>><>>>><<<><>>><>><<<<>>>><<<<>><<<>>>><<<<>><<<<>>>><<<<>>>><><<<<>>>><<<>><<>>>><>>>><>>>><><<>>>><<><><<<><>>>><>>>><<<><<<>>>><<<<>>>><<<>>><<<<><<<><<>>>><<>>>><<>>><<<>><>><<<><<<<><<>>>><<<>><<>><>>>><>>>><<>>>><<<>>><>>><>>><<>><<><<<>><<>><<<<>>>><<>><<>>>><<<>><<<<>>><<<><<<<><<<<>>><>><<>><><<>><<<>><<<<>>>><<<><<>><>><<<><<>><<<<>>><<<>><<<<><<>>>><<>><<<<><<>>>><<<><>>><<><<<<>><<<<>><<>><<<<>>>><<><<<<>>><<<><<<>><<>>><<<<>>>><<><>>><<<>>><<>>><<<<>>><<<>><<<<>>><<<>>>><>>>><<>><<<<>><<>>>><<<<>>>><<>>>><<<<>><<>>>><>>>><>>><<<<>>><<<<><<<>>><<>><<<<>>><>><<<<><<<<><>>>><<<<>>>><>>>><<<<>>><<>><<<>>><<<>>><<<>>>><>>>><<<>>><<<<><<>>><<<<>>><<<<><<<<>>>><>><>>>><>>><>><<<<>>>><<>><<<>>>><<>><<>><<<<>><<<>>><<>>>><<<><<<>>><<<<>><<<<><>>>><<<<>><<<<><><<>>><<<>>>><<<<>>>><<<>>>><<>>>><<>><>><>>><<>><<<<>>>><<<>><<>><<<>>>><<<>><<<>><<<<>>><<<><<>><<<<>>>><<<<>><>>><<<>><<><<<>>><<<>>>><>>>><<<>>>><<<>><<>>>><>>>><><<<<>><<>><><>><<<<>>>><<>><<<<>>><<>><>>><<<>>><<<><<<>>><<<<>>><<>>>><<<>><<>><<>>><<<>>><>><<>><>><<<>>>><<<<>><>>><<>>><<<<>><<>><>>>><>><<<>>><<>>>><>>><><<>>>><<><<<<>>>><<<>>>><<<<><<>>><<<<><><<>>><>><>>>><<>><<<>>><<>>><<>>><<<<>><>>>><<<<>><<<<><<>>>><<<><<<<>>>><<>><<>>>><>>><<<><<<>>><<<<>>><<>>><<<<>><<<><<<<><<<>><<<<>><<>>><<<<><<<<><>>>><<<<>><<><>><<>><<>>>><><<<>>><<><>>>><<<><<<>><<<><<<>><<<<>>><<<>>>><<<>>><<>><<<";
}
