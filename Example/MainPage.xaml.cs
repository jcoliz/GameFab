using Example.Controls;
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
                // Change Y by +15
                var top = (double)Astro_Cat.GetValue(Canvas.TopProperty);
                Astro_Cat.SetValue(Canvas.TopProperty, top + 15);
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Up)
            {
                // Change Y by -15
                var top = (double)Astro_Cat.GetValue(Canvas.TopProperty);
                Astro_Cat.SetValue(Canvas.TopProperty, top - 15);
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Left)
            {
                // Change X by -15
                var left = (double)Astro_Cat.GetValue(Canvas.LeftProperty);
                Astro_Cat.SetValue(Canvas.LeftProperty, left - 15);
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Right)
            {
                // Change X by +15
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
                    // Change Y by +2
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                    {
                        var top = (double)me.GetValue(Canvas.TopProperty);
                        me.SetValue(Canvas.TopProperty, top + 2);
                    });
                    await Task.Delay(300);
                    // Change Y by -2
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
                            // Play sound Zap.wav
                            var player = new MediaPlayer() { AutoPlay = true };
                            player.MediaEnded += (s, a) => { player.Dispose(); };
                            player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/02/Zap.wav"));

                            // Lightning.Hide()
                            Lightning.Visibility = Visibility.Collapsed;

                            // Change Opacity by -0.2
                            if (me.Opacity >= 0.2)
                                me.Opacity -= 0.2;
                            if (me.Opacity < 0.2)
                            {
                                // Hide();
                                me.Visibility = Visibility.Collapsed;

                                // Say "You Lose"
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
            var me = sender as Sprite;

            Task.Run(async () =>
            {
                await me.SetCostume("02/4.png");

                int i = 3;
                while (i-- > 0)
                {
                    await me.Show();
                    await Delay(1000);
                    await me.Hide();
                    await Delay(500);
                }
            });
        }

        private void Lightning_Loaded(object sender, RoutedEventArgs e)
        {
            var me = sender as Sprite;

            Task.Run(async () =>
            {
                await me.SetCostume("02/5.png");
                await Delay(1000);
                while (true)
                {
                    await Delay(Random(0, 1500));
                    await me.SetPosition(Random(0, 1000), 10);
                    await me.Show();

                    var i = 8;
                    while (i-- > 0)
                    {
                        await me.ChangeYby(40);
                        await Delay(300);
                    }

                    await me.Hide();
                }
            });
        }

        Random random = new Random();

        private int Random(int from, int to)
        {
            return random.Next(from, to);
        }

        private Task Delay(int ms)
        {
            return Task.Delay(ms);
        }

        private void String_Loaded(object sender,RoutedEventArgs e)
        {
            var me = sender as Sprite;
            Task.Run(async () =>
            {
                await me.SetCostume("02/1.png");
                await Delay(1000);

                int i = 7;
                while (i-- > 0)
                {
                    await me.SetPosition(Random(0, 1000), Random(0, 500));
                    await me.Show();

                    while (!await me.IsTouching(Astro_Cat))
                    {
                        await Delay(200);
                    }

                    await me.PlaySound("02/Humming.wav");
                    await me.Hide();
                    await me.Say("Got it!");
                    await Delay(300);
                }

                await me.SetCostume("02/2.png");
                await me.Show();
                await me.Say("Stargate Opened!!!");
            });
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
