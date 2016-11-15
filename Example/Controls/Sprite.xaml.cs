using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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
        List<BitmapImage> costumes;
        int? nextcostumeindex = null;
        double? heading = null;

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
            All.Add(this);
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
        public async Task SetCostume(ImageSource source)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                Costume.Source = source;
            });
        }

        public async Task SetCostumes(params string[] assets)
        {
            try
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                {
                    var images = assets.Select(asset => new BitmapImage(new Uri($"ms-appx:///Assets/{asset}")));
                    costumes = new List<BitmapImage>(images);
                });
                nextcostumeindex = 1;

                await SetCostume(costumes.First());

            }
            catch (Exception ex)
            {

            }


        }

        public async Task NextCostume()
        {
            var costume = nextcostumeindex.Value;
            if (costume >= costumes.Count)
                costume = 0;
            nextcostumeindex = costume + 1;
            await SetCostume(costumes[costume]);
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
            var sem = new SemaphoreSlim(1);
            await sem.WaitAsync();
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                SetValue(Canvas.LeftProperty, x);
                SetValue(Canvas.TopProperty, y);
                sem.Release();
            });
            await sem.WaitAsync();
        }
        public async Task SetPosition(Point where)
        {
            await SetPosition(where.X, where.Y);
        }

        public async Task<Point> GetPosition()
        {
            var result = new Point();

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                result.X = (double)GetValue(Canvas.LeftProperty);
                result.Y = (double)GetValue(Canvas.TopProperty);
            });

            return result;
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

        public async Task PointTowards(FrameworkElement fe)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var target = new Point();
                target.X = (double)fe.GetValue(Canvas.LeftProperty);
                target.Y = (double)fe.GetValue(Canvas.TopProperty);

                var current = new Point();
                current.X = (double)GetValue(Canvas.LeftProperty);
                current.Y = (double)GetValue(Canvas.TopProperty);

                heading = HeadingBetween(current, target);
            });
        }

        /// <summary>
        /// Point the sprite in the direction indicated
        /// </summary>
        /// <remarks>
        /// Note that all angles are in degrees where zero is up
        /// </remarks>
        /// <param name="angle">Angle to point, in degrees, where zero is up</param>
        /// <returns></returns>
        public async Task PointInDirection_Heading(double angle)
        {
            angle -= 90;
            var radians = angle * Math.PI / 180.0;
            heading = radians;
        }

        public async Task Move(double steps)
        {
            if (!heading.HasValue)
                return;

            var current = await GetPosition();
            var moveto = ProgressToward(current, heading.Value, steps);
            await SetPosition(moveto);
        }

        public async Task IfOnEdgeBounce()
        {
            if (!heading.HasValue)
                return;

            var position = await GetPosition();
            var needstomove = false;

            if (position.X < 10.0)
            {
                needstomove = true;
                position.X = 10.0;
                heading = Math.PI - heading;
            }
            if (position.X > 990.0)
            {
                needstomove = true;
                position.X = 990.0;
                heading = Math.PI - heading;
            }
            if (position.Y < 10.0)
            {
                needstomove = true;
                position.Y = 10.0;
                heading = - heading;
            }
            if (position.Y > 490.0)
            {
                needstomove = true;
                position.Y = 490.0;
                heading = - heading;
            }
            if (needstomove)
                await SetPosition(position);
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
        public async Task PointInDirection_Rotate(double degrees)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                Rotate.CenterX = ActualWidth / 2;
                Rotate.CenterY = ActualHeight / 2;
                Rotate.Angle = degrees;
            });
        }

        /// <summary>
        /// Arguments which are passed in for a MessageReceived event
        /// </summary>
        public struct MessageReceivedArgs
        {
            /// <summary>
            /// The contents of the message
            /// </summary>
            public string message;
        }

        /// <summary>
        /// Event fired when we received a message
        /// </summary>
        public event EventHandler<MessageReceivedArgs> MessageReceived;

        /// <summary>
        /// Broadcast this message to all sprites
        /// </summary>
        /// <remarks>
        /// To handle this, create a MessageReceived event handler
        /// </remarks>
        /// <param name="message"></param>
        public static void Broadcast(string message)
        {
            foreach (var sprite in All)
                sprite.MessageReceived?.Invoke(sprite, new MessageReceivedArgs() { message = message });
        }

        private static List<Sprite> All = new List<Sprite>();

        public async Task Glide(double total_time,Point destination)
        {
            Point starting_position = await GetPosition();
            const double update_frequency_ms = 50;
            double elapsed_time = 0;
            double total_distance = DistanceBetween(starting_position, destination);
            double heading = HeadingBetween(starting_position, destination);
            while(elapsed_time < total_time)
            {
                // distance we should be at THIS moment
                double now_distance = elapsed_time * total_distance / total_time;

                // new position is along the heading by that far
                Point new_position = ProgressToward(starting_position,heading,now_distance);

                await this.SetPosition(new_position.X, new_position.Y);
                await Task.Delay((int)update_frequency_ms);

                elapsed_time += update_frequency_ms;
            }
            await this.SetPosition(destination.X, destination.Y);
        }

        private static double DistanceBetween(Point first, Point second)
        {
            var x_distance = second.X - first.X;
            var y_distance = second.Y - first.Y;

            return Math.Sqrt(x_distance * x_distance + y_distance * y_distance);
        }

        private static double HeadingBetween(Point first,Point second)
        {
            var x_distance = second.X - first.X;
            var y_distance = second.Y - first.Y;

            return Math.Atan2(y_distance, x_distance);
        }

        private static Point ProgressToward(Point from,double heading,double distance)
        {
            var x_distance = Math.Cos(heading) * distance;
            var y_distance = Math.Sin(heading) * distance;

            return new Point( from.X + x_distance, from.Y + y_distance);
        }
    }
}
