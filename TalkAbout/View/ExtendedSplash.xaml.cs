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
using Windows.UI.Core;
using Windows.ApplicationModel.Activation;
using System.Threading.Tasks;
using TalkAbout.ViewModel;
using TalkAbout.Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TalkAbout.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtendedSplash : Page
    {
        internal Rect splashImageRect;
        private SplashScreen splash;
        internal bool dismissed = false;
        internal Frame rootFrame;

        private Categories _categories;
        private Model.Abbreviations _abbreviations;
        private Model.Pronunciations _pronunciations;



        public ExtendedSplash(SplashScreen splashscreen, bool loadState)
        {
            this.InitializeComponent();

            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(ExtendedSplash_OnResize);

            splash = splashscreen;

            if (splash != null)
            {
                splash.Dismissed += new TypedEventHandler<SplashScreen, object>(DismissedEventHandler);

                splashImageRect = splash.ImageLocation;
                PositionImage();

                PositionRing();
            }

            rootFrame = new Frame();
            
            _categories = Categories.Instance;
            _abbreviations = Model.Abbreviations.Instance;
            _pronunciations = Model.Pronunciations.Instance;
            
        }
        
        void PositionImage()
        {
            extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
            extendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y);
            extendedSplashImage.Height = splashImageRect.Height;
            extendedSplashImage.Width = splashImageRect.Width;
        }

        void PositionRing()
        {
            splashProgressRing.SetValue(Canvas.LeftProperty, splashImageRect.X + (splashImageRect.Width * 0.5) - (splashProgressRing.Width * 0.5));
            splashProgressRing.SetValue(Canvas.TopProperty, splashImageRect.Y + splashImageRect.Height + splashImageRect.Height * 0.1);
        }

        private async void DismissedEventHandler(SplashScreen sender, object args)
        {
            dismissed = true;
            await _loadFiles();
            await DismissExtendedSplash();
        

        }
        
        private async Task _loadFiles()
        {
            await _categories.LoadCategoriesFromFile();
            await _abbreviations.LoadAbbreviationsFromFile();
            await _pronunciations.LoadPronunciationsFromFile();
        }

        private void ExtendedSplash_OnResize(object sender, WindowSizeChangedEventArgs e)
        {
            if (splash != null)
            {
                splashImageRect = splash.ImageLocation;
                PositionImage();
                PositionRing();
            }
        }



        async Task DismissExtendedSplash()
        {

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                rootFrame.Navigate(typeof(MainPage));
                Window.Current.Content = rootFrame;
            });

            
        }

       
    }
}
