using static System.Console;

public class Day2
{
    public async Task Part1()
    {
        var games = await ReadData("Day2.txt");
        var target = (r: 12, g: 13, b: 14);

        var sum = 0;
        foreach (var game in games)
        {
            var gamePossible = true;
            WriteLine(game.Id);
            foreach (var cubeSet in game.CubeSets)
            {
                var possible = (cubeSet.r <= target.r && cubeSet.g <= target.g && cubeSet.b <= target.b);
                ForegroundColor = possible
                  ? ConsoleColor.Green
                  : ConsoleColor.Red;

                Write($"{cubeSet} ; ");

                ResetColor();
                if (!possible) gamePossible = false;
            }
            if (gamePossible) sum += game.Id;

            WriteLine();
        }

        WriteLine("SUM: " + sum);
    }

    public async Task Part2()
    {
        var games = await ReadData("Day2.txt");

        var sum = 0;
        foreach (var game in games)
        {
            var maxR = 0;
            var maxG = 0;
            var maxB = 0;

            foreach (var cubeSet in game.CubeSets)
            {
                maxR = int.Max(cubeSet.r, maxR);
                maxG = int.Max(cubeSet.g, maxG);
                maxB = int.Max(cubeSet.b, maxB);
            }

            sum += maxR*maxG*maxB;
        }

        WriteLine("POWER: " + sum);
    }

    private async Task<Game[]> ReadData(string filename)
    {
        List<Game> games = new();
        foreach (var line in await File.ReadAllLinesAsync(filename))
        {
            if (string.IsNullOrEmpty(line)) continue;

            var gameSplitIdx = line.IndexOf(':');
            var id = int.Parse(line[5..gameSplitIdx]);
            var cubeSets = new List<(int, int, int)>();

            foreach (var part in line[(gameSplitIdx+1)..].Split(';', StringSplitOptions.TrimEntries))
            {
                var r = 0;
                var g = 0;
                var b = 0;

                foreach (var cube in part.Split(',', StringSplitOptions.TrimEntries))
                {
                    var cubeParts = cube.Split(' ');

                    switch (cubeParts[1])
                    {
                        case "red": r = int.Parse(cubeParts[0]); break;
                        case "green": g = int.Parse(cubeParts[0]); break;
                        case "blue": b = int.Parse(cubeParts[0]); break;
                    }
                }

                cubeSets.Add((r, g, b));
            }
            games.Add(new(id, cubeSets.ToArray()));
        }

        return games.ToArray();
    }

    public readonly record struct Game(int Id, (int r, int g, int b)[] CubeSets);
}