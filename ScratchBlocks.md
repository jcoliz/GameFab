# Motion blocks

Block | Function | Available? | Comments
--- | --- | --- | ---
Move () Steps | Moves the sprite forward the amount of steps in the direction the sprite is facing. | Yes
Turn () Degrees (Clockwise) | Turns the sprite (clockwise) the specified amount. | Yes
Turn () Degrees (Counter-clockwise) | Turns the sprite (counter-clockwise) the specified amount. | Yes
Point in Direction () | Points the sprite in the direction. | Yes
Point Towards () | Points the sprite towards the mouse-pointer or another sprite. | Yes
Go to X: () Y: () | Moves the sprite to the specified X and Y position. | Yes
Go to () | Moves the sprite to the mouse-pointer or another sprite. | Yes | Use me.SetPosition(othersprite.Position)
Glide () Secs to X: () Y: () | Glides the sprite to the location, taking as long as the specified amount of time. | Yes
Change X by () | Changes the sprite's X position by the amount. | Yes
Set X to () | Sets the sprite's X position to the amount. | Yes
Change Y by () | Changes the sprite's Y position by the amount. | Yes
Set Y to () | Sets the sprite's Y position to the amount. | Yes
If on Edge, Bounce | If touching the edge of the screen, the sprite's direction flips over. | Yes
Set Rotation Style () | This sets the rotation style of a sprite. | Yes
X Position | The X position of the sprite. | Yes | me.Position.X
Y Position | The Y position of the sprite. | Yes | me.Position.Y
Direction | The direction of the sprite. | Yes

# Looks blocks

Block | Function | Available? | Comments
--- | --- | --- | ---
Say () for () Secs | A speech bubble appears over the sprite and stays for the specified amount of time. | No | Use Say() and Wait
Say () | A speech bubble appears over the sprite and will not go away over time. | Yes
Think () for () Secs | A thought bubble appears over the sprite and stays for the specified amount of time. | No | Use Say
Think () | A thought bubble appears over the sprite and will not go away over time. | No | Use Say
Show | Shows the sprite. | Yes
Hide | Hides the sprite. | Yes
Switch Costume to () | Changes the sprite's/Stage's costume/backdrop to the specified one. | Yes |
Switch Backdrop to () | | Yes
Switch Backdrop to () and wait | Like the Switch to Backdrop () block, though it waits until all of the hat blocks triggered by this have completed. (Stage only) | No
Next Costume | Changes the sprite's/Stage's costume/backdrop to the next one in the costume list. | Yes
Next Backdrop | | No
Change () Effect by () | Changes the specified effect by the amount. | No | Can use Opacity property for 'change ghost effect'
Set () Effect to () | Sets the specified effect to the amount. | No
Clear Graphic Effects | Clears all graphic effects on the sprite. | No
Change Size by () | Changes the sprite's size by the amount. | Yes
Set Size to ()% | Sets the sprite's size to the amount. | Yes
Go to Front | Puts a sprite in the front. | Yes
Go Back () Layers | Changes the sprite's layer value by the amount. | Yes
Costume # (for sprites) | The number of the sprite/Stage's current costume/backdrop in the list. | Yes | Use me.Costume to get the custome name.
Backdrop # (for the Stage) | | No
Backdrop Name | Reports the name of the current backdrop. | No
Size | The sprite's size. | Yes | Use me.CostumeSize

# Sound blocks

Block | Function | Available? | Comments
--- | --- | --- | ---
Play Sound () | Plays a sound without pausing the script. | Yes
Play Sound () Until Done | Plays a sound and pauses the script until it finishes.
Stop All Sounds | Stops all playing sounds.
Play Drum () for () Beats | Plays the specified drum for the amount of beats.
Rest for () Beats | Pauses the script for the amount of time.
Play Note () for () Beats | Plays the note for the amount of beats.
Set Instrument to () | Sets the instrument to the specified one.
Change Volume by () | Changes the volume by the amount.
Set Volume to () % | Sets the volume to the amount.
Change Tempo by () | Changes the tempo by the amount.
Set Tempo to () bpm | Sets the tempo to the amount.
Volume | The volume.
Tempo | The tempo.

# Pen blocks

Pen blocks are completely not available.

Clear — Removes all pen marks put on the screen.
Stamp — Pens the sprite's image on the screen. Can be removed using clear.
Pen Down — Puts the sprite's pen down.
Pen Up — Puts the sprite's pen up.
Set Pen Color to () (color-picker) | Sets the pen color to the specified color shown on the picture.
Change Pen Color by () | Changes the pen color by the amount.
Set Pen Color to () (number) | Sets the pen color to the amount.
Change Pen Shade by () | Changes the pen shade by the amount.
Set Pen Shade to () | Sets the pen shade to the amount.
Change Pen Size by () | Changes the pen size by the amount.
Set Pen Size to () | Sets the pen size to the amount.

# Variables blocks

Block | Function | Available? | Comments
--- | --- | --- | ---
Set () to () | Sets the specified variable to the amount. | Yes | Use C# variables or Variable<T> to display it
Change () by () | Changes the specified variable by the amount. | Yes | Use C#
Show Variable () | Shows the variable's Stage Monitor. | No
Hide Variable () | Hides the variable's Stage Monitor. | No

# List blocks

List blocks can all be replaced by C# List<T> operations.

