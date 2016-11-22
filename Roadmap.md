# Roadmap

1. Direction/Rotation. Current direction system is pretty half-baked. 
1. Layers. Implement Go To Front and Go Back () Layers
1. Shrink to fit on small screens. Readme says screen is shrunk when window is smaller than 1024x720, but this doesn't actually happen.
1. More book chapters. Implmenent chapter 5 and so on.
1. Consider Scene.StartScript() to replace Task.Run(). This would allow the scene to keep track of scripts, and (hopefully) cancel them all when done.
1. Better exception handling. Now, background scripts swallow exceptions and just stop running with no indication to the user.
