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

        private async void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Down)
            {
                await Astro_Cat.ChangeYby(15);
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Up)
            {
                await Astro_Cat.ChangeYby(-15);
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Left)
            {
                await Astro_Cat.ChangeXby(-15);
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Right)
            {
                await Astro_Cat.ChangeXby(15);
            }
        }

        private void Astro_Cat_Loaded(object sender, RoutedEventArgs e)
        {
            var me = sender as Sprite;

            Task.Run(async () =>
            {
                while (true)
                {
                    await me.ChangeYby(2);
                    await Delay(300);
                    await me.ChangeYby(-2);
                    await Delay(300);
                }
            });

            Task.Run(async () =>
            {
                await me.SetCostume("02/6.png");
                while (true)
                {
                    if (await me.IsTouching(Lightning))
                    {
                        await me.PlaySound("02/Zap.wav");
                        await Lightning.Hide();
                        double opacity = await me.ReduceOpacityBy(0.2);
                        if (opacity < 0.2)
                        {
                            await me.Hide();
                            await me.Say("You lose!!");
                        }
                    }
                    else
                        await Delay(200);
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
    }
}
