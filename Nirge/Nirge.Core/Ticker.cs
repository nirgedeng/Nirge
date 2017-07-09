/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Reflection;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using System;

namespace Nirge.Core
{
    public class CTicker
    {
        Thread _tick;
        int _interval;
        bool _quit;

        public event Action<object, int> Ticked;

        public CTicker(int interval = 0)
        {
            _interval = interval;
            if (_interval < 10)
                _interval = 10;

            _tick = new Thread(() =>
            {
                while (!_quit)
                {
                    Thread.Sleep(_interval);
                    if (Ticked != null)
                        Ticked(this, _interval);
                }
            });
            _tick.IsBackground = true;

            _quit = false;
        }

        public void Init()
        {
            _tick.Start();
        }

        public void Destroy()
        {
            _quit = true;
            _tick.Join();
        }
    }
}
