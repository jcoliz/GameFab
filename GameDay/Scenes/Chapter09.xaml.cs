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
        protected override IEnumerable<string> Assets => new[] { "09/43.png", "09/18.png", "09/17.png" };

        Sprite Player;
        Sprite Dark;

        // Variables we want to display
        Variable<int> PlayerHP = new Variable<int>(20);
        Variable<int> CpuHP = new Variable<int>(30);

        public void Scene_Loaded(object sender, RoutedEventArgs args)
        {
            SetBackdrop("09/43.png");
            Player = CreateSprite(Player_Loaded);
            Dark = CreateSprite(Dark_Loaded);
        }

        private void Player_Loaded(Sprite me)
        {
            me.SetPosition(LeftEdge/2, BottomEdge/2);
            me.SetVariable<double>("starty", me.Position.Y);
            me.SetCostume("09/18.png");
            me.SetRotationStyle(Sprite.RotationStyle.LeftRight);
            me.Show();
            me.KeyPressed += Player_KeyPressed;
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

        private void Dark_Loaded(Sprite me)
        {
            me.SetPosition(RightEdge / 2, BottomEdge/2);
            me.SetCostume("09/17.png");
            me.SetRotationStyle(Sprite.RotationStyle.LeftRight);
            me.PointInDirection(-90);
            me.Show();
        }
    }
}
