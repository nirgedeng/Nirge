/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Reflection;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using System;

namespace Nirge.Core
{
    public enum RpcProto
    {
        C2sRpcCall = 1,

        S2cRpcCall = 11,
        S2cRpcCallException,
    }
}