Add () to () — Adds an item to the list (the item goes at the bottom of the list of items) with the specified content in it.
Delete () of () — Deletes the item of the list.
Insert () at () of () — Adds an item to the list (the item goes where you specify in the list of items) with the specified content in it.
Replace Item () of () with () — Replaces the item's content with the specified content.
Show List () — Shows a list.
Hide List () — Hides a list.
() — The list's value.
Item () of () — The item's value.
Length of () — How many items there are in the specified list.
() Contains () — The condition for checking if an item's content is the specified text.

# Event blocks

Block | Function | Available? | Comments
--- | --- | --- | ---
When Green Flag Clicked | When the flag is clicked, the script activates. | Yes | Use Loaded event
When () Key Pressed | When the specified key is pressed, the script activates. | Yes | Use KeyPressed event
When This Sprite Clicked | When the sprite is clicked, the script activates. | No
When Backdrop Switches to () | When the backdrop switches to the one chosen, the script activates. | No
When () is greater than () | When the first value is greater than the second value, the script activates. | No
When I Receive () | When the broadcast is received, the script activates. | Yes | Use MessageReceived event
Broadcast () | Sends a broadcast throughout the Scratch program, activating When I Receive () blocks that are set to that broadcast. | Yes
Broadcast () and Wait | Like the Broadcast () block, but pauses the script until all scripts activated by the broadcast are completed. | No

# Control blocks

Block | Function | Available? | Comments
--- | --- | --- | ---
When I Start as a Clone (sprites only) | This hat block is triggered whenever a clone is created, and will only be run by that clone. | Yes | Send your script to CreateSprite()
Wait () Secs | Pauses the script for the amount of time. | Yes | Use await Delay()
Wait Until () | Pauses the script until the condition is true. | Yes | Use while(!condition) await Delay()
Create Clone of () | Creates the specified clone. | Yes | Use CreateSprite()
Repeat () | A loop that repeats the specified amount of times. | Yes | Use while()
Forever | A loop that will never end. | Yes | Use while(Running)
If () Then | Checks the condition so that if the condition is true, the blocks inside it will activate. | Yes | Use C# 'if'
If () Then, Else | Checks the condition so that if the condition is true, the blocks inside the first C will activate and if the condition is false, the blocks inside the second C will activate. | Yes | Use C# 'if'/'else'
Repeat Until () | A loop that will stop once the condition is true. | Yes
Stop () | Stops the scripts chosen through the drop-down menu. Can also be a stack block when "other scripts in this sprite" is chosen. | No
Delete This Clone (sprites only) | Deletes a clone. | Yes | Use me.Remove()

# Sensing blocks

Block | Function | Available? | Comments
--- | --- | --- | ---
Ask () and Wait | An input box appears — you type the value in and it stores the value in the answer variable. | No
Reset Timer | Resets the timer. | No
Turn Video () | Turns the video on. | No
Set Video Transparency to ()% | Sets the transparency of the video. | No
Touching ()? | The condition for checking if the sprite is touching the mouse-pointer or another sprite. | Yes
Touching Color ()? | The condition for checking if the sprite is touching a specific color. | No
Color () is Touching ()? | The condition for checking if a color on the sprite is touching a specific color. | No
Key () Pressed? | The condition for checking if the specified key is being pressed. | No | Use KeyPressed event
Mouse Down? | The condition for checking if the mouse is down. | No | Use PointerPressed event
Distance to () | The distance from the sprite to the mouse-pointer or another sprite. | No
Answer | The most recent input with the Ask () And Wait block. | No
Mouse X | The mouse-pointer's X position. | No | Use PointerPressed event
Mouse Y | The mouse-pointer's Y position. | No | Use PointerPressed event
Loudness | How loud the noise is that the microphone is sensing. | No
Timer | How much time has passed since the Scratch program was opened or the timer reset. | No
Video () on () | The video motion or direction of video motion on an object. | No
() of () | The X position, Y position, direction, costume, size or volume of the Stage or a sprite. | No
Current () | The specified time unit selected. | Yes | Use C# DateTime.Now
Days Since 2000 | The number of days since 2000. | Yes | Use C# DateTime.Now - new DateTime(2000,1,1)
Username | The username of a user. | No

# Operators blocks

Block | Function | Available? | Comments
--- | --- | --- | ---
() &lt; () | The condition for checking if a value is less than the other. | Yes
() = () | The condition for checking if two values are equal. | Yes
() &gt; () | The condition for checking if a value is greater than the other. | Yes
() and () | Joins two conditions. | Yes 
() or () | Joins two conditions, but they function separately. | Yes
Not () | Makes the condition checked if it is false, not true. | Yes
() + () | The value of the addition. | Yes
() - () | The value of the subtraction. | Yes
() * () | The value of the multiplication. | Yes
() / () | The value of the division. | Yes
Pick Random () to () | Picks a random number between the two limits. | Yes | Use Random(from,to)
Join ()() | The two values put right next to each other. | Yes
Letter () of () | The specified character of the value. | Yes
Length of () | The length of the value. | Yes
() Mod () | The remainder of the division. | Yes
Round () | Rounds the value to the nearest whole number. | Yes | Use Math.Round
() of () | The absolute value (abs), square root (sqrt), sine (sin), cosine (cos), tangent (tan), asine (asin), acosine (acos), atangent (atan), natural logarithm (ln), logarithm (log), exponential function (e^), or base 10 exponential function (10^) of a specified value. | Yes | These functions are available in the 'Math' namespace, e.g. Math.Sin
