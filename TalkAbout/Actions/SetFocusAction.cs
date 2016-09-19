using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TalkAbout.Actions
{
    class SetFocusAction : DependencyObject, IAction
    {
        public static readonly DependencyProperty TargetObjectProperty = 
            DependencyProperty.Register(
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
                
                target = GetValue(TargetObjectProperty);

            }
            else
            {
                target = sender;
            }

            if (target != null)
            {
                if (target is TextBox)
                {
                    TextBox targetTextBox = (TextBox)target;
                    targetTextBox.Focus(FocusState.Programmatic);
                    result = true;
                }
                else if (target is Control)
                {
                    Control targetControl = (Control)target;
                    if (targetControl.Visibility == Visibility.Visible)
                    {
                        targetControl.Focus(FocusState.Programmatic);
                    }
                    result = true;
                }
            }

            return result;

        }
    }
}
