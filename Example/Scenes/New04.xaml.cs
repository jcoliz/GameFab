using Example.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Example.Scenes
{
    public sealed partial class New_04 : Scene
    {
        public New_04()
        {
            this.InitializeComponent();
        }

        public Scene.Variable<int> Chances = new Variable<int>(5);
        public Scene.Variable<int> Score = new Variable<int>();
        public Scene.Variable<int> Timer = new Variable<int>();

        private bool Running { get; set; } = true;

        protected override IEnumerable<string> Assets => new[] { "04/21.png", "04/7.png", "04/8.png", "04/V.png", "04/I.png", "04/R.png", "04/U.png", "04/S.png", "04/1.png" };

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as FrameworkElement).Visibility = Visibility.Collapsed;
            Broadcast("start");

            Task.Run(async () =>
            {
                while (Running)
                {
                    ++Timer.Value;
                    await Delay(1.0);
                }
            });
        }

        private void Virus_MessageReceived(Models.Sprite me, Models.Sprite.MessageReceivedArgs what)
        {
            if (what.message == "start")
            {
                Task.Run(async () =>
                {
                    me.SetCostumes("04/V.png", "04/I.png", "04/R.png", "04/U.png", "04/S.png");
                    me.Show();
                    while (Running)
                    {
                        await Delay(0.3);
                        me.NextCostume();
                    }
                });
                Task.Run(async () =>
                {
                    var deadly = true;
                    me.SetPosition(500, 200);
                    me.PointTowards(Neo_Cat.Position);
                    while (Running)
                    {
                        if (await me.IsTouching(Neo_Cat))
                        {
                            me.PointInDirection_Heading(Random(-45, 45));
                            Score.Value++;

                            if (Score.Value >= 30)
                            {
                                Sprite.Broadcast("win");
                                Running = false;
                            }

                            me.Move(100);
                        }
                        else if ((await me.IsTouching(Server1) || await me.IsTouching(Server2)) && deadly)
                        {
                            deadly = false;
                            Sprite.Broadcast("oh");
                            Chances.Value--;
                            if (Chances.Value <= 0)
                            {
                                Running = false;
                                Sprite.Broadcast("lose");
                            }

                            var ignore = Task.Run(async () =>
                            {
                                await Delay(1.0);
                                deadly = true;
                            });
                        }
                        me.Move(30);
                        await Delay(0.075);
                        me.IfOnEdgeBounce();
                    }
                    me.Hide();
                });
            }
        }

        private void Neo_Cat_MessageReceived (Models.Sprite me, Models.Sprite.MessageReceivedArgs what)
        {
            if (what.message == "start")
            {
                Task.Run(() =>
                {
                    me.SetCostume("04/7.png");
                    me.Show();
                });
            }
            else if (what.message == "oh")
            {
                Task.Run(async () =>
                {
                    if (Chances.Value > 0)
                    {
                        me.Say("Oh no!!");
                        await Delay(0.5);
                        me.Say();
                    }
                    else
                        me.Say("Try again...");
                });
            }
            else if (what.message == "win")
            {
                Task.Run(() =>
                {
                    me.Say("WINNER!!");
                });
            }
        }

        private void Neo_Cat_PointerPressed(Models.Sprite me, Models.Sprite.PointerArgs what)
        {
            Task.Run(async () =>
            {
                me.SetCostume("04/8.png");
                await me.Glide(0.1, what.mousepoint);
            });
        }

        private void Neo_Cat_PointerReleased(Models.Sprite me, Models.Sprite.PointerArgs what)
        {
            Task.Run(() =>
            {
                me.SetCostume("04/7.png");
            });
        }

        private void Neo_Cat_Loaded(Models.Sprite me)
        {
            Task.Run(() =>
            {
                me.SetCostume("04/7.png");
                me.SetPosition(176, 307);
            });
        }

        private Models.Sprite Neo_Cat = null;
        private Models.Sprite Virus = null;
        private Models.Sprite Server1 = null;
        private Models.Sprite Server2 = null;

        private async void Scene_Loaded(object sender, RoutedEventArgs e)
        {
            base.SetBackground("04/21.png");

            Neo_Cat = await CreateSprite(this.Neo_Cat_Loaded);
            Neo_Cat.MessageReceived += Neo_Cat_MessageReceived;
            Neo_Cat.PointerPressed += Neo_Cat_PointerPressed;
            Neo_Cat.PointerReleased += Neo_Cat_PointerReleased;

            Virus = await CreateSprite();
            Virus.MessageReceived += Virus_MessageReceived;

            Server1 = await CreateSprite((me) => 
            {
                me.SetCostume("04/1.png");
                me.SetPosition(10, 500);
                me.Show();
            });
            Server2 = await CreateSprite((me) =>
            {
                me.SetCostume("04/1.png");
                me.SetPosition(510, 500);
                me.Show();
            });
        }
    }
}
