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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GameDay.Scenes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Chapter05 : Scene
    {
        public Chapter05()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Assets needed by your scene
        /// </summary>
        /// <remarks>
        /// Replace this with all the backdrops and costumes you'll need in the scene
        /// </remarks>
        protected override IEnumerable<string> Assets => new[] { "05/1.png", "05/2.png", "05/3.png", "05/4.png", "05/5.png", "05/6.png", "05/7.png", "05/8.png", "05/10.png", "05/12.png", "05/14.png", "05/15.png", "05/16.png", };

        Sprite Keeper;
        Sprite Bullseye;
        Sprite Ball;
        Sprite Wave;
        Sprite Cloud;
        Sprite Banner;

        Variable<int> Score = new Variable<int>();
        Variable<int> BallsLeft = new Variable<int>(8);

        public void Scene_Loaded(object sender, RoutedEventArgs args)
        {
            SetBackground("05/16.png");
            CreateSprite((me)=> 
            {
                me.SetCostume("05/4.png");
                me.Show();
            });
            CreateSprite((me) =>
            {
                me.SetCostume("05/1.png");
                me.Show();
            });
            Keeper = CreateSprite(Keeper_Loaded);
            Ball = CreateSprite(Ball_Loaded);
            Bullseye = CreateSprite(Bullseye_Loaded);
        }

        private void Bullseye_Loaded(Sprite me)
        {
            me.SetPosition(-110, 78);
            me.Show();
            me.SetCostume("05/6.png");
        }

        private void Ball_Loaded(Sprite me)
        {
            me.SetPosition(0, BottomEdge / 2);
            me.Show();
            me.SetCostume("05/15.png");
        }

        private void Keeper_Loaded(Sprite me)
        {
            me.SetPosition(0, 0);
            me.Show();
            me.SetCostume("05/7.png");
        }
    }
}
