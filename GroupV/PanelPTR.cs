using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPCamera.GroupV
{
    class PanelPTR : Panel
    {
        public List<IntPtr> Handlers = new List<IntPtr>();
        public IntPtr[] HandlersArray
        {
            get => Handlers.ToArray();
        }
        public void AddHandlers(IntPtr ptr)
        {
            Handlers.Add(ptr, true);
        }
    }
}
