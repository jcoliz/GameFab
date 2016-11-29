using GameFab;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GameDay.Scenes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartMenu : Page
    {
        public class MenuItem
        {
            public Type Destination;
            public SceneMenuEntryAttribute Details { get; set; }
        }

        public ObservableCollection<MenuItem> Items { get; } = new ObservableCollection<MenuItem>();

        bool Running = true;

        public StartMenu()
        {
            var types = Application.Current.GetType().GetTypeInfo().Assembly.GetTypes();
            foreach (var t in types)
            {
                var p = t.GetTypeInfo().GetCustomAttributes<SceneMenuEntryAttribute>();
                if (p.Count() > 0)
                {
                    // TODO: Ordering!!
                    Items.Add(new MenuItem() { Destination = t, Details = p.First() });
                }
            }

            this.InitializeComponent();

            ListItems.Loaded += (s, e) => { ListItems.SelectedIndex = 0; };

            GamepadButtons old_buttons = GamepadButtons.None;
            Task.Run(async () => 
            {
                while (Running)
                {
                    await Task.Delay(1000 / 15);
                    if (Gamepad.Gamepads.Count > 0)
                    {
                        var buttons = Gamepad.Gamepads[0].GetCurrentReading().Buttons;
                        if (buttons.HasFlag(GamepadButtons.DPadDown) && !old_buttons.HasFlag(GamepadButtons.DPadDown))
                        {
                            var ignore = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => 
                            {
                                if (ListItems.SelectedIndex < Items.Count - 1)
                                    ++ListItems.SelectedIndex;
                            });
                        }
                        if (buttons.HasFlag(GamepadButtons.DPadUp) && !old_buttons.HasFlag(GamepadButtons.DPadUp))
                        {
                            var ignore = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                if (ListItems.SelectedIndex > 0)
                                    --ListItems.SelectedIndex;
                            });
                        }
                        if (buttons.HasFlag(GamepadButtons.A) && !old_buttons.HasFlag(GamepadButtons.A))
                        {
                            var ignore = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                var m = ListItems.SelectedItem as MenuItem;
                                Running = false;
                                Frame.Navigate(m.Destination);
                            });
                        }
                        old_buttons = buttons;
                    }
                }

            });

        }

        public int Index { get; set; } = 0;

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var m = e.ClickedItem as MenuItem;
            Running = false;
            Frame.Navigate(m.Destination);
        }
    }
}
