using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace app22.Classes
{
    public class DateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DateTime.TryParse(value?.ToString(), out DateTime date))
                return date.ToString("dd/MM/yyyy");

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
