# Bionicle-Area-Editor

### SETUP

- Copy the contents of \data\levels into the corresponding folder within the project's Resources folder (can copy the game's files as-is, then click and drag all of a level's BLKs onto the extractor script, then delete the BLKs... or whatever works for you)
- Same goes for \data\characters if you'd like
- To prep an area for loading, select all the x's and drag them onto LOMNTool to convert them to .dae

The results should look like this (the x's can then be deleted if you want to save space - they're the files that aren't the ones with the blue cube icons):

![](https://i.imgur.com/pbJGte4.png)

Then, in Scene.unity, click on the Manager object, and you can do things:

![](https://i.imgur.com/uOCWF95.png)

![](https://i.imgur.com/ZIsZ5Tq.png)

### MISC TIPS
I'm assuming people using this may not have used Unity much or at all before, so:
- Unity creates .meta files for everything imported; this is how it keeps track of things. You won't see these in the unity editor itself, just if you're browsing the folders in explorer or whatever. For the purposes of this project you probably don't really have to worry about them, as far as imported files from the game go. I wouldn't recommend messing with any assets/files for core editor functionality unless you know what you're doing.
- Press f to focus the editor's camera on whatever object you have selected.
- If you have multiple SLB contents loaded with the same name, and try to save an SLB by that name, it'll save whichever the first one it sees is.
- 3D coordinates systems are never consistent between anything, ever. In this case, x is mirrored between LoMN and Unity; for example 100 on x in-game would be -100 in Unity.
- Rotations seem to be applied in a different order between LoMN and Unity, or something like that. I'm not entirely sure. In practice it doesn't matter much - it just means that objects rotated on multiple axes will visually appear at a different rotation in Unity than in-game. There's very few objects in the game that are rotated at all, let alone on multiple axes, and I'm not sure there'd be a really elegant solution to it, so whatever. Details are in one of the txt files in the project folder.
