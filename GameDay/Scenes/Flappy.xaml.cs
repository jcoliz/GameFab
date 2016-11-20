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
    public sealed partial class Flappy : Scene
    {
        public Flappy()
        {
            this.InitializeComponent();
        }

        private void Scene_Loaded(object sender, RoutedEventArgs e)
        {
            // Spawn the needed number of pillars over the correct time.
            Task.Run(async () => 
            {
                var player = CreateSprite(Player_SceneLoaded);
                player.KeyPressed += Player_KeyPressed;
                while (true)
                {
                    CreateSprite(this.Pillar_SceneLoaded);
                    await Delay(3.0);
                }
            });

            // Syncrhonize the visual updates by sending out a message on a regular time
            Task.Run(async () => 
            {
                while(true)
                {
                    await Delay(0.05);
                    Broadcast("update");
                }
            });
        }

        double yspeed = 0;
        double gravity = -2; // in pixels per tick squared

        protected override IEnumerable<string> Assets => new[] { "Flappy/Pillar.png", "Flappy/Player.png" };

        private void Player_SceneLoaded(Sprite me)
        {
            Task.Run(async () => 
            {
                me.SetCostume("Flappy/Player.png");
                me.SetPosition(LeftEdge / 2, 0);
                me.Show();

                // Apply gravity
                while(true)
                {
                    yspeed += gravity;
                    me.ChangeYby(yspeed);
                    await Delay(0.1);
                }
            });
        }
        private void Player_KeyPressed(Sprite me, Windows.UI.Core.KeyEventArgs what)
        {
            // Apply upward force
            if (what.VirtualKey == Windows.System.VirtualKey.Space)
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
                double pillar_image_height = 300;

                me.SetCostume("Flappy/Pillar.png");
                me.SetPosition(RightEdge, opening_bottom - pillar_image_height / 2 );

                var top = CreateSprite();
                me.Variable["top"] = top;
                top.SetCostume("Flappy/Pillar.png");
                top.SetPosition(RightEdge, opening_top + pillar_image_height / 2);
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
                top.ChangeXby(-5);
                var x = me.ChangeXby(-5);
                if (x < LeftEdge)
                {
                    me.MessageReceived -= Pillar_MessageReceived;
                    me.Destroy();
                    top.Destroy();
                }
            }
        }
    }
}
