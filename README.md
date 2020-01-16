# Bionicle Area Editor

### SETUP

- Open LOMN/Import in Unity.
- Select the version (the system will import the files into that version folder).
- Add source path (the data folder, which has the levels and characters folders)
- Click Import (if data has BLK file, you have to have python 2 or 3 on your pc).

The results should look like this:

![](https://i.imgur.com/pbJGte4.png)

Then, in Scene.unity, click on the Manager object, and you can do things:

![](https://i.imgur.com/uOCWF95.png)

![](https://i.imgur.com/ZIsZ5Tq.png)

You can also change the shading mode to make it easier to work in dark areas or place realtime lights/shadows that match the baked lighting, or just to see how the area would look with the in-game night vision effect:

![](https://i.imgur.com/PMxJ438.gif)

### MISC TIPS
I'm assuming people using this may not have used Unity much or at all before. I do assume the user has general familiarity with the game itself (and its quirks) though.
- Unity creates .meta files for everything imported; this is how it keeps track of things. You won't see these in the Unity editor itself, just if you're browsing the folders in explorer or whatever. For the purposes of this project you probably don't really have to worry about them, as far as imported files from the game go. I wouldn't recommend messing with any assets/files for core editor functionality unless you know what you're doing.
- Press f to focus the editor's camera on whatever object you have selected.
- If you have multiple SLB contents loaded with the same name, and try to save an SLB by that name, it'll save whichever the first one it sees is.
- 3D coordinates systems are never consistent between anything, ever. In this case, x is mirrored between LoMN and Unity; for example 100 on x in-game would be -100 in Unity. You don't have to even think about this unless you're manually copying coordinates from the in-game debug menu or a hex editor to Unity or something like that.
- Rotations seem to be applied in a different order between LoMN and Unity, or something like that. I'm not entirely sure. In practice it doesn't matter much - it just means that objects rotated on multiple axes will visually appear at a different rotation in Unity than in-game. There's very few objects in the game that are rotated at all, and even fewer that are rotated on multiple axes, and I'm not sure there'd be a really elegant solution for it, so whatever. Details are in one of the txt files in the project folder.
- For pickup snapping, make sure the model you want to snap to has "Generate Colliders" enabled in its import settings. If you enable this option after loading the object SLB, you'll have to delete/reload it to get the colliders.
