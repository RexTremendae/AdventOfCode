using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace Day11
{
    public class Item : INotifyPropertyChanged
    {
        public Item(string id)
        {
            Id = id;
        }

        FontWeight _fontWeight;
        private SolidColorBrush _outerBorderBrush;
        private SolidColorBrush _innerBorderBrush;
        private SolidColorBrush _backgroundBrush;
        private bool _isSelected;
        private string _displayText;

        public string DisplayText
        {
            get { return _displayText; }
            set
            {
                _displayText = value;
                RaisePropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (_isSelected)
                {
                    OuterBorderBrush = new SolidColorBrush(Colors.Black);
                    InnerBorderBrush = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    OuterBorderBrush = new SolidColorBrush(Colors.Transparent);
                    InnerBorderBrush = new SolidColorBrush(Colors.Gray);
                }
            }
        }

        public SolidColorBrush BackgroundBrush
        {
            get { return _backgroundBrush; }
            set { _backgroundBrush = value; RaisePropertyChanged(); }
        }

        public SolidColorBrush OuterBorderBrush
        {
            get { return _outerBorderBrush; }
            set { _outerBorderBrush = value; RaisePropertyChanged(); }
        }

        public SolidColorBrush InnerBorderBrush
        {
            get { return _innerBorderBrush; }
            set { _innerBorderBrush = value; RaisePropertyChanged(); }
        }

        public FontWeight FontWeight
        {
            get { return _fontWeight; }
            set { _fontWeight = value; RaisePropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Id { get; }

        internal void ToggleSelected()
        {
            IsSelected = _isSelected ? false : true;
        }
    }
}
