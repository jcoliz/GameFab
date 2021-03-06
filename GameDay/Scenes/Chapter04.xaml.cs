﻿using GameFab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace GameDay.Scenes
{
    [SceneMenuEntry(Label = "Play Virus Attack", Order = 4)]
    public sealed partial class Chapter04 : Scene
    {
        public Chapter04()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Running = false;
        }

        public Scene.Variable<int> Chances = new Variable<int>(5);
        public Scene.Variable<int> Score = new Variable<int>();
        public Scene.Variable<int> Timer = new Variable<int>();

        protected override IEnumerable<string> Assets => new[] { "04/21.png", "04/7.png", "04/8.png", "04/V.png", "04/I.png", "04/R.png", "04/U.png", "04/S.png", "04/1.png" };

        private Sprite Neo_Cat = null;
        private Sprite Virus = null;
        private Sprite Server1 = null;
        private Sprite Server2 = null;

        private void Scene_Loaded(object sender, RoutedEventArgs e)
        {
            SetBackdrop("04/21.png");

            Neo_Cat = CreateSprite(this.Neo_Cat_Loaded);
            Neo_Cat.MessageReceived += Neo_Cat_MessageReceived;
            Neo_Cat.PointerPressed += Neo_Cat_PointerPressed;
            Neo_Cat.PointerReleased += Neo_Cat_PointerReleased;

            Virus = CreateSprite();
            Virus.MessageReceived += Virus_MessageReceived;

            Server1 = CreateSprite((me) =>
            {
                me.SetCostume("04/1.png");
                me.SetPosition(LeftEdge / 2, BottomEdge + 10);
                me.Show();
            });
            Server2 = CreateSprite((me) =>
            {
                me.SetCostume("04/1.png");
                me.SetPosition(RightEdge / 2, BottomEdge + 10);
                me.Show();
            });

        }

        private void StartButton_Click(object sender=null, RoutedEventArgs e=null)
        {
            StartButton.Visibility = Visibility.Collapsed;
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

        private void Virus_MessageReceived(Sprite me, Sprite.MessageReceivedArgs what)
        {
            if (what.message == "start")
            {
                me.SetCostumes("04/V.png", "04/I.png", "04/R.png", "04/U.png", "04/S.png");
                me.SetPosition(RightEdge, 0);
                me.PointTowards(Neo_Cat.Position);
                me.SetRotationStyle(Sprite.RotationStyle.DoNotRotate);
                me.Show();

                Task.Run(async () =>
                {
                    while (Running)
                    {
                        await Delay(0.3);
                        me.NextCostume();
                    }
                    while (true)
                    {
                        if (IsGamePadButtonPressed(GamepadButtons.Menu))
                            GoBack();
                        await Delay(0.1);
                    }
                });
                Task.Run(async () =>
                {
                    var deadly = true;
                    while (Running)
                    {
                        if (me.IsTouching(Neo_Cat))
                        {
                            me.PointInDirection(Random(-45, 45));
                            Score.Value++;

                            if (Score.Value >= 30)
                            {
                                Sprite.Broadcast("win");
                                Running = false;
                            }

                            me.Move(100);
                        }
                        else if ((me.IsTouching(Server1) || me.IsTouching(Server2)) && deadly)
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

        private void Neo_Cat_MessageReceived (Sprite me, Sprite.MessageReceivedArgs what)
        {
            if (what.message == "start")
            {
                me.SetCostume("04/7.png");
                me.Show();
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
                me.Say("WINNER!!");
            }
        }

        private async void Neo_Cat_PointerPressed(Sprite me, Sprite.PointerArgs what)
        {
            me.SetCostume("04/8.png");
            await me.Glide(0.1, what.mousepoint);
        }

        private void Neo_Cat_PointerReleased(Sprite me, Sprite.PointerArgs what)
        {
            me.SetCostume("04/7.png");
        }

        private void Neo_Cat_Loaded(Sprite me)
        {
            me.SetCostume("04/7.png");
            me.SetPosition(0, 0);

            Task.Factory.StartNew(async () =>
            {
                while (Running)
                {
                    var stick = GetGamePadLeftStick();
                    if (stick.HasValue)
                    {
                        me.ChangeXby(stick.Value.X * 30);
                        me.ChangeYby(stick.Value.Y * 30);
                    }
                    if (IsGamePadButtonPressed(GamepadButtons.A))
                    {
                        var ignore = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => 
                        {
                            if (StartButton.Visibility == Visibility.Visible)
                                StartButton_Click();
                        });
                    }

                    await Delay(0.05);
                }

            }, TaskCreationOptions.LongRunning);

        }
    }
}
