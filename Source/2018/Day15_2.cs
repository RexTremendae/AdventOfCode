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

        public Unit Clone()
        {
            return new Unit
            {
                Position = Position,
                Type = Type,
                HP = HP
            };
        }
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
            var (board, units) = ParseInput(args[0]);
            new Program().Run(board, units);
        }

        public void Run(List<List<char>> board, List<Unit> unitsFromInput)
        {
            int elfKillCount = int.MaxValue;
            int elfAttackPower = 3;
            int i = 0;
            List<Unit> units;
            do
            {
                elfAttackPower++;
                units = new List<Unit>();
                foreach (var u in unitsFromInput) units.Add(u.Clone());

                var elfCount = units.Count(x => x.Type == 'E');
                i = 0;
                while (units.Any(x => x.Type == 'G') && units.Any(x => x.Type == 'E'))
                {
                    WriteLine();
                    WriteLine($"  =====  Round {i+1} begins  ===== ");
                    WriteLine();
                    Fight(board, units, elfAttackPower);
                    i++;
                }
                elfKillCount = elfCount - units.Count(x => x.Type == 'E');
            } while (elfKillCount > 0);

            int hpSum = 0;
            foreach (var elf in units)
            {
                hpSum += elf.HP;
            }

            WriteLine();
            WriteLine($"Elf attack power: {elfAttackPower}");
            // Off-by-one, decreasing i by one gave the correct answer
            WriteLine($"Outcome: {i-1} * {hpSum} = {(i-1)*hpSum}");
            WriteLine($"Elves killed: {elfKillCount}");
        }

        public void Fight(List<List<char>> board, List<Unit> units, int elfAttackPower)
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
                    selectedEnemy.HP -= (selectedEnemy.Type == 'G' ? elfAttackPower : 3);
                    if (selectedEnemy.HP <= 0)
                    {
                        WriteLine($"{unit.Description} kills {selectedEnemy.Description}");
                        units.Remove(selectedEnemy);
                        killedUnits.Add(selectedEnemy);
                    }
                }
            }
        }

        public static (List<List<char>> board, List<Unit> units) ParseInput(string filepath)
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
            var allUnits = new Dictionary<(int, int), Unit>();
            foreach (var u in _allUnits)
            {
                allUnits.Add(u.Position, u);
            }

            var layers = new List<HashSet<(int x, int y)>>();
            layers.Add(new HashSet<(int x, int y)>());
            var nextLayer = new HashSet<(int x, int y)>();
            layers.Add(nextLayer);

            var visited = new HashSet<(int x, int y)>();
            foreach (var c in GetCandidates(unit.Position))
            {
                visited.Add(c);
                if (!allUnits.ContainsKey(c))
                    nextLayer.Add(c);
            }

            var enemyCandidates = new List<Unit>();
            int distance = 2;
            while (nextLayer.Any())
            {
                var nextNextLayer = new HashSet<(int x, int y)>();
                foreach (var n in nextLayer)
                {
                    foreach (var c in GetCandidates(n))
                    {
                        if (visited.Contains(c)) continue;

                        if (allUnits.TryGetValue(c, out var unitFound))
                        {
                            if (unitFound.Type != unit.Type)
                            {
                                enemyCandidates.Add(unitFound);
                            }
                            continue;
                        }
                        nextNextLayer.Add(c);
                        visited.Add(c);
                    }
                }

                layers.Add(nextNextLayer);
                if (enemyCandidates.Any()) break;

                nextLayer = nextNextLayer;
                distance++;
            }

            if (enemyCandidates.None()) return null;

            var queue = new Queue<(int distance, (int x, int y) position)>();
            var stepCandidates = new List<(int x, int y)>();
            foreach (var ec in enemyCandidates)
            {
                queue.Enqueue((distance, ec.Position));
            }

            while (queue.Any())
            {
                var q = queue.Dequeue();
                foreach (var c in GetCandidates(q.position))
                {
                    if (q.distance == 1)
                    {
                        stepCandidates.Add(q.position);
                    }
                    else if (q.distance > 1 && layers[q.distance - 1].Contains(c))
                    {
                        queue.Enqueue((q.distance - 1, c));
                    }
                }
            }

            return stepCandidates.OrderBy(d => d.y)
                                 .ThenBy(d => d.x)
                                 .FirstOrDefault();
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

        public static int Distance(this (int x, int y) from, (int x, int y) to)
        {
            return Math.Abs(from.x - to.x) + Math.Abs(from.y - to.y);
        }

        public static int Distance(this Unit unit, Unit other)
        {
            return Math.Abs(unit.Position.x - other.Position.x) + Math.Abs(unit.Position.y - other.Position.y);
        }
    }
}
