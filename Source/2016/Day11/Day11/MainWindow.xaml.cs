using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Day11
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private int _width;
        private int _height;
        private int _elevatorYPos;
        private int _moveCounter;
        private int Cols, Rows;
        private bool _gameOver;

        private string[][] _initialState = new[]
        {
            new[] { "",  "",   "",   "",   "" },
            new[] { "",  "",   "",   "LG", "" },
            new[] { "",  "HG", "",   "",   "" },
            new[] { "E", "",   "HM", "",   "LM" }
        };

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Items = new ObservableCollection<Item>();
            _itemDictionary = new Dictionary<string, Item>();

            Cols = _initialState[0].Length;
            Rows = _initialState.Length;

            ContentWidth = Cols * 50;
            ContentHeight = Rows * 37 - 2;

            for (int y = 0; y < Rows; y++)
                for (int x = 0; x < Cols; x++)
                {
                    int i = y * Cols + x;
                    var item = new Item(i.ToString())
                    {
                        DisplayText = string.Empty,
                        InnerBorderBrush = new SolidColorBrush(Colors.Gray),
                        OuterBorderBrush = new SolidColorBrush(Colors.Transparent),
                        BackgroundBrush = new SolidColorBrush(x != 0 ? Colors.DarkGray : Colors.Gray),
                        FontWeight = x == 0 ? FontWeights.Bold : FontWeights.Normal
                    };

                    Items.Add(item);
                    _itemDictionary.Add(item.Id, item);
                }

            Reset_Click(null, null);
        }

        public int ContentWidth
        {
            get { return _width; }
            set
            {
                _width = value;
                RaisePropertyChanged();
            }
        }

        public int ContentHeight
        {
            get { return _height; }
            set
            {
                _height = value;
                RaisePropertyChanged();
            }
        }

        public int MoveCounter
        {
            get { return _moveCounter; }
            set
            {
                _moveCounter = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Item> Items { get; set; }
        private Dictionary<string, Item> _itemDictionary;


        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            MoveElevator(_elevatorYPos - 1);
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            MoveElevator(_elevatorYPos + 1);
        }

        private void Item_Click(object sender, MouseButtonEventArgs e)
        {
            if (_gameOver) return;

            var tag = (sender as Label).Tag as string;
            int id = int.Parse(tag);
            var item = _itemDictionary[tag];

            if (id % Cols == 0) return;
            if (item.DisplayText == "") return;

            int row = id / Cols;
            if (row != _elevatorYPos) return;

            if (item.IsSelected)
                item.IsSelected = false;
            else
            {
                int selectedCount = _itemDictionary.Values.Sum(x => x.IsSelected ? 1 : 0);
                if (selectedCount < 2)
                    item.IsSelected = true;
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _elevatorYPos = Rows - 1;

            MoveCounter = 0;
            for (int y = 0; y < _initialState.Length; y++)
                for (int x = 0; x < _initialState[y].Length; x++)
                {
                    string id = (y * Cols + x).ToString();
                    _itemDictionary[id].DisplayText = _initialState[y][x];
                    if (x != 0)
                        _itemDictionary[id].BackgroundBrush = new SolidColorBrush(Colors.LightGray);
                }

            _gameOver = false;
        }

        public void MoveElevator(int newYPos)
        {
            if (_gameOver) return;

            int selectedCount = _itemDictionary.Values.Sum(x => x.IsSelected ? 1 : 0);
            if (selectedCount == 0) return;
            if (newYPos < 0) return;
            if (newYPos >= Rows) return;

            string id = (_elevatorYPos * Cols).ToString();
            _itemDictionary[id].DisplayText = "";
            int oldYPos = _elevatorYPos;
            _elevatorYPos = newYPos;
            id = (_elevatorYPos * Cols).ToString();
            _itemDictionary[id].DisplayText = "E";

            foreach (var selected in _itemDictionary.Values.Where(x => x.IsSelected))
            {
                int col = int.Parse(selected.Id) % Cols;
                var t = selected.DisplayText;
                selected.DisplayText = "";
                _itemDictionary[(newYPos * Cols + col).ToString()].DisplayText = t;
            }

            foreach (var item in _itemDictionary.Values) item.IsSelected = false;

            MoveCounter++;

            CheckForFriedMicrochips(oldYPos);
            CheckForFriedMicrochips(newYPos);
            CheckForFinishedState();
        }

        public void CheckForFinishedState()
        {
            bool finished = true;

            for (int i = 1; i < Cols; i++)
            {
                if (_itemDictionary[i.ToString()].DisplayText == string.Empty)
                {
                    finished = false;
                }
            }

            if (finished)
            {
                for (int i = 1; i < Cols; i++)
                    _itemDictionary[i.ToString()].BackgroundBrush = new SolidColorBrush(Colors.LightGreen);
                _gameOver = true;
            }
        }

        public void CheckForFriedMicrochips(int yPos)
        {
            List<Item> rowItems = new List<Item>();
            for (int x = 1; x < Cols; x++)
            {
                rowItems.Add(_itemDictionary[(yPos * Cols + x).ToString()]);
            }

            foreach (var item in rowItems)
            {
                if (item.DisplayText.Last() != "M")
                    continue;

                string id = item.DisplayText.AllButLast();

                bool friendlyGenerator = false;
                bool hostileGenerator = false;

                foreach (var v in rowItems)
                {
                    if (v.DisplayText.Last() == "G")
                    {
                        if (v.DisplayText.AllButLast() == id)
                            friendlyGenerator = true;
                        else
                            hostileGenerator = true;
                    }
                }

                if (hostileGenerator && !friendlyGenerator)
                {
                    item.BackgroundBrush = new SolidColorBrush(Colors.Red);
                    _gameOver = true;
                    return;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class StringExtension
    {
        public static string Last(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input.Substring(input.Length - 1, 1);
        }

        public static string AllButLast(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input.Substring(0, input.Length - 1);
        }
    }
}
