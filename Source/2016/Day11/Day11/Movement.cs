using System.Text;

namespace Day11
{
    public class Movement
    {
        public string[][] State;
        public int ElevatorYPosBefore;
        public MovementDirection MovementDirection;
        public int[] SelectionXPositions;

        public Movement Clone()
        {
            Movement clone = new Movement();

            clone.State = Extensions.Clone(State);
            clone.ElevatorYPosBefore = ElevatorYPosBefore;
            clone.MovementDirection = MovementDirection;
            clone.SelectionXPositions = Extensions.Clone(SelectionXPositions);

            return clone;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Movement;
            if (other == null)
                return false;

            for (int y = 0; y < State.Length; y++)
                for (int x = 0; x < State[y].Length; x++)
                    if (State[y][x] != other.State[y][x])
                        return false;
/*
            if (ElevatorYPosBefore != other.ElevatorYPosBefore)
                return false;

            if (MovementDirection != other.MovementDirection)
                return false;

            if (SelectionXPositions.Length != other.SelectionXPositions.Length)
                return false;

            for (int i = 0; i < SelectionXPositions.Length; i++)
                if (SelectionXPositions[i] != other.SelectionXPositions[i])
                    return false;
                    */
            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            int paddedWidth = 5;

            sb.AppendLine("State:");
            for (int y = 0; y < State.Length; y++)
            {
                for (int x = 0; x < State[y].Length; x++)
                {
                    string data = State[y][x];
                    if (string.IsNullOrEmpty(data))
                        data = ".";

                    sb.Append(data);
                    for (int i = 0; i < paddedWidth - data.Length; i++)
                        sb.Append(" ");
                }
                sb.AppendLine();
            }

            sb.AppendLine($"Elevator pos: {ElevatorYPosBefore}");
            sb.Append("Selection: ");
            for (int i = 0; i < SelectionXPositions.Length; i++)
            {
                sb.Append($"{SelectionXPositions[i]}  ");
            }
            sb.AppendLine();
            sb.AppendLine($"Movement direction: {MovementDirection}");

            return sb.ToString();
        }
    }
}
