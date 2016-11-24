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
    [SceneMenuEntry(Label = "Play Final Fight", Order = 9)]
    public sealed partial class Chapter09 : Scene
    {
        public Chapter09()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Assets needed by your scene
        /// </summary>
        /// <remarks>
        /// Replace this with all the backdrops and costumes you'll need in the scene
        /// </remarks>
        protected override IEnumerable<string> Assets => new[] { "09/43.png", "09/18.png", "09/17.png", "09/5.png" };

        Sprite Player;
        Sprite Dark;
        Sprite Fireball;

        // Variables we want to display
        Variable<int> PlayerHP = new Variable<int>(20);
        Variable<int> CpuHP = new Variable<int>(30);

        public void Scene_Loaded(object sender, RoutedEventArgs args)
        {
            SetBackdrop("09/43.png");
            Player = CreateSprite(Player_Loaded);
            Dark = CreateSprite(Dark_Loaded);
            Fireball = CreateSprite(Fireball_Loaded);
        }

        private async void Fireball_Loaded(Sprite me)
        {
            me.SetCostume("09/5.png");
            me.SetRotationStyle(Sprite.RotationStyle.AllAround);

            while (Running)
            {
                await Delay(Random(1, 5));
                me.SetPosition(Dark.Position);
                me.SetRotationStyle(Sprite.RotationStyle.AllAround);
                me.PointTowards(Player.Position);
                me.Show();

                int i = 60;
                while (i-- > 0 && Running)
                {
                    await Delay(0.1);
                    me.Move(20);
                    if (me.IsTouching(Player))
                    {
                        await Delay(0.25);
                        break;
                    }
                    if (me.IsTouchingEdge())
                        break;
                }

                me.Hide();
            }
        }

        private void Player_Loaded(Sprite me)
        {
            me.SetPosition(LeftEdge/2, BottomEdge/2);
            me.SetVariable<double>("starty", me.Position.Y);
            me.SetCostume("09/18.png");
            me.SetRotationStyle(Sprite.RotationStyle.LeftRight);
            me.Show();
            me.KeyPressed += Player_KeyPressed;
            me.MessageReceived += Player_MessageReceived;
            me.Touched += Player_Touched;
        }

        private void Player_Touched(Sprite me, Sprite.OtherSpriteArgs what)
        {
            if (what.sprite.Equals(Fireball) || what.sprite.Equals(Dark))
            {
                Broadcast("hit");
            }
        }

        bool invincible = false;

        private void Player_MessageReceived(Sprite me, Sprite.MessageReceivedArgs what)
        {
            if (what.message == "hit" && ! invincible)
            {
                invincible = true;
                me.PlaySound("09/3.wav");
                --PlayerHP.Value;
                Task.Run(async () => 
                {
                    for(int i=10;i>0;i--)
                    {
                        me.ReduceOpacityBy(0.1);
                        await Delay(0.05);
                    }
                    for (int i = 10; i > 0; i--)
                    {
                        me.ReduceOpacityBy(-0.1);
                        await Delay(0.05);
                    }
                    invincible = false;
                });

                if (PlayerHP.Value <= 0)
                {
                    Broadcast("lose");
                }
            }
            if (what.message == "lose")
            {
                Running = false;
            }

        }

        double yspeed = 0;
        double gravity = -4;

        private void Player_KeyPressed(Sprite me, Windows.UI.Core.KeyEventArgs what)
        {
            var starty = me.GetVariable<double>("starty");

            if (what.VirtualKey == Windows.System.VirtualKey.Left)
            {
                me.ChangeXby(-80);
                if (me.Position.X < LeftEdge)
                    me.SetX(LeftEdge);
            }
            if (what.VirtualKey == Windows.System.VirtualKey.Right)
            {
                me.ChangeXby(80);
                if (me.Position.X > RightEdge)
                    me.SetX(RightEdge);
            }
            if (what.VirtualKey == Windows.System.VirtualKey.Up && me.Position.Y == starty)
            {
                yspeed = 40;
                Task.Run(async () => 
                {
                    me.ChangeYby(yspeed);
                    while(me.Position.Y > starty)
                    {
                        await Delay(0.05);
                        yspeed += gravity;
                        me.ChangeYby(yspeed);
                    }
                    me.SetY(starty);

                });

            }
        }

        private async void Dark_Loaded(Sprite me)
        {
            me.SetPosition(RightEdge / 2, BottomEdge/3);
            me.SetCostume("09/17.png");
            me.SetRotationStyle(Sprite.RotationStyle.LeftRight);
            me.PointInDirection(-90);
            me.Show();

            while(Running)
            {
                await Delay(1.0);
                await me.Glide(Random(0.5, 2), new Point(Random(LeftEdge / 2, RightEdge * 2 / 3),BottomEdge/3));
            }

        }
    }
}
