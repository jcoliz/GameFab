# Welcome to Game Fab
This project is for kids who are good at Scratch, and now want to progress to something a bit more powerful. The idea is to give kids access to the power of a full programming language, on a complete modern platform, using paradigms they're already familiar with from Scratch.

#Choice of Technology

I have chosen to approach this problem using C# and .NET to create Universal Windows Platform apps. This gives Scratch-fluent kids access to the Windows Store, so they can publish to both Windows PC's and Xbox.

This initial choice is largely because it is the environment I am most familiar with, so it's easiest to get up and running. In the future, I would love to give kids the ability to build and distribute on iOS as well. I expect to use Xamarin for this, though I am open to a direct swift/SpriteKit implementation.

Under the hood, the graphics system runs on Win2D, which makes Direct2D immediately accessible to UWP apps. This is an excellent system, and I highly recommend it.

# How to Get Started

This project contains the core library and a sample with several game scenes. These games were adapted from the "Getting Started With Scratch" book from No Starch Press.

1. Clone the 'release' branch of the repository
2. Open GameFabJr.sln using Visual Studio 2015
3. Press F5 to build and debug, which will launch the "GameDay" sample.

# How far along is it?

Really, I am just getting started. My immediate goal is to fully implement all the projects in the "Getting Started with Scratch" book. Right now, I have completed two of them to a fully functional level. There are some edge case blocks, such as the 'color' effect, which I have put off until later.

Shortly, I hope to generate a documentation page so you can see exactly at a glance which blocks are implemented. Right now, you can check the comments in the Sprite.cs and Scene.cs files for this information.

# Are you taking contributions?

Yes! I am most interested in contributions that fill out missing Scratch blocks, plus of course testing in new environments, and filing bugs. Please see the "Major Known Issues" section below.

# How does it work?

Game Fab Jr. provides "Scenes" and "Sprites", just like Scratch. Scenes are XAML page files, such as the file "VirusAttack.xaml" under the "Scenes" folder in the sample. 

This is the simplest scene. We create a .XAML page containing a class derived from GameFab.Models. It only contains a Win32 canvas where we will draw into. Later, we could add UI elements on top of this to show the score, or provide a menu.

 <m:Scene
    x:Class="GameDay.Scenes._02"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:win2d="using:Microsoft.Graphics.Canvas.UI.Xaml"
    xmlns:m="using:GameFab.Models" 
    Loaded="Scene_Loaded">

    <win2d:CanvasAnimatedControl Draw="CanvasAnimatedControl_Draw" CreateResources="CanvasAnimatedControl_CreateResources" />
 </m:Scene>

