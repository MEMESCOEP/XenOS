# XenOS
XenOS is an operating system written in .NET C# and is made possible by the Cosmos OS project.
<br/>
***Please note that this operating system is still in its alpha phase. It may break, crash, or cause data loss, so please don't use it on a computer you care about!***
<br/>
## Features
* Simple File I/O (Works best in VMware and only supports FAT32)
* Fat32 Filesystem (Works best in VMware)
* Simple GUI (Works best in VMware)
* Text Editor
* Autoexec (Auto start on boot)
* Basic networking (DHCP IP Assignment, ping, etc)
* FTP Server (Works best in VMware)
* PS/2 Keyboard and mouse support
* AC97 Audio Driver (Requires an AC97 compatible audio card, Works best in Virtualbox)
* Basic .NET executable support
* Basic login system
* Basic settings
* INI Configuration file parser

## Screenshots
<img src="https://github.com/MEMESCOEP/XenOS/raw/main/src/XenOS/Art/Screenshots/GUI.png" />
<img src="https://github.com/MEMESCOEP/XenOS/raw/main/src/XenOS/Art/Screenshots/Console.png" />

## System Requirements
* A 32-bit or 64-bit CPU (x86_64)
* At least 64 MB of RAM (128 MB or larger recommended)
* At least 16 MB graphics RAM
* PS/2 Keyboard (Or a USB keyboard with PS/2 emulation)
* (OPTIONAL) PS/2 Mouse, for GUI
* (OPTIONAL) 512 MB Hard disk, formatted with the FAT32 Filesystem
* (OPTIONAL) Intel E1000 Network card or PCnet-FAST III network card

## Building XenOS
Prerequisites:
* The XenOS source code (You can download it from https://github.com/MEMESCOEP/XenOS/archive/refs/heads/main.zip)
* VMWare Player
* Visual Studio 2022
* The latest version of the Cosmos OS Devkit (https://github.com/CosmosOS/Cosmos, take a look at the wiki for help with installation)

Starting the build:
<br/>
*NOTE: If you would like to compile outside of visual studio you may run `dotnet build`.*
1. Open the project in Visual Studio (Be sure to check if there are any errors!).
2. Press the `F5` key or click the button labeled "Cosmos" in the toolbar.
3. Once VMWare launches, you're done!

## TO-DO
* Add USB support
* Add NTFS and/or EXT Filesystem support
* Fix bugs lol
* Add Sound Blaster 16 audio driver
* Add more features to .NET exec
