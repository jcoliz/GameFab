# Roadmap

1. More book chapters. Implmenent chapter 5 and so on.
1. Consider Scene.StartScript() to replace Task.Run(). This would allow the scene to keep track of scripts, and (hopefully) cancel them all when done.
1. Better exception handling. Now, background scripts swallow exceptions and just stop running with no indication to the user.
1. Full screen mode
1. Pen blocks. Draw to an off-screen canvas, place that between background and sprites
1. Documentation for how to accomplish list blocks