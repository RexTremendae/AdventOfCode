using static System.Console;

public class Day22
{
    public enum Direction
    {
        Right = 0,
        Down = 1,
        Left = 2,
        Up = 3
    }

    public async Task Part1()
    {
        var (map, movements) = await ReadInput("Day22.txt");
        var mapWidth = map[0].Length;
        var mapHeight = map.Length;
        var y = 1;
        var x = 0;
        while (map[y][x] != '.') x++;

        var direction = Direction.Right;

        var moveIdx = 0;
        for(;;)
        {
            var startIdx = moveIdx;
            while (char.IsNumber(movements[moveIdx]))
            {
                moveIdx++;
            }
            var steps = int.Parse(movements[startIdx..moveIdx]);

            var delta = direction switch
            {
                Direction.Right => (x: 1, y:  0),
                Direction.Left  => (x:-1, y:  0),
                Direction.Down  => (x: 0, y:  1),
                Direction.Up    => (x: 0, y: -1),
            };

            foreach (var s in 0.To(steps))
            {
                var doMove = true;
                var (nx, ny) = (x+delta.x, y+delta.y);
                if (map[ny][nx] == '#') break;
                if (map[ny][nx] == ' ')
                {
                    doMove = false;
                    switch (direction)
                    {
                        case Direction.Right:
                            nx = 0;
                            while (map[ny][nx] == ' ') nx++;
                            if (map[ny][nx] == '#') break;
                            doMove = true;
                        break;

                        case Direction.Left:
                            nx = mapWidth - 1;
                            while (map[ny][nx] == ' ') nx--;
                            if (map[ny][nx] == '#') break;
                            doMove = true;
                        break;

                        case Direction.Up:
                            ny = mapHeight - 1;
                            while (map[ny][nx] == ' ') ny--;
                            if (map[ny][nx] == '#') break;
                            doMove = true;
                        break;

                        case Direction.Down:
                            ny = 0;
                            while (map[ny][nx] == ' ') ny++;
                            if (map[ny][nx] == '#') break;
                            doMove = true;
                        break;
                    }
                }

                if (!doMove) break;
                (x, y) = (nx, ny);
            }

            var dirChange = movements[moveIdx];

            if (dirChange == ' ') break;
            if (dirChange == 'R') direction = direction switch
                {
                    Direction.Right => Direction.Down,
                    Direction.Down => Direction.Left,
                    Direction.Left => Direction.Up,
                    Direction.Up => Direction.Right,
                };
            else if (dirChange == 'L') direction = direction switch
                {
                    Direction.Right => Direction.Up,
                    Direction.Up => Direction.Left,
                    Direction.Left => Direction.Down,
                    Direction.Down => Direction.Right,
                };
            else
                throw new InvalidOperationException(dirChange.ToString());

            moveIdx++;

        }
        WriteLine(y*1000 + 4*x + direction);
    }

    public async Task<(string[] map, string movements)> ReadInput(string filename)
    {
        var map = new List<string>();

        var mapWidth = 0;

        var movements = string.Empty;
        var isMap = true;
        await foreach(var line in System.IO.File.ReadLinesAsync(filename))
        {
            if (string.IsNullOrEmpty(line))
            {
                isMap = false;
                continue;
            }

            if (isMap)
            {
                mapWidth = Math.Max(mapWidth, line.Length);
                map.Add(line);
            }
            else
            {
                movements = line;
            }
        }

        mapWidth += 2;
        map.Insert(0, "".PadLeft(mapWidth, ' '));
        map.Add("".PadLeft(mapWidth, ' '));

        foreach (var lineIdx in 1.To(map.Count-1))
        {
            map[lineIdx] = $" {map[lineIdx].PadRight(mapWidth-2, ' ')} ";
        }

        return (map.ToArray(), movements + " ");
    }
}
