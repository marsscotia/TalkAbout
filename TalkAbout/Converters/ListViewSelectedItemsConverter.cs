using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace TalkAbout.Converters
{
    class ListViewSelectedItemsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Debug.WriteLine("ListViewSelectedItemsConverter.cs: Initial type is " + value.GetType().ToString());
            var listView = value as ListView;
            Debug.WriteLine("ListViewSelectedItemsConverter.cs: Converted type is " + listView.SelectedItems.GetType().ToString());
            Debug.WriteLine("ListViewSelectedItemsConverter.cs: Type of SelectedRanges is " + listView.SelectedRanges.GetType().ToString());
            return listView.SelectedItems;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
