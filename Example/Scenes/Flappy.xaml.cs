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
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var s = new Sprite();
                    Screen.Children.Add(s);
                    Task.Run(() => { Pillar_SceneLoaded(s); });
                });
                await Delay(3.0);
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var s = new Sprite();
                    Screen.Children.Add(s);
                    Task.Run(() => { Pillar_SceneLoaded(s); });
                });
                await Delay(3.0);
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var s = new Sprite();
                    Screen.Children.Add(s);
                    Task.Run(() => { Pillar_SceneLoaded(s); });
                });
                await Delay(3.0);
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var s = new Sprite();
                    Screen.Children.Add(s);
                    Task.Run(() => { Pillar_SceneLoaded(s); });
                });
                /*
                await Delay(1.0);
                Spawn();
                await Delay(1.0);
                Spawn();
                */
            });
        }

        private void Player_PointerPressed(Sprite me, Sprite.PointerArgs what)
        {

        }

        private void Player_Loaded(object sender, RoutedEventArgs e)
        {

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
                    var position = await me.GetPosition();
                    position.Y += yspeed;
                    await me.SetPosition(position);
                    await Delay(0.1);
                }
            });
        }

        private void Pillar_SceneLoaded(Sprite me)
        {
            Task.Run(async () => 
            {
                double y = Random(200,500);
                await me.SetCostume("Flappy/Pillar.png");
                await me.SetPosition(1000.0, y);

                Sprite top = null;
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => 
                {
                    top = new Sprite();
                    Screen.Children.Add(top);
                });
                await top.SetCostume("Flappy/Pillar.png");
                await top.SetPosition(1000.0, y - 500);

                while (true)
                {
                    await Delay(0.2);
                    await me.ChangeXby(-20);
                    await top.ChangeXby(-20);
                    var position = await me.GetPosition();
                    if (position.X < -100.0)
                    {
                        position.X = 1000.0;
                        position.Y = Random(200,500);
                        await me.SetPosition(position);
                        position.Y -= 500;
                        await top.SetPosition(position);
                    }
                }
            });
        }

        private async void Player_KeyPressed(Sprite me, Windows.UI.Core.KeyEventArgs what)
        {
            // Apply upward force
            if (what.VirtualKey == Windows.System.VirtualKey.Space)
            {
                yspeed = -20;
                var position = await me.GetPosition();
                position.Y += yspeed;
                await me.SetPosition(position);
            }
        }
    }
}
