using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using TalkAbout.Model;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TalkAbout.View
{
    public sealed partial class ShortcutPanel : UserControl
    {
        public static readonly DependencyProperty CommandLabelProperty =
            DependencyProperty.Register("CommandLabel", typeof(string), typeof(ShortcutPanel), null);

        public static readonly DependencyProperty CommandShortcutProperty =
            DependencyProperty.Register("CommandShortcut", typeof(string), typeof(ShortcutPanel), null);

        public static readonly DependencyProperty SymbolTextProperty =
            DependencyProperty.Register("SymbolText", typeof(string), typeof(ShortcutPanel), null);

        public static readonly DependencyProperty PlainTextProperty =
            DependencyProperty.Register("PlainText", typeof(string), typeof(ShortcutPanel), null);

        private const int _iconsAndLabels = 1;
        private const int _iconsOnly = 2;
        private const int _labelsOnly = 3;

        public string CommandLabel
        {
            get
            {
                return (string)GetValue(CommandLabelProperty);
            }
            set
            {
                SetValue(CommandLabelProperty, value);
            }
        }

        public string CommandShortcut
        {
            get
            {
                return (string)GetValue(CommandShortcutProperty);
            }
            set
            {
                SetValue(CommandShortcutProperty, value);
            }
        }

        public string SymbolText
        {
            get
            {
                return (string)GetValue(SymbolTextProperty);

            }
            set
            {
                SetValue(SymbolTextProperty, value);
            }
        }

        public string PlainText
        {
            get
            {
                return (string)GetValue(PlainTextProperty);
            }
            set
            {
                SetValue(PlainTextProperty, value);
            }
        }

        public bool ShortcutVisible
        {
            get
            {
                return _settings.ShowShortcuts;
            }
            set
            {
                _settings.ShowShortcuts = value;
            }
        }

        public bool IconVisible
        {
            get
            {
                bool result = true;
                int showIconsAndLabels = _settings.ShowIconsAndLabels;
                if (showIconsAndLabels == _iconsAndLabels || showIconsAndLabels == _iconsOnly)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
                return result;
            }
        }

        public bool LabelVisible
        {
            get
            {
                bool result = true;
                int showIconsAndLabels = _settings.ShowIconsAndLabels;
                if (showIconsAndLabels == _iconsAndLabels || showIconsAndLabels == _labelsOnly)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
                return result;
            }
        }

        private Model.Settings _settings;

        public ShortcutPanel()
        {
            this.InitializeComponent();
            _settings = Model.Settings.Instance;
        }
    }
}
