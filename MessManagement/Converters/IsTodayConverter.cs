using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessManagement.Converters
{
    public class IsTodayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            DateTime date;

            // Handle different possible types
            switch (value)
            {
                case DateTime dt:
                    date = dt.Date;
                    break;
                case DateTimeOffset dto:
                    date = dto.Date;
                    break;
                case string s:
                    if (!DateTime.TryParse(s, out date))
                        return false;
                    date = date.Date;
                    break;
                default:
                    // Try parsing via ToString() as a last resort
                    if (!DateTime.TryParse(value.ToString(), out date))
                        return false;
                    date = date.Date;
                    break;
            }

            return date == DateTime.Today;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