The entry point into our .CS code-behind is the "Scene_Loaded" event handler.

    public sealed partial class _02 : Scene
    {
        public _02()
        {
            this.InitializeComponent();
        }

        private Sprite Player;

        protected override IEnumerable<string> Assets => new[] { "02/1.png", "02/2.png" };

        private async void Scene_Loaded(object sender, RoutedEventArgs e)
        {
            Astro_Cat = await CreateSprite(Astro_Cat_Loaded);
            SetBackground("02/1.png");
        }
        private void Astro_Cat_Loaded(Sprite me)
        {
            me.SetCostume("02/2.png");
            me.SetPosition(176,307);
            me.Show();
}

This is now a very simple scene, with a single player. The player doesn't do anything yet, just shows itself. Here, in the Scene_Loaded handler, we create the sprite and tell it what to do when it loads, plus we load the background. Also important is implementing the Assets property, where we will tell the system which images need to be loaded into the scene.

We could take another step to receive input, such as mouse/tap events or key presses. First, within Astro_Cat_Loaded, we'll need to wire up these handlers:

	Me.KeyPressed += Astro_Cat_KeyPressed
	Me.PointerPressed += Astro_Cat_PointerPressed

        private void Astro_Cat_KeyPressed(Sprite me, Windows.UI.Core.KeyEventArgs what)
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

        private async void Neo_Cat_PointerPressed(Models.Sprite me, Sprite.PointerArgs what)
        {
            await me.Glide(0.1, what.mousepoint);
        }

The "Loaded" event for a sprite is where scripts for Scratch's "When green flag pressed" are located. We will put one block of code for EACH of our scripts that would have had a green flag launch block. We use C# asynchronous programming to accomplish this. We create one Task for each green flag script, which will each run independently.

In this example, we have a script which bobs the player up and down repeatedly, another script which listens for keyboard input and moves the player, and another which checks for collision against the sprite named 'Obstacle' before broadcasting a message.

 Private void SuperCat_Loaded(Sprite me)
 {
	Task.Run(()=>
	{
		While(true)
		{
			Me.ChangeYby(-2);
			Await Delay(0.5);
			Me.ChangeYby(2);
			Await Delay(0.5);
		}
	});
	Task.Run(()=>
	{
		While(true)
		{
			If (await me.IsTouching(Obstacle))
				Broadcast("ouch");
			Await Delay(0.1);
		}
	});
}

Most Scratch blocks will have an analog in code, and most are implemented on the Sprite class. So a reference to "Me" is needed any time we are directly controlling a specific sprite.  Some scratch blocks which are more general and don't apply to a specific sprite, such as Delay and Broadcast, are implemented on the Scene itself, and so don't need a modifier.

The keyword 'await' is applied to any line where we want to wait for one thing to happen before starting another. In Scratch, this is automatic. In C#, we need to tell the system to wait. This is only needed for blocks which take time, such as Delay or SayFor.

Receiving messages

Much like the 'Loaded' event for the green flag, if we have different messages we can handle, and/or different scripts we want to run concurrently for the same message, all scripts are launched in a 'MessageReceived' event.

First we will have wired up the event in the Loaded event:

Me.MessageReceived += SuperCat_MessageReceived

The write the event handler itself:

Private void SuperCat_MessageReceived(Sprite me, String message)
{
Switch(message)
{
Case "lose":
	Task.Run()=>
	{
		me.Say("I lost!");
		Await Delay(0.5);
		me.Say("Bummer!");
		Await Delay(0.5);
		me.Hide();
	});
	Break;
Case "ouch":
	Task.Run()=>
	{
		// Manage taking a hit
		--Health;
		If (Health < 0)
			Broadcast("lose");
		Await me.SayFor("Ouch!",0.5);
	});
	Task.Run()=>
	{
		// Turn player to a ghost for a short while
		Await me.SetOpacity(0.5);
		Await Delay(2.0);
		Await me.SetOpacity(1.0);
	});
	
	Break;
}
}

Assets: Costumes, Sounds files, etc.

Place all visual and audio assets to be in the "Assets" folder of an app. Within that folder, you can organize or name them however you like. 

Within your scene, you'll override the "Assets" property of the Scene, and return a list of all image assets used in the scene. This ensures they are loaded into the Win2D drawing system, and ready for drawing.

        protected override IEnumerable<string> Assets => new[] { "04/21.png", "04/7.png", "04/8.png", "04/V.png", "04/I.png", "04/R.png", "04/U.png", "04/S.png", "04/1.png" };

To refer to these assets within blocks, refer to their name and/or subfolder within 'Assets'. Scratch blocks which would have taken an asset name, such as SetCostume, here take a filename string, which should be in the Assets directory, for example:

	me.SetCostume("Vampire/Fly01.png");
	PlaySound("Effects/Flap.mp3");

Major Known Issues

The biggest problems in the code that's already implemented are the coordinate system and the window extents.

The coordinate system uses the upper-left corner for positioning, which means the upper-left of all sprites is their "position". And the center of the screen could be anywhere, depending on how big the window is. I really need to move to a center-based coordinate system where a sprite's position is its center, and (0,0) is always the center of the screen despite how big the window is.

The window extents are not fixed. So on a large monitor, you could have a scene that's 2500x1500, or the window could be resized to something like 1000x500, which works pretty well, or on a phone, it could be 500x350. I haven't quite figured out what to do about this.
