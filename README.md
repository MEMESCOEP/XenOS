# XenOS
XenOS is an operating system written in .NET C# and is made possible by the Cosmos OS project.
<br/>
## Features
* Simple File I/O
* Fat32 Filesystem
* Simple GUI
* Text Editor
* Autoexec (Auto start on boot)
* Networking
* FTP Server
* PS/2 Keyboard and mouse support
* AC97 Audio Driver
* Basic .NET executable support

## Screenshots
<img src="https://github.com/MEMESCOEP/XenOS/raw/main/XenOS/Art/Screenshots/GUI.png" />
<img src="https://github.com/MEMESCOEP/XenOS/raw/main/XenOS/Art/Screenshots/Console.png" />

## System Requirements
* An x86 CPU
* 48 MB Minimum RAM, 64 MB or larger recommended
* At least 16 MB Graphics RAM
* PS/2 Keyboard
* (OPTIONAL) PS/2 Mouse, for GUI
* (OPTIONAL) 512 MB Hard disk, formatted with the FAT32 Filesystem
* (OPTIONAL) Intel E1000 Network card or PCnet-FAST III network card

## Change log:
* Added AC97 Audio driver
* Added simple window manager
* Added append, write, and ping commands
* Added ability to change username
* Added time command
* Added about command
* Added .NET executable support

## TO-DO
* Add USB Support
* Add NTFS / EXT Filesystem support
* Fix bugs lol
* Add a notepad to the GUI
* Add Sound Blaster 16 Audio Driver

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

## Commands and usage
1. shutdown - turns of the computer
2. reboot - reboots the computer
3. ls - shows a list of files and directories in the current working directory
4. cd {directory name} - changes the directory
5. mkdir {directory name} - creates a directory
6. rmdir {directory name} - deletes a directory
7. mkf {file name} - creates a file
8. rm {file name} - deletes a file
9. cat {file name} - displays the contents of a file
10. mv {file name} {directory name} - moves a file into the specified directory
11. append {file name} {text} - appends text to a file
12. write {file name} {text} - replaces a file's content with the specified text
13. cls - clear the console
14. sysinfo - display system information
15. exec {file name} - execute a file that contains one or more commands
16. panic - force a kernel panic
17. echo {text} - display text in the console
18. time - displays the current time
19. about - display OS information
20. audio {file name} - plays a wav audio file
21. testaudio - tests the audio
22. uname {username} - changes the current username
23. beep {frequency} - makes the PC Speaker beep at the specified frequency (37 Hz to 32767 Hz)
24. ipaddr - show the current IP address
25. urltoip {url} - converts an HTTP url to an IP address
26. ftpserver - starts the ftp server
27. ping {IP address} - ping an IP address
28. gui - starts the gui
29. modes - displays all supported resolutions
30. app - executes a .NET application
  
Please note that this Readme is still being edited.
