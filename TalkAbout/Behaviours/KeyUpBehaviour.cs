using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.System;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Input;
using Windows.UI.Core;
using System.Diagnostics;

namespace TalkAbout.Behaviours
{
    [ContentProperty(Name = nameof(Actions))]
    [TypeConstraint(typeof(UIElement))]
    public class KeyUpBehaviour : DependencyObject, IBehavior
    {        
        public DependencyObject AssociatedObject
        { get; private set; }

        public VirtualKey Key { get; set; } = VirtualKey.None;
        public bool AndControl { get; set; } = false;
        public bool AndShift { get; set; } = false;
        public bool AndAlt { get; set; } = false;

        public Kinds Event { get; set; } = Kinds.KeyUp;

        public enum Kinds { KeyUp, KeyDown }

        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObject = associatedObject;
            if (Event == Kinds.KeyUp)
            {
                (AssociatedObject as UIElement).KeyUp += UIElement_KeyHandler;
            }
            else
            {
                (AssociatedObject as UIElement).KeyDown += UIElement_KeyHandler;
            }
        }

        public void Detach()
        {
            (AssociatedObject as UIElement).KeyUp -= UIElement_KeyHandler;
            (AssociatedObject as UIElement).KeyDown -= UIElement_KeyHandler;
            AssociatedObject = null;
        }

        private void UIElement_KeyHandler(object sender, KeyRoutedEventArgs e)
        {
            bool execute = false;

            if (e.Key == Key)
            {
                execute = true;

                if (AndAlt)
                {
                    var altKeyState = Window.Current.CoreWindow.GetKeyState(VirtualKey.Menu);
                    var alt = (altKeyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
                    if (!alt)
                    {
                        execute = false;
                    }
                }

                if (AndControl)
                {
                    var controlKeyState = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
                    var control = (controlKeyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
                    if (!control)
                    {
                        execute = false;
                    } 
                }

                if (AndShift)
                {
                    var shiftKeyState = Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift);
                    var shift = (shiftKeyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
                    if (!shift)
                    {
                        execute = false;
                    }
                }
            }

            if (execute)
            {
                Interaction.ExecuteActions(AssociatedObject, Actions, null);
                e.Handled = true;
            }
        }

        public ActionCollection Actions
        {
            get
            {
                var actions = (ActionCollection)base.GetValue(ActionsProperty);
                if (actions == null)
                {
                    SetValue(ActionsProperty, actions = new ActionCollection());
                }
                return actions;
            }
        }

        public readonly DependencyProperty ActionsProperty = 
            DependencyProperty.Register(nameof(Actions), typeof(ActionCollection), typeof(KeyUpBehaviour), new PropertyMetadata(null));
    }
}
