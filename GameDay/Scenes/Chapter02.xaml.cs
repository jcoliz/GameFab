using GameFab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GameDay.Scenes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Chapter02 : Scene
    {
        public Chapter02()
        {
            this.InitializeComponent();
        }

        private Sprite Lightning;
        private Sprite String;
        private Sprite Astro_Cat;
        private Sprite Banner;

        protected override IEnumerable<string> Assets => new[] { "02/1.png", "02/2.png", "02/4.png", "02/5.png", "02/6.png", "02/8.png" };

        private void Scene_Loaded(object sender, RoutedEventArgs e)
        {
            Lightning = CreateSprite(Lightning_Loaded);
            String = CreateSprite(String_Loaded);
            Astro_Cat = CreateSprite(Astro_Cat_Loaded);
            Banner = CreateSprite(Banner_Loaded);

            SetBackground("02/8.png");
        }

        private void Astro_Cat_Loaded(Sprite me)
        {
            me.SetCostume("02/6.png");
            me.Show();
            me.SetPosition(0,0);
            me.KeyPressed += Astro_Cat_KeyPressed;

            Task.Run(async () =>
            {
                while (Running)
                {
                    me.ChangeYby(2);
                    await Delay(0.3);
                    me.ChangeYby(-2);
                    await Delay(0.3);
                }
            });

            Task.Run(async () =>
            {
                while (Running)
                {
                    if (me.IsTouching(Lightning))
                    {
                        me.PlaySound("02/Zap.wav");
                        Lightning.Hide();
                        double opacity = me.ReduceOpacityBy(0.2);
                        if (opacity < 0.2)
                        {
                            me.Hide();
                            me.Say("You lose!!");
                        }
                    }
                    else
                        await Delay(0.2);
                }
            });
        }

        private void Astro_Cat_KeyPressed(Sprite me, Windows.UI.Core.KeyEventArgs what)
        {
            if (what.VirtualKey == Windows.System.VirtualKey.Down)
            {
                me.ChangeYby(-15);
            }
            if (what.VirtualKey == Windows.System.VirtualKey.Up)
            {
                me.ChangeYby(15);
            }
            if (what.VirtualKey == Windows.System.VirtualKey.Left)
            {
                me.ChangeXby(-15);
            }
            if (what.VirtualKey == Windows.System.VirtualKey.Right)
            {
                me.ChangeXby(15);
            }
        }

        private void Banner_Loaded(Sprite me)
        {
            me.SetCostume("02/4.png");
            me.SetPosition(0, 0);

            Task.Run(async () =>
            {
                int i = 3;
                while (i-- > 0)
                {
                    me.Show();
                    await Delay(1);
                    me.Hide();
                    await Delay(0.5);
                }
            });
        }

        private void Lightning_Loaded(Sprite me)
        {
            Task.Run(async () =>
            {
                me.SetCostume("02/5.png");
                await Delay(1);
                while (Running)
                {
                    await Delay(Random(0, 1.5));
                    me.SetPosition(Random(LeftEdge, RightEdge), TopEdge);
                    me.Show();

                    while (me.Position.Y > BottomEdge)
                    {
                        me.ChangeYby(-40);
                        await Delay(0.3);
                    }

                    me.Hide();
                }
            });
        }

        private void String_Loaded(Sprite me)
        {
            Task.Run(async () =>
            {
                me.SetCostume("02/1.png");
                await Delay(1);

                int i = 7;
                while (i-- > 0 && Running)
                {
                    me.SetPosition(Random(LeftEdge, RightEdge), Random(BottomEdge, TopEdge));
                    me.Show();

                    while (!me.IsTouching(Astro_Cat) && Running)
                    {
                        me.ChangeYby(1);
                        me.TurnBy(Sprite.Direction.Right, 5);
                        await Delay(0.2);
                        me.ChangeYby(-1);
                        me.TurnBy(Sprite.Direction.Right, 5);
                        await Delay(0.2);
                    }

                    me.PlaySound("02/Humming.wav");
                    Astro_Cat.Say("Got it!");
                    await Delay(0.5);
                    Astro_Cat.Say();
                    me.Hide();
                }

                me.SetCostume("02/2.png");
                me.PointInDirection_Rotate(0);
                me.SetPosition(0, 0);
                me.Show();
                me.Say("Stargate Opened!!!");

                while (!me.IsTouching(Astro_Cat) && Running)
                {
                    await Delay(0.2);
                }
                me.Hide();
                Astro_Cat.Say("Winner!");
                Running = false;

            });
        }
    }
}
