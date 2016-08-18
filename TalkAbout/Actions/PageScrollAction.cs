using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace TalkAbout.Actions
{
    /// <summary>
    /// Class provides a page scrolling action for a scrollviewer.
    /// 
    /// Up or Down must be set to true to provide a direction for the 
    /// scroll. 
    /// </summary>
    class PageScrollAction : DependencyObject, IAction
    {
        public static readonly DependencyProperty TargetObjectProperty =
            DependencyProperty.Register("TargetObject",
                typeof(object),
                typeof(PageScrollAction),
                null);

        public bool Up { get; set; } = false;
        public bool Down { get; set; } = false;

        public object TargetObject
        {
            get
            {
                return GetValue(TargetObjectProperty);
            }
            set
            {
                SetValue(TargetObjectProperty, value);
            }
        }

        public object Execute(object sender, object parameter)
        {
            bool result = false;
            object target;
            if (ReadLocalValue(TargetObjectProperty) != DependencyProperty.UnsetValue)
            {
                target = TargetObject;
            }
            else
            {
                target = sender;
            }

            if (target != null)
            {
                if (target.GetType() == typeof(SemanticZoom))
                {
                    SemanticZoom semanticZoom = (SemanticZoom)target;
                    ListViewBase view = null;
                    ScrollViewer scroll = null;
                    if (semanticZoom.IsZoomedInViewActive)
                    {
                        view = (ListViewBase)semanticZoom.ZoomedInView;
                    }
                    else
                    {
                        view = (ListViewBase)semanticZoom.ZoomedOutView;
                    }
                    scroll = _getScrollViewer(view);

                    if (Up)
                    {
                        if (scroll.VerticalOffset > scroll.ViewportHeight / 2)
                        {
                            scroll.ChangeView(null, scroll.VerticalOffset - (scroll.ViewportHeight / 2), null);
                            
                        }
                        else
                        {
                            scroll.ChangeView(null, 0, null);
                        }
                        result = true;
                    }
                    if (Down)
                    {
                        if (scroll.ScrollableHeight - scroll.VerticalOffset > 0)
                        {
                            if (scroll.ScrollableHeight - scroll.VerticalOffset > scroll.ViewportHeight / 2)
                            {
                                scroll.ChangeView(null, scroll.VerticalOffset + (scroll.ViewportHeight / 2), null);
                            }
                            else
                            {
                                scroll.ChangeView(null, scroll.ScrollableHeight, null);
                            } 
                        }
                        
                        result = true;
                    }

                }
            }

            

            return result;
        }

        private ScrollViewer _getScrollViewer(DependencyObject element)
        {
            ScrollViewer viewer = null;

            if (element is ScrollViewer)
            {
                viewer = (ScrollViewer)element;
            }
            else
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {
                    var child = VisualTreeHelper.GetChild(element, i);
                    var result = _getScrollViewer(child);
                    if (result != null)
                    {
                        viewer = result;
                    }
                }
            }
        

            return viewer;
        }
    }
}
