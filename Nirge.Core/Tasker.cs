/*------------------------------------------------------------------
    Copyright ? : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

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
    public class CTasker
    {
        List<Thread> _procs;
        ILog _log;
        Queue<ITask> _tasks;
        int _tasksCount;
        Queue<ITask> _tasksAfter;
        int _tasksAfterCount;
        bool _quit;

        public int TasksCount
        {
            get
            {
                return _tasksCount + _tasksAfterCount;
            }
        }

        CTasker(int procs, ILog log)
        {
            _procs = new List<Thread>(procs);
            for (var i = 0; i < procs; ++i)
            {
                var e = new Thread(Callback, 8388608);
                e.IsBackground = true;
                _procs.Add(e);
            }

            _log = log;

            _tasks = new Queue<ITask>(32);
            _tasksAfter = new Queue<ITask>(32);

            foreach (var proc in _procs)
                proc.Start();
        }
        public CTasker(ILog log)
        :
        this(1, log)
        {
        }

        public void Exec(IEnumerable<ITask> tasks)
        {
            lock (_tasks)
            {
                foreach (var task in tasks)
                {
                    _tasks.Enqueue(task);
                    ++_tasksCount;
                }
                Monitor.Pulse(_tasks);
            }
        }
        public void Exec(ITask task)
        {
            lock (_tasks)
            {
                _tasks.Enqueue(task);
                ++_tasksCount;
                Monitor.Pulse(_tasks);
            }
        }
        void Callback()
        {
            lock (_tasks)
            {
                while (true)
                {
                    if (_tasksCount == 0)
                    {
                        if (_quit)
                            break;
                        else
                            Monitor.Wait(_tasks);
                    }
                    else
                    {
                        while (_tasks.Count > 0)
                            _tasksAfter.Enqueue(_tasks.Dequeue());
                        _tasksAfterCount = _tasksCount;
                        _tasksCount = 0;
                        Monitor.Exit(_tasks);

                        do
                        {
                            var task = _tasksAfter.Dequeue();
                            try
                            {
                                task.Exec();
                            }
                            catch (Exception exception)
                            {
                                _log.Error(string.Format("[Task]Exec exception, type:\"{0}\"", task.GetType()), exception);
                            }
                        }
                        while (--_tasksAfterCount > 0);

                        Monitor.Enter(_tasks);
                    }
                }
            }
        }

        public void Clear()
        {
            lock (_tasks)
            {
                _tasks.Clear();
                _tasksCount = 0;
            }
        }

        public void Close()
        {
            lock (_tasks)
            {
                _quit = true;
                Monitor.Pulse(_tasks);
            }

            foreach (var proc in _procs)
                proc.Join();
        }
    }
}
