/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace Nirge.Core
{
    #region 

    public class DataEventArgs
    {
        public static DataEventArgs<TArg1> Create<TArg1>(TArg1 arg1)
        {
            return new DataEventArgs<TArg1>(arg1);
        }
        public static DataEventArgs<TArg1, TArg2> Create<TArg1, TArg2>(TArg1 arg1, TArg2 arg2)
        {
            return new DataEventArgs<TArg1, TArg2>(arg1, arg2);
        }
        public static DataEventArgs<TArg1, TArg2, TArg3> Create<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return new DataEventArgs<TArg1, TArg2, TArg3>(arg1, arg2, arg3);
        }
        public static DataEventArgs<TArg1, TArg2, TArg3, TArg4> Create<TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            return new DataEventArgs<TArg1, TArg2, TArg3, TArg4>(arg1, arg2, arg3, arg4);
        }
        public static DataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5> Create<TArg1, TArg2, TArg3, TArg4, TArg5>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            return new DataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5>(arg1, arg2, arg3, arg4, arg5);
        }
        public static DataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
        {
            return new DataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(arg1, arg2, arg3, arg4, arg5, arg6);
        }
        public static DataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7)
        {
            return new DataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
        public static DataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8)
        {
            return new DataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }
        public static DataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9)
        {
            return new DataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }
    }

    #endregion

    #region 

    public class DataEventArgs<TArg1> : EventArgs, IObjCtor<TArg1>, IObjDtor
    {
        public TArg1 Arg1
        {
            get;
            set;
        }

        public DataEventArgs(TArg1 arg1)
        {
            Init(arg1);
        }

        public void Init(TArg1 arg1)
        {
            Arg1 = arg1;
        }
        public void Destroy()
        {
            Arg1 = default(TArg1);
        }
    }

    public class DataEventArgs<TArg1, TArg2> : EventArgs, IObjCtor<TArg1, TArg2>, IObjDtor
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

        public DataEventArgs(TArg1 arg1, TArg2 arg2)
        {
            Init(arg1, arg2);
        }

        public void Init(TArg1 arg1, TArg2 arg2)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }
        public void Destroy()
        {
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
        }
    }

    public class DataEventArgs<TArg1, TArg2, TArg3> : EventArgs, IObjCtor<TArg1, TArg2, TArg3>, IObjDtor
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

        public DataEventArgs(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            Init(arg1, arg2, arg3);
        }

        public void Init(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }
        public void Destroy()
        {
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
        }
    }

    public class DataEventArgs<TArg1, TArg2, TArg3, TArg4> : EventArgs, IObjCtor<TArg1, TArg2, TArg3, TArg4>, IObjDtor
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

        public DataEventArgs(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            Init(arg1, arg2, arg3, arg4);
        }

        public void Init(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
        }
        public void Destroy()
        {
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
            Arg4 = default(TArg4);
        }
    }

    public class DataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5> : EventArgs, IObjCtor<TArg1, TArg2, TArg3, TArg4, TArg5>, IObjDtor
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

        public DataEventArgs(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            Init(arg1, arg2, arg3, arg4, arg5);
        }

        public void Init(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
        }
        public void Destroy()
        {
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
            Arg4 = default(TArg4);
            Arg5 = default(TArg5);
        }
    }

    public class DataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> : EventArgs, IObjCtor<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>, IObjDtor
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

        public DataEventArgs(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
        {
            Init(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public void Init(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
        {
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
            Arg4 = arg4;
            Arg5 = arg5;
            Arg6 = arg6;
        }
        public void Destroy()
        {
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
            Arg4 = default(TArg4);
            Arg5 = default(TArg5);
            Arg6 = default(TArg6);
        }
    }

    public class DataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> : EventArgs, IObjCtor<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>, IObjDtor
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

        public DataEventArgs(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7)
        {
            Init(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public void Init(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7)
        {
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
            Arg1 = default(TArg1);
            Arg2 = default(TArg2);
            Arg3 = default(TArg3);
            Arg4 = default(TArg4);
            Arg5 = default(TArg5);
            Arg6 = default(TArg6);
            Arg7 = default(TArg7);
        }
    }

    public class DataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> : EventArgs, IObjCtor<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>, IObjDtor
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

        public DataEventArgs(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8)
        {
            Init(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public void Init(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8)
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
        public void Destroy()
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

    public class DataEventArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> : EventArgs, IObjCtor<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>, IObjDtor
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

        public DataEventArgs(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9)
        {
            Init(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public void Init(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, TArg9 arg9)
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
        public void Destroy()
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
