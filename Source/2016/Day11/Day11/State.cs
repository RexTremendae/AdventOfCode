using System.Collections.Generic;
using System.Linq;

namespace Day11
{
    public class State
    {
        public class StateStorage
        {
            public StateStorage(int x, int y, string data)
            {
                XPos = (char)x;
                YPos = (char)y;
                Data = data;
            }

            public char XPos { get; }
            public char YPos { get; }
            public string Data { get; }
        }

        public StateStorage[] Storage;
 
        public int Width { get; }
        public int Height { get; }

        private State(int widht, int height, StateStorage[] storage)
        {
            Width = widht;
            Height = height;
            Storage = storage;
        }

        public State(string[][] rawState)
        {
            var storageList = new List<StateStorage>();

            Height = rawState.Length;
            Width = rawState[0].Length;

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    var data = rawState[y][x];
                    if (data != string.Empty)
                        storageList.Add(new StateStorage(x, y, data));
                }

            Storage = storageList.ToArray();
        }

        public string[][] Expand()
        {
            string[][] expandedstate = new string[Height][];

            for (int y = 0; y < Height; y++)
            {
                expandedstate[y] = new string[Width];
                for (int x = 0; x < Width; x++)
                    expandedstate[y][x] = string.Empty;
            }

            foreach (var data in Storage)
            {
                expandedstate[data.YPos][data.XPos] = data.Data;
            }

            return expandedstate;
        }

        public State Clone()
        {
            return new State(Width, Height, Extensions.Clone(Storage));
        }

        public override bool Equals(object obj)
        {
            var other = obj as State;
            if (other == null) return false;

            if (other.Width != Width || other.Height != Height) return false;

            foreach (var data in Storage)
            {
                var otherData = other.Storage.SingleOrDefault(x => x.Data == data.Data);
                if (otherData == null) return false;
                if (otherData.XPos != data.XPos || otherData.YPos != data.YPos) return false;
            }

            return true;
        }

        public void MoveOrAdd(int x, int y, string data)
        {
            var newData = new StateStorage(x, y, data);

            int index = 0;
            for (; index < Storage.Length; index++)
                if (Storage[index].Data == data)
                {
                    Storage[index] = newData;
                    return;
                }

            var newStorage = new StateStorage[Storage.Length + 1];
            for (int i = 0; i < Storage.Length; i++)
                newStorage[i] = Storage[i];

            newStorage[Storage.Length] = newData;
            Storage = newStorage;
        }

        public string GetData(int x, int y)
        {
            var hit = Storage.SingleOrDefault(d => d.XPos == x && d.YPos == y);

            return hit == null ? string.Empty : hit.Data;
        }
    }
}
