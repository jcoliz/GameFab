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
    public sealed partial class _04 : Page
    {
        Random random = new Random();
        Point mousepoint;
        bool mousepressed = false;

        public _04()
        {
            this.InitializeComponent();
            this.Loaded += Scene_Loaded;
        }

        private void Scene_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
            Window.Current.CoreWindow.PointerReleased += CoreWindow_PointerReleased;
        }

        private void CoreWindow_PointerReleased(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            mousepressed = false;
        }

        private void CoreWindow_PointerPressed(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            var clicked = args.CurrentPoint.Position;
            mousepoint = new Point(clicked.X, clicked.Y);
            mousepressed = true;
        }

        private async void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Down)
            {
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Up)
            {
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Left)
            {
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Right)
            {
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Space)
            {
                Sprite.Broadcast("start");
            }
        }

        private void Instructions_Loaded(object sender, RoutedEventArgs e)
        {
            var me = sender as Sprite;

            Task.Run(async () =>
            {
                await me.SetPosition(100, 100);
                await me.SetCostume("04/4.png");
            });

        }

        private void Instructions_MessageReceived(object sender, Sprite.MessageReceivedArgs e)
        {
            var me = sender as Sprite;

            if (e.message == "start")
            {
                var ignore = me.Hide();
            }
        }
    }
}
