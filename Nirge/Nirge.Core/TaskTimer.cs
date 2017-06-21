/*------------------------------------------------------------------
    Copyright ? : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using System;

namespace Nirge.Core
{
    public class CTaskTimer
    {
        const int gIntervalMin = 10;

        struct CTimer
        {
            public ITask _task;
            public int _interval;
            public int _pass;
            public int _count;
        }

        CTasker _proc;
        ILog _log;
        Thread _tick;
        int _timerid;
        Dictionary<int, CTimer> _timers;
        List<int> _timersAfter;
        bool _quit;

        public CTaskTimer(CTasker proc, ILog log)
        {
            _proc = proc;
            _log = log;

            _tick = new Thread(() =>
            {
                while (!_quit)
                {
                    Thread.Sleep(gIntervalMin);
                    _proc.Exec(CCall.Create(Exec));
                }
            });
            _tick.IsBackground = true;

            _timerid = 0;
            _timers = new Dictionary<int, CTimer>(32);
            _timersAfter = new List<int>(32);

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

            Clear();
        }

        public int Reg(ITask task, int interval, int count = -1)
        {
            if (task == null)
                return -1;

            if (interval < gIntervalMin)
                interval = gIntervalMin;

            var timerid = ++_timerid;
            _timers.Add(timerid, new CTimer()
            {
                _task = task,
                _interval = interval,
                _pass = 0,
                _count = count,
            });

            return timerid;
        }

        public bool Unreg(int timer)
        {
            return _timers.Remove(timer);
        }

        public void Clear()
        {
            _timerid = 0;
            _timers.Clear();
            _timersAfter.Clear();
        }

        void Exec()
        {
            if (_timers.Count == 0)
                return;

            _timersAfter.AddRange(_timers.Keys);

            foreach (var i in _timersAfter)
            {
                CTimer timer;
                if (_timers.TryGetValue(i, out timer))
                    Exec(i, timer);
            }

            _timersAfter.Clear();
        }

        void Exec(int timerid, CTimer timer)
        {
            timer._pass += gIntervalMin;
            if (timer._pass < timer._interval)
                return;
            timer._pass = 0;

            if (timer._count > 0)
                --timer._count;

            try
            {
                timer._task.Exec();
            }
            catch (Exception exception)
            {
                _log.Error(string.Format("[Timer]Exec exception, timer:\"{0}\", task:\"{1}\"", timerid, timer._task.GetType()), exception);
            }

            if (timer._count == 0)
                _timers.Remove(timerid);
        }
    }
}
