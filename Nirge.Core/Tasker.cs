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
    public struct CTaskerArgs
    {
        public int Procs
        {
            get;
            set;
        }

        public int TaskQueueSize
        {
            get;
            set;
        }

        public ILog Log
        {
            get;
            set;
        }
    }

    public class CTasker
    {
        CTaskerArgs _args;
        List<Thread> _procs;
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

        CTasker(CTaskerArgs args)
        {
            _args = args;

            _procs = new List<Thread>(_args.Procs);

            for (int i = 0, len = _procs.Count; i < len; ++i)
            {
                var proc = new Thread(Exec, 67108864);
                proc.IsBackground = true;

                _procs.Add(proc);
            }

            _tasks = new Queue<ITask>(_args.TaskQueueSize);
            _tasksCount = 0;
            _tasksAfter = new Queue<ITask>(_args.TaskQueueSize);
            _tasksAfterCount = 0;

            _quit = false;
        }
        public CTasker(ILog log)
            :
            this(new CTaskerArgs() { Procs = 1, TaskQueueSize = 1024, Log = log, })
        {
        }

        public void Init()
        {
            foreach (var proc in _procs)
                proc.Start();
        }

        public void Destroy()
        {
            Close();
        }

        void Close()
        {
            lock (_tasks)
            {
                _quit = true;
                Monitor.Pulse(_tasks);
            }

            foreach (var proc in _procs)
                proc.Join();

            _procs.Clear();
        }

        public void Clear()
        {
            lock (_tasks)
            {
                if (_quit)
                    return;

                _tasks.Clear();
                _tasksCount = 0;
            }
        }

        public void Exec(IEnumerable<ITask> tasks)
        {
            lock (_tasks)
            {
                if (_quit)
                    return;

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
                if (_quit)
                    return;

                _tasks.Enqueue(task);
                ++_tasksCount;
                Monitor.Pulse(_tasks);
            }
        }
        void Exec()
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
                                _args.Log.Error(string.Format("[Task]Exec exception, type:\"{0}\"", task.GetType()), exception);
                            }
                        }
                        while (--_tasksAfterCount > 0);

                        Monitor.Enter(_tasks);
                    }
                }
            }
        }
    }
}
