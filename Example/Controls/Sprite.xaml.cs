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
        MediaPlayer player = new MediaPlayer();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// You don't need to call this yourself. Simply including a sprite in XAML will
        /// construct it.
        /// </remarks>
        public Sprite()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Set the current costume of the sprite
        /// </summary>
        /// <param name="asset">File name of an image file in the Assets directory</param>
        /// <returns>Awaitable task</returns>
        public async Task SetCostume(string asset)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                Costume.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{asset}"));
            });
        }

        /// <summary>
        /// Show the sprite on the screen
        /// </summary>
        /// <returns></returns>
        public async Task Show()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                Visibility = Visibility.Visible;
            });
        }

        /// <summary>
        /// Hide the sprite so it doesn't show
        /// </summary>
        /// <returns></returns>
        public async Task Hide()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                Visibility = Visibility.Collapsed;
            });
        }

        /// <summary>
        /// Set the position of the sprite within the scene
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public async Task SetPosition(double x,double y)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                SetValue(Canvas.LeftProperty, x);
                SetValue(Canvas.TopProperty, y);
            });
        }

        /// <summary>
        /// Move the sprite down (or up) by this amount
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public async Task ChangeYby(double y)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                SetValue(Canvas.TopProperty, (double)GetValue(Canvas.TopProperty) + y);
            });
        }

        /// <summary>
        /// Move the sprite right (or left) by this amount
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public async Task ChangeXby(double y)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                SetValue(Canvas.LeftProperty, (double)GetValue(Canvas.LeftProperty) + y);
            });
        }

        /// <summary>
        /// Test whether this sprite is touching another sprite
        /// </summary>
        /// <param name="fe2">Which sprite to test against</param>
        /// <returns></returns>
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

        /// <summary>
        /// Play a particular soound
        /// </summary>
        /// <param name="asset">Name of a WAV file in the Assets directory</param>
        /// <returns></returns>
        public async Task PlaySound(string asset)
        {
            player.Source = MediaSource.CreateFromUri(new Uri($"ms-appx:///Assets/{asset}"));
            player.Play();
        }

        public async Task Say(string text = null)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                if (text == null)
                {
                    SayTextContainer.Visibility = Visibility.Collapsed;
                    SayText.Text = string.Empty;
                }
                else
                {
                    SayText.Text = text;
                    SayTextContainer.Visibility = Visibility.Visible;
                }
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

        public enum Direction { None=0,Left,Right } ;

        public async Task TurnBy(Direction direction, double degrees)
        {
            if (direction == Direction.Left)
                degrees = - degrees;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                Rotate.CenterX = ActualWidth / 2;
                Rotate.CenterY = ActualHeight / 2;
                Rotate.Angle += degrees;
            });
        }
        public async Task PointInDirection(double degrees)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                Rotate.CenterX = ActualWidth / 2;
                Rotate.CenterY = ActualHeight / 2;
                Rotate.Angle = degrees;
            });
        }
    }
}
