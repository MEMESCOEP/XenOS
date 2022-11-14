using IL2CPU.API.Attribs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenOS
{
    internal class Power
    {
        // Resource files
        [ManifestResourceStream(ResourceName = "XenOS.Audio.ShutdownSound.wav")]
        private readonly static byte[] ShutdownSound;

        public void shutdown()
        {
            //BootChime bootChime = new BootChime();
            //bootChime.PlayShutdownChime();
            if (Drivers.AudioEnabled)
            {
                AudioPlayer audioPlayer = new AudioPlayer();
                audioPlayer.PlayWAVFromBytes(ShutdownSound);
            }
            Cosmos.System.Power.Shutdown();
            Kernel.KernelPanic("Shutdown Failed!", "Unknown Exception");
        }

        public void reboot()
        {
            Cosmos.System.Power.Reboot();
            Kernel.KernelPanic("Reboot Failed!", "Unknown Exception");
        }
    }
}
