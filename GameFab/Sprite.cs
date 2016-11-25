using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace GameFab
{
    public class Sprite
    {
        #region Public Methods which implement Scratch-Like 'Blocks'

        /// <summary>
        /// Set the current costume of the sprite
        /// </summary>
        /// <param name="asset">File name of an image file in the Assets directory</param>
        /// <returns>Awaitable task</returns>
        public void SetCostume(string asset)
        {
            Costume = asset;
        }

        /// <summary>
        /// Set a series of costumes. This is used when we call 'NextCostume'
        /// </summary>
        /// <param name="assets"></param>
        public void SetCostumes(params string[] assets)
        {
            try
            {
                costumes = new List<string>(assets);
                nextcostumeindex = 1;

                SetCostume(costumes.First());
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Change to the next available costume. Be sure to set the costumes first with
        /// SetCostumes
        /// </summary>
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

        /// <summary>
        /// Set the position of the sprite within the scene
        /// </summary>
        public void SetPosition(Point where)
        {
            Position = where;
        }

        /// <summary>
        /// Sets the X position to the value guven
        /// </summary>
        /// <param name="x"></param>
        public void SetX(double x)
        {
            Position = new Point(x, Position.Y);
        }

        /// <summary>
        /// Sets the X position to the value guven
        /// </summary>
        /// <param name="x"></param>
        public void SetY(double y)
        {
            Position = new Point(Position.X, y);
        }

        /// <summary>
        /// Get the position of the sprite within the scene
        /// </summary>
        public Point GetPosition()
        {
            return Position;
        }

        /// <summary>
        /// Move the sprite down (or up) by this amount
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public double ChangeYby(double y)
        {
            Position = new Point(Position.X, Position.Y + y);

            return Position.Y;
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

        /// <summary>
        /// Set the size of this relative to its costume size
        /// </summary>
        /// <param name="size">1.0 is normal size, 0.5 is half, 2.0 is double</param>
        public void SetSize(double size)
        {
            Scale = Math.Max(size, 0.01);
        }

        /// <summary>
        /// Increase or reduce size by this amount
        /// </summary>
        /// <param name="size"></param>
        public void ChangeSizeBy(double size)
        {
            SetSize(Scale + size);
        }

        /// <summary>
        /// The direction we are facing, in degrees, where 0 is straight up
        /// </summary>
        /// <remarks>
        /// Valid values are &gt; -180 and &lt;= 180
        /// </remarks>
        public double Direction => 90 - Heading / Math.PI * 180;

        /// <summary>
        /// Point the sprite toward this target
        /// </summary>
        /// <param name="target"></param>
        public void PointTowards(Point target)
        {
            Heading = HeadingBetween(Position, target);

            if (rotationstyle == RotationStyle.AllAround)
            {
                RotationAngle = -Heading;
            }
        }

        /// <summary>
        /// Point the sprite in the direction indicated
        /// </summary>
        /// <remarks>
        /// Note that all angles are in degrees where zero is up
        /// </remarks>
        /// <param name="angle">Angle to point, in degrees, where zero is up. Valid values are -179 to 180</param>
        /// <returns></returns>
        public void PointInDirection(double angle)
        {
            Heading = DegreesToRadians(angle);

            if (rotationstyle == RotationStyle.AllAround)
            {
                PointInDirection_Rotate(angle);
            }
            else 
                PointInDirection_Rotate(90);
        }

        public enum RotationStyle { AllAround = 0, LeftRight, DoNotRotate };

        public void SetRotationStyle(RotationStyle style)
        {
            rotationstyle = style;

            if (rotationstyle != RotationStyle.AllAround)
            {
                PointInDirection_Rotate(90);
            }
        }

        /// <summary>
        /// Convert degrees in Scratch coordinate space to radians in internal coordinate space
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="degrees">Degrees from -179 to 180</param>
        /// <returns>Radians from 0 to 2*PI</returns>
        private double DegreesToRadians(double degrees)
        {
            return (90-degrees) / 180.0 * Math.PI;
        }

        public enum Facing { None = 0, Left, Right };

        /// <summary>
        /// Increase/decrease our visual rotation by the indicated amount
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="degrees"></param>
        public void TurnBy(Facing direction, double degrees)
        {
            if (direction == Facing.Left)
                degrees = -degrees;

            RotationAngle += degrees / 180 * Math.PI;
        }

        /// <summary>
        /// Set the angle of rotation for the sprite
        /// </summary>
        /// <param name="degrees"></param>
        private void PointInDirection_Rotate(double degrees)
        {
            RotationAngle = (degrees - 90) / 180 * Math.PI;
        }

        /// <summary>
        /// Move in the heading we're pointing by the indicated distance
        /// </summary>
        /// <param name="steps"></param>
        public void Move(double steps)
        {
            Position = ProgressToward(Position, Heading, steps);
        }

        /// <summary>
        /// Bounce off the screen edges, changing the heading to reflect off the
        /// edge if we are in range of an edge
        /// </summary>
        /// <remarks>
        /// This uses an 'edge' of 1000x500, which isn't always right.
        /// </remarks>
        public void IfOnEdgeBounce()
        {
            var position = GetPosition();
            var needstomove = false;

            if (position.X < -Owner.Dimensions.Width / 2)
            {
                needstomove = true;
                position.X = -Owner.Dimensions.Width / 2;
                Heading = Math.PI - Heading;
            }
            if (position.X > Owner.Dimensions.Width / 2)
            {
                needstomove = true;
                position.X = Owner.Dimensions.Width / 2;
                Heading = Math.PI - Heading;
            }
            if (position.Y < -Owner.Dimensions.Height / 2)
            {
                needstomove = true;
                position.Y = -Owner.Dimensions.Height / 2;
                Heading = -Heading;
            }
            if (position.Y > Owner.Dimensions.Height / 2)
            {
                needstomove = true;
                position.Y = Owner.Dimensions.Height / 2;
                Heading = -Heading;
            }
            if (needstomove)
                SetPosition(position);
        }

        /// <summary>
        /// Test whether this sprite is touching another sprite
        /// </summary>
        /// <param name="fe2">Which sprite to test against</param>
        /// <returns></returns>
        public bool IsTouching(Sprite fe2)
        {
            try
            {
                Rect rect1;
                Rect rect2;

                if (!Visible || !fe2.Visible)
                    return false;

                if (CollisionRadius.HasValue && fe2.CollisionRadius.HasValue)
                    return DistanceBetween(Position, fe2.Position) < CollisionRadius.Value + fe2.CollisionRadius.Value;

                rect1 = new Rect(new Point(Position.X - CostumeSize.Width / 2, Position.Y - CostumeSize.Height / 2), CostumeSize);
                rect2 = new Rect(new Point(fe2.Position.X - fe2.CostumeSize.Width / 2, fe2.Position.Y - fe2.CostumeSize.Height / 2), fe2.CostumeSize);

                if (CollisionRadius.HasValue)
                {
                    rect2 = new Rect(rect2.X - CollisionRadius.Value, rect2.Y - CollisionRadius.Value, rect2.Width + 2 * CollisionRadius.Value, rect2.Height + 2 * CollisionRadius.Value);
                    return rect2.Contains(Position);
                }

                if (fe2.CollisionRadius.HasValue)
                {
                    rect1 = new Rect(rect1.X - fe2.CollisionRadius.Value, rect1.Y - fe2.CollisionRadius.Value, rect1.Width + 2 * fe2.CollisionRadius.Value, rect1.Height + 2 * fe2.CollisionRadius.Value);
                    return rect1.Contains(fe2.Position);
                }

                rect1.Intersect(rect2);

                return !rect1.IsEmpty;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsTouchingEdge()
        {
            if (!Visible)
                return false;

            var screen = new Rect(Owner.LeftEdge, Owner.BottomEdge, Owner.Dimensions.Width, Owner.Dimensions.Height);

            return !screen.Contains(Position);
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

        /// <summary>
        /// Show this text in a 'speech bubble' near the sprite
        /// </summary>
        /// <param name="text">Text to show, or null to close the bubble</param>
        public void Say(string text = null)
        {
            Saying = text;
        }

        /// <summary>
        /// Reduce the sprites opacity. This is the 'ghost' effect
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double ReduceOpacityBy(double value)
        {
            Opacity -= value;
            if (Opacity < 0)
                Opacity = 0;

            return Opacity;
        }
        /// <summary>
        /// Move the sprite over time to a certain position
        /// </summary>
        /// <param name="total_time_seconds">How long it should take</param>
        /// <param name="destination">Where to go</param>
        /// <returns>async Task</returns>
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

        /// <summary>
        /// Bring to the front-most layer
        /// </summary>
        public void GoToFront()
        {
            lock (Sprites)
            {
                var frontest = Sprites.OrderBy(x => -x.Layer).First();
                Layer = frontest.Layer + 1;
            }
        }

        /// <summary>
        /// Move sprite deeper into the scene (away from camera) this many layers
        /// </summary>
        /// <param name="numlayers"></param>
        public void GoBackLayers(int numlayers)
        {
            Layer -= numlayers;
        }

        #endregion

        #region Event handlers you use to launch scripts

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

        /// <summary>
        /// Arguments which are passed in for an event involving the pointer
        /// </summary>
        public struct PointerArgs
        {
            /// <summary>
            /// The position of the mouse pointer when the event occurred
            /// </summary>
            public Point mousepoint;
        }

        /// <summary>
        /// Arguments which are passed in for an event involving another sprite
        /// </summary>
        public struct OtherSpriteArgs
        {
            /// <summary>
            /// The other sprite involved
            /// </summary>
            public Sprite sprite;
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

        /// <summary>
        /// Event fired when a key is pressed
        /// </summary>
        public event SpriteEventHandler<Windows.UI.Core.KeyEventArgs> KeyPressed;

        /// <summary>
        /// Event fired when a sprite NEWLY touches the indicated sprite.
        /// </summary>
        /// <remarks>
        /// If the sprite has already been touching it, this is NOT thrown
        /// </remarks>
        public event SpriteEventHandler<OtherSpriteArgs> Touched;

        #endregion

        #region Public Properties/Methods you might use from scripts, but are not strictly Scratch blocks

        /// <summary>
        /// Stash sprite-specific varaiables here
        /// </summary>
        public Dictionary<string, object> Variable { get; } = new Dictionary<string, object>();

        public void SetVariable<T>(string name, T value)
        {
            Variable[name] = value;
        }

        public T GetVariable<T>(string name)
        {
            if (Variable.ContainsKey(name))
                return (T)Variable[name];
            else
                return default(T);
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
        /// Setting this will make this sprite act like a circle for the purposes of
        /// collision detection. The radius of the circle is what you're setting here.
        /// Or set to null to have it behave like a regular rectangle.
        /// </summary>
        public double? CollisionRadius { get; set; } = null;
        
        #endregion

        #region Drawing parameters used by the scene to render us

        public Point Position { get; private set; }

        public string Costume { get; private set; }

        public bool Visible { get; private set; } = false;

        public double Opacity { get; private set; } = 1.0;

        public double Scale { get; private set; } = 1.0;

        /// <summary>
        /// Angle of rotation in radians
        /// </summary>
        public double RotationAngle { get; private set; } = 0.0;

        public string Saying { get; private set; } = null;

        /// <summary>
        /// In case anyone wants to know how large we are, it's the size of our costume.
        /// This is set by the scene when we are drawn.
        /// </summary>
        public Size CostumeSize { get; set; }

        /// <summary>
        /// Visual layer. Higher numbers are closer to the viewer
        /// </summary>
        public int Layer { get; set; } = 0;

        public bool FlipHorizontal => rotationstyle == RotationStyle.LeftRight && (Heading > Math.PI / 2 && Heading < 3 * Math.PI / 2);

        public static IReadOnlyList<Sprite> Sprites => All;

        #endregion

        #region Private properties
        Random random = new Random();
        MediaPlayer player = new MediaPlayer();
        List<string> costumes;
        int? nextcostumeindex = null;
        bool Removed = false; // If this has been removed from consideration, we will want to clean these up at some point
        RotationStyle rotationstyle = RotationStyle.AllAround;

        /// <summary>
        /// The direction of motion in which we are heading
        /// </summary>
        /// <remarks>
        /// Valid values are &gt;=0 and &lt;=2PI. Right is 0. Up is PI/2. Left is PI. Down is 3PI/2
        /// </remarks>
        double Heading
        {
            get
            {
                return _Heading;
            }
            set
            {
                _Heading = value;
                while (_Heading < 0)
                    _Heading += 2 * Math.PI;
                while (_Heading >= 2 * Math.PI)
                    _Heading -= 2 * Math.PI;
            }
        }
        double _Heading = 0.0;

        private static List<Sprite> All = new List<Sprite>();

        private static IEnumerable<Sprite> AllActive
        {
            get
            {
                IEnumerable<Sprite> result = null;
                lock (Sprites)
                {
                    result = All.Where(x => !x.Removed);
                }
                return result.ToList();
            }
        }

        private static uint NextID = 1;

        /// <summary>
        /// Unique identifier we can use when that's needed
        /// </summary>
        private uint ID = NextID++;

        public override int GetHashCode() => (int)ID;

        public override bool Equals(object obj) => (obj is Sprite && (obj as Sprite).ID == ID);

        #endregion

        #region Construction/Creation
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
        #endregion

        #region Public methods meant to be called by the Scene

        /// <summary>
        /// The scene who is containing & drawing us
        /// </summary>
        public Scene Owner { get; set; }
        public static void ClearAll()
        {
            lock (Sprites)
            {
                All.Clear();
            }
        }

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

        public static void SendTouched(Sprite first,Sprite second)
        {
            first.Touched?.Invoke(first, new OtherSpriteArgs() { sprite = second });
            second.Touched?.Invoke(first, new OtherSpriteArgs() { sprite = first });
        }

        #endregion

        #region Private helper methods
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
        #endregion

    }

}
