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
using Microsoft.Graphics.Canvas.Text;
using Windows.UI;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;

namespace GameFab
{
    public abstract class Scene: Page
    {

        #region Public methods which implement Scratch 'blocks'

        /// <summary>
        /// Generate a random number between these parameters (inclusive)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        protected double Random(double from, double to)
        {
            return random.Next((int)(from * 1000), (int)(to * 1000)) / 1000.0;
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

        #endregion

        #region Other public methods which are interesting from scripts

        protected Sprite CreateSprite(Sprite.SpriteEventHandler loaded = null)
        {
            var s = Sprite.Create();
            s.Owner = this;
            loaded?.Invoke(s);
            return s;
        }

        protected void SetBackground(string v)
        {
            background = v;
        }

        #endregion

        #region Private properties
        Random random = new Random();
        Point mousepoint;
        #endregion

        #region Protected properties. These are available from scripts if you need them

        abstract protected IEnumerable<string> Assets { get; }

        /// <summary>
        /// Whether the scripts in this scene should continue running
        /// </summary>
        protected bool Running { get; set; } = true;

        /// <summary>
        /// Whether the mouse/pointer is currently being held down
        /// </summary>
        protected bool IsMousePressed { get; private set; }

        protected Point MousePoint { get; private set; }

        public Size Dimensions { get; set; } = new Size(1280, 720);

        public double LeftEdge => -Dimensions.Width / 2;
        public double RightEdge => Dimensions.Width / 2;
        public double TopEdge => Dimensions.Height / 2;
        public double BottomEdge => -Dimensions.Height / 2;

        #endregion

        #region Constructor

        protected Scene()
        {
            Sprite.ClearAll();
            base.Loaded += Scene_Loaded;
            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;
        }

        #endregion

        #region Internals

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                Frame.CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
            Window.Current.CoreWindow.PointerPressed -= CoreWindow_PointerPressed;
            Window.Current.CoreWindow.PointerReleased -= CoreWindow_PointerReleased;
            base.Loaded -= Scene_Loaded;
            SystemNavigationManager.GetForCurrentView().BackRequested -= SystemNavigationManager_BackRequested;
            Running = false;
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

            var x =  args.CurrentPoint.Position.X - (sender.Bounds.Right - sender.Bounds.Left) / 2;
            var y = (sender.Bounds.Bottom - sender.Bounds.Top) / 2 - args.CurrentPoint.Position.Y;

            x /= scale;
            y /= scale;

            MousePoint = new Point(x,y);
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

        #endregion

        #region Drawing

        string background;
        Dictionary<string, CanvasBitmap> bitmaps = new Dictionary<string, CanvasBitmap>();
        float scale = 1.0f;

        protected void CanvasAnimatedControl_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            if (Monitor.TryEnter(Sprite.Sprites))
            {
                try
                {
                    if (sender.Size.Width < Dimensions.Width || sender.Size.Height < Dimensions.Height)
                        scale = (float)Math.Min(sender.Size.Width / Dimensions.Width, sender.Size.Height / Dimensions.Height);
                    else
                        scale = 1.0f;

                    var origin = new Point() { X = (sender.Size.Width - Dimensions.Width) / 2, Y = (sender.Size.Height - Dimensions.Height) / 2 };
                    var destrect = new Rect(origin, Dimensions);
                    args.DrawingSession.Transform = Matrix3x2.CreateScale(scale,new Vector2((float)(sender.Size.Width/2), (float)(sender.Size.Height / 2)));

                    using (args.DrawingSession.CreateLayer(1.0f, destrect))
                    {
                        if (background != null && bitmaps.ContainsKey(background))
                        {
                            args.DrawingSession.DrawImage(bitmaps[background], destrect);
                        }

                        var center = new Point(sender.Size.Width / 2, sender.Size.Height / 2);
                        foreach (var sprite in Sprite.Sprites.OrderBy(x => x.Layer))
                        {
                            if (sprite.Costume != null && sprite.Visible && bitmaps.ContainsKey(sprite.Costume))
                            {
                                var bitmap = bitmaps[sprite.Costume];
                                sprite.CostumeSize = bitmap.Size;
                                ICanvasImage drawme = bitmap;
                                if (sprite.Opacity < 1.0)
                                {
                                    drawme = new OpacityEffect()
                                    {
                                        Source = drawme,
                                        Opacity = (float)sprite.Opacity
                                    };
                                }
                                if (sprite.RotationAngle != 0.0)
                                {
                                    drawme = new Transform2DEffect()
                                    {
                                        Source = drawme,
                                        TransformMatrix = Matrix3x2.CreateRotation((float)sprite.RotationAngle, new Vector2((float)bitmap.Size.Width / 2, (float)bitmap.Size.Height / 2))
                                    };
                                }
                                var draw_at = new Point(center.X + sprite.Position.X - sprite.CostumeSize.Width / 2, center.Y - sprite.Position.Y - sprite.CostumeSize.Height / 2);
                                args.DrawingSession.DrawImage(drawme, (float)draw_at.X, (float)draw_at.Y);

                                // Render the 'saying'
                                if (sprite.Saying?.Length > 0)
                                {
                                    var drawingSession = args.DrawingSession;
                                    var format = new CanvasTextFormat { FontSize = 30.0f, WordWrapping = CanvasWordWrapping.NoWrap };
                                    var textLayout = new CanvasTextLayout(drawingSession, sprite.Saying, format, 0.0f, 0.0f);

                                    float xcenter = (float)(center.X + sprite.Position.X);
                                    float ytop = (float)(center.Y - sprite.Position.Y + bitmap.Size.Height / 2 + 10.0);

                                    var theRectYouAreLookingFor = new Rect(xcenter - textLayout.LayoutBounds.Width / 2 - 5, ytop, textLayout.LayoutBounds.Width + 10, textLayout.LayoutBounds.Height);
                                    drawingSession.FillRectangle(theRectYouAreLookingFor, Colors.White);
                                    drawingSession.DrawTextLayout(textLayout, xcenter - (float)textLayout.LayoutBounds.Width / 2, ytop, Colors.Black);
                                }
                            }
                        }

                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {
                    Monitor.Exit(Sprite.Sprites);
                }
            }
        }

        protected void CanvasAnimatedControl_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            sender.TargetElapsedTime = TimeSpan.FromMilliseconds(1000 / 30);
            args.TrackAsyncAction(LoadResources(sender).AsAsyncAction());
        }

        private async Task LoadResources(CanvasAnimatedControl sender)
        {
            try
            {
                foreach(var i in Assets)
                    bitmaps[i] = await CanvasBitmap.LoadAsync(sender, new Uri($"ms-appx:///Assets/{i}"));
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}
