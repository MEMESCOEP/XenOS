using IL2CPU.API.Attribs;
using XenOS.Code.Audio;

namespace XenOS.Code.Sys.Power
{
    internal class Power
    {
        // Resource files
        [ManifestResourceStream(ResourceName = "XenOS.Audio.ShutdownSound.wav")]
        private readonly static byte[] ShutdownSound;

        public static void Shutdown()
        {
            //BootChime bootChime = new BootChime();
            //bootChime.PlayShutdownChime();
            if (Drivers.Drivers.AudioEnabled)
            {
                try
                {
                    AudioPlayer audioPlayer = new AudioPlayer();
                    audioPlayer.PlayWAVFromBytes(ShutdownSound);
                    while (!Drivers.Drivers.audioStream.Depleted) ;
                }
                catch
                {

                }
            }
            try
            {
                Drivers.Drivers.shutdown();
                Cosmos.System.Power.Shutdown();
            }
            catch
            {
                Kernel.KernelPanic("Shutdown Failed!", "Unknown Exception");
            }
        }

        public static void Reboot()
        {
            Drivers.Drivers.shutdown();
            Cosmos.System.Power.Reboot();
            Kernel.KernelPanic("Reboot Failed!", "Unknown Exception");
        }
    }
}
