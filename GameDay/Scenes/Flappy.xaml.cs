﻿using GameFab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace GameDay.Scenes
{
    [SceneMenuEntry(Label = "Play Flappy", Order = 10)]
    public sealed partial class Flappy : Scene
    {
        public Flappy()
        {
            this.InitializeComponent();
        }

        public Variable<int> Score = new Variable<int>(0);

        Sprite Player;

        private void Scene_Loaded(object sender, RoutedEventArgs e)
        {
            Player = CreateSprite(Player_SceneLoaded);

            SetBackdrop("Flappy/Butterfly/Background.png");

            // Spawn the needed number of pillars over the correct time.
            Task.Run(async () => 
            {
                while (Running)
                {
                    CreateSprite(this.Pillar_SceneLoaded);
                    await Delay(3.0);
                }
            });

            // Syncrhonize the visual updates by sending out a message on a regular time
            Task.Run(async () => 
            {
                while(Running)
                {
                    await Delay(0.05);
                    Broadcast("update");
                }
            });
        }

        private void Player_MessageReceived(Sprite me, Sprite.MessageReceivedArgs what)
        {
            if (what.message == "gameover")
            {
                Running = false;
                Player.Say("Game over!");
            }
        }

        double yspeed = 0;
        double gravity = -2; // in pixels per tick squared

        protected override IEnumerable<string> Assets => new[] { "Flappy/Butterfly/Background.png", "Flappy/Butterfly/Obstacle-Top-1.png", "Flappy/Butterfly/Obstacle-Bottom-1.png", "Flappy/Butterfly/Player.png" };

        private void Player_SceneLoaded(Sprite me)
        {
            Task.Factory.StartNew(async () => 
            {
                while (Running)
                {
                    if (IsGamePadButtonPressed(GamepadButtons.A))
                        Player_Jump(me);
                    await Delay(0.05);
                }

            }, TaskCreationOptions.LongRunning);

            Task.Run(async () => 
            {
                // Set up the player with all initial values and event handlers
                me.SetCostume("Flappy/Butterfly/Player.png");
                me.SetPosition(LeftEdge / 2, 0);
                me.CollisionRadius = 5.0;
                me.GoToFront();
                me.Show();
                me.KeyPressed += Player_KeyPressed;
                me.MessageReceived += Player_MessageReceived;
                me.PointerPressed += Player_PointerPressed;

                // Apply gravity
                while (Running)
                {
                    yspeed += gravity;
                    var y = me.ChangeYby(yspeed);
                    if (y > TopEdge || y < BottomEdge)
                        Broadcast("gameover");
                    await Delay(0.1);
                }
                while(true)
                {
                    if (IsGamePadButtonPressed(GamepadButtons.Menu))
                        GoBack();
                    await Delay(0.1);
                }
            });
        }

        private void Player_PointerPressed(Sprite me, Sprite.PointerArgs what)
        {
            Player_Jump(me);
        }

        private void Player_KeyPressed(Sprite me, Windows.UI.Core.KeyEventArgs what)
        {
            // Apply upward force
            if (what.VirtualKey == Windows.System.VirtualKey.Space)
            {
                Player_Jump(me);
            }
        }

        private void Player_Jump(Sprite me)
        {
            if (Running)
            {
                yspeed = 20;
                me.ChangeYby(yspeed);
            }
        }

        private void Pillar_SceneLoaded(Sprite me)
        {
            Task.Run(() => 
            {
                double opening_center = Random(BottomEdge + 200, TopEdge - 200);
                double opening_bottom = opening_center - 100;
                double opening_top = opening_center + 100;
                double pillar_image_height = 472;

                me.SetCostume("Flappy/Butterfly/Obstacle-Bottom-1.png");
                me.SetPosition(RightEdge + 50, opening_bottom - pillar_image_height / 2 );

                var top = CreateSprite();
                me.Variable["top"] = top;
                top.SetCostume("Flappy/Butterfly/Obstacle-Top-1.png");
                top.SetPosition(RightEdge + 50, opening_top + pillar_image_height / 2);
                me.Show();
                top.Show();

                me.MessageReceived += Pillar_MessageReceived;
            });
        }

        private void Pillar_MessageReceived(Sprite me, Sprite.MessageReceivedArgs what)
        {
            if (what.message == "update")
            {
                var top = me.Variable["top"] as Sprite;
                var x_before = me.GetPosition().X;
                top.ChangeXby(-5);
                var x = me.ChangeXby(-5);
                if (x < Player.Position.X && x_before >= Player.Position.X)
                    ++Score.Value;

                if (x < LeftEdge - 100)
                {
                    me.MessageReceived -= Pillar_MessageReceived;
                    me.Destroy();
                    top.Destroy();
                }

                if (me.IsTouching(Player) || top.IsTouching(Player))
                {
                    Broadcast("gameover");
                }
            }
        }
    }
}
