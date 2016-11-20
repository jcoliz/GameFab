﻿using System;
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
        Random random = new Random();
        Point mousepoint;

        abstract protected IEnumerable<string> Assets { get; }

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

        /// <summary>
        /// Whether the mouse/pointer is currently being held down
        /// </summary>
        protected bool IsMousePressed { get; private set; }

        protected Point MousePoint { get; private set; }

        protected Sprite CreateSprite(Sprite.SpriteEventHandler loaded = null)
        {
            var s = Sprite.Create();
            loaded?.Invoke(s);
            return s;
        }

        public Scene()
        {
            Sprite.ClearAll();
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

        string background;
        Dictionary<string, CanvasBitmap> bitmaps = new Dictionary<string, CanvasBitmap>();

        protected void CanvasAnimatedControl_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            if (Monitor.TryEnter(Sprite.Sprites))
            {
                try
                {
                    if (background != null && bitmaps.ContainsKey(background))
                    {
                        var origin = new Point() { X = 0, Y = 0 };
                        var destsize = sender.Size;
                        var destrect = new Rect(origin, destsize);
                        args.DrawingSession.DrawImage(bitmaps[background], destrect);

                    }
                    foreach (var sprite in Sprite.Sprites)
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
                                    TransformMatrix = Matrix3x2.CreateRotation((float)sprite.RotationAngle, new Vector2( (float)bitmap.Size.Width/2, (float)bitmap.Size.Height/2) )
                                };
                            }
                            args.DrawingSession.DrawImage(drawme, (float)sprite.Position.X, (float)sprite.Position.Y);


                            // Render the 'saying'
                            if (sprite.Saying?.Length > 0)
                            {
                                var drawingSession = args.DrawingSession;
                                var format = new CanvasTextFormat { FontSize = 30.0f, WordWrapping = CanvasWordWrapping.NoWrap };
                                var textLayout = new CanvasTextLayout(drawingSession, sprite.Saying, format, 0.0f, 0.0f);

                                float xcenter = (float)(sprite.Position.X + bitmap.Size.Width / 2.0);
                                float ytop = (float)(sprite.Position.Y + bitmap.Size.Height + 10.0);

                                var theRectYouAreLookingFor = new Rect(xcenter - textLayout.LayoutBounds.Width / 2 - 5, ytop, textLayout.LayoutBounds.Width + 10, textLayout.LayoutBounds.Height);
                                drawingSession.FillRectangle(theRectYouAreLookingFor, Colors.White);
                                drawingSession.DrawTextLayout(textLayout, xcenter - (float)textLayout.LayoutBounds.Width / 2, ytop, Colors.Black);
                            }
                        }
                    }
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

        public void SetBackground(string v)
        {
            background = v;
        }
    }
}