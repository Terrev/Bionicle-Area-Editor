# Bionicle-Area-Editor

I'm going to bed soon, will add a proper readme later

FOR NOW, in short, you'll want to

- copy the contents of \data\levels into the project's Resources folder (can copy the game's files as-is, then click and drag all of a level's BLKs onto the extractor script, then delete the BLKs... or whatever works for you)
- to prep an area for loading, select all the x's and drag them onto lomntool to convert them to .dae (I mean, obj would work too but that wouldn't preserve vertex colors in a way unity would read)

the results should look like this (the x's can then be deleted if you want to save space - they're the files that aren't the ones with the blue cube icons)

![](https://i.imgur.com/eNu8eig.png)

then, in Scene.unity, click on the Manager object, and you can do things

![](https://i.imgur.com/2EopWEE.png)

![](https://i.imgur.com/ViePNfO.png)

![](https://i.imgur.com/dORLSuy.png)

SOME MISC TIPS

- unity creates .meta files which is how it keeps track of files. you won't see these in the unity editor itself, just if you're browsing the folders in explorer or whatever. for the purposes of this project you probably don't really have to worry about them, as far as imported files from the game go. messing with or deleting things for the core functionality will break things though.
- press f to focus the editor's camera on whatever object you have selected
- if you have multiple SLB contents loaded with the same name, and try to save an SLB by that name, it'll save whichever the first one it sees is. not 100% sure which one that would nessicarily be
- I'm going to bed, if you wanna know more yell at me on discord, I'll have a more fleshed out guide/maybe video and more functionality done later
