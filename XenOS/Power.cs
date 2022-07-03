using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenOS
{
    internal class Power
    {
        public void shutdown()
        {
            //BootChime bootChime = new BootChime();
            //bootChime.PlayShutdownChime();
            Cosmos.System.Power.Shutdown();
        }

        public void reboot()
        {
            Cosmos.System.Power.Reboot();
        }
    }
}
