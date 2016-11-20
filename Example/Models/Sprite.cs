using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace Example.Models
{
    public class Sprite
    {
        #region Drawing Parameters
        public Point Position { get; private set; }

        public string Costume { get; private set; }

        public bool Visible { get; private set; } = false;

        public double Opacity { get; private set; } = 1.0;

        public double RotationAngle { get; private set; } = 0;

        public string Saying { get; private set; } = null;
        #endregion


        Random random = new Random();
        MediaPlayer player = new MediaPlayer();
        List<string> costumes;
        int? nextcostumeindex = null;
        double? heading = null;
        bool Removed = false; // If this has been removed from consideration, we will want to clean these up at some point

        /// <summary>
        /// Stash sprite-specific varaiables here
        /// </summary>
        public Dictionary<string, object> Variable { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// You don't need to call this yourself. Simply including a sprite in XAML will
        /// construct it.
        /// </remarks>
        private Sprite()
        {
            lock(Sprites)
            {
                All.Add(this);
            }
        }

        /// <summary>
        /// Creates a sprite for your use
        /// </summary>
        /// <remarks>
        /// This will recycle an existing removed sprite if there is one available
        /// </remarks>
        /// <returns></returns>
        public static Sprite Create()
        {
            // Is there a removed sprite we could recylce?
            var removed = All.Where(x => x.Removed).FirstOrDefault();
            if (removed != null)
            {
                removed.Removed = false;
                return removed;
            }
            else
                return new Sprite();

        }

        /// <summary>
        /// Flag the sprite as no longer needed
        /// </summary>
        /// <remarks>
        /// Be sure to remove all event hanlders first!!
        /// </remarks>
        public void Destroy()
        {
            this.Hide();
            this.SetCostume(null);
            this.Removed = true;
        }

        /// <summary>
        /// Set the current costume of the sprite
        /// </summary>
        /// <param name="asset">File name of an image file in the Assets directory</param>
        /// <returns>Awaitable task</returns>
        public void SetCostume(string asset)
        {
            Costume = asset;
        }

        public void SetCostumes(params string[] assets)
        {
            try
            {
                costumes = new List<string>(assets);
                nextcostumeindex = 1;

                SetCostume(costumes.First());
            }
            catch (Exception ex)
            {

            }
        }

        public void NextCostume()
        {
            var costume = nextcostumeindex.Value;
            if (costume >= costumes.Count)
                costume = 0;
            nextcostumeindex = costume + 1;
            SetCostume(costumes[costume]);
        }

        /// <summary>
        /// Show the sprite on the screen
        /// </summary>
        /// <returns></returns>
        public void Show()
        {
            Visible = true;
        }

        /// <summary>
        /// Hide the sprite so it doesn't show
        /// </summary>
        /// <returns></returns>
        public void Hide()
        {
            Visible = false;
        }

        /// <summary>
        /// Set the position of the sprite within the scene
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public void SetPosition(double x, double y)
        {
            Position = new Point(x, y);
        }
        public void SetPosition(Point where)
        {
            Position = where;
        }

        public Point GetPosition()
        {
            return Position;
        }

        /// <summary>
        /// Move the sprite down (or up) by this amount
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public void ChangeYby(double y)
        {
            Position = new Point(Position.X, Position.Y + y);
        }

        /// <summary>
        /// Move the sprite right (or left) by this amount
        /// </summary>
        /// <param name="y"></param>
        /// <returns>Resulting X position</returns>
        public double ChangeXby(double x)
        {
            Position = new Point(Position.X + x, Position.Y);

            return Position.X;
        }

        public void PointTowards(Point target)
        {
            heading = HeadingBetween(Position, target);
        }

        /// <summary>
        /// Point the sprite in the direction indicated
        /// </summary>
        /// <remarks>
        /// Note that all angles are in degrees where zero is up
        /// </remarks>
        /// <param name="angle">Angle to point, in degrees, where zero is up</param>
        /// <returns></returns>
        public void PointInDirection_Heading(double angle)
        {
            angle -= 90;
            var radians = angle * Math.PI / 180.0;
            heading = radians;
        }

        public void Move(double steps)
        {
            if (!heading.HasValue)
                return;

            Position = ProgressToward(Position, heading.Value, steps);
        }

        public void IfOnEdgeBounce()
        {
            if (!heading.HasValue)
                return;

            var position = GetPosition();
            var needstomove = false;

            if (position.X < 10.0)
            {
                needstomove = true;
                position.X = 10.0;
                heading = Math.PI - heading;
            }
            if (position.X > 990.0)
            {
                needstomove = true;
                position.X = 990.0;
                heading = Math.PI - heading;
            }
            if (position.Y < 10.0)
            {
                needstomove = true;
                position.Y = 10.0;
                heading = -heading;
            }
            if (position.Y > 490.0)
            {
                needstomove = true;
                position.Y = 490.0;
                heading = -heading;
            }
            if (needstomove)
                SetPosition(position);
        }

        public Size CostumeSize { get; set; }

        /// <summary>
        /// Test whether this sprite is touching another sprite
        /// </summary>
        /// <param name="fe2">Which sprite to test against</param>
        /// <returns></returns>
        public async Task<bool> IsTouching(Sprite fe2)
        {
            try
            {
                Rect rect1;
                Rect rect2;

                if (Visible)
                    rect1 = new Rect(Position, CostumeSize);
                else
                    rect1 = Rect.Empty;

                if (fe2.Visible)
                    rect2 = new Rect(fe2.Position, fe2.CostumeSize);
                else
                    rect2 = Rect.Empty;

                rect1.Intersect(rect2);

                return !rect1.IsEmpty;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Play a particular soound
        /// </summary>
        /// <param name="asset">Name of a WAV file in the Assets directory</param>
        /// <returns></returns>
        public void PlaySound(string asset)
        {
            player.Source = MediaSource.CreateFromUri(new Uri($"ms-appx:///Assets/{asset}"));
            player.Play();
        }

        public void Say(string text = null)
        {
            Saying = text;
        }

        public double ReduceOpacityBy(double value)
        {
            Opacity -= value;
            if (Opacity < 0)
                Opacity = 0;

            return Opacity;
        }

        public enum Direction { None = 0, Left, Right };

        public void TurnBy(Direction direction, double degrees)
        {
            if (direction == Direction.Left)
                degrees = -degrees;

            RotationAngle += degrees;
        }
        public void PointInDirection_Rotate(double degrees)
        {
            RotationAngle = degrees;
        }

        /// <summary>
        /// Arguments which are passed in for a MessageReceived event
        /// </summary>
        public struct MessageReceivedArgs
        {
            /// <summary>
            /// The contents of the message
            /// </summary>
            public string message;
        }

        public struct PointerArgs
        {
            public Point mousepoint;
        }

        /// <summary>
        /// Represents the method that will handle an event when the event provides data
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="me"></param>
        /// <param name="what"></param>
        public delegate void SpriteEventHandler<TEventArgs>(Sprite me, TEventArgs what);

        /// <summary>
        /// Represents the method that will handle an event that has no event data.
        /// </summary>
        public delegate void SpriteEventHandler(Sprite me);

        /// <summary>
        /// Event fired when we received a message
        /// </summary>
        public event SpriteEventHandler<MessageReceivedArgs> MessageReceived;

        /// <summary>
        /// Event fired when the pointer was pressed
        /// </summary>
        public event SpriteEventHandler<PointerArgs> PointerPressed;

        /// <summary>
        /// Event fired when the pointer was released
        /// </summary>
        public event SpriteEventHandler<PointerArgs> PointerReleased;

        /// <summary>
        /// Event fired when the scene is loaded
        /// </summary>
        public event SpriteEventHandler SceneLoaded;

        public event SpriteEventHandler<Windows.UI.Core.KeyEventArgs> KeyPressed;

        /// <summary>
        /// Broadcast this message to all sprites
        /// </summary>
        /// <remarks>
        /// To handle this, create a MessageReceived event handler
        /// </remarks>
        /// <param name="message"></param>
        public static void Broadcast(string message)
        {
            foreach (var sprite in AllActive)
                sprite.MessageReceived?.Invoke(sprite, new MessageReceivedArgs() { message = message });
        }

        public static void SendPointerPressed(Point point)
        {
            foreach (var sprite in AllActive)
                sprite.PointerPressed?.Invoke(sprite, new PointerArgs() { mousepoint = point });
        }

        public static void SendPointerReleased(Point point)
        {
            foreach (var sprite in AllActive)
                sprite.PointerReleased?.Invoke(sprite, new PointerArgs() { mousepoint = point });
        }

        public static void SendSceneLoaded()
        {
            foreach (var sprite in AllActive)
                sprite.SceneLoaded?.Invoke(sprite);
        }

        public static void SendKeyPressed(Windows.UI.Core.KeyEventArgs keys)
        {
            foreach (var sprite in AllActive)
                sprite.KeyPressed?.Invoke(sprite, keys);
        }

        public static IReadOnlyList<Sprite> Sprites => All;

        private static List<Sprite> All = new List<Sprite>();

        private static IEnumerable<Sprite> AllActive
        {
            get
            {
                IEnumerable<Sprite> result = null;
                lock(Sprites)
                {
                    result = All.Where(x => !x.Removed);
                }
                return result.ToList();
            }
        }

        public static void ClearAll()
        {
            lock(Sprites)
            {
                All.Clear();
            }
        }

        public async Task Glide(double total_time_seconds, Point destination)
        {
            double total_time = total_time_seconds * 1000;
            Point starting_position = GetPosition();
            const double update_frequency_ms = 25;
            double elapsed_time = 0;
            double total_distance = DistanceBetween(starting_position, destination);
            double heading = HeadingBetween(starting_position, destination);
            while (elapsed_time < total_time)
            {
                // distance we should be at THIS moment
                double now_distance = elapsed_time * total_distance / total_time;

                // new position is along the heading by that far
                Point new_position = ProgressToward(starting_position, heading, now_distance);

                this.SetPosition(new_position.X, new_position.Y);
                await Task.Delay((int)update_frequency_ms);

                elapsed_time += update_frequency_ms;
            }
            this.SetPosition(destination.X, destination.Y);
        }

        private static double DistanceBetween(Point first, Point second)
        {
            var x_distance = second.X - first.X;
            var y_distance = second.Y - first.Y;

            return Math.Sqrt(x_distance * x_distance + y_distance * y_distance);
        }

        private static double HeadingBetween(Point first, Point second)
        {
            var x_distance = second.X - first.X;
            var y_distance = second.Y - first.Y;

            return Math.Atan2(y_distance, x_distance);
        }

        private static Point ProgressToward(Point from, double heading, double distance)
        {
            var x_distance = Math.Cos(heading) * distance;
            var y_distance = Math.Sin(heading) * distance;

            return new Point(from.X + x_distance, from.Y + y_distance);
        }
    }

}
