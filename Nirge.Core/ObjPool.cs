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

    public interface IObjCtor
    {
        void Init();
    }
    public interface IObjCtor<TArg1>
    {
        void Init(TArg1 arg1);
    }
    public interface IObjCtor<TArg1, TArg2>
    {
        void Init(TArg1 arg1, TArg2 arg2);
    }
    public interface IObjCtor<TArg1, TArg2, TArg3>
    {
        void Init(TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }
    public interface IObjCtor<TArg1, TArg2, TArg3, TArg4>
    {
        void Init(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
    }
    public interface IObjCtor<TArg1, TArg2, TArg3, TArg4, TArg5>
    {
        void Init(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);
    }
    public interface IObjCtor<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
    {
        void Init(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6);
    }
    public interface IObjCtor<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>
    {
        void Init(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7);
    }
    public interface IObjCtor<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>
    {
        void Init(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8);
    }
    public interface IObjCtor<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>
    {
        void Init(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9);
    }
    public interface IObjCtor<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>
    {
        void Init(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10);
    }
    public interface IObjDtor
    {
        void Destroy();
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

            public static T Fetch()
            {
                T obj;
                if (_objs.TryPop(out obj))
                    return obj;
                else
                    return (T)FormatterServices.GetUninitializedObject(typeof(T));
            }
            public static void Back(T obj)
            {
                Objs().Push(obj);
            }
        }

        #endregion

        #region

        static T AlwaysFetch<T>()
        {
            var obj = CAlloc<T>.Fetch();
            if (obj == null)
            {
                return (T)FormatterServices.GetUninitializedObject(typeof(T));
            }
            return obj;
        }
        public static T Fetch<T>(out T obj) where T : IObjCtor
        {
            obj = AlwaysFetch<T>();
            obj.Init();
            return obj;
        }
        public static T Fetch<T, TArg1>(out T obj, TArg1 arg1) where T : IObjCtor<TArg1>
        {
            obj = AlwaysFetch<T>();
            obj.Init(arg1);
            return obj;
        }
        public static T Fetch<T, TArg1, TArg2>(out T obj, TArg1 arg1, TArg2 arg2) where T : IObjCtor<TArg1, TArg2>
        {
            obj = AlwaysFetch<T>();
            obj.Init(arg1, arg2);
            return obj;
        }
        public static T Fetch<T, TArg1, TArg2, TArg3>(out T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3) where T : IObjCtor<TArg1, TArg2, TArg3>
        {
            obj = AlwaysFetch<T>();
            obj.Init(arg1, arg2, arg3);
            return obj;
        }
        public static T Fetch<T, TArg1, TArg2, TArg3, TArg4>(out T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) where T : IObjCtor<TArg1, TArg2, TArg3, TArg4>
        {
            obj = AlwaysFetch<T>();
            obj.Init(arg1, arg2, arg3, arg4);
            return obj;
        }
        public static T Fetch<T, TArg1, TArg2, TArg3, TArg4, TArg5>(out T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5) where T : IObjCtor<TArg1, TArg2, TArg3, TArg4, TArg5>
        {
            obj = AlwaysFetch<T>();
            obj.Init(arg1, arg2, arg3, arg4, arg5);
            return obj;
        }
        public static T Fetch<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(out T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6) where T : IObjCtor<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
        {
            obj = AlwaysFetch<T>();
            obj.Init(arg1, arg2, arg3, arg4, arg5, arg6);
            return obj;
        }
        public static T Fetch<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(out T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7) where T : IObjCtor<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>
        {
            obj = AlwaysFetch<T>();
            obj.Init(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return obj;
        }
        public static T Fetch<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(out T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8) where T : IObjCtor<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>
        {
            obj = AlwaysFetch<T>();
            obj.Init(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return obj;
        }
        public static T Fetch<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(out T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9) where T : IObjCtor<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>
        {
            obj = AlwaysFetch<T>();
            obj.Init(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return obj;
        }
        public static T Fetch<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(out T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9, TArg10 arg10) where T : IObjCtor<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>
        {
            obj = AlwaysFetch<T>();
            obj.Init(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            return obj;
        }
        public static void Back<T>(T obj) where T : IObjDtor
        {
            obj.Destroy();
            CAlloc<T>.Back(obj);
        }

        #endregion

        #region

        public static Stack<T> Fetch<T>(out Stack<T> obj)
        {
            obj = CAlloc<Stack<T>>.Fetch();
            if (obj == null)
            {
                obj = new Stack<T>();
            }
            return obj;
        }
        public static void Back<T>(Stack<T> obj)
        {
            if (obj != null)
            {
                obj.Clear();
                CAlloc<Stack<T>>.Back(obj);
            }
        }

        public static Queue<T> Fetch<T>(out Queue<T> obj)
        {
            obj = CAlloc<Queue<T>>.Fetch();
            if (obj == null)
            {
                obj = new Queue<T>();
            }
            return obj;
        }
        public static void Back<T>(Queue<T> obj)
        {
            if (obj != null)
            {
                obj.Clear();
                CAlloc<Queue<T>>.Back(obj);
            }
        }

        public static List<T> Fetch<T>(out List<T> obj)
        {
            obj = CAlloc<List<T>>.Fetch();
            if (obj == null)
            {
                obj = new List<T>();
            }
            return obj;
        }
        public static void Back<T>(List<T> obj)
        {
            if (obj != null)
            {
                obj.Clear();
                CAlloc<List<T>>.Back(obj);
            }
        }

        public static Dictionary<TKey, TValue> Fetch<TKey, TValue>(out Dictionary<TKey, TValue> obj)
        {
            obj = CAlloc<Dictionary<TKey, TValue>>.Fetch();
            if (obj == null)
            {
                obj = new Dictionary<TKey, TValue>();
            }
            return obj;
        }
        public static void Back<TKey, TValue>(Dictionary<TKey, TValue> obj)
        {
            if (obj != null)
            {
                obj.Clear();
                CAlloc<Dictionary<TKey, TValue>>.Back(obj);
            }
        }

        #endregion
    }

    #endregion
}
