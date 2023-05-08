# Block Party

## Description

Block Party is a classic Breakout-style with a small twist: all the blocks have effects! Some blocks will explode, some will spawn more blocks, and some will even spawn powerups! The game is currently in development.

### How to Play

The game is currently in development, so there is no way to play it yet. However, you can download the source code and run it in Unity. For a stable version, fork the [`main`](https://github.com/DigitalNaut/BlockParty/tree/main) branch. For the latest version, fork the [`dev`](https://github.com/DigitalNaut/BlockParty/tree/dev) branch.

## For Developers

While I try to keep a clear and discrete commit history, I don't always succeed during the early phases of development when it's less impactful. So be warned that half of the game files may change from one commit to the next!

### How to Contribute

If you want to contribute to the project, fork the [`dev`](https://github.com/DigitalNaut/BlockParty/tree/dev) branch and make a pull request. I'll review it and merge it into the main branch if it's good. It helps if you provide a description of what you changed and why you changed it.

### Disabled Unity settings that need to be restored

- Baked global illumination was turned off because it was causing a render queue time of 6 hours or more for whatever reason. It needs to be turned back on.