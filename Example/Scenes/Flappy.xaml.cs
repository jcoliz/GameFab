using Example.Models;
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

namespace Example.Scenes
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
                var player = await CreateSprite(Player_SceneLoaded);
                player.KeyPressed += Player_KeyPressed;
                await CreateSprite(this.Pillar_SceneLoaded);
                await Delay(3.0);
                await CreateSprite(this.Pillar_SceneLoaded);
                await Delay(3.0);
                await CreateSprite(this.Pillar_SceneLoaded);
                await Delay(3.0);
                await CreateSprite(this.Pillar_SceneLoaded);
            });

            // Syncrhonize the visual updates by sending out a message on a regular time
            Task.Run(async () => 
            {
                while(true)
                {
                    await Delay(0.2);
                    Broadcast("update");
                }
            });
        }

        double yspeed = 0;
        double gravity = 2; // in pixels per tick squared

        protected override IEnumerable<string> Assets => new[] { "Flappy/Pillar.png", "Flappy/Player.png" };

        private void Player_SceneLoaded(Sprite me)
        {
            Task.Run(async () => 
            {
                me.SetCostume("Flappy/Player.png");
                me.SetPosition(250.0, 250.0);
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
        private async void Player_KeyPressed(Sprite me, Windows.UI.Core.KeyEventArgs what)
        {
            // Apply upward force
            if (what.VirtualKey == Windows.System.VirtualKey.Space)
            {
                yspeed = -20;
                me.ChangeYby(yspeed);
            }
        }

        private void Pillar_SceneLoaded(Sprite me)
        {
            Task.Run(async () => 
            {
                double y = Random(200,500);
                me.SetCostume("Flappy/Pillar.png");
                me.SetPosition(1000.0, y);

                var top = await CreateSprite();
                me.Variable["top"] = top;
                top.SetCostume("Flappy/Pillar.png");
                top.SetPosition(1000.0, y - 500);
                me.Show();
                top.Show();

                me.MessageReceived += Pillar_MessageReceived;
            });
        }

        private async void Pillar_MessageReceived(Sprite me, Sprite.MessageReceivedArgs what)
        {
            if (what.message == "update")
            {
                var top = me.Variable["top"] as Sprite;
                top.ChangeXby(-20);
                var x = me.ChangeXby(-20);
                if (x < -100.0)
                {
                    var y = Random(200, 500);
                    me.SetPosition(1000,y);
                    top.SetPosition(1000,y-500);
                }
            }
        }
    }
}
