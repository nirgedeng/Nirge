/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System;

namespace Nirge.Core
{
    public enum eTcpError
    {
        None,

        Exception,
        SocketError,

        WrongState,
        ArgumentNullRange,
        ArgumentOutOfRange,
        PkgSizeOutOfRange,
        SendQueueFull,
        RecvQueueFull,
        CliOutOfRange,
    }
}
