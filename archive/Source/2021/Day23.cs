namespace _2021;

public class Day23
{
    public void Part1()
    {
        Run("""
            #############
            #...........#
            ###C#B#D#D###
              #B#C#A#A#
              #########
        """,
        2,
        10_321);
    }

    public void Part2()
    {
        Run("""
            #############
            #...........#
            ###C#B#D#D###
              #D#C#B#A#
              #D#B#A#C#
              #B#C#A#A#
              #########
        """,
        4,
        46_451);
    }

    public void Run(string boardString, int depth, long maxEnergy)
    {
        var board = FromBoardString(boardString);

        Console.Clear();
        Console.CursorVisible = false;
        var boardPos = Console.GetCursorPosition();

        var selectionHistory = new List<(int x, int y)>();
        var selectedPos = (x: 0, y: 0);
        var totalEnergy = 0L;
        var history = new List<(List<char[]> board, (int x, int y) selectedPos, long totalEnergy)>();

        for(;;)
        {
            PrintBoard(board, boardPos, selectedPos);
            Console.WriteLine();
            Console.ForegroundColor =
                totalEnergy < maxEnergy ? ConsoleColor.White
                : totalEnergy == maxEnergy ? ConsoleColor.Green
                : ConsoleColor.Red;
            Console.WriteLine(totalEnergy.ToString("### ### ##0").PadLeft(11));
            Console.ResetColor();

            var key = Console.ReadKey(true).Key;
            var findSelection = ' ';
            var movement = (x: 0, y: 0);
            var undo = false;
            var reset = false;

            switch (key)
            {
                case ConsoleKey.A:
                    findSelection = 'A';
                    break;

                case ConsoleKey.B:
                    findSelection = 'B';
                    break;

                case ConsoleKey.C:
                    findSelection = 'C';
                    break;

                case ConsoleKey.D:
                    findSelection = 'D';
                    break;

                case ConsoleKey.U:
                    undo = true;
                    break;

                case ConsoleKey.R:
                    reset = true;
                    break;

                case ConsoleKey.UpArrow:
                    movement = (0, -1);
                    break;

                case ConsoleKey.DownArrow:
                    movement = (0, 1);
                    break;

                case ConsoleKey.LeftArrow:
                    movement = (-1, 0);
                    break;

                case ConsoleKey.RightArrow:
                    movement = (1, 0);
                    break;
            }

            if (findSelection != ' ')
            {
                if (selectionHistory.Any())
                {
                    var firstSelection = selectionHistory.First();
                    if (selectionHistory.Count == depth ||
                        board[firstSelection.y][firstSelection.x] != findSelection)
                    {
                        selectionHistory.Clear();
                    }
                }

                selectedPos = FindSelection(board, findSelection, selectionHistory);
                selectionHistory.Add(selectedPos);
            }
            else if (movement != (0, 0))
            {
                var selectedEntity = board[selectedPos.y][selectedPos.x];
                var moveToPos = (x: selectedPos.x+movement.x, y: selectedPos.y+movement.y);

                if (moveToPos.x >= 0 && moveToPos.y >= 0 &&
                    moveToPos.y < board.Count && moveToPos.x <= board[0].Length &&
                    board[selectedPos.y+movement.y][selectedPos.x+movement.x] == '.' &&
                    selectedEntity >= 'A' && selectedEntity <= 'D')
                {
                    history.Add((board.Select(_ => _.ToArray()).ToList(), selectedPos, totalEnergy));
                    board[selectedPos.y][selectedPos.x] = '.';
                    board[moveToPos.y][moveToPos.x] = selectedEntity;
                    selectedPos = moveToPos;
                    totalEnergy += selectedEntity switch
                    {
                        'A' => 1,
                        'B' => 10,
                        'C' => 100,
                        'D' => 1000,
                        _ => 0
                    };
                    selectionHistory.Clear();
                }
            }
            else if (undo)
            {
                if (history.Any())
                {
                    var state = history.Last();
                    history.RemoveAt(history.Count-1);
                    board = state.board;
                    selectedPos = state.selectedPos;
                    totalEnergy = state.totalEnergy;
                    selectionHistory.Clear();
                }
            }
            else if (reset)
            {
                selectionHistory.Clear();
                history.Clear();
                board = FromBoardString(boardString);
                totalEnergy = 0;
            }
        }
    }

    private List<char[]> FromBoardString(string boardString)
    {
        return boardString
            .Replace("\r", "")
            .Split("\n")
            .Select(_ => _.ToCharArray())
            .ToList();
    }

    private (int x, int y) FindSelection(List<char[]> board, char findSelection, List<(int x, int y)> selectionHistory)
    {
        for (var y = 0; y < board.Count; y ++)
        for (int x = 0; x < board[y].Length; x++)
        {
            if (board[y][x] == findSelection)
            {
                if (selectionHistory.Contains((x, y)))
                {
                    continue;
                }

                return (x, y);
            }
        }

        return (0, 0);
    }

    private void PrintBoard(List<char[]> lines, (int left, int top) boardPos, (int x, int y) selectedPos)
    {
        Console.SetCursorPosition(boardPos.left, boardPos.top);

        for (var y = 0; y < lines.Count; y++)
        {
            for (var x = 0; x < lines[y].Length; x++)
            {
                var ch = lines[y][x];

                Console.ForegroundColor = ch switch
                {
                    var _ when (x, y) == selectedPos && ch == 'A' => ConsoleColor.Green,
                    var _ when (x, y) == selectedPos && ch == 'B' => ConsoleColor.Magenta,
                    var _ when (x, y) == selectedPos && ch == 'C' => ConsoleColor.Yellow,
                    var _ when (x, y) == selectedPos && ch == 'D' => ConsoleColor.Cyan,
                    '#' => ConsoleColor.DarkGray,
                    'A' => ConsoleColor.DarkGreen,
                    'B' => ConsoleColor.DarkMagenta,
                    'C' => ConsoleColor.DarkYellow,
                    'D' => ConsoleColor.DarkBlue,
                    _ => ConsoleColor.Gray
                };
                Console.Write(ch);
            }
            Console.WriteLine();
        }

        Console.ResetColor();
    }
}
