using System;

namespace XenOS
{
    internal class BootChime
    {
        // Functions
        public void PlayBootChime()
        {
            int freq = 500;
            for(int x = 0; x < 5; x++)
            {
                freq += 100;
#pragma warning disable CA1416 // Validate platform compatibility
                Console.Beep(freq, 50);
#pragma warning restore CA1416 // Validate platform compatibility
            }
        }
        public void PlayShutdownChime()
        {
            int freq = 1000;
            for (int x = 0; x < 10; x++)
            {
                freq -= 100;
#pragma warning disable CA1416 // Validate platform compatibility
                Console.Beep(freq, 50);
#pragma warning restore CA1416 // Validate platform compatibility
            }
        }
    }
}
