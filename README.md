# TES3Unity

TES3Unity is an attempt to recreate the Morrowind Engine into the Unity3D game engine.

## Status
| Feature | Status |
|---------|--------|
| World Loading | OK |
| NPC Loading | In progress |
| Creature Loading | In Progress |
| Script support | Not started |
| Mod support | Not started |
| Animation support | Need Help |
| UI recreation | In progress |
| Interactions | Started |
| Player Management | Started |
| VR support | OK |


## Getting Started

**TES3Unity requires a valid installation of Morrowind to run!**, you can get it on Steam or Gog.com.

To get started, go to the release tab and download the latest release for your device.
Alternatively you can download the source code as a ZIP file, extract it, and open the TES3Unity folder in Unity.

Take a look at the `README-Config.md` file to tweak parameters. If you want to enjoy VR, take a look at `README-VR.md`. Finally input mapping is located at `README-Input.md`.

### Desktop
The game will ask you where is the **Data Files** folder the first time you start the game. If you move your Morrowind installation, the game will ask you again where is the **Data Files** folder.
  
### Mobile
You've to copy the **Data Files** folder into a folder named `TES3Unity` on your SDCard. If the game can't open the game, please open an issue.

### Supported Platforms
|Platform | Status | Graphics API |
|---------|--------|--------------|
| Windows | Supported | Direct3D 11 |
| Linux | Experimental | Vulkan |
| Macos | Experimental | Metal |
| Android (Flat) | Experimental | Vulkan & OpenGL ES3.1 |
| Oculus Quest | Supported | Vulkan & OpenGL ES3.1 |


## Contribute
Bugs and feature requests are listed on the [issues page](https://github.com/demonixis/TES3Unity/issues). 
Please take a look at the wiki for more informations about the Morrowind file formats. Don't hesitate to contact me if you want to help but if you don't know where to start. We've have a lot of tasks, some very easy, other hard.

### Thanks
Special thanks to [Cole Dean Shepherd](https://github.com/ColeDeanShepherd) which have started [this work](https://github.com/ColeDeanShepherd/TESUnity).

## License
This project is released under the [MIT License](https://github.com/demonixis/TES3Unity/blob/master/LICENSE.txt)