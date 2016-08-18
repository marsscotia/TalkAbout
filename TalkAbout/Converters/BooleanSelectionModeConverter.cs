using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace TalkAbout.Converters
{
    class BooleanSelectionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ListViewSelectionMode selectionMode;
            if ((bool)value)
            {
                selectionMode = ListViewSelectionMode.Multiple;
            }
            else
            {
                selectionMode = ListViewSelectionMode.Single;
            }
            return selectionMode;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
