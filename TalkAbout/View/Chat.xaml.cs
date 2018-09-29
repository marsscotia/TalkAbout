using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TalkAbout.View;
using TalkAbout.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TalkAbout
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            _viewModel = new ViewModelChat();
            ChatPage.DataContext = ViewModel;
            _viewModel.VMMessage.VoiceBox.Media = Media;
        }

        private ViewModelChat _viewModel;

        public Command NavigateToSettingsCommand
        {
            get
            {
                return new Command(NavigateToSettings);
            }
        }

        public Command NavigateToAbbreviationsCommand
        {
            get
            {
                return new Command(NavigateToAbbreviations);
            }
        }

        public Command NavigateToPronunciationsCommand
        {
            get
            {
                return new Command(NavigateToPronunciations);
            }
        }

        public ViewModelChat ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToSettings();
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            NavigateToAbbreviations();
        }

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            NavigateToPronunciations();
        }

        private void PhraseSelected(object sender, ItemClickEventArgs e)
        {
            ViewModel.PhraseSelected((ViewModelPhrase)e.ClickedItem);
        }

        private void NavigateToSettings()
        {
            NavigateTo(typeof(Settings));
        }

        private void NavigateToAbbreviations()
        {
            NavigateTo(typeof(Abbreviations));
        }

        private void NavigateToPronunciations()
        {
            NavigateTo(typeof(Pronunciations));
        }

        private void NavigateTo(Type aDestination)
        {
            Frame root = Window.Current.Content as Frame;
            root.Navigate(aDestination);
        }
        
    }
}
