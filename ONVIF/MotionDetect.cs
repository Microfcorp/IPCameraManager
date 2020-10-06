using IPCamera.OEVE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPCamera.ONVIF
{
    class MotionDetect
    {
        public static bool IsDetect(NotificationMessageHolderType[] mess)
        {
            foreach (var item in mess)
            {
                return bool.Parse(item.Message.ChildNodes[3].ChildNodes[1].Attributes[1].Value);
            }
            return false;
        }
    }
}
