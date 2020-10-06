using IPCamera.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace IPCamera.PTZScenes
{
    class PTZRunning
    {
        public class cm
        {
            public PTZScene sc; public Structures camera;

            public cm(PTZScene sc, Structures camera)
            {
                this.sc = sc;
                this.camera = camera;
            }
        }
        public static void Run(PTZScene sc, Structures camera)
        {
            Thread th = new Thread(new ParameterizedThreadStart(_run));
            th.Start(new cm(sc, camera));
        }

        private static void _run(object o)
        {
            var sc = o as cm;
            Thread.Sleep(sc.sc.Timeout);
            foreach (var item in sc.sc.Tiles)
            {
                sc.camera.GetPTZController().CountiniousMove(item.Vector, item.Steep * (int)Settings.StaticMembers.PTZSettings.StepTimeout);
                Thread.Sleep(item.SleepTime);
            }
        }
    }
}
