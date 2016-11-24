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
    public sealed partial class StartMenu : Page
    {
        public StartMenu()
        {
            this.InitializeComponent();
        }

        private void Scene_02_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Scenes.Chapter02));
        }

        private void Scene_04_2_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Scenes.Chapter04));
        }

        private void Scene_Flappy_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Scenes.Flappy));
        }
        private void Scene_TestDirection_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Scenes.TestDirection));
        }
    }
}
