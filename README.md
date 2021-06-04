# Draco Parkitect Mod Junk

Various bits of Parkitect modding exploration and not much in the way of art asssets because I am not an artist.

This repository is meant as a working example of various effects that anyone may apply to their own projects (refer to the [license](LICENSE)).

You will need the [Parkitect Asset Editor](https://github.com/Parkitect/ParkitectAssetEditor/) package.

## Note:

While this is a Unity project, the .csproj file is non-standard from what Unity likes to generate.

Unity will add several references that aren't needed if it ever regenerates the project file and the netstandar reference will not play nice with Parkitect.

In general the Unity module dll references are fine, I've just not included them until I need to. Thus the .csproj.bak file.