# Welcome to Game Fab

This project is for kids who are good at [Scratch](https://scratch.mit.edu/), and now want to progress to something a bit more powerful. The idea is to give kids access to the power of a full programming language, on a complete modern platform, using paradigms they're already familiar with from Scratch.

# Choice of Technology

I have chosen to approach this problem using C# and .NET to create Universal Windows Platform apps. This gives Scratch-fluent kids access to the Windows Store, so they can publish to both Windows PC's and Xbox.

This initial choice is largely because it is the environment I am most familiar with, so it's easiest to get up and running. In the future, I would love to give kids the ability to build and distribute on iOS as well. I expect to use Xamarin for this, though I am open to a direct swift/SpriteKit implementation.

Under the hood, the graphics system runs on [Win2D](https://github.com/Microsoft/Win2D), which makes Direct2D immediately accessible to UWP apps. This is an excellent system--I highly recommend it!

# How to Get Started

This project contains the core library and a sample with several game scenes. It includes mini-games were adapted from the [Super Scratch Programming Adventure!](https://www.nostarch.com/scratch) book from No Starch press.

1. Clone the 'release' branch of the repository
1. Open GameFabJr.sln using Visual Studio 2015
1. Set the default project to the 'GameDay' project
1. Set the platform to x64
1. Press F5 to build and debug, which will launch the "GameDay" sample.

# How far along is it?

Really, I am just getting started. My immediate goal is to fully implement all the projects contained in the [Super Scratch Programming Adventure!](https://www.nostarch.com/scratch) book from No Starch press. Our children used this book to learn Scratch, so I have followed it as a guide when implementing Game Fab.

Right now, I have completed two of the book's chapters to a fully functional mini-game. There are some edge case blocks, such as the 'color' effect, which I have put off until later.

Shortly, I hope to generate a documentation page so you can see exactly at a glance which blocks are implemented. Right now, you can check the comments in the Sprite.cs and Scene.cs files for this information.

# Are you taking contributions?

Yes! I am most interested in contributions that fill out missing Scratch blocks, plus of course testing in new environments, and filing bugs. New blocks should include new sample levels which demonstrate how to use the blocks.

# How does it work?

Game Fab Jr. provides "Scenes" and "Sprites", just like Scratch. Scenes are XAML page files, such as the file "VirusAttack.xaml" under the "Scenes" folder in the sample. 

Here is an example of the simplest scene. We create a .XAML page containing a class derived from GameFab.Scene. It only contains a Win2D canvas where we will draw into. Later, we could add UI elements on top of this to show the score, or provide a menu.

```xaml
	<gfab:Scene
		x:Class="GameDay.Scenes._02"
		xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
		xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
		xmlns:win2d="using:Microsoft.Graphics.Canvas.UI.Xaml"
		xmlns:gfab="using:GameFab" 
		Loaded="Scene_Loaded">

		<win2d:CanvasAnimatedControl 
			Draw="CanvasAnimatedControl_Draw" 
			CreateResources="CanvasAnimatedControl_CreateResources" />
	</gfab:Scene>
```

The entry point into our .CS code-behind is the "Scene_Loaded" event handler.

```c#
	public sealed partial class _02 : Scene
	{
		public _02()
		{
			this.InitializeComponent();
		}

		private Sprite Player;

		protected override IEnumerable<string> Assets => new[] { "02/Background.png", "02/Player.png" };

		private void Scene_Loaded(object sender, RoutedEventArgs e)
		{
			Player = CreateSprite(Player_Loaded);
			SetBackground("02/Background.png");
		}
	
		private void Player_Loaded(Sprite me)
		{
			me.SetCostume("02/Player.png");
			me.SetPosition(176,307);
			me.Show();
		}
	}
```

This is now a very simple scene, with a single player. The player doesn't do anything yet, just shows itself. Here, in the Scene_Loaded handler, we create the sprite and tell it what to do when it loads, plus we load the background. Also important is implementing the Assets property, where we will tell the system which images need to be loaded into the scene.

We could take another step to receive input, such as mouse/tap events or key presses. First, within Player_Loaded, we'll wire up these handlers:

```c#
	me.KeyPressed += Player_KeyPressed
	me.PointerPressed += Player_PointerPressed
```

And write the handlers themselves:

```c#
	private void Player_KeyPressed(Sprite me, Windows.UI.Core.KeyEventArgs what)
	{
		if (what.VirtualKey == Windows.System.VirtualKey.Down)
		{
			me.ChangeYby(15);
		}
		if (what.VirtualKey == Windows.System.VirtualKey.Up)
		{
			me.ChangeYby(-15);
		}
		if (what.VirtualKey == Windows.System.VirtualKey.Left)
		{
			me.ChangeXby(-15);
		}
		if (what.VirtualKey == Windows.System.VirtualKey.Right)
		{
			me.ChangeXby(15);
		}
	}

	private async void Player_PointerPressed(Models.Sprite me, Sprite.PointerArgs what)
	{
		await me.Glide(0.1, what.mousepoint);
	}
```

The "Loaded" event for a sprite is where scripts for Scratch's "When green flag pressed" are located. We will put one block of code for EACH of our scripts that would have had a green flag launch block. We use C# asynchronous programming to accomplish this. We create one Task for each green flag script, which will each run independently.

In this example, we have a script which bobs the player up and down repeatedly, another script which listens for keyboard input and moves the player, and another which checks for collision against the sprite named 'Obstacle' before broadcasting a message.

```c#
	private void Player_Loaded(Sprite me)
	{
		Task.Run(async ()=>
		{
			while(true)
			{
				me.ChangeYby(-2);
				await Delay(0.5);
				me.ChangeYby(2);
				await Delay(0.5);
			}
		});
		Task.Run(async ()=>
		{
			while(true)
			{
				If (await me.IsTouching(Obstacle))
					Broadcast("ouch");
				await Delay(0.1);
			}
		});
	}
```

Most Scratch blocks will have an analog in code, and most are implemented on the Sprite class. So a reference to "Me" is needed any time we are directly controlling a specific sprite.  Some scratch blocks which are more general and don't apply to a specific sprite, such as Delay and Broadcast, are implemented on the Scene itself, and so don't need a modifier.

The keyword 'await' is applied to any line where we want to wait for one thing to happen before starting another. In Scratch, this is automatic. In C#, we need to tell the system to wait. This is only needed for blocks which take time, such as Delay or SayFor.

# Receiving messages

Much like the 'Loaded' event for the green flag, if we have different messages we can handle, and/or different scripts we want to run concurrently for the same message, all scripts are launched in a 'MessageReceived' event.

First we will have wired up the event in the Loaded event:

```c#
	me.MessageReceived += Player_MessageReceived
```

Then write the event handler itself:

```c#
	private void Player_MessageReceived(Sprite me, String message)
	{
		switch(message)
		{
		case "lose":
			Task.Run(async ()=>
			{
				me.Say("I lost!");
				await Delay(0.5);
				me.Say("Bummer!");
				await Delay(0.5);
				me.Hide();
			});
			break;
		case "ouch":
			Task.Run(async ()=>
			{
				// Manage taking a hit
				--Health;
				If (Health < 0)
					Broadcast("lose");
				await me.SayFor("Ouch!",0.5);
			});
			Task.Run(async ()=>
			{
				// Turn player to a ghost for a short while
				me.SetOpacity(0.5);
				await Delay(2.0);
				me.SetOpacity(1.0);
			});	
			break;
		}
	}
```

# Assets: Costumes, Sounds files, etc.

Place all visual and audio assets in the "Assets" folder of an app. Within that folder, you can organize or name them however you like. 

Within your scene, you'll override the "Assets" property of the Scene, to return a list of all image assets used in the scene. This ensures they are loaded into the Win2D drawing system and ready for drawing.

```c#
	protected override IEnumerable<string> Assets => new[] 
	{ 
		"04/21.png", "04/7.png", "04/8.png", "04/V.png", "04/I.png", 
		"04/R.png", "04/U.png", "04/S.png", "04/1.png" 
	};
```

To refer to these assets within blocks, describe their name and/or subfolder within 'Assets'. Scratch blocks which would have taken an asset name, such as SetCostume, here take a filename string, which should be in the Assets directory, for example:

``` c#
	me.SetCostume("Vampire/Fly01.png");
	PlaySound("Effects/Flap.mp3");
```

# Stage dimensions

In Scratch, the dimensions of the stage are fixed at 480x360 pixels. Using GameFab, you can set them to whatever you like. Unless you change them, the stage will be 1280x720 pixels. When the application is in a window larger than this size, the stage is surrounded by a black mat. If the application is in a smaller window, all the assets will be automatically scaled down to fit the smaller window.

To change the dimensions, set the 'Dimensions' property of the scene to a new size. In a script, simply do this:

```c#
	Dimensions = new Size(480,360);
```

The scene includes helper properties to describe the visisble edges of the scene. For example, imagine a sprite which constantly moves left, and then wraps around to the right edge of the screen after moving off the left edge. We might do this:

```c#
	me.ChangeXby(-10);
	if (me.GetPosition().X < LeftEdge)
	{
		me.SetPosition(RightEdge,me.GetPosition().Y);
	}
```

# Coordinates

As with Scratch, the center of the screen is 0,0. To move a sprite to the right, increase its X position. To move it up, increase its Y position. Sprites can be rotated to any angle, expressed in degrees (0-360), where 0 points straight up.

# Adding UI

The Scene definition in XAML allows us to add in other UI elements on top of the drawing canvas. For example, here is the XAML scene definition for the Flappy example. This adds an additional bit of XAML UI atop the drawn scene.

```xaml
	<gfab:Scene Loaded="Scene_Loaded"
		x:Class="GameDay.Scenes.Flappy"
		xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
		xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
		xmlns:win2d="using:Microsoft.Graphics.Canvas.UI.Xaml"
		xmlns:gfab="using:GameFab">

	  <Grid Background="Black">
		<win2d:CanvasAnimatedControl 
			Draw="CanvasAnimatedControl_Draw" 
			CreateResources="CanvasAnimatedControl_CreateResources" />
		<StackPanel HorizontalAlignment="Center" Margin="20,0">
		  <TextBlock 
			Text="Score" 
			FontSize="30" 
			Foreground="White" 
			HorizontalAlignment="Center"/>
		  <TextBlock 
			Text="{x:Bind Score.Value,Mode=OneWay}" 
			FontSize="100" 
			Foreground="White" 
			HorizontalAlignment="Center" 
			Margin="0,-20"/>
		</StackPanel>
	  </Grid>
	</gfab:Scene>
```

This example uses the special 'Variable' class which is designed to make this easy. From our code-behind, we will declare one such variable:

```c#
    public Variable<int> Score = new Variable<int>(0);
```

Then, in our script we can simply update its value, and the system will automatically display the updated Value:

```c#
	++Score.Value;
```