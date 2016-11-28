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
        protected override IEnumerable<string> Assets => new[] { "09/4.png", "09/16.png", "09/43.png", "09/18.png", "09/17.png", "09/5.png", "09/19.png", "09/20.png", "09/21.png", "09/22.png", "09/23.png", "09/24.png", "09/25.png", "09/26.png", "09/27.png", "09/28.png", "09/42.png" };

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
            CreateSprite(Banner_Loaded);
        }

        private void Banner_Loaded(Sprite me)
        {
            me.MessageReceived += Banner_MessageReceived;
        }

        private void Banner_MessageReceived(Sprite me, Sprite.MessageReceivedArgs what)
        {
            if (what.message == "win")
            {
                me.SetCostume("09/4.png");
                me.Show();
                Running = false;
            }
            if (what.message == "lose")
            {
                me.SetCostume("09/16.png");
                me.Show();
                Running = false;
            }
        }

        private async void Fireball_Loaded(Sprite me)
        {
            me.SetCostume("09/5.png");
            me.SetRotationStyle(Sprite.RotationStyle.AllAround);
            me.MessageReceived += Fireball_MessageReceived;
        }

        private async void Fireball_MessageReceived(Sprite me, Sprite.MessageReceivedArgs what)
        {
            if (what.message == "fire" && ! me.Visible)
            {
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
            if (what.message == "hit" && ! invincible && ! lethal)
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
        }

        double yspeed = 0;
        double gravity = -4;
        bool lethal = false;
        bool canattack = true;
        bool canmove = true;

        private void Player_KeyPressed(Sprite me, Windows.UI.Core.KeyEventArgs what)
        {
            me.PointTowards(Dark.Position);
            Dark.PointTowards(me.Position);

            var starty = me.GetVariable<double>("starty");

            if (what.VirtualKey == Windows.System.VirtualKey.Left && canmove)
            {
                me.ChangeXby(-80);
                if (me.Position.X < LeftEdge)
                    me.SetX(LeftEdge);
            }
            if (what.VirtualKey == Windows.System.VirtualKey.Right && canmove)
            {
                me.ChangeXby(80);
                if (me.Position.X > RightEdge)
                    me.SetX(RightEdge);
            }
            if (what.VirtualKey == Windows.System.VirtualKey.Up && me.Position.Y == starty && canmove)
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
            if (what.VirtualKey == Windows.System.VirtualKey.O && canattack)
            {
                lethal = true;
                canattack = false;
                Task.Run(async () => 
                {
                    me.SetCostumes("09/18.png", "09/19.png", "09/20.png", "09/21.png", "09/22.png", "09/23.png", "09/24.png", "09/25.png", "09/26.png", "09/27.png", "09/28.png");
                    for (int i = 36; i > 0; i--)
                    {
                        me.NextCostume();
                        await Delay(0.1);
                    }
                    me.SetCostume("09/18.png");
                    lethal = false;
                    await Delay(1.0);
                    canattack = true;
                });
            }
            if (what.VirtualKey == Windows.System.VirtualKey.P && canattack)
            {
                lethal = true;
                canattack = false;
                canmove = false;
                Task.Run(async () =>
                {
                    me.SetCostume("09/42.png");
                    me.PlaySound("09/6.wav");
                    await Delay(2.0);
                    me.SetCostume("09/18.png");
                    lethal = false;
                    canmove = true;
                    await Delay(1.0);
                    canattack = true;
                });
            }
        }

        private async void Dark_Loaded(Sprite me)
        {
            var leftmax = LeftEdge / 2;
            var rightmax = RightEdge * 2 / 3;
            me.SetVariable<double>("leftmax", leftmax);
            me.SetVariable<double>("rightmax", rightmax);

            me.SetPosition(RightEdge / 2, BottomEdge/3);
            me.SetCostume("09/17.png");
            me.SetRotationStyle(Sprite.RotationStyle.LeftRight);
            me.PointInDirection(-90);
            me.Show();
            me.Touched += Dark_Touched;
        }

        // For 2nd player control
        private void Dark_KeyPressed(Sprite me, Windows.UI.Core.KeyEventArgs what)
        {
            me.PointTowards(Player.Position);
            Player.PointTowards(me.Position);

            var leftmax = me.GetVariable<double>("leftmax"); 
            var rightmax = me.GetVariable<double>("rightmax");

            if (what.VirtualKey == Windows.System.VirtualKey.Z)
            {
                me.ChangeXby(-80);
                if (me.Position.X < leftmax)
                    me.SetX(leftmax);
            }
            if (what.VirtualKey == Windows.System.VirtualKey.C)
            {
                me.ChangeXby(80);
                if (me.Position.X > rightmax)
                    me.SetX(rightmax);
            }
            if (what.VirtualKey == Windows.System.VirtualKey.X)
            {
                Broadcast("fire");
            }
        }

        private void Dark_Touched(Sprite me, Sprite.OtherSpriteArgs what)
        {
            if (what.sprite.Equals(Player) && lethal)
            {
                --CpuHP.Value;
                if (CpuHP.Value <= 0)
                    Broadcast("win");

                Task.Run(async () =>
                {
                    for (int i = 10; i > 0; i--)
                    {
                        me.ReduceOpacityBy(0.1);
                        await Delay(0.05);
                    }
                    for (int i = 10; i > 0; i--)
                    {
                        me.ReduceOpacityBy(-0.1);
                        await Delay(0.05);
                    }
                });

            }
        }

        private void Button_1P_Click(object sender, RoutedEventArgs e)
        {
            StartPanel.Visibility = Visibility.Collapsed;

            // CPU Controls Dark
            Task.Run(async () => 
            {
                var leftmax = Dark.GetVariable<double>("leftmax");
                var rightmax = Dark.GetVariable<double>("rightmax");

                while (Running)
                {
                    await Delay(1.0);
                    await Dark.Glide(Random(0.5, 2), new Point(Random(leftmax, rightmax),BottomEdge/3));
                }
            });

            // CPU Controls Fireball launches
            Task.Run(async () => 
            {
                while (Running)
                {
                    await Delay(Random(1, 5));
                    Broadcast("fire");
                }

            });
        }

        private void Button_2P_Click(object sender, RoutedEventArgs e)
        {
            StartPanel.Visibility = Visibility.Collapsed;

            // Player Controls Dark
            Dark.KeyPressed += Dark_KeyPressed;
        }
    }
}
