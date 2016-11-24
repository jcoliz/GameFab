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

        // Sprites we need to keep track of so we can refer to them
        Sprite Keeper;
        Sprite Bullseye;
        Sprite Net;

        // Variables we want to display
        Variable<int> Score = new Variable<int>();
        Variable<int> Chances = new Variable<int>(8);
        Variable<int> Timer = new Variable<int>(0);

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
            Bullseye = CreateSprite(Bullseye_Loaded);
            CreateSprite(Ball_Loaded);
            CreateSprite(Wave_Loaded);
            CreateSprite(Banner_Loaded);
        }

        private async void Banner_Loaded(Sprite me)
        {
            me.SetCostume("05/10.png");
            me.MessageReceived += Banner_MessageReceived;

            me.Show();
            me.PlaySound("05/whistle.wav");
            await Delay(0.5);
            me.Hide();
        }

        private void Banner_MessageReceived(Sprite me, Sprite.MessageReceivedArgs what)
        {
            if (what.message == "win")
            {
                Running = false;
                me.SetCostume("05/12.png");
                me.Show();
            }
            if (what.message == "lose")
            {
                Running = false;
                me.SetCostume("05/14.png");
                me.Show();
            }
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

            // Wait until the net is fully loaded and so has a size
            while (Net.CostumeSize.Height == 0)
                await Delay(0.1);

            // Figure out the positions of the net, and use that for
            // where to roam the bullseye
            var top = Net.Position.Y + Net.CostumeSize.Height / 3;
            var bottom = Net.Position.Y - Net.CostumeSize.Height / 3;
            var center = Net.Position.Y ;
            var left = Net.Position.X - Net.CostumeSize.Width / 2;
            var right = Net.Position.X + Net.CostumeSize.Width / 2;

            me.SetPosition(left,top);
            me.Show();

            while (Running)
            {
                await me.Glide(1.0, new Point(right, top));
                await me.Glide(1.0, new Point(left, center));
                await me.Glide(1.0, new Point(right, center));
                await me.Glide(1.0, new Point(left, bottom));
                await me.Glide(1.0, new Point(right, bottom));
                await me.Glide(0.5, new Point(left, top));
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
            me.SetVariable<Point>("start", me.Position);
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
                me.Say();
                me.SetSize(1.0);
                me.SetVariable<Boolean>("ready", true);
                var start = me.GetVariable<Point>("start");
                me.SetPosition(start);
            }
        }

        private void Ball_KeyPressed(Sprite me, Windows.UI.Core.KeyEventArgs what)
        {
            bool ready = me.GetVariable<Boolean>("ready");

            if (what.VirtualKey == Windows.System.VirtualKey.Space && ready)
            {
                --Chances.Value;
                me.SetVariable<Boolean>("ready", false);
                Broadcast("shoot");
                bool donemoving = false;
                Task.Run(async () => 
                {
                    me.PlaySound("05/kick.wav");
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
                    }
                    else if (Score.Value + Chances.Value < 5)
                    {
                        Broadcast("lose");
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
            me.SetVariable<Point>("start", me.Position);
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
            // Figure out the positions of the net, and use that for
            // where to roam the keeper
            var top = Net.Position.Y + Net.CostumeSize.Height / 3;
            var bottom = Net.Position.Y - Net.CostumeSize.Height / 3;
            var center = Net.Position.Y;
            var left = Net.Position.X - Net.CostumeSize.Width / 2;
            var right = Net.Position.X + Net.CostumeSize.Width / 2;

            if (what.message == "shoot")
            {
                await me.Glide(0.5, new Point(Random(left, right), Random(bottom, top)));
            }
            if (what.message == "reset")
            {
                var start = me.GetVariable<Point>("start");
                me.SetPosition(start);
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
