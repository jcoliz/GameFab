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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GameDay.Scenes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
        Variable<int> BallsLeft = new Variable<int>(8);

        public void Scene_Loaded(object sender, RoutedEventArgs args)
        {
            SetBackdrop("05/16.png");
            CreateSprite((me)=> 
            {
                me.SetCostume("05/4.png");
                me.SetPosition(0, TopEdge / 3 + 30);
                me.SetSize(2.0);
                me.Show();
            });
            Net = CreateSprite((me) =>
            {
                me.SetCostume("05/1.png");
                me.SetPosition(0, TopEdge / 3);
                me.SetSize(2.0);
                me.Show();
            });
            Keeper = CreateSprite(Keeper_Loaded);
            Ball = CreateSprite(Ball_Loaded);
            Bullseye = CreateSprite(Bullseye_Loaded);
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
            me.Show();
            me.SetCostume("05/15.png");
            me.KeyPressed += Ball_KeyPressed;
        }

        private void Ball_KeyPressed(Sprite me, Windows.UI.Core.KeyEventArgs what)
        {
            if (what.VirtualKey == Windows.System.VirtualKey.Space)
            {
                Broadcast("shoot");
                bool donemoving = false;
                Task.Run(async () => 
                {
                    await me.Glide(0.7, Bullseye.Position);
                    donemoving = true;
                    Broadcast("reset");

                    me.SetSize(1.0);
                    me.SetPosition((me.Variable["start"] as Point?).Value);
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
        }
    }
}
