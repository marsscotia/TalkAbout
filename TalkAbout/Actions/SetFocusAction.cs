using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TalkAbout.Actions
{
    class SetFocusAction : DependencyObject, IAction
    {
        public static readonly DependencyProperty TargetObjectProperty = DependencyProperty.Register(
            "TargetObject",
            typeof(object),
            typeof(SetFocusAction),
            null);

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
                if (target.GetType() == typeof(TextBox))
                {
                    TextBox targetTextBox = (TextBox)target;
                    targetTextBox.Focus(FocusState.Programmatic);
                    result = true;
                }
            }

            return result;

        }
    }
}
