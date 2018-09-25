/*------------------------------------------------------------------
    Copyright ? : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System;

namespace Nirge.Core
{
    #region

    public class CDataEventArgs : EventArgs, IObjAlloc, IObjCollect
    {
        public static CDataEventArgs<TArg1> Alloc<TArg1>(TArg1 arg1)
        {
            return new CDataEventArgs<TArg1>(arg1);
        }
        public static CDataEventArgs<TArg1, TArg2> Alloc<TArg1, TArg2>(TArg1 arg1, TArg2 arg2)
        {
            return new CDataEventArgs<TArg1, TArg2>(arg1, arg2);
        }
        public static CDataEventArgs<TArg1, TArg2, TArg3> Alloc<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return new CDataEventArgs<TArg1, TArg2, TArg3>(arg1, arg2, arg3);
        }
        public static CDataEventArgs<TArg1, TArg2, TArg3, TArg4> Alloc<TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            return new CDataEventArgs<TArg1, TArg2, TArg3, TArg4>(arg1, arg2, arg3, arg4);
        }
        public static CDataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5> Alloc<TArg1, TArg2, TArg3, TArg4, TArg5>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            return new CDataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5>(arg1, arg2, arg3, arg4, arg5);
        }
        public static CDataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> Alloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
        {
            return new CDataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(arg1, arg2, arg3, arg4, arg5, arg6);
        }
        public static CDataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> Alloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7)
        {
            return new CDataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
        public static CDataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> Alloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8)
        {
            return new CDataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }
        public static CDataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> Alloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9)
        {
            return new CDataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public CDataEventArgs()
        {
            Alloc();
        }

        public void Alloc()
        {
        }
        public void Collect()
        {
        }
    }

    #endregion

    #region

    public class CDataEventArgs<TArg1> : EventArgs, IObjAlloc<TArg1>, IObjCollect
    {
        public TArg1 Arg1
        {
            get;
            set;
        }

        public CDataEventArgs(TArg1 arg1)
        {
            Alloc(arg1);
        }

        public void Alloc(TArg1 arg1)
        {
            Arg1 = arg1;
        }
        public void Collect()
        {
            Arg1 = default(TArg1);
        }
    }

    public class CDataEventArgs<TArg1, TArg2> : EventArgs, IObjAlloc<TArg1, TArg2>, IObjCollect
    {
        public TArg1 Arg1
        {
            get;
            set;
        }
        public TArg2 Arg2
        {
            get;
            set;
        }

        public CDataEventArgs(TArg1 arg1, TArg2 arg2)
        {
            Alloc(arg1, arg2);
        }

        public void Alloc(TArg1 arg1, TArg2 arg2)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }
        public void Collect()
        {
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
        }
    }

    public class CDataEventArgs<TArg1, TArg2, TArg3> : EventArgs, IObjAlloc<TArg1, TArg2, TArg3>, IObjCollect
    {
        public TArg1 Arg1
        {
            get;
            set;
        }
        public TArg2 Arg2
        {
            get;
            set;
        }
        public TArg3 Arg3
        {
            get;
            set;
        }

        public CDataEventArgs(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            Alloc(arg1, arg2, arg3);
        }

        public void Alloc(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }
        public void Collect()
        {
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
        }
    }

    public class CDataEventArgs<TArg1, TArg2, TArg3, TArg4> : EventArgs, IObjAlloc<TArg1, TArg2, TArg3, TArg4>, IObjCollect
    {
        public TArg1 Arg1
        {
            get;
            set;
        }
        public TArg2 Arg2
        {
            get;
            set;
        }
        public TArg3 Arg3
        {
            get;
            set;
        }
        public TArg4 Arg4
        {
            get;
            set;
        }

        public CDataEventArgs(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            Alloc(arg1, arg2, arg3, arg4);
        }

        public void Alloc(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }
        public void Collect()
        {
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
            Arg4 = default(TArg4);
        }
    }

    public class CDataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5> : EventArgs, IObjAlloc<TArg1, TArg2, TArg3, TArg4, TArg5>, IObjCollect
    {
        public TArg1 Arg1
        {
            get;
            set;
        }
        public TArg2 Arg2
        {
            get;
            set;
        }
        public TArg3 Arg3
        {
            get;
            set;
        }
        public TArg4 Arg4
        {
            get;
            set;
        }
        public TArg5 Arg5
        {
            get;
            set;
        }

        public CDataEventArgs(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            Alloc(arg1, arg2, arg3, arg4, arg5);
        }

        public void Alloc(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
        }
        public void Collect()
        {
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
            Arg4 = default(TArg4);
            Arg5 = default(TArg5);
        }
    }

    public class CDataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> : EventArgs, IObjAlloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>, IObjCollect
    {
        public TArg1 Arg1
        {
            get;
            set;
        }
        public TArg2 Arg2
        {
            get;
            set;
        }
        public TArg3 Arg3
        {
            get;
            set;
        }
        public TArg4 Arg4
        {
            get;
            set;
        }
        public TArg5 Arg5
        {
            get;
            set;
        }
        public TArg6 Arg6
        {
            get;
            set;
        }

        public CDataEventArgs(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
        {
            Alloc(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public void Alloc(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
        }
        public void Collect()
        {
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
            Arg4 = default(TArg4);
            Arg5 = default(TArg5);
            Arg6 = default(TArg6);
        }
    }

    public class CDataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> : EventArgs, IObjAlloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>, IObjCollect
    {
        public TArg1 Arg1
        {
            get;
            set;
        }
        public TArg2 Arg2
        {
            get;
            set;
        }
        public TArg3 Arg3
        {
            get;
            set;
        }
        public TArg4 Arg4
        {
            get;
            set;
        }
        public TArg5 Arg5
        {
            get;
            set;
        }
        public TArg6 Arg6
        {
            get;
            set;
        }
        public TArg7 Arg7
        {
            get;
            set;
        }

        public CDataEventArgs(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7)
        {
            Alloc(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public void Alloc(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
        }
        public void Collect()
        {
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
            Arg4 = default(TArg4);
            Arg5 = default(TArg5);
            Arg6 = default(TArg6);
            Arg7 = default(TArg7);
        }
    }

    public class CDataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> : EventArgs, IObjAlloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>, IObjCollect
    {
        public TArg1 Arg1
        {
            get;
            set;
        }
        public TArg2 Arg2
        {
            get;
            set;
        }
        public TArg3 Arg3
        {
            get;
            set;
        }
        public TArg4 Arg4
        {
            get;
            set;
        }
        public TArg5 Arg5
        {
            get;
            set;
        }
        public TArg6 Arg6
        {
            get;
            set;
        }
        public TArg7 Arg7
        {
            get;
            set;
        }
        public TArg8 Arg8
        {
            get;
            set;
        }

        public CDataEventArgs(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8)
        {
            Alloc(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public void Alloc(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
            Arg7 = arg7;
            Arg8 = arg8;
        }
        public void Collect()
        {
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
            Arg4 = default(TArg4);
            Arg5 = default(TArg5);
            Arg6 = default(TArg6);
            Arg7 = default(TArg7);
            Arg8 = default(TArg8);
        }
    }

    public class CDataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> : EventArgs, IObjAlloc<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>, IObjCollect
    {
        public TArg1 Arg1
        {
            get;
            set;
        }
        public TArg2 Arg2
        {
            get;
            set;
        }
        public TArg3 Arg3
        {
            get;
            set;
        }
        public TArg4 Arg4
        {
            get;
            set;
        }
        public TArg5 Arg5
        {
            get;
            set;
        }
        public TArg6 Arg6
        {
            get;
            set;
        }
        public TArg7 Arg7
        {
            get;
            set;
        }
        public TArg8 Arg8
        {
            get;
            set;
        }
        public TArg9 Arg9
        {
            get;
            set;
        }

        public CDataEventArgs(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9)
        {
            Alloc(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public void Alloc(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9)
        {
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
        public void Collect()
        {
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
    }

    #endregion
}
