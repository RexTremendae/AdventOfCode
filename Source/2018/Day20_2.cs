using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using static System.Console;

namespace Day20
{
    public enum Data
    {
        Wall = 0,
        Start,
        Door,
        OpenSpace
    }

    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            var path = ParseInput("Day20.txt");

            var map = new Map();

            var dir = new Dictionary<char, Direction>
            {
                { 'N', Direction.North },
                { 'E', Direction.East },
                { 'S', Direction.South },
                { 'W', Direction.West }
            };
            var queue = new Queue<((int x, int y) pos, int length, Path path)>();
            queue.Enqueue(((0, 0), 0, path));

            int maxLength = 0;
            while (queue.Any())
            {
                var current = queue.Dequeue();
                //WriteLine($"Dequeueing {current.path.Piece} with id {current.path._counter}");
                map.Position = current.pos;
                int length = current.length;

                foreach (var movement in current.path.Piece)
                {
                    map.Move(dir[movement]);
                    length ++;
                }

                if (length > maxLength) maxLength = length;

                foreach (var fork in current.path.Forks) queue.Enqueue(((map.Position), length, fork));
                if (current.path.Next != null) queue.Enqueue(((map.Position), length, current.path.Next));
            }

            map.Print();

            maxLength = map.FindNumberOfRoomsAtLeast1000Doors();
            // TODO: Need to mark visited when traversing map
            WriteLine($"Max length: {maxLength}");
        }

        private Path ParseInput(string filename)
        {
            var indata = File.ReadLines(filename).First();
            return Path.Parse(indata);
        }
    }

    public class Map
    {
        private (int x, int y) _minStorage;
        private (int x, int y) _maxStorage;
        private (int x, int y) _minUsage;
        private (int x, int y) _maxUsage;
        private (int x, int y) _position;

        private List<Data[]> _map;

        public Map()
        {
            _map = new List<Data[]>();
            _map.Add(new Data[1] { Data.Start });
            _minStorage = (0, 0);
            _maxStorage = (0, 0);
            _minUsage = (0, 0);
            _maxUsage = (0, 0);
            _position = (0, 0);
        }

        public int FindNumberOfRoomsAtLeast1000Doors()
        {
            var distances = new List<int[]>();

            (int x, int y) startPos = (0, 0);
            for (int y = 0; y < _map.Count; y++)
            {
                distances.Add(new int[_map[y].Length]);
                for (int x = 0; x < _map[y].Length; x++)
                {
                    if (_map[y][x] == Data.Start)
                    {
                        startPos = (x, y);
                    }

                    if (startPos.x > 0 || startPos.y > 0) break;
                }
            }
            distances.Add(new int[_maxUsage.x - _minUsage.x + 1]);

            var queue = new Queue<(int distance, (int x, int y) position)>();
            queue.Enqueue((0, startPos));

            while (queue.Any())
            {
                var item = queue.Dequeue();
                var newDistance = item.distance + 1;
                var pos = item.position;

                if (_map[pos.y][pos.x-1] == Data.Door && distances[pos.y][pos.x-2] == 0)
                {
                    distances[pos.y][pos.x-2] = newDistance;
                    queue.Enqueue((newDistance, (pos.x-2, pos.y)));
                }
                if (_map[pos.y][pos.x+1] == Data.Door && distances[pos.y][pos.x+2] == 0)
                {
                    distances[pos.y][pos.x+2] = newDistance;
                    queue.Enqueue((newDistance, (pos.x+2, pos.y)));
                }
                if (_map[pos.y-1][pos.x] == Data.Door && distances[pos.y-2][pos.x] == 0)
                {
                    distances[pos.y-2][pos.x] = newDistance;
                    queue.Enqueue((newDistance, (pos.x, pos.y-2)));
                }
                if (_map[pos.y+1][pos.x] == Data.Door && distances[pos.y+2][pos.x] == 0)
                {
                    distances[pos.y+2][pos.x] = newDistance;
                    queue.Enqueue((newDistance, (pos.x, pos.y+2)));
                }
            }

            int roomCount = 0;
            for (int y = 0; y < distances.Count; y++)
            {
                for (int x = 0; x < distances[y].Length; x++)
                {
                    if (distances[y][x] >= 1000) roomCount++;
                }
            }

            return roomCount;
        }

        public void Print()
        {
            //WriteLine("Printing...");

            int firstRow = -1;
            int lastRow = -1;

            int firstCol = int.MaxValue;
            int lastCol = -1;

            for (int row = 0; row < _map.Count; row++)
            {
                bool hasData = false;
                for (int col = 0; col < _map[row].Length; col++)
                {
                    if (_map[row][col] != Data.Wall)
                    {
                        if (col < firstCol) firstCol = col;
                        if (col > lastCol) lastCol = col;
                        hasData = true;
                    }
                }
                if (hasData)
                {
                    if (firstRow < 0) firstRow = row;
                    if (lastRow < row) lastRow = row;
                }
            }

            var data = new Dictionary<Data, char>
            {
                { Data.Wall,      '#' },
                { Data.Start,     'X' },
                { Data.Door,      '+' },
                { Data.OpenSpace, ' ' }
            };

            for (int row = firstRow-1; row <= lastRow+1; row++)
            {
                for (int col = firstCol-1; col <= lastCol+1; col++)
                {
                    Write(data[_map[row][col]]);
                }
                WriteLine();
            }
        }

        public (int x, int y) Position
        {
            get
            {
                return _position;
            }

            set
            {
                if (value.x < _minUsage.x || value.x > _maxUsage.x || value.y < _minUsage.y || value.y > _maxUsage.y)
                {
                    throw new ArgumentException();
                }

                _position = value;
            }
        }

        private void IncreaseSize(int diff)
        {
            var oldMap = _map;
            var oldMin = _minStorage;
            var oldMax = _maxStorage;

            _map = new List<Data[]>();

            _maxStorage = (_maxStorage.x + diff, _maxStorage.y + diff);
            _minStorage = (_minStorage.x - diff, _minStorage.y - diff);

            for (int y = _minStorage.y; y <= _maxStorage.y; y++)
            {
                var yPos = y - _minStorage.y;
                _map.Add(new Data[_maxStorage.x - _minStorage.x + 1]);
                for (int x = _minStorage.x; x <= _maxStorage.x; x++)
                {
                    var xPos = x - _minStorage.x;
                    //Write($"({x}, {y})  [{xPos}, {yPos}]");

                    if (x >= oldMin.x && x <= oldMax.x && y >= oldMin.y && y <= oldMax.y)
                    {
                        //Write($" => ({x - oldMin.y}, {y - oldMin.y})");
                        _map[yPos][xPos] = oldMap[y - oldMin.y][x - oldMin.x];
                    }

                    //WriteLine();
                }
            }
        }

        public void Move(Direction direction)
        {
            var offset = direction.ToCoordinateOffset();
            var doorPosition = (x: _position.x + offset.x, y: _position.y + offset.y);
            _position = (doorPosition.x + offset.x, doorPosition.y + offset.y);
            if (_position.x < _minStorage.x || _position.x > _maxStorage.x || _position.y < _minStorage.y || _position.y > _maxStorage.y)
            {
                IncreaseSize(100);
            }

            //WriteLine($"Door at ({doorPosition.x - _minStorage.x}, {doorPosition.y - _minStorage.y})");

            _map[doorPosition.y - _minStorage.y][doorPosition.x - _minStorage.x] = Data.Door;
            _map[_position.y - _minStorage.y][_position.x - _minStorage.x] = Data.OpenSpace;

            if (_position.x < _minUsage.x) _minUsage.x = _position.x;
            else if (_position.x > _maxUsage.x) _maxUsage.x = _position.x;

            if (_position.y < _minUsage.y) _minUsage.y = _position.y;
            else if (_position.y > _maxUsage.y) _maxUsage.y = _position.y;
        }
    }

    public class Path
    {
        private List<Path> _forks;
        private static StringBuilder _pieceBuilder = new StringBuilder();
        private static int Counter = 0;
        public int _counter;

        public Path()
        {
            _counter = Counter;
            //WriteLine($"Creating {_counter}");
            Counter++;
            Piece = string.Empty;
            _forks = new List<Path>();
        }

        public static Path Parse(string input)
        {
            int idx = 1;
            var path = new Path();
            var result = path;
            var current = (char)0;

            while (current != '$')
            {
                current = input[idx];
                switch(current)
                {
                    case '(':
                        path.StorePiece();
                        path = path.AddFork();
                        break;

                    case '|':
                        path.StorePiece();
                        path = path.Prev;
                        path = path.AddFork();
                        break;

                    case ')':
                        path.StorePiece();
                        path = path.Prev;
                        if (path == null) throw new Exception($"Error at {idx}");
                        path.Next = new Path();
                        path.Next.Prev = path.Prev;
                        path = path.Next;
                        break;

                    case '$':
                        path.StorePiece();
                        break;

                    default:
                        _pieceBuilder.Append(current);
                        break;
                }

                idx++;
            }

            return result;
        }

        private void StorePiece()
        {
            if (!string.IsNullOrEmpty(Piece)) throw new InvalidDataException();

            Piece = _pieceBuilder.ToString();
            //WriteLine($"Storing {Piece} in {_counter}");
            _pieceBuilder.Clear();
        }

        private Path AddFork()
        {
            var fork = new Path();
            _forks.Add(fork);
            fork.Prev = this;
            return fork;
        }

        public string Piece { get; private set; }
        public Path Prev { get; private set; }
        public Path Next { get; private set; }
        public IEnumerable<Path> Forks => _forks;
    }

    public static class Extensions
    {
        public static bool NotIn(this char ch, params char[] array)
        {
            return !ch.In(array);
        }

        public static bool In(this char ch, params char[] array)
        {
            return array.Contains(ch);
        }

        public static (int x, int y) ToCoordinateOffset(this Direction direction)
        {
            switch (direction)
            {
                case Direction.North: return ( 0, -1);
                case Direction.East:  return (+1,  0);
                case Direction.South: return ( 0, +1);
                case Direction.West:  return (-1,  0);
                default: throw new ArgumentException();
            }
        }
    }
}
