using static System.Console;

public class Day7
{
    public async Task Part1()
    {
        var allDirs = await ParseFileTree();

        WriteLine(allDirs
            .Where(_ => _.Size <= 100000)
            .Sum(_ => _.Size));
    }

    public async Task Part2()
    {
        var allDirs = await ParseFileTree();
        var totalDiskSpace = 70_000_000;
        var totalOccupiedSpace = Dir.Root.Size;
        var freeSpaceNeeded = 30_000_000;

        var smallest = long.MaxValue;

        foreach (var dir in allDirs)
        {
            var freeSpace = totalDiskSpace - totalOccupiedSpace + dir.Size;
            if (freeSpace < freeSpaceNeeded) continue;
            smallest = Math.Min(smallest, dir.Size);
        }

        WriteLine($"{smallest:### ### ###}");
    }

    public async Task<List<Dir>> ParseFileTree()
    {
        bool isListing = false;
        var currentdir = Dir.Root;

        var allDirs = new List<Dir>();

        await foreach(var line in System.IO.File.ReadLinesAsync("Day7.txt"))
        {
            if (string.IsNullOrEmpty(line)) continue;

            var parts = line.Split();

            if (parts[0] == "$")
            {
                var cmd = parts[1];
                var arg = parts.Length > 2 ? parts[2] : "";

                switch (cmd)
                {
                    case "ls":
                        isListing = true;
                        break;

                    case "cd":
                        if (arg == "/")
                        {
                            currentdir = Dir.Root;
                        }
                        else if (arg == "..")
                        {
                            currentdir = currentdir.Parent;
                        }
                        else
                        {
                            if (!currentdir.SubDirs.TryGetValue(arg, out var dir))
                            {
                                dir = new(arg, currentdir);
                                currentdir.SubDirs.Add(arg, dir);
                                allDirs.Add(dir);
                            }
                            currentdir = dir;
                        }
                        break;
                }
            }
            else if (isListing)
            {
                if (long.TryParse(parts[0], out var size))
                {
                    currentdir.Files.Add(parts[1], new() { Name = parts[1], Size = size});
                }
                else if (parts[0] == "dir")
                {
                    var dir = new Dir(parts[1], currentdir);
                    currentdir.SubDirs.Add(parts[1], dir);
                    allDirs.Add(dir);
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected start of line: {parts[0]}");
                }
            }
            else
            {
                throw new InvalidOperationException($"Unknown state");
            }
        }

        return allDirs;
    }

    public record struct File
    {
        public string Name;
        public long Size;
    }

    public class Dir
    {
        private Dir(string name)
        {
            Name = name;
            Parent = this;
        }

        public Dir(string name, Dir parent)
        {
            Name = name;
            Parent = parent;
        }

        public static readonly Dir Root = new("/");

        public string Name { get; }
        public Dir Parent { get; init; }
        public Dictionary<string, Dir> SubDirs { get; } = new();
        public Dictionary<string, File> Files { get; } = new();

        private long _size = -1;
        public long Size
        {
            get
            {
                if (_size >= 0) return _size;

                _size = Files.Sum(_ => _.Value.Size) + SubDirs.Sum(_ => _.Value.Size);
                return _size;
            }
        }
    }
}
