﻿using GameFab;
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
    public sealed partial class TestDirection : Scene
    {
        public TestDirection()
        {
            this.InitializeComponent();
        }

        protected override IEnumerable<string> Assets => new[] { "04/7.png" };

        private void Scene_Loaded(object sender, RoutedEventArgs e)
        {
            CreateSprite(Player_SceneLoaded_AllAround);
            CreateSprite(Player_SceneLoaded_DoNotRotate);
            CreateSprite(Player_SceneLoaded_LeftRight);
        }

        private void Player_SceneLoaded_AllAround(Sprite me)
        {
            // Set up the player with all initial values and event handlers
            me.SetPosition(LeftEdge * 2 / 3, 0);
            me.Show();
            me.SetCostume("04/7.png");

            Task.Run(async () =>
            {
                await Square(me);
            });
        }

        private void Player_SceneLoaded_DoNotRotate(Sprite me)
        {
            // Set up the player with all initial values and event handlers
            me.SetPosition(0, 0);
            me.Show();
            me.SetCostume("04/7.png");

            Task.Run(async () =>
            {
                await Square(me);
            });
        }

        private void Player_SceneLoaded_LeftRight(Sprite me)
        {
            // Set up the player with all initial values and event handlers
            me.SetPosition(RightEdge * 2 / 3, 0);
            me.Show();
            me.SetCostume("04/7.png");

            Task.Run(async () =>
            {
                await Square(me);
            });
        }

        private async Task Move(Sprite me)
        {
            int times = 10;
            while (times-- > 0 && Running)
            {
                await Delay(0.2);
                me.Move(10);
            }
        }

        private async Task Square(Sprite me)
        {
            while (Running)
            {
                me.PointInDirection_Heading(0);
                me.Say("Up");
                await Move(me);
                me.PointInDirection_Heading(90);
                me.Say("Right");
                await Move(me);
                me.PointInDirection_Heading(180);
                me.Say("Down");
                await Move(me);
                me.PointInDirection_Heading(-90);
                me.Say("Left");
                await Move(me);
            }
        }

    }
}