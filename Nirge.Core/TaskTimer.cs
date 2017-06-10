using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;
using System;
using log4net;

namespace Nirge.Core
{
    public class CTaskTimer
    {
        struct CTimer
        {
            public ITask _task;
            public int _interval;
            public int _pass;
            public int _count;
        }

        const int gIntervalMin = 10;

        Timer _tick;
        CTasker _proc;
        ILog _log;
        int _timerid;
        Dictionary<int, CTimer> _timers;
        List<int> _timersAfter;

        public CTaskTimer(CTasker proc, ILog log)
        {
            _proc = proc;
            _log = log;
            _timers = new Dictionary<int, CTimer>(32);
            _timersAfter = new List<int>(32);
            Clear();
        }

        public void Init()
        {
            _tick = new Timer((e) =>
            {
                _proc.Exec(CCall.Create(Exec));
            }, this, 0, gIntervalMin);
        }

        public void Destroy()
        {
            Clear();
            _tick.Dispose();
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

        public bool Unreg(int tiemr)
        {
            return _timers.Remove(_timerid);
        }

        public void Clear()
        {
            _timerid = 0;
            _timers.Clear();
            _timersAfter.Clear();
        }

        void Exec()
        {
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
                _log.Error(string.Format("[Timer]Exec exception, timer:\"{0}\", task:\"{0}\"", timerid, timer._task.GetType()), exception);
            }

            if (timer._count == 0)
                _timers.Remove(timerid);
        }
    }
}
