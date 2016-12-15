using System.Text;

namespace Day11
{
    public class Movement
    {
        public State State;
        public int ElevatorYPosBefore;
        public MovementDirection MovementDirection;
        public int[] SelectionXPositions;

        public Movement Clone()
        {
            Movement clone = new Movement();

            clone.State = State.Clone();
            clone.ElevatorYPosBefore = ElevatorYPosBefore;
            clone.MovementDirection = MovementDirection;
            clone.SelectionXPositions = Extensions.Clone(SelectionXPositions);

            return clone;
        }

        public bool IsReverseOf(Movement otherMovement)
        {
            if (otherMovement == null)
                return false;

            if (MovementDirection == otherMovement.MovementDirection)
                return false;

            if (SelectionXPositions.Length != otherMovement.SelectionXPositions.Length)
                return false;

            for (int i = 0; i < SelectionXPositions.Length; i++)
                if (SelectionXPositions[i] != otherMovement.SelectionXPositions[i])
                    return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Movement;
            if (other == null)
                return false;

            return other.State.Equals(State);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            int paddedWidth = 5;

            sb.AppendLine("State:");
            var rawState = State.Expand();
            for (int y = 0; y < rawState.Length; y++)
            {
                for (int x = 0; x < rawState[y].Length; x++)
                {
                    string data = rawState[y][x];
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
