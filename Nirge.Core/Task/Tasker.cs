/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Concurrent;
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
    #region

    public class CTaskerArgs
    {
        int _procs;
        int _taskCapacity;

        public int Procs
        {
            get
            {
                return _procs;
            }
        }

        public int TaskCapacity
        {
            get
            {
                return _taskCapacity;
            }
        }

        public CTaskerArgs(int procs = 0, int taskCapacity = 0)
        {
            _procs = procs;
            _taskCapacity = taskCapacity;

            if (_procs == 0)
                _procs = 1;
            else if (_procs < 1)
                _procs = 1;
            else if (_procs > Environment.ProcessorCount)
                _procs = Environment.ProcessorCount;
            _taskCapacity = 1024;
        }
    }

    #endregion

    public class CTasker
    {
        CTaskerArgs _args;
        ILog _log;
        List<Thread> _procs;
        ConcurrentQueue<ITask> _tasks;
        bool _quit;

        public int TasksCount
        {
            get
            {
                return _tasks.Count;
            }
        }

        CTasker(CTaskerArgs args, ILog log)
        {
            _args = args;
            _log = log;

            _procs = new List<Thread>(_args.Procs);
            for (int i = 0, len = _args.Procs; i < len; ++i)
            {
                var proc = new Thread(Exec);
                proc.IsBackground = true;
                _procs.Add(proc);
            }

            _tasks = new ConcurrentQueue<ITask>();
            _quit = false;
        }

        public CTasker(ILog log)
            :
            this(new CTaskerArgs(), log)
        {
        }

        public void Init()
        {
            foreach (var i in _procs)
                i.Start();
        }

        public void Destroy()
        {
            Close();
        }

        void Close()
        {
            _quit = true;

            foreach (var i in _procs)
                i.Join();

            _procs.Clear();
        }

        public void Clear()
        {
            _tasks.Clear();
        }

        public void Exec(IEnumerable<ITask> tasks)
        {
            if (_quit)
                return;

            foreach (var i in tasks)
                _tasks.Enqueue(i);
        }
        public void Exec(ITask task)
        {
            if (_quit)
                return;

            _tasks.Enqueue(task);
        }
        void Exec()
        {
            lock (_tasks)
            {
                while (true)
                {
                    if (_tasks.TryDequeue(out var task))
                    {
                        try
                        {
                            task.Exec();
                        }
                        catch (Exception exception)
                        {
                            _log.Error(string.Format("[Task]Exec exception, type:\"{0}\""
                                , task.GetType()), exception);
                        }
                    }
                    else
                    {
                        if (_tasks.Count == 0)
                        {
                            if (_quit)
                                break;
                            else
                                Thread.Sleep(1);
                        }
                    }
                }
            }
        }
    }
}
