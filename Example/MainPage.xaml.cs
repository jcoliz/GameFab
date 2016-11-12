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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Example
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MediaPlayer bgplayer;

        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            //bgplayer  = new MediaPlayer() { AutoPlay = true, IsLoopingEnabled = true };
            //bgplayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/02/Techno.wav"));
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

        private void Astro_Cat_Loaded(object sender, RoutedEventArgs e)
        {
            var me = sender as FrameworkElement;

            Task.Run(async () =>
            {
                while (true)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                    {
                        var top = (double)me.GetValue(Canvas.TopProperty);
                        me.SetValue(Canvas.TopProperty, top + 2);
                    });
                    await Task.Delay(300);
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                    {
                        var top = (double)me.GetValue(Canvas.TopProperty);
                        me.SetValue(Canvas.TopProperty, top - 2);
                    });
                    await Task.Delay(300);
                }
            });

            Task.Run(async () => 
            {
                while (true)
                {
                    if (await IsTouching(me, Lightning))
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                        {
                            var player = new MediaPlayer() { AutoPlay = true };
                            player.MediaEnded += (s, a) => { player.Dispose(); };
                            player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/02/Zap.wav"));

                            Lightning.Visibility = Visibility.Collapsed;
                            if (me.Opacity >= 0.2)
                                me.Opacity -= 0.2;
                            if (me.Opacity < 0.2)
                            {
                                me.Visibility = Visibility.Collapsed;
                                new MessageDialog("You lose!!!").ShowAsync();
                            }
                        });
                    }
                    else
                        await Task.Delay(200);
                }
            });
        }

        private void Banner_Loaded(object sender, RoutedEventArgs e)
        {
            var me = sender as FrameworkElement;

            Task.Run(async () =>
            {
                int i = 3;
                while (i-- > 0)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                    {
                        me.Visibility = Visibility.Visible;
                    });
                    await Task.Delay(1000);
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                    {
                        me.Visibility = Visibility.Collapsed;
                    });
                    await Task.Delay(500);
                }
            });

        }

        private void Lightning_Loaded(object sender, RoutedEventArgs e)
        {
            var me = sender as FrameworkElement;

            Task.Run(async () => 
            {
                var random = new Random();
                await Task.Delay(1000);
                while(true)
                {
                    await Task.Delay((int)(random.NextDouble() * 1500.0));

                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                    {
                        me.SetValue(Canvas.LeftProperty, (int)(random.NextDouble() * 1000));
                        me.SetValue(Canvas.TopProperty, 10);
                        me.Visibility = Visibility.Visible;
                    });

                    var i = 8;
                    while(i-- > 0)
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                        {
                            var top = (double)me.GetValue(Canvas.TopProperty);
                            me.SetValue(Canvas.TopProperty, top + 40);
                        });
                        await Task.Delay(300);
                    }

                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                    {
                        me.Visibility = Visibility.Collapsed;
                    });


                }
            });
        }

        private void String_Loaded(object sender, RoutedEventArgs e)
        {
            var me = sender as Image;

            Task.Run(async () =>
            {
                var random = new Random();
                await Task.Delay(1000);

                int i = 1;
                while (i-- > 0)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                    {
                        me.SetValue(Canvas.LeftProperty, (int)(random.NextDouble() * 1000));
                        me.SetValue(Canvas.TopProperty, (int)(random.NextDouble() * 500));
                        me.Visibility = Visibility.Visible;
                    });

                    while (!await IsTouching(me, Astro_Cat))
                    {
                        await Task.Delay(200);
                    }

                    var player = new MediaPlayer() { AutoPlay = true };
                    player.MediaEnded += (s, a) => { player.Dispose(); };
                    player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/02/Humming.wav"));

                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, async () =>
                    {
                        me.Visibility = Visibility.Collapsed;
                        await new MessageDialog("Got it!").ShowAsync();
                    });

                    await Task.Delay(300);
                }

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, async () =>
                {
                    me.Source = new BitmapImage(new Uri("ms-appx:///Assets/02/2.png"));
                    me.Visibility = Visibility.Visible;
                    await new MessageDialog("Stargate Opened!!!").ShowAsync();
                });
            });
        }

        private void Player_MediaEnded(MediaPlayer sender, object args)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> IsTouching(FrameworkElement fe1, FrameworkElement fe2)
        {
            try
            {
                Rect rect1;
                Rect rect2;

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                {
                    if (fe1.Visibility == Visibility.Visible)
                        rect1 = new Rect((double)fe1.GetValue(Canvas.LeftProperty), (double)fe1.GetValue(Canvas.TopProperty), fe1.ActualWidth, fe1.ActualHeight);
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
    }
}
