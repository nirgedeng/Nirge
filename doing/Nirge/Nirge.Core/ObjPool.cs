/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System;

namespace Nirge.Core
{
    #region

    public interface IObjAlloc
    {
        void Alloc();
    }
    public interface IObjAlloc<TArg1>
    {
        void Alloc(TArg1 arg1);
    }
    public interface IObjAlloc<TArg1, TArg2>
    {
        void Alloc(TArg1 arg1, TArg2 arg2);
    }
    public interface IObjAlloc<TArg1, TArg2, TArg3>
    {
        void Alloc(TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }
    public interface IObjAlloc<TArg1, TArg2, TArg3, TArg4>
    {
        void Alloc(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
    }
    public interface IObjAlloc<TArg1, TArg2, TArg3, TArg4, TArg5>
    {
        void Alloc(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);
    }
    public interface IObjAlloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
    {
        void Alloc(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6);
    }
    public interface IObjAlloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>
    {
        void Alloc(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7);
    }
    public interface IObjAlloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>
    {
        void Alloc(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8);
    }
    public interface IObjAlloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>
    {
        void Alloc(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9);
    }
    public interface IObjAlloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>
    {
        void Alloc(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10);
    }
    public interface IObjCollect
    {
        void Collect();
    }

    #endregion

    #region 

    public static class CObjPool
    {
        #region

        static class CAlloc<T>
        {
            static ConcurrentStack<T> _objs;

            static ConcurrentStack<T> Objs()
            {
                return _objs
                       ??
                       (_objs = new ConcurrentStack<T>());
            }

            public static T Alloc()
            {
                T obj;
                if (_objs.TryPop(out obj))
                    return obj;
                else
                    return default(T);
            }
            public static void Collect(T obj)
            {
                Objs().Push(obj);
            }
        }

        #endregion

        #region

        static T AlwaysAlloc<T>()
        {
            var obj = CAlloc<T>.Alloc();
            if (obj == null)
            {
                return (T)FormatterServices.GetUninitializedObject(typeof(T));
            }
            return obj;
        }
        public static T Alloc<T>(out T obj) where T : IObjAlloc
        {
            obj = AlwaysAlloc<T>();
            obj.Alloc();
            return obj;
        }
        public static T Alloc<T, TArg1>(out T obj, TArg1 arg1) where T : IObjAlloc<TArg1>
        {
            obj = AlwaysAlloc<T>();
            obj.Alloc(arg1);
            return obj;
        }
        public static T Alloc<T, TArg1, TArg2>(out T obj, TArg1 arg1, TArg2 arg2) where T : IObjAlloc<TArg1, TArg2>
        {
            obj = AlwaysAlloc<T>();
            obj.Alloc(arg1, arg2);
            return obj;
        }
        public static T Alloc<T, TArg1, TArg2, TArg3>(out T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3) where T : IObjAlloc<TArg1, TArg2, TArg3>
        {
            obj = AlwaysAlloc<T>();
            obj.Alloc(arg1, arg2, arg3);
            return obj;
        }
        public static T Alloc<T, TArg1, TArg2, TArg3, TArg4>(out T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) where T : IObjAlloc<TArg1, TArg2, TArg3, TArg4>
        {
            obj = AlwaysAlloc<T>();
            obj.Alloc(arg1, arg2, arg3, arg4);
            return obj;
        }
        public static T Alloc<T, TArg1, TArg2, TArg3, TArg4, TArg5>(out T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5) where T : IObjAlloc<TArg1, TArg2, TArg3, TArg4, TArg5>
        {
            obj = AlwaysAlloc<T>();
            obj.Alloc(arg1, arg2, arg3, arg4, arg5);
            return obj;
        }
        public static T Alloc<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(out T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6) where T : IObjAlloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
        {
            obj = AlwaysAlloc<T>();
            obj.Alloc(arg1, arg2, arg3, arg4, arg5, arg6);
            return obj;
        }
        public static T Alloc<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(out T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7) where T : IObjAlloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>
        {
            obj = AlwaysAlloc<T>();
            obj.Alloc(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return obj;
        }
        public static T Alloc<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(out T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8) where T : IObjAlloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>
        {
            obj = AlwaysAlloc<T>();
            obj.Alloc(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return obj;
        }
        public static T Alloc<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(out T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9) where T : IObjAlloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>
        {
            obj = AlwaysAlloc<T>();
            obj.Alloc(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return obj;
        }
        public static T Alloc<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(out T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10) where T : IObjAlloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>
        {
            obj = AlwaysAlloc<T>();
            obj.Alloc(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            return obj;
        }
        public static void Collect<T>(T obj) where T : IObjCollect
        {
            obj.Collect();
            CAlloc<T>.Collect(obj);
        }

        #endregion

        #region

        public static Stack<T> Alloc<T>(out Stack<T> obj)
        {
            obj = CAlloc<Stack<T>>.Alloc();
            if (obj == null)
            {
                obj = new Stack<T>();
            }
            return obj;
        }
        public static void Collect<T>(Stack<T> obj)
        {
            if (obj != null)
            {
                obj.Clear();
                CAlloc<Stack<T>>.Collect(obj);
            }
        }

        public static Queue<T> Alloc<T>(out Queue<T> obj)
        {
            obj = CAlloc<Queue<T>>.Alloc();
            if (obj == null)
            {
                obj = new Queue<T>();
            }
            return obj;
        }
        public static void Collect<T>(Queue<T> obj)
        {
            if (obj != null)
            {
                obj.Clear();
                CAlloc<Queue<T>>.Collect(obj);
            }
        }

        public static List<T> Alloc<T>(out List<T> obj)
        {
            obj = CAlloc<List<T>>.Alloc();
            if (obj == null)
            {
                obj = new List<T>();
            }
            return obj;
        }
        public static void Collect<T>(List<T> obj)
        {
            if (obj != null)
            {
                obj.Clear();
                CAlloc<List<T>>.Collect(obj);
            }
        }

        public static Dictionary<TKey, TValue> Alloc<TKey, TValue>(out Dictionary<TKey, TValue> obj)
        {
            obj = CAlloc<Dictionary<TKey, TValue>>.Alloc();
            if (obj == null)
            {
                obj = new Dictionary<TKey, TValue>();
            }
            return obj;
        }
        public static void Collect<TKey, TValue>(Dictionary<TKey, TValue> obj)
        {
            if (obj != null)
            {
                obj.Clear();
                CAlloc<Dictionary<TKey, TValue>>.Collect(obj);
            }
        }

        #endregion
    }

    #endregion
}
