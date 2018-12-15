using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day15
{
    public class Unit
    {
        public (int x, int y) Position { get; set; }
        public char Type { get; set; }
        public int HP { get; set; }
        public string TypeString => Type == 'G' ? "Goblin" : "Elf";
        public string Description => $"{TypeString} at position ({Position.x}, {Position.y})";
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                WriteLine("Please provide a filename for input.");
                return;
            }
            int maxRounds = int.MaxValue;
            if (args.Length > 1)
            {
                int.TryParse(args[1], out maxRounds);
            }
            new Program().Run(args[0], maxRounds);
        }

        public void Run(string filepath, int maxRounds)
        {
            var (board, units) = ParseInput(filepath);
            int i = 0;
            while (units.Any(x => x.Type == 'G') && units.Any(x => x.Type == 'E'))
            {
                WriteLine();
                WriteLine($"  =====  Round {i+1} begins  ===== ");
                WriteLine();
                Fight(board, units);
                i++;
                if (i >= maxRounds) break;
            }

            foreach (var u in units)
            {
                board[u.Position.y][u.Position.x] = u.Type;
            }

            WriteLine();
            WriteLine("Battle is over!");
            WriteLine();
            foreach (var row in board)
            {
                foreach (var col in row)
                {
                    Write(col);
                }
                WriteLine();
            }
            WriteLine();
            WriteLine("Survivors:");
            var hpSum = 0;
            foreach (var u in units)
            {
                WriteLine($"{u.Description} has {u.HP} HP left.");
                hpSum += u.HP;
            }

            i--;
            if (i < maxRounds)
            {
                WriteLine();
                WriteLine($"Outcome: {i} * {hpSum} = {i*hpSum}");
            }
        }

        public void Fight(List<List<char>> board, List<Unit> units)
        {
            var killedUnits = new List<Unit>();
            foreach (var unit in units.OrderBy(u => u.Position.y).ThenBy(u => u.Position.x))
            {
                if (killedUnits.Contains(unit)) continue;
                var enemies = units.Where(x => x.IsEnemy(unit));
                //WriteLine($"Found {enemies.Count()} enemies.");
                var adjacentEnemies = enemies.Where(x => x.Distance(unit) == 1);
                if (adjacentEnemies.None())
                {
                    var next = new NextStepFinder(enemies, board, units).Find(unit);
                    if (next != null)
                    {
                        WriteLine($"{unit.Description} moves to ({next.Value.x}, {next.Value.y})");
                        unit.Position = next.Value;
                    }
                    adjacentEnemies = enemies.Where(x => x.Distance(unit) == 1);
                }

                var selectedEnemy = adjacentEnemies.OrderBy(e => e.HP)
                                                   .ThenBy(e => e.Position.y)
                                                   .ThenBy(e => e.Position.x)
                                                   .FirstOrDefault();
                if (selectedEnemy != null)
                {
                    WriteLine($"{unit.Description} attacks {selectedEnemy.Description}");
                    selectedEnemy.HP -= 3;
                    if (selectedEnemy.HP <= 0)
                    {
                        WriteLine($"{unit.Description} kills {selectedEnemy.Description}");
                        units.Remove(selectedEnemy);
                        killedUnits.Add(selectedEnemy);
                    }
                }
            }
        }

        public (List<List<char>> board, List<Unit> units) ParseInput(string filepath)
        {
            var board = new List<List<char>>();
            var units = new List<Unit>();

            int y = 0;
            foreach (var lineRaw in File.ReadAllLines(filepath))
            {
                if (String.IsNullOrEmpty(lineRaw)) { break; }
                var line = new List<char>();
                board.Add(line);

                int x = 0;
                foreach (var c in lineRaw)
                {
                    if (c == 'G' || c == 'E')
                    {
                        units.Add(new Unit { Position = (x, y), Type = c, HP = 200 });
                        line.Add('.');
                    }
                    else
                    {
                        line.Add(c);
                    }

                    x++;
                }

                y++;
            }

            return (board, units);
        }
    }

    public class NextStepFinder
    {
        private List<List<char>> _board;
        private IEnumerable<Unit> _allUnits;
        private IEnumerable<Unit> _enemies;

        public NextStepFinder(IEnumerable<Unit> enemies, List<List<char>> board, IEnumerable<Unit> allUnits)
        {
            _board = board;
            _enemies = enemies;
            _allUnits = allUnits;
        }

        public (int x, int y)? Find(Unit unit)
        {
            var queue = new Queue<QueueEntity>();
            var entity = new QueueEntity();
            entity.Visited.Add(unit.Position);
            entity.CurrentPosition = unit.Position;
            queue.Enqueue(entity);
            var minDistancePaths = new List<(int distance, (int x, int y) nextStep)>();
            int minDistance = 0;
            bool goon = true;
//            WriteLine($"Finding movement for {(unit.Type == 'G' ? "Goblin" : "Elf")} at position ({unit.Position.x}, {unit.Position.y})");
            while(queue.Any() && goon)
            {
                entity = queue.Dequeue();
                foreach (var candidate in GetCandidates(entity.CurrentPosition))
                {
                    var otherUnit = _allUnits.SingleOrDefault(e => e.Position.x == candidate.x && e.Position.y == candidate.y);
                    if (otherUnit != null)
                    {
                        if (otherUnit.Type == unit.Type) continue;
                        if (minDistance == 0 || entity.PathLength == minDistance)
                        {
                            minDistance = entity.PathLength;
                            minDistancePaths.Add((entity.PathLength, entity.FirstStep.Value));
                        }
                        else if (entity.PathLength != minDistance)
                        {
                            goon = false;
                            break;
                        }
                    }
                    else if (!entity.Visited.Contains(candidate))
                    {
                        var clone = entity.Clone();
                        clone.CurrentPosition = candidate;
                        clone.Visited.Add(candidate);
                        clone.PathLength++;
                        if (clone.FirstStep == null) 
                        {
                            clone.FirstStep = candidate;
                        }
                        queue.Enqueue(clone);
                    }
                }
            }
/*
            foreach (var v in minDistancePaths)
            {
                WriteLine($"{v.distance} ({v.nextStep.x}, {v.nextStep.y})");
            }
*/
            if (minDistancePaths.None()) return null;
            return minDistancePaths.Where(d => d.distance == minDistance)
                                   .OrderBy(d => d.nextStep.y)
                                   .ThenBy(d => d.nextStep.x)
                                   .FirstOrDefault().nextStep;
        }

        private IEnumerable<(int x, int y)> GetCandidates((int x, int y) current)
        {
            int cx, cy;

            cx = current.x + 1;
            cy = current.y;
            if (cx < _board[0].Count && _board[cy][cx] == '.')
                yield return (cx, cy);

            cx = current.x - 1;
            cy = current.y;
            if (cx >= 0 && _board[cy][cx] == '.')
                yield return (cx, cy);

            cx = current.x;
            cy = current.y + 1;
            if (cy < _board.Count && _board[cy][cx] == '.')
                yield return (cx, cy);

            cx = current.x;
            cy = current.y - 1;
            if (cy >= 0 && _board[cy][cx] == '.')
                yield return (cx, cy);
        }

        private class QueueEntity
        {
            public QueueEntity()
            {
                Visited = new HashSet<(int, int)>();
            }

            public QueueEntity Clone()
            {
                var clone = new QueueEntity();
                clone.CurrentPosition = CurrentPosition;
                clone.PathLength = PathLength;
                clone.FirstStep = FirstStep;
                foreach (var v in Visited) clone.Visited.Add(v);
                return clone;
            }

            public HashSet<(int, int)> Visited { get; }
            public int PathLength { get; set; }
            private (int, int)? _firstStep;
            public (int, int)? FirstStep
            {
                get
                {
                    return _firstStep;
                }
                set
                { 
                    _firstStep = value;
                }
            }
            public (int, int) CurrentPosition { get; set; }
        }
    }

    public static class Extensions
    {
        public static bool None<T>(this IEnumerable<T> source)
        {
            return !source.Any();
        }

        public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return !source.Any(predicate);
        }

        public static IEnumerable<(int x, int y)> Neighbours(this (int x, int y) position, List<List<char>> board)
        {
            int x = position.x;
            int y = position.y;

            int xx = x -1;
            int yy = y;
            if (xx >= 0 && board[yy][xx] == '.') yield return (xx, yy);

            xx = x + 1;
            if (xx < board[0].Count && board[yy][xx] == '.') yield return (xx, yy);

            xx = x;
            yy = y - 1;
            if (yy >= 0 && board[yy][xx] == '.') yield return (xx, yy);

            yy = y - 1;
            if (yy < board.Count && board[yy][xx] == '.') yield return (xx, yy);
        }

        public static bool IsEnemy(this Unit unit, Unit other)
        {
            return unit.Type != other.Type;
        }

        public static int Distance(this Unit unit, Unit other)
        {
            return Math.Abs(unit.Position.x - other.Position.x) + Math.Abs(unit.Position.y - other.Position.y);
        }
    }
}
