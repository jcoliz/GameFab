using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Example.Controls
{
    public sealed partial class Sprite : UserControl
    {
        Random random = new Random();

        public Sprite()
        {
            this.InitializeComponent();
        }

        public async Task SetCostume(string asset)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                Costume.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{asset}"));
            });
        }

        public async Task Show()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                Visibility = Visibility.Visible;
            });
        }
        public async Task Hide()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                Visibility = Visibility.Collapsed;
            });
        }

        public async Task SetPosition(double x,double y)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                SetValue(Canvas.LeftProperty, x);
                SetValue(Canvas.TopProperty, y);
            });
        }

        public async Task ChangeYby(double y)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                SetValue(Canvas.TopProperty, (double)GetValue(Canvas.TopProperty) + y);
            });
        }
        public async Task ChangeXby(double y)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                SetValue(Canvas.LeftProperty, (double)GetValue(Canvas.LeftProperty) + y);
            });
        }

        public async Task<bool>IsTouching(FrameworkElement fe2)
        {
            try
            {
                Rect rect1;
                Rect rect2;

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                {
                    if (Visibility == Visibility.Visible)
                        rect1 = new Rect((double)GetValue(Canvas.LeftProperty), (double)GetValue(Canvas.TopProperty), ActualWidth, ActualHeight);
                    else
                        rect1 = Rect.Empty;

                    if (fe2.Visibility == Visibility.Visible)
                        rect2 = new Rect((double)fe2.GetValue(Canvas.LeftProperty), (double)fe2.GetValue(Canvas.TopProperty), fe2.ActualWidth, fe2.ActualHeight);
                    else
                        rect2 = Rect.Empty;
                });

                rect1.Intersect(rect2);

                return !rect1.IsEmpty;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        MediaPlayer player = new MediaPlayer();

        public async Task PlaySound(string asset)
        {
            player.Source = MediaSource.CreateFromUri(new Uri($"ms-appx:///Assets/{asset}"));
            player.Play();
        }

        public async Task Say(string text)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, async () =>
            {
                await new MessageDialog(text).ShowAsync();
            });
        }

        public async Task<double> ReduceOpacityBy(double value)
        {
            double opacity = 1.0;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                opacity = this.Opacity;
                opacity -= value;
                if (opacity < 0)
                    opacity = 0;
                this.Opacity = opacity;
            });

            return opacity;
        }
    }
}
