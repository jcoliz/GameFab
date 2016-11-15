using Example.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Example.Scenes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class _04 : Page, INotifyPropertyChanged
    {
        Random random = new Random();
        Point mousepoint;
        bool mousepressed = false;

        private int Random(int from, int to)
        {
            return random.Next(from, to);
        }

        private Task Delay(int ms)
        {
            return Task.Delay(ms);
        }

        public _04()
        {
            this.InitializeComponent();
            this.Loaded += Scene_Loaded;
        }

        private void Scene_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
            Window.Current.CoreWindow.PointerReleased += CoreWindow_PointerReleased;
        }

        private void CoreWindow_PointerReleased(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            mousepressed = false;
            Sprite.Broadcast("mouseup");
        }

        private void CoreWindow_PointerPressed(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            var clicked = args.CurrentPoint.Position;
            mousepoint = new Point(clicked.X, clicked.Y);
            mousepressed = true;

            Sprite.Broadcast("mousedown");
        }

        private async void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Down)
            {
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Up)
            {
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Left)
            {
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Right)
            {
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Space)
            {
                Sprite.Broadcast("start");
            }
        }

        public int Score
        {
            get
            {
                return _Score;
            }
            set
            {
                _Score = value;
                Context?.Post(DoPropertyChanged, nameof(Score));
            }
        }
        private int _Score = 0;

        public int Chances
        {
            get
            {
                return _Chances;
            }
            set
            {
                _Chances = value;
                Context?.Post(DoPropertyChanged, nameof(Chances));
            }
        }
        private int _Chances = 5;

        private SynchronizationContext Context = SynchronizationContext.Current;
        public event PropertyChangedEventHandler PropertyChanged;
        
        private void DoPropertyChanged(object property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property as string));
        }

        private void Instructions_Loaded(object sender, RoutedEventArgs e)
        {
            var me = sender as Sprite;

            Task.Run(async () =>
            {
                await me.SetPosition(100, 100);
                await me.SetCostume("04/4.png");
            });

        }

        private void Instructions_MessageReceived(object sender, Sprite.MessageReceivedArgs e)
        {
            var me = sender as Sprite;

            if (e.message == "start")
            {
                var ignore = me.Hide();
            }

        }

        private void Neo_Cat_Loaded(object sender, RoutedEventArgs e)
        {
            var me = sender as Sprite;
        }

        private void Neo_Cat_MessageReceived(object sender, Sprite.MessageReceivedArgs e)
        {
            var me = sender as Sprite;

            if (e.message == "start")
            {
                Task.Run(async () => 
                {
                    await me.SetCostume("04/7.png");
                    await me.Show();
                });
            }
            else if (e.message == "mousedown")
            {
                Task.Run(async () => 
                {
                    await me.SetCostume("04/8.png");
                    await me.Glide(100, mousepoint);
                });
            }
            else if (e.message == "mouseup")
            {
                var ignore = me.SetCostume("04/7.png");
            }
            else if (e.message == "oh")
            {
                Task.Run(async () => 
                {
                    if (Chances > 0)
                    {
                        await me.Say("Oh no!!");
                        await Delay(500);
                        await me.Say();
                    }
                    else
                        await me.Say("Try again...");
                });
            }
            else if (e.message == "win")
            {
                var ignore = me.Say("WINNER!!");
            }
            else if (e.message == "lose")
            {
                var ignore = me.Say("Try again...");
            }
        }

        private void Virus_MessageReceived(object sender, Sprite.MessageReceivedArgs e)
        {
            var me = sender as Sprite;
            var running = true;

            if (e.message == "start")
            {
                var ignore = Task.Run(async () => 
                {
                    await me.SetCostumes("04/V.png","04/I.png", "04/R.png", "04/U.png", "04/S.png");
                    await me.Show();
                    while (running)
                    {
                        await Delay(300);
                        await me.NextCostume();
                    }
                });
                ignore = Task.Run(async () =>
                {
                    var benign = false;
                    await me.SetPosition(500, 200);
                    await me.PointTowards(Neo_Cat);
                    while (running)
                    {
                        if (await me.IsTouching(Neo_Cat))
                        {
                            await me.PointInDirection_Heading(Random(-45, 45));
                            this.Score++;

                            if (Score >= 30)
                            {
                                running = false;
                                Sprite.Broadcast("win");
                            }

                            await me.Move(100);
                        }
                        await me.Move(30);
                        if ((await me.IsTouching(Server1) || await me.IsTouching(Server2)) && !benign)
                        {
                            benign = true;
                            Sprite.Broadcast("oh");
                            Chances--;
                            if (Chances <= 0)
                            {
                                running = false;
                                Sprite.Broadcast("lose");
                            }

                            var ignxore = Task.Run(async () => 
                            {
                                await Delay(1000);
                                benign = false;
                            });
                         }
                        await Delay(75);
                        await me.IfOnEdgeBounce();
                    }
                    await me.Hide();
                });

            }
        }
    }
}
