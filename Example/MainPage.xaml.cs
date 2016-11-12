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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Example
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Down)
            {
                var top = (double)Astro_Cat.GetValue(Canvas.TopProperty);
                Astro_Cat.SetValue(Canvas.TopProperty, top + 15);
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Up)
            {
                var top = (double)Astro_Cat.GetValue(Canvas.TopProperty);
                Astro_Cat.SetValue(Canvas.TopProperty, top - 15);
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Left)
            {
                var left = (double)Astro_Cat.GetValue(Canvas.LeftProperty);
                Astro_Cat.SetValue(Canvas.LeftProperty, left - 15);
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Right)
            {
                var left = (double)Astro_Cat.GetValue(Canvas.LeftProperty);
                Astro_Cat.SetValue(Canvas.LeftProperty, left + 15);
            }
        }

    }
}
