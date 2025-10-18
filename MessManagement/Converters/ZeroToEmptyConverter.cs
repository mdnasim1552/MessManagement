using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessManagement.Converters
{
    public class ZeroToEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            if (decimal.TryParse(value.ToString(), out decimal number))
            {
                return number == 0 ? string.Empty : number.ToString("0.##");
            }

            return string.Empty;
        }

        // Save back to property: if empty, set 0
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
                return 0m; // default 0

            if (decimal.TryParse(value.ToString(), out decimal number))
                return number;

            return 0m;
        }
    }
}
