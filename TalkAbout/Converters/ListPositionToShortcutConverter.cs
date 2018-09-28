using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkAbout.ViewModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace TalkAbout.Converters
{
    class ListPositionToShortcutConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ListViewItemPresenter item)
            {


                var lv = FindAncestor<ListView>(item);
                var lvi = FindAncestor<ListViewItem>(item);
                var lvife = item as FrameworkElement;
                var vmp = lvife.DataContext as ViewModelPhrase;
                if (lv != null && lvi != null)
                {
                    int index = lv.IndexFromContainer(lvi);
                    if (index < 9)
                    {
                        return "alt + " + (index + 1);
                    }
                    else if (index == 9)
                    {
                        return "alt + 0";
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public T FindAncestor<T>(DependencyObject from) where T: class
        {
            if (from == null)
            {
                return null;
            }

            var candidate = from as T;
            return candidate ?? FindAncestor<T>(VisualTreeHelper.GetParent(from));
        }

        /// <summary>
        /// Method finds first descendant in visual tree of type T, or returns null if
        /// none found.
        /// </summary>
        /// <typeparam name="T">The type of descendant to be found</typeparam>
        /// <param name="parent">The element from which to begin descending the visual tree</param>
        /// <returns></returns>
        public T FindDescendant<T>(DependencyObject parent) where T: class
        {
            return AllChildren(parent).Where(c => c is T).FirstOrDefault() as T;
        }

        public static List<DependencyObject> AllChildren(DependencyObject parent)
        {
            List<DependencyObject> list = new List<DependencyObject>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is DependencyObject)
                {
                    list.Add(child);
                }
                list.AddRange(AllChildren(child));
            }
            return list;
        }

        public childItem FindVisualChild<childItem>(DependencyObject obj)
    where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
    }
}
