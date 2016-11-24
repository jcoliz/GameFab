using GameFab;
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

namespace GameDay.Scenes
{
    [SceneMenuEntry(Label = "Play Shootout", Order = 5)]
    public sealed partial class Chapter05 : Scene
    {
        public Chapter05()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Assets needed by your scene
        /// </summary>
        /// <remarks>
        /// Replace this with all the backdrops and costumes you'll need in the scene
        /// </remarks>
        protected override IEnumerable<string> Assets => new[] { "05/1.png", "05/2.png", "05/3.png", "05/4.png", "05/5.png", "05/6.png", "05/7.png", "05/8.png", "05/10.png", "05/12.png", "05/14.png", "05/15.png", "05/16.png", };

        Sprite Keeper;
        Sprite Bullseye;
        Sprite Ball;
        Sprite Net;
        Sprite Wave;
        Sprite Cloud;
        Sprite Banner;

        Variable<int> Score = new Variable<int>();
        Variable<int> Chances = new Variable<int>(8);

        public void Scene_Loaded(object sender, RoutedEventArgs args)
        {
            SetBackdrop("05/16.png");
            Net = CreateSprite((me) =>
            {
                me.SetCostume("05/1.png");
                me.SetPosition(0, TopEdge / 3);
                me.SetSize(2.5);
                me.Show();
            });
            CreateSprite((me) =>
            {
                me.SetCostume("05/4.png");
                me.SetPosition(0, TopEdge / 3 + 30);
                me.SetSize(2.5);
                me.Show();
            });
            Keeper = CreateSprite(Keeper_Loaded);
            Ball = CreateSprite(Ball_Loaded);
            Bullseye = CreateSprite(Bullseye_Loaded);
            CreateSprite(Wave_Loaded);
        }

        private async void Wave_Loaded(Sprite me)
        {
            me.SetCostume("05/2.png");
            me.SetSize(2.666);
            me.SetPosition(0, BottomEdge + 50);
            me.Show();
            while (Running)
            {
                int i = 3;
                while (i-- > 0)
                {
                    await Delay(0.2);
                    me.ChangeYby(-25);
                }
                i = 3;
                while (i-- > 0)
                {
                    await Delay(0.2);
                    me.ChangeYby(25);
                }
            }
        }

        private async void Bullseye_Loaded(Sprite me)
        {
            me.SetSize(2.0);
            me.SetCostume("05/6.png");
            me.MessageReceived += Bullseye_MessageReceived;

            await Delay(0.1);
            var net_top = Net.Position.Y + Net.CostumeSize.Height / 3;
            var net_bottom = Net.Position.Y - Net.CostumeSize.Height / 3;
            var net_center = Net.Position.Y ;
            var net_left = Net.Position.X - Net.CostumeSize.Width / 2;
            var net_right = Net.Position.X + Net.CostumeSize.Width / 2;

            me.SetPosition(net_left,net_top);
            me.Show();

            while (Running)
            {
                await me.Glide(1.0, new Point(net_right, net_top));
                await me.Glide(1.0, new Point(net_left, net_center));
                await me.Glide(1.0, new Point(net_right, net_center));
                await me.Glide(1.0, new Point(net_left, net_bottom));
                await me.Glide(1.0, new Point(net_right, net_bottom));
                await me.Glide(0.5, new Point(net_left, net_top));
            }
        }

        private void Bullseye_MessageReceived(Sprite me, Sprite.MessageReceivedArgs what)
        {
            if (what.message == "shoot")
            {
                me.Hide();
            }
            if (what.message == "reset")
            {
                me.Show();
            }
        }

        private void Ball_Loaded(Sprite me)
        {
            me.SetPosition(0, BottomEdge / 2);
            me.Variable["start"] = me.Position;
            me.Variable["ready"] = true;
            me.Show();
            me.SetCostume("05/15.png");
            me.KeyPressed += Ball_KeyPressed;
            me.MessageReceived += Ball_MessageReceived;
        }

        private void Ball_MessageReceived(Sprite me, Sprite.MessageReceivedArgs what)
        {
            if (what.message == "reset")
            {
                me.SetSize(1.0);
                me.SetPosition((me.Variable["start"] as Point?).Value);
                me.Variable["ready"] = true;
                me.Say();
            }
        }

        private void Ball_KeyPressed(Sprite me, Windows.UI.Core.KeyEventArgs what)
        {
            bool? ready = me.Variable["ready"] as bool?;

            if (what.VirtualKey == Windows.System.VirtualKey.Space && ready.Value)
            {
                --Chances.Value;
                me.Variable["ready"] = false;
                Broadcast("shoot");
                bool donemoving = false;
                Task.Run(async () => 
                {
                    await me.Glide(0.7, Bullseye.Position);
                    donemoving = true;

                    if (me.IsTouching(Keeper))
                    {
                        me.PlaySound("05/miss.wav");
                        Broadcast("miss");
                        me.Say("Miss!");
                    }
                    else if (me.IsTouching(Net))
                    {
                        me.PlaySound("05/goal.wav");
                        Broadcast("goal");
                        me.Say("Goal!");
                        ++Score.Value;
                    }

                    await Delay(1.0);

                    if (Score.Value >= 5)
                    {
                        Broadcast("win");
                        Running = false;
                    }
                    else if (Score.Value + Chances.Value < 5)
                    {
                        Broadcast("lose");
                        Running = false;
                    }
                    else
                        Broadcast("reset");
                });
                Task.Run(async () =>
                {
                    do
                    {
                        me.ChangeSizeBy(-0.075);
                        await Delay(0.1);
                    }
                    while (!donemoving);
                });
            }
        }

        private async void Keeper_Loaded(Sprite me)
        {
            me.SetPosition(0, TopEdge/3 - 30);
            me.Variable["start"] = me.Position;
            me.Show();
            me.SetCostumes("05/7.png","05/8.png");
            me.MessageReceived += Keeper_MessageReceived;

            while(Running)
            {
                await Delay(0.5);
                me.NextCostume();
            }
        }

        private async void Keeper_MessageReceived(Sprite me, Sprite.MessageReceivedArgs what)
        {
            var net_top = Net.Position.Y + Net.CostumeSize.Height / 3;
            var net_bottom = Net.Position.Y - Net.CostumeSize.Height / 3;
            var net_center = Net.Position.Y;
            var net_left = Net.Position.X - Net.CostumeSize.Width / 2;
            var net_right = Net.Position.X + Net.CostumeSize.Width / 2;

            if (what.message == "shoot")
            {
                await me.Glide(0.5, new Point(Random(net_left, net_right), Random(net_bottom, net_top)));
            }
            if (what.message == "reset")
            {
                me.SetPosition((me.Variable["start"] as Point?).Value);
            }
            if (what.message == "win")
            {
                me.Say("Great job!");
            }
            if (what.message == "lose")
            {
                me.Say("Try again!");
            }
        }
    }
}
