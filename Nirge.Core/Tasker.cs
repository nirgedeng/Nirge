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
        Queue<ITask> _eTasks;
        int _eTasksCount;
        bool _quit;

        public int TasksCount
        {
            get
            {
                return _tasksCount + _eTasksCount;
            }
        }

        CTasker(int procs, ILog log)
        {
            _procs = new List<Thread>(procs);
            for (var i = 0; i < procs; ++i)
            {
                var e = new Thread(Callback);
                e.IsBackground = true;
                _procs.Add(e);
            }

            _log = log;

            _tasks = new Queue<ITask>(32);
            _eTasks = new Queue<ITask>(32);

            foreach (var i in _procs)
                i.Start();
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
                        for (int i = 0, len = _tasksCount; i < len; ++i)
                            _eTasks.Enqueue(_tasks.Dequeue());
                        _eTasksCount = _tasksCount;
                        _tasksCount = 0;
                        Monitor.Exit(_tasks);

                        do
                        {
                            var task = _eTasks.Dequeue();
                            try
                            {
                                task.Exec();
                            }
                            catch (Exception exception)
                            {
                                _log.Error("[Task] Exec Exception!", exception);
                            }
                        }
                        while (--_eTasksCount > 0);

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
