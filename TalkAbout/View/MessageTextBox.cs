using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TalkAbout.View
{
    /// <summary>
    /// This class extends the system TextBox, to allow
    /// properties such as SelectionStart and Length to be
    /// bound.
    /// </summary>
    public class MessageTextBox: TextBox
    {
        public static readonly DependencyProperty BindableSelectionStartProperty =
            DependencyProperty.Register(
                "BindableSelectionStart",
                typeof(int),
                typeof(MessageTextBox),
                new PropertyMetadata(null, OnBindableSelectionStartChanged));

        public static readonly DependencyProperty BindableSelectionLengthProperty =
            DependencyProperty.Register(
                "BindableSelectionLength",
                typeof(int),
                typeof(MessageTextBox),
                new PropertyMetadata(null, OnBindableSelectionLengthChanged));

        private bool changeFromUI;

        public MessageTextBox(): base()
        {
            this.SelectionChanged += this.OnSelectionChanged;
        }

        public int BindableSelectionStart
        {
            get
            {
                return (int)GetValue(BindableSelectionStartProperty);
            }
            set
            {
                SetValue(BindableSelectionStartProperty, value);
            }
        }

        public int BindableSelectionLength
        {
            get
            {
                return (int)GetValue(BindableSelectionLengthProperty);
            }
            set
            {
                SetValue(BindableSelectionLengthProperty, value);
            }
        }

        private static void OnBindableSelectionStartChanged(DependencyObject dependencyObject, 
            DependencyPropertyChangedEventArgs args)
        {
            var textBox = dependencyObject as MessageTextBox;

            if (!textBox.changeFromUI)
            {
                int newValue = (int)args.NewValue;
                textBox.SelectionStart = newValue;
            }
            else
            {
                textBox.changeFromUI = false;
            }
        }

        private static void OnBindableSelectionLengthChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            var textBox = dependencyObject as MessageTextBox;

            if (!textBox.changeFromUI)
            {
                int newValue = (int)args.NewValue;
                textBox.SelectionLength = newValue;
            }
            else
            {
                textBox.changeFromUI = false;
            }
        }

        private void OnSelectionChanged(object sender, RoutedEventArgs args)
        {
            if (BindableSelectionStart != SelectionStart)
            {
                changeFromUI = true;
                BindableSelectionStart = SelectionStart;
            }
            if (BindableSelectionLength != SelectionLength)
            {
                changeFromUI = true;
                BindableSelectionLength = SelectionLength;
            }
        }
        
    }
}
