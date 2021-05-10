using System;
using System.Collections.Generic;
using System.Text;

namespace Frotcom.Challenge.SendTrackingDataWorker.Helpers
{
    public class DateTimeHelper
    {
        public static string GetDay()
        {
            return DateTime.Now.Date.ToString().Split(' ')[0];
        }

        public static string GetTime()
        {
            return DateTime.Now.TimeOfDay.ToString().Split('.')[0];
        }
    }
}
