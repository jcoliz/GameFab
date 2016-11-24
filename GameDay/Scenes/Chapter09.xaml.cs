using GameFab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        public void Scene_Loaded(object sender, RoutedEventArgs args)
        {
            SetBackdrop("09/43.png");
            Player = CreateSprite(Player_Loaded);
            Dark = CreateSprite(Dark_Loaded);
        }

        private void Player_Loaded(Sprite me)
        {
            me.SetPosition(LeftEdge/2, BottomEdge/2);
            me.SetCostume("09/18.png");
            me.SetRotationStyle(Sprite.RotationStyle.LeftRight);
            me.Show();
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
