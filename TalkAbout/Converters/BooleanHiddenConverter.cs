using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace TalkAbout.Converters
{
    public class BooleanHiddenConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {

            Visibility result;
            if ((bool)value)
            {
                result = Visibility.Visible;
            }
            else
            {
                result = Visibility.Collapsed;
            }
            return result;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
