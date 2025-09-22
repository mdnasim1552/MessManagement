using MessManagement.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessManagement.Converters
{
    public class IndexToMemberHeadingConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is MessMemberModel member && parameter is CollectionView collectionView)
            {
                if (collectionView.ItemsSource is IList<MessMemberModel> members)
                {
                    int index = members.IndexOf(member);
                    int number = index + 1;

                    return number switch
                    {
                        1 => "1st Member",
                        2 => "2nd Member",
                        3 => "3rd Member",
                        _ => $"{number}th Member"
                    };
                }
            }
            return "";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
