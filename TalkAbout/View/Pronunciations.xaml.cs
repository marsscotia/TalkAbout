﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TalkAbout.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TalkAbout.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Pronunciations : Page
    {
        private ViewModelPronunciations _viewModel;

        public Command GoBackCommand
        {
            get
            {
                return new Command(GoBack);
            }
        }

        public ViewModelPronunciations ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

        public Pronunciations()
        {
            this.InitializeComponent();
            _viewModel = new ViewModelPronunciations();
            PronunciationsPage.DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Frame root = Window.Current.Content as Frame;
            if (root.CanGoBack)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            
        }

        private void GoBack()
        {
            Frame root = Window.Current.Content as Frame;
            if (root.CanGoBack)
            {
                root.GoBack();
            }
        }
    }
}
