﻿using GameFab;
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
    [SceneMenuEntry(Label = "Empty Scene", Order = 15)]
    public sealed partial class EmptyScene : Scene
    {
        public EmptyScene()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Assets needed by your scene
        /// </summary>
        /// <remarks>
        /// Replace this with all the backdrops and costumes you'll need in the scene
        /// </remarks>
        protected override IEnumerable<string> Assets => new[] { "04/7.png" };

        Sprite Player;

        public void Scene_Loaded(object sender, RoutedEventArgs args)
        {
            Player = CreateSprite(Player_Loaded);
        }

        private void Player_Loaded(Sprite me)
        {
            me.SetPosition(0, 0);
            me.Show();
            me.SetCostume("04/7.png");
        }
    }
}
