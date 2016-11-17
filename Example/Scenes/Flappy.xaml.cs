using Example.Controls;
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

            this.Loaded += Scene_Loaded;
        }

        private void Scene_Loaded(object sender, RoutedEventArgs e)
        {
            // Spawn the needed number of pillars over the correct time.
            Task.Run(async () => 
            {
                await CreateSprite(Screen, this.Pillar_SceneLoaded);
                await Delay(3.0);
                await CreateSprite(Screen, this.Pillar_SceneLoaded);
                await Delay(3.0);
                await CreateSprite(Screen, this.Pillar_SceneLoaded);
                await Delay(3.0);
                await CreateSprite(Screen, this.Pillar_SceneLoaded);
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

        private void Player_SceneLoaded(Sprite me)
        {
            Task.Run(async () => 
            {
                await me.SetCostume("Flappy/Player.png");
                await me.SetPosition(250.0, 250.0);

                // Apply gravity
                while(true)
                {
                    yspeed += gravity;
                    await me.ChangeYby(yspeed);
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
                await me.ChangeYby(yspeed);
            }
        }

        private void Pillar_SceneLoaded(Sprite me)
        {
            Task.Run(async () => 
            {
                double y = Random(200,500);
                await me.SetCostume("Flappy/Pillar.png");
                await me.SetPosition(1000.0, y);

                var top = await CreateSprite(Screen);
                me.Variable["top"] = top;
                await top.SetCostume("Flappy/Pillar.png");
                await top.SetPosition(1000.0, y - 500);

                me.MessageReceived += Pillar_MessageReceived;
            });
        }

        private async void Pillar_MessageReceived(Sprite me, Sprite.MessageReceivedArgs what)
        {
            if (what.message == "update")
            {
                var top = me.Variable["top"] as Sprite;
                await top.ChangeXby(-20);
                var x = await me.ChangeXby(-20);
                if (x < -100.0)
                {
                    var y = Random(200, 500);
                    await me.SetPosition(1000,y);
                    await top.SetPosition(1000,y-500);
                }
            }
        }
    }
}
