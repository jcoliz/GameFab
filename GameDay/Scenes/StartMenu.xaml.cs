using GameFab;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public class MenuItem
        {
            public Type Destination;
            public SceneMenuEntryAttribute Details { get; set; }
        }

        public ObservableCollection<MenuItem> Items { get; } = new ObservableCollection<MenuItem>();

        public StartMenu()
        {
            this.InitializeComponent();

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
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var m = e.ClickedItem as MenuItem;
            Frame.Navigate(m.Destination);
        }
    }
}
