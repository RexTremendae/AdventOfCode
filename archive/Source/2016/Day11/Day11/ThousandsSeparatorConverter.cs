using System;
using System.Globalization;
using System.Windows.Data;

namespace Day11
{
    public class ThousandsSeparatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value.ToString();

            string result = string.Empty;

            for (int i = str.Length; i >= 0; i -= 3)
            {
                if (i >= 3)
                    result = str.Substring(i - 3, 3) + " " + result;
                else
                    result = str.Substring(0, i) + " " + result;
            }

            return result.Trim();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
