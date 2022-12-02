# XenOS
XenOS is an operating system written in .NET C# and is made possible by the Cosmos OS project.
<br/>
## Features
* Simple File I/O (Currently supports vmware only)
* Fat32 Filesystem (Currently supports vmware only)
* Simple GUI
* Text Editor
* Autoexec (Auto start on boot)
* Networking
* FTP Server
* PS/2 Keyboard and mouse support
* AC97 Audio Driver
* Basic .NET executable support
* Basic login system
* Settings file

## Screenshots
<img src="https://github.com/MEMESCOEP/XenOS/raw/main/src/XenOS/Art/Screenshots/GUI.png" />
<img src="https://github.com/MEMESCOEP/XenOS/raw/main/src/XenOS/Art/Screenshots/Console.png" />

## System Requirements
* An x86 CPU
* 64 MB Minimum RAM, 128 MB or larger recommended
* At least 16 MB Graphics RAM
* PS/2 Keyboard
* (OPTIONAL) PS/2 Mouse, for GUI
* (OPTIONAL) 512 MB Hard disk, formatted with the FAT32 Filesystem
* (OPTIONAL) Intel E1000 Network card or PCnet-FAST III network card

## Building XenOS
Prerequisites:
* The XenOS source code (You can download it from https://github.com/MEMESCOEP/XenOS/archive/refs/heads/main.zip)
* VMWare Player
* Visual Studio 2022
* The latest version of the Cosmos OS Devkit (https://github.com/CosmosOS/Cosmos, take a look at the wiki for help on installing the devkit)

Starting the build:
1. Open the project in Visual Studio (Be sure to check if there are any errors!).
2. Press the `F5` key or click the button labeled "Cosmos" in the toolbar.
3. Once VMWare launches, you're done!

## TO-DO
* Add USB Support
* Add NTFS / EXT Filesystem support
* Fix bugs lol
* Add Sound Blaster 16 Audio Driver
* Add more features to .NET exec

**Please note that this Readme is still being edited.**
