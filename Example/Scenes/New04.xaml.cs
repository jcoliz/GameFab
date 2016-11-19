﻿using Example.Controls;
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

        private void Virus_MessageReceived(Sprite me, Sprite.MessageReceivedArgs what)
        {
            if (what.message == "start")
            {
                Task.Run(async () =>
                {
                    await me.SetCostumes("04/V.png", "04/I.png", "04/R.png", "04/U.png", "04/S.png");
                    await me.Show();
                    while (Running)
                    {
                        await Delay(0.3);
                        await me.NextCostume();
                    }
                });
                Task.Run(async () =>
                {
                    var deadly = true;
                    await me.SetPosition(500, 200);
                    await me.PointTowards(Neo_Cat);
                    while (Running)
                    {
                        if (await me.IsTouching(Neo_Cat))
                        {
                            await me.PointInDirection_Heading(Random(-45, 45));
                            Score.Value++;

                            if (Score.Value >= 30)
                            {
                                Sprite.Broadcast("win");
                                Running = false;
                            }

                            await me.Move(100);
                        }
                        /*else if ((await me.IsTouching(Server1) || await me.IsTouching(Server2)) && deadly)
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
                        }*/
                        await me.Move(30);
                        await Delay(0.075);
                        await me.IfOnEdgeBounce();
                    }
                    await me.Hide();
                });
            }
        }

        private void Neo_Cat_MessageReceived (Sprite me, Sprite.MessageReceivedArgs what)
        {
            if (what.message == "start")
            {
                Task.Run(async () =>
                {
                    await me.SetCostume("04/7.png");
                    await me.Show();
                });
            }
            else if (what.message == "oh")
            {
                Task.Run(async () =>
                {
                    if (Chances.Value > 0)
                    {
                        await me.Say("Oh no!!");
                        await Delay(0.5);
                        await me.Say();
                    }
                    else
                        await me.Say("Try again...");
                });
            }
            else if (what.message == "win")
            {
                Task.Run(async () =>
                {
                    await me.Say("WINNER!!");
                });
            }
        }

        private void Neo_Cat_PointerPressed(Sprite me, Sprite.PointerArgs what)
        {
            Task.Run(async () =>
            {
                await me.SetCostume("04/8.png");
                await me.Glide(0.1, what.mousepoint);
            });
        }

        private void Neo_Cat_PointerReleased(Sprite me, Sprite.PointerArgs what)
        {
            Task.Run(async () =>
            {
                await me.SetCostume("04/7.png");
            });
        }

        private void Neo_Cat_Loaded(Sprite me)
        {
            Task.Run(async () =>
            {
                await me.SetCostume("04/7.png");
                await me.SetPosition(176, 307);
            });
        }


        private void CanvasAnimatedControl_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            base.Draw(sender,args);
        }

        private void CanvasAnimatedControl_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            base.CreateResources(sender,args);
        }

        private Sprite Neo_Cat = null;
        private Sprite Virus = null;

        private async void Scene_Loaded(object sender, RoutedEventArgs e)
        {
            Neo_Cat = await CreateSprite(Screen, this.Neo_Cat_Loaded);
            Neo_Cat.MessageReceived += Neo_Cat_MessageReceived;
            Neo_Cat.PointerPressed += Neo_Cat_PointerPressed;
            Neo_Cat.PointerReleased += Neo_Cat_PointerReleased;

            Virus = await CreateSprite(Screen);
            Virus.MessageReceived += Virus_MessageReceived;
        }
    }
}
