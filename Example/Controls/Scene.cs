using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas.Brushes;

namespace Example.Controls
{
    public class Scene: Page
    {
        Random random = new Random();
        Point mousepoint;

        /// <summary>
        /// Generate a random number between these parameters (inclusive)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        protected int Random(int from, int to)
        {
            return random.Next(from, to);
        }

        /// <summary>
        /// Delay the current task by this many seconds
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        protected Task Delay(double seconds)
        {
            return Task.Delay((int)(seconds * 1000.0));
        }

        /// <summary>
        /// Broadcast this message to all sprites
        /// </summary>
        /// <param name="message"></param>
        protected void Broadcast(string message)
        {
            Sprite.Broadcast(message);
        }

        /// <summary>
        /// Whether the mouse/pointer is currently being held down
        /// </summary>
        protected bool IsMousePressed { get; private set; }

        protected Point MousePoint { get; private set; }

        protected async Task<Sprite> CreateSprite(Canvas parent, Sprite.SpriteEventHandler loaded = null)
        {
            Sprite s = null;
            var sem = new SemaphoreSlim(1);
            await sem.WaitAsync();
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                s = new Sprite();
                parent.Children.Add(s);
                if (loaded != null)
                    Task.Run(() => { loaded(s); });

                sem.Release();
            });
            await sem.WaitAsync();
            return s;
        }

        public Scene()
        {
            base.Loaded += Scene_Loaded;
            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                Frame.CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled && this.Frame.CanGoBack)
            {
                e.Handled = true;
                this.Frame.GoBack();
            }
        }

        private void Scene_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
            Window.Current.CoreWindow.PointerReleased += CoreWindow_PointerReleased;

            Sprite.SendSceneLoaded();
        }

        private void CoreWindow_PointerReleased(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            IsMousePressed = false;
            MousePoint = args.CurrentPoint.Position;
            Sprite.SendPointerReleased(MousePoint);
        }

        private void CoreWindow_PointerPressed(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            IsMousePressed = true;
            MousePoint = args.CurrentPoint.Position;
            Sprite.SendPointerPressed(MousePoint);
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            Sprite.SendKeyPressed(args);
        }

        public class Variable<T>: INotifyPropertyChanged
        {
            public Variable(T value = default(T))
            {
                _Value = value;
            }
            public T Value
            {
                get
                {
                    return _Value;
                }
                set
                {
                    _Value = value;
                    Context?.Post(o => 
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                    }, null);                    
                }
            }
            private T _Value;
            private SynchronizationContext Context = SynchronizationContext.Current;

            public event PropertyChangedEventHandler PropertyChanged;
        }


        CanvasBitmap background = null;

        public void Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender,CanvasAnimatedDrawEventArgs args)
        {
            var origin = new Point() { X = 0, Y = 0 };
            var destsize = sender.Size;
            var destrect = new Rect(origin,destsize);
            args.DrawingSession.DrawImage(background, destrect);
        }

        internal void CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(LoadResources(sender).AsAsyncAction());
        }

        private async Task LoadResources(CanvasAnimatedControl sender)
        {
            try
            {
                background = await CanvasBitmap.LoadAsync(sender, new Uri( "ms-appx:///Assets/04/21.png"));
            }
            catch (Exception ex)
            {

            }
        }
    }
}
