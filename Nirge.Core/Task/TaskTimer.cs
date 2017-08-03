/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
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

        class CTimer
        {
            public ITask _task;
            public int _interval;
            public int _pass;
            public int _count;
        }

        CTasker _proc;
        ILog _log;
        int _timerId;
        Dictionary<int, CTimer> _timers;
        List<int> _timersAfter;

        public CTaskTimer(CTasker proc, ILog log)
        {
            _proc = proc;
            _log = log;

            _timerId = 0;
            _timers = new Dictionary<int, CTimer>(32);
            _timersAfter = new List<int>(32);
        }

        public void Init()
        {
        }

        public void Destroy()
        {
            Clear();
        }

        public int Reg(ITask task, int interval, int count = -1)
        {
            if (task == null)
                return -1;

            if (interval < gIntervalMin)
                interval = gIntervalMin;

            var timerId = ++_timerId;
            _timers.Add(timerId, new CTimer()
            {
                _task = task,
                _interval = interval,
                _pass = 0,
                _count = count,
            });

            return timerId;
        }

        public bool Unreg(int timer)
        {
            return _timers.Remove(timer);
        }

        public void Clear()
        {
            _timerId = 0;
            _timers.Clear();
            _timersAfter.Clear();
        }

        public void Exec(int tick)
        {
            Trace.Assert(tick > 0 && tick <= gIntervalMin, string.Format("{0} > 0 && {0} <= gIntervalMin", tick));
            eExec(tick);
        }

        void eExec(int tick)
        {
            if (_timers.Count == 0)
                return;

            _timersAfter.AddRange(_timers.Keys);

            foreach (var i in _timersAfter)
            {
                CTimer timer;
                if (_timers.TryGetValue(i, out timer))
                    eExec(tick, i, timer);
            }

            _timersAfter.Clear();
        }

        void eExec(int tick, int timerId, CTimer timer)
        {
            timer._pass += tick;
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
                _log.Error(string.Format("[Timer]Exec exception, timer:\"{0}\", task:\"{1}\"", timerId, timer._task.GetType()), exception);
            }

            if (timer._count == 0)
                _timers.Remove(timerId);
        }
    }
}
