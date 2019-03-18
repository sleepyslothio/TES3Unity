# TESUnity

World viewers for Elder Scrolls games in the Unity game engine with VR support supporting Oculus, OSVR and OpenVR. For advanced VR support, please read the README.md located into the `Vendors` folder.

## Getting Started

**TESUnity requires a valid installation of Morrowind to run!**, you can get it on Steam or Gog.com.

To get started, go to the release tab and download the latest release for your device.
Alternatively you can download the source code as a ZIP file, extract it, and open the TESUnity folder in Unity.

Take a look at the `README-Config.md` file to tweak parameters. If you want to enjoy VR, take a look at `README-VR.md`. Finally input mapping is located at `README-Input.md`.

### Desktop
The game will ask you where is the **Data Files** folder the first time you start the game. If you move your Morrowind installation, the game will ask you again where is the **Data Files** folder.
  
### Mobile
You've to copy the content of the **Data Files** folder into a folder named `TESUnityXR` on your SDCard.
For now, the path is hardcoded, if the game can't open the game, please open an issue.

## Contribute

Bugs and feature requests are listed on the [GitHub issues page](https://github.com/ColeDeanShepherd/TESUnity/issues). Feel free to fork the source code and contribute, or use it in any way that falls under the [MIT License](https://github.com/ColeDeanShepherd/TESUnity/blob/master/LICENSE.txt).

Please create a branch from develop for each "feature" (see [this article](http://nvie.com/posts/a-successful-git-branching-model/)).

Morrowind Data Format Resources
-------------------------------

* [ESM File Format Specification](http://www.mwmythicmods.com/argent/tech/es_format.html)
* [BSA File Format Specification](http://www.uesp.net/wiki/Tes3Mod:BSA_File_Format)
* [NIF File Format Specification](https://github.com/niftools/nifxml/blob/develop/nif.xml)
* [NIF Viewer/Data Inspector](https://github.com/niftools/nifskope)