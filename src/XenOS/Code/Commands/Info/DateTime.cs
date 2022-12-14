using System;

namespace XenOS.Code.Commands.Info
{
    internal class DateTime
    {
        public static void ShowDate()
        {
            Console.WriteLine(Cosmos.HAL.RTC.Month + "-" + Cosmos.HAL.RTC.DayOfTheMonth + "-" + Cosmos.HAL.RTC.Year);
        }

        public static void ShowTime()
        {
            Console.WriteLine(Cosmos.HAL.RTC.Hour + ":" + Cosmos.HAL.RTC.Minute + ":" + Cosmos.HAL.RTC.Second);
        }

        public static void ShowDateTime()
        {
            Console.WriteLine(Cosmos.HAL.RTC.Month + "-" + Cosmos.HAL.RTC.DayOfTheMonth + "-" + Cosmos.HAL.RTC.Year);
            Console.WriteLine(Cosmos.HAL.RTC.Hour + ":" + Cosmos.HAL.RTC.Minute + ":" + Cosmos.HAL.RTC.Second);
        }
    }
}
