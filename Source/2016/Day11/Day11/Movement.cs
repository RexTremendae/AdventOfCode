namespace Day11
{
    public class Movement
    {
        public string[][] State;
        public int ElevatorYPosBefore;
        public MovementDirection MovementDirection;
        public string[] Selections;

        public Movement Clone()
        {
            Movement clone = new Movement();

            clone.State = StringExtensions.Clone(State);
            clone.ElevatorYPosBefore = ElevatorYPosBefore;
            clone.MovementDirection = MovementDirection;
            clone.Selections = StringExtensions.Clone(Selections);

            return clone;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Movement;
            if (other == null) return false;

            for (int y = 0; y < State.Length; y++)
                for (int x = 0; x < State[y].Length; x++)
                    if (State[y][x] != other.State[y][x])
                        return false;

            if (ElevatorYPosBefore != other.ElevatorYPosBefore) return false;

            if (MovementDirection != other.MovementDirection) return false;

            if (Selections.Length != other.Selections.Length) return false;

            for (int i = 0; i < Selections.Length; i++)
                if (Selections[i] != other.Selections[i]) return false;

            return true;
        }
    }
}
