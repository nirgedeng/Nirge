/*------------------------------------------------------------------
    Copyright ? : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System;

namespace Nirge.Core
{
    public class CCall : ITask, IObjCtor<Action>, IObjDtor, IEquatable<Action>, IEquatable<CCall>
    {
        #region

        public static CCall Create(Action callback)
        {
            return new CCall(callback);
        }
        public static CCall<TArg1> Create<TArg1>(Action<TArg1> callback, TArg1 arg1)
        {
            return new CCall<TArg1>(callback, arg1);
        }
        public static CCall<TArg1, TArg2> Create<TArg1, TArg2>(Action<TArg1, TArg2> callback, TArg1 arg1, TArg2 arg2)
        {
            return new CCall<TArg1, TArg2>(callback, arg1, arg2);
        }
        public static CCall<TArg1, TArg2, TArg3> Create<TArg1, TArg2, TArg3>(Action<TArg1, TArg2, TArg3> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return new CCall<TArg1, TArg2, TArg3>(callback, arg1, arg2, arg3);
        }
        public static CCall<TArg1, TArg2, TArg3, TArg4> Create<TArg1, TArg2, TArg3, TArg4>(Action<TArg1, TArg2, TArg3, TArg4> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            return new CCall<TArg1, TArg2, TArg3, TArg4>(callback, arg1, arg2, arg3, arg4);
        }
        public static CCall<TArg1, TArg2, TArg3, TArg4, TArg5> Create<TArg1, TArg2, TArg3, TArg4, TArg5>(Action<TArg1, TArg2, TArg3, TArg4, TArg5> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            return new CCall<TArg1, TArg2, TArg3, TArg4, TArg5>(callback, arg1, arg2, arg3, arg4, arg5);
        }
        public static CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
        {
            return new CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(callback, arg1, arg2, arg3, arg4, arg5, arg6);
        }
        public static CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7)
        {
            return new CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(callback, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
        public static CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8)
        {
            return new CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(callback, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }
        public static CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9)
        {
            return new CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(callback, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        #endregion

        Action _callback;

        private CCall(Action callback)
        {
            Init(callback);
        }

        public void Init(Action callback)
        {
            _callback = callback;
        }
        public void Destroy()
        {
            _callback = null;
        }

        public void Exec()
        {
            _callback();
        }

        public static bool operator ==(CCall x, CCall y)
        {
            return object.ReferenceEquals(x, y) || ((object)x != null && (object)y != null && x.Equals(y));
        }
        public static bool operator !=(CCall x, CCall y)
        {
            return !(x == y);
        }
        public bool Equals(Action other)
        {
            return (object)other != null &&
                   other == _callback;
        }
        public bool Equals(CCall other)
        {
            return (object)other != null &&
                   Equals(other._callback);
        }
        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj.GetType() == GetType() &&
                   Equals(obj as CCall);
        }

        public override int GetHashCode()
        {
            return _callback.GetHashCode();
        }
    }
    public class CCall<TArg1> : ITask, IObjCtor<Action<TArg1>, TArg1>, IObjDtor, IEquatable<Action<TArg1>>, IEquatable<CCall<TArg1>>
    {
        Action<TArg1> _callback;

        public TArg1 Arg1
        {
            get;
            private set;
        }

        internal CCall(Action<TArg1> callback, TArg1 arg1)
        {
            Init(callback, arg1);
        }

        public void Init(Action<TArg1> callback, TArg1 arg1)
        {
            _callback = callback;
            Arg1 = arg1;
        }
        public void Destroy()
        {
            _callback = null;
            Arg1 = default(TArg1);
        }

        public void Exec()
        {
            _callback(Arg1);
        }

        public static bool operator ==(CCall<TArg1> x, CCall<TArg1> y)
        {
            return object.ReferenceEquals(x, y) || ((object)x != null && (object)y != null && x.Equals(y));
        }
        public static bool operator !=(CCall<TArg1> x, CCall<TArg1> y)
        {
            return !(x == y);
        }
        public bool Equals(Action<TArg1> other)
        {
            return (object)other != null &&
                   other == _callback;
        }
        public bool Equals(CCall<TArg1> other)
        {
            return (object)other != null &&
                   Equals(other._callback);
        }
        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj.GetType() == GetType() &&
                   Equals(obj as CCall<TArg1>);
        }

        public override int GetHashCode()
        {
            return _callback.GetHashCode();
        }
    }
    public class CCall<TArg1, TArg2> : ITask, IObjCtor<Action<TArg1, TArg2>, TArg1, TArg2>, IObjDtor, IEquatable<Action<TArg1, TArg2>>, IEquatable<CCall<TArg1, TArg2>>
    {
        Action<TArg1, TArg2> _callback;

        public TArg1 Arg1
        {
            get;
            private set;
        }
        public TArg2 Arg2
        {
            get;
            private set;
        }

        internal CCall(Action<TArg1, TArg2> callback, TArg1 arg1, TArg2 arg2)
        {
            Init(callback, arg1, arg2);
        }

        public void Init(Action<TArg1, TArg2> callback, TArg1 arg1, TArg2 arg2)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
        }
        public void Destroy()
        {
            _callback = null;
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
        }

        public void Exec()
        {
            _callback(Arg1, Arg2);
        }

        public static bool operator ==(CCall<TArg1, TArg2> x, CCall<TArg1, TArg2> y)
        {
            return object.ReferenceEquals(x, y) || ((object)x != null && (object)y != null && x.Equals(y));
        }
        public static bool operator !=(CCall<TArg1, TArg2> x, CCall<TArg1, TArg2> y)
        {
            return !(x == y);
        }
        public bool Equals(Action<TArg1, TArg2> other)
        {
            return (object)other != null &&
                   other == _callback;
        }
        public bool Equals(CCall<TArg1, TArg2> other)
        {
            return (object)other != null &&
                   Equals(other._callback);
        }
        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj.GetType() == GetType() &&
                   Equals(obj as CCall<TArg1, TArg2>);
        }

        public override int GetHashCode()
        {
            return _callback.GetHashCode();
        }
    }

    public class CCall<TArg1, TArg2, TArg3> : ITask, IObjCtor<Action<TArg1, TArg2, TArg3>, TArg1, TArg2, TArg3>, IObjDtor, IEquatable<Action<TArg1, TArg2, TArg3>>, IEquatable<CCall<TArg1, TArg2, TArg3>>
    {
        Action<TArg1, TArg2, TArg3> _callback;

        public TArg1 Arg1
        {
            get;
            private set;
        }
        public TArg2 Arg2
        {
            get;
            private set;
        }
        public TArg3 Arg3
        {
            get;
            private set;
        }

        internal CCall(Action<TArg1, TArg2, TArg3> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            Init(callback, arg1, arg2, arg3);
        }

        public void Init(Action<TArg1, TArg2, TArg3> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }
        public void Destroy()
        {
            _callback = null;
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
        }

        public void Exec()
        {
            _callback(Arg1, Arg2, Arg3);
        }

        public static bool operator ==(CCall<TArg1, TArg2, TArg3> x, CCall<TArg1, TArg2, TArg3> y)
        {
            return object.ReferenceEquals(x, y) || ((object)x != null && (object)y != null && x.Equals(y));
        }
        public static bool operator !=(CCall<TArg1, TArg2, TArg3> x, CCall<TArg1, TArg2, TArg3> y)
        {
            return !(x == y);
        }
        public bool Equals(Action<TArg1, TArg2, TArg3> other)
        {
            return (object)other != null &&
                   other == _callback;
        }
        public bool Equals(CCall<TArg1, TArg2, TArg3> other)
        {
            return (object)other != null &&
                   Equals(other._callback);
        }
        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj.GetType() == GetType() &&
                   Equals(obj as CCall<TArg1, TArg2, TArg3>);
        }

        public override int GetHashCode()
        {
            return _callback.GetHashCode();
        }
    }

    public class CCall<TArg1, TArg2, TArg3, TArg4> : ITask, IObjCtor<Action<TArg1, TArg2, TArg3, TArg4>, TArg1, TArg2, TArg3, TArg4>, IObjDtor, IEquatable<Action<TArg1, TArg2, TArg3, TArg4>>, IEquatable<CCall<TArg1, TArg2, TArg3, TArg4>>
    {
        Action<TArg1, TArg2, TArg3, TArg4> _callback;

        public TArg1 Arg1
        {
            get;
            private set;
        }
        public TArg2 Arg2
        {
            get;
            private set;
        }
        public TArg3 Arg3
        {
            get;
            private set;
        }
        public TArg4 Arg4
        {
            get;
            private set;
        }

        internal CCall(Action<TArg1, TArg2, TArg3, TArg4> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            Init(callback, arg1, arg2, arg3, arg4);
        }

        public void Init(Action<TArg1, TArg2, TArg3, TArg4> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }
        public void Destroy()
        {
            _callback = null;
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
            Arg4 = default(TArg4);
        }

        public void Exec()
        {
            _callback(Arg1, Arg2, Arg3, Arg4);
        }

        public static bool operator ==(CCall<TArg1, TArg2, TArg3, TArg4> x, CCall<TArg1, TArg2, TArg3, TArg4> y)
        {
            return object.ReferenceEquals(x, y) || ((object)x != null && (object)y != null && x.Equals(y));
        }
        public static bool operator !=(CCall<TArg1, TArg2, TArg3, TArg4> x, CCall<TArg1, TArg2, TArg3, TArg4> y)
        {
            return !(x == y);
        }
        public bool Equals(Action<TArg1, TArg2, TArg3, TArg4> other)
        {
            return (object)other != null &&
                   other == _callback;
        }
        public bool Equals(CCall<TArg1, TArg2, TArg3, TArg4> other)
        {
            return (object)other != null &&
                   Equals(other._callback);
        }
        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj.GetType() == GetType() &&
                   Equals(obj as CCall<TArg1, TArg2, TArg3, TArg4>);
        }

        public override int GetHashCode()
        {
            return _callback.GetHashCode();
        }
    }

    public class CCall<TArg1, TArg2, TArg3, TArg4, TArg5> : ITask, IObjCtor<Action<TArg1, TArg2, TArg3, TArg4, TArg5>, TArg1, TArg2, TArg3, TArg4, TArg5>, IObjDtor, IEquatable<Action<TArg1, TArg2, TArg3, TArg4, TArg5>>, IEquatable<CCall<TArg1, TArg2, TArg3, TArg4, TArg5>>
    {
        Action<TArg1, TArg2, TArg3, TArg4, TArg5> _callback;

        public TArg1 Arg1
        {
            get;
            private set;
        }
        public TArg2 Arg2
        {
            get;
            private set;
        }
        public TArg3 Arg3
        {
            get;
            private set;
        }
        public TArg4 Arg4
        {
            get;
            private set;
        }
        public TArg5 Arg5
        {
            get;
            private set;
        }

        internal CCall(Action<TArg1, TArg2, TArg3, TArg4, TArg5> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            Init(callback, arg1, arg2, arg3, arg4, arg5);
        }

        public void Init(Action<TArg1, TArg2, TArg3, TArg4, TArg5> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
        }
        public void Destroy()
        {
            _callback = null;
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
            Arg4 = default(TArg4);
            Arg5 = default(TArg5);
        }

        public void Exec()
        {
            _callback(Arg1, Arg2, Arg3, Arg4, Arg5);
        }

        public static bool operator ==(CCall<TArg1, TArg2, TArg3, TArg4, TArg5> x, CCall<TArg1, TArg2, TArg3, TArg4, TArg5> y)
        {
            return object.ReferenceEquals(x, y) || ((object)x != null && (object)y != null && x.Equals(y));
        }
        public static bool operator !=(CCall<TArg1, TArg2, TArg3, TArg4, TArg5> x, CCall<TArg1, TArg2, TArg3, TArg4, TArg5> y)
        {
            return !(x == y);
        }
        public bool Equals(Action<TArg1, TArg2, TArg3, TArg4, TArg5> other)
        {
            return (object)other != null &&
                   other == _callback;
        }
        public bool Equals(CCall<TArg1, TArg2, TArg3, TArg4, TArg5> other)
        {
            return (object)other != null &&
                   Equals(other._callback);
        }
        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj.GetType() == GetType() &&
                   Equals(obj as CCall<TArg1, TArg2, TArg3, TArg4, TArg5>);
        }

        public override int GetHashCode()
        {
            return _callback.GetHashCode();
        }
    }

    public class CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> : ITask, IObjCtor<Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>, IObjDtor, IEquatable<Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>>, IEquatable<CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>>
    {
        Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> _callback;

        public TArg1 Arg1
        {
            get;
            private set;
        }
        public TArg2 Arg2
        {
            get;
            private set;
        }
        public TArg3 Arg3
        {
            get;
            private set;
        }
        public TArg4 Arg4
        {
            get;
            private set;
        }
        public TArg5 Arg5
        {
            get;
            private set;
        }
        public TArg6 Arg6
        {
            get;
            private set;
        }

        internal CCall(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
        {
            Init(callback, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public void Init(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
        }
        public void Destroy()
        {
            _callback = null;
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
            Arg4 = default(TArg4);
            Arg5 = default(TArg5);
            Arg6 = default(TArg6);
        }

        public void Exec()
        {
            _callback(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6);
        }

        public static bool operator ==(CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> x, CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> y)
        {
            return object.ReferenceEquals(x, y) || ((object)x != null && (object)y != null && x.Equals(y));
        }
        public static bool operator !=(CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> x, CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> y)
        {
            return !(x == y);
        }
        public bool Equals(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> other)
        {
            return (object)other != null &&
                   other == _callback;
        }
        public bool Equals(CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> other)
        {
            return (object)other != null &&
                   Equals(other._callback);
        }
        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj.GetType() == GetType() &&
                   Equals(obj as CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>);
        }

        public override int GetHashCode()
        {
            return _callback.GetHashCode();
        }
    }

    public class CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> : ITask, IObjCtor<Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>, IObjDtor, IEquatable<Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>>, IEquatable<CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>>
    {
        Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> _callback;

        public TArg1 Arg1
        {
            get;
            private set;
        }
        public TArg2 Arg2
        {
            get;
            private set;
        }
        public TArg3 Arg3
        {
            get;
            private set;
        }
        public TArg4 Arg4
        {
            get;
            private set;
        }
        public TArg5 Arg5
        {
            get;
            private set;
        }
        public TArg6 Arg6
        {
            get;
            private set;
        }
        public TArg7 Arg7
        {
            get;
            private set;
        }

        internal CCall(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7)
        {
            Init(callback, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public void Init(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
        }
        public void Destroy()
        {
            _callback = null;
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
            Arg4 = default(TArg4);
            Arg5 = default(TArg5);
            Arg6 = default(TArg6);
            Arg7 = default(TArg7);
        }

        public void Exec()
        {
            _callback(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7);
        }

        public static bool operator ==(CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> x, CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> y)
        {
            return object.ReferenceEquals(x, y) || ((object)x != null && (object)y != null && x.Equals(y));
        }
        public static bool operator !=(CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> x, CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> y)
        {
            return !(x == y);
        }
        public bool Equals(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> other)
        {
            return (object)other != null &&
                   other == _callback;
        }
        public bool Equals(CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> other)
        {
            return (object)other != null &&
                   Equals(other._callback);
        }
        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj.GetType() == GetType() &&
                   Equals(obj as CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>);
        }

        public override int GetHashCode()
        {
            return _callback.GetHashCode();
        }
    }

    public class CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> : ITask, IObjCtor<Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>, IObjDtor, IEquatable<Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>>, IEquatable<CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>>
    {
        Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> _callback;

        public TArg1 Arg1
        {
            get;
            private set;
        }
        public TArg2 Arg2
        {
            get;
            private set;
        }
        public TArg3 Arg3
        {
            get;
            private set;
        }
        public TArg4 Arg4
        {
            get;
            private set;
        }
        public TArg5 Arg5
        {
            get;
            private set;
        }
        public TArg6 Arg6
        {
            get;
            private set;
        }
        public TArg7 Arg7
        {
            get;
            private set;
        }
        public TArg8 Arg8
        {
            get;
            private set;
        }

        internal CCall(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8)
        {
            Init(callback, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public void Init(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
        }
        public void Destroy()
        {
            _callback = null;
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
            Arg4 = default(TArg4);
            Arg5 = default(TArg5);
            Arg6 = default(TArg6);
            Arg7 = default(TArg7);
            Arg8 = default(TArg8);
        }

        public void Exec()
        {
            _callback(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8);
        }

        public static bool operator ==(CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> x, CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> y)
        {
            return object.ReferenceEquals(x, y) || ((object)x != null && (object)y != null && x.Equals(y));
        }
        public static bool operator !=(CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> x, CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> y)
        {
            return !(x == y);
        }
        public bool Equals(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> other)
        {
            return (object)other != null &&
                   other == _callback;
        }
        public bool Equals(CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> other)
        {
            return (object)other != null &&
                   Equals(other._callback);
        }
        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj.GetType() == GetType() &&
                   Equals(obj as CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>);
        }

        public override int GetHashCode()
        {
            return _callback.GetHashCode();
        }
    }

    public class CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> : ITask, IObjCtor<Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>, IObjDtor, IEquatable<Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>>, IEquatable<CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>>
    {
        Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> _callback;

        public TArg1 Arg1
        {
            get;
            private set;
        }
        public TArg2 Arg2
        {
            get;
            private set;
        }
        public TArg3 Arg3
        {
            get;
            private set;
        }
        public TArg4 Arg4
        {
            get;
            private set;
        }
        public TArg5 Arg5
        {
            get;
            private set;
        }
        public TArg6 Arg6
        {
            get;
            private set;
        }
        public TArg7 Arg7
        {
            get;
            private set;
        }
        public TArg8 Arg8
        {
            get;
            private set;
        }
        public TArg9 Arg9
        {
            get;
            private set;
        }

        internal CCall(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9)
        {
            Init(callback, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public void Init(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> callback, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9)
        {
            _callback = callback;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
            Arg9 = arg9;
        }
        public void Destroy()
        {
            _callback = null;
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
            Arg4 = default(TArg4);
            Arg5 = default(TArg5);
            Arg6 = default(TArg6);
            Arg7 = default(TArg7);
            Arg8 = default(TArg8);
            Arg9 = default(TArg9);
        }

        public void Exec()
        {
            _callback(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7, Arg8, Arg9);
        }

        public static bool operator ==(CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> x, CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> y)
        {
            return object.ReferenceEquals(x, y) || ((object)x != null && (object)y != null && x.Equals(y));
        }
        public static bool operator !=(CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> x, CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> y)
        {
            return !(x == y);
        }
        public bool Equals(Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> other)
        {
            return (object)other != null &&
                   other == _callback;
        }
        public bool Equals(CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> other)
        {
            return (object)other != null &&
                   Equals(other._callback);
        }
        public override bool Equals(object obj)
        {
            return obj != null &&
                   obj.GetType() == GetType() &&
                   Equals(obj as CCall<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>);
        }

        public override int GetHashCode()
        {
            return _callback.GetHashCode();
        }
    }
}
