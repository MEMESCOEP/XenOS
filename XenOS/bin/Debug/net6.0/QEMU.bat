@echo off
echo Starting QEMU...
"C:\Program Files\qemu\qemu-system-x86_64.exe" -cdrom .\XenOS.iso -device VGA,vgamem_mb=64 -device ac97 -m 64