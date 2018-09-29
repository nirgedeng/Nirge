/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Net;
using System;
using log4net;
using System.Runtime.CompilerServices;

namespace Nirge.Core
{
    public interface ITcpClientPkg
    {
        eTcpError Fill(object pkg, Queue<ArraySegment<byte>> target, ITcpClientCache cache);
    }

    public class CTcpClientArraySegment : ITcpClientPkg
    {
        public eTcpError Fill(object pkg, Queue<ArraySegment<byte>> target, ITcpClientCache cache)
        {
            if (pkg == null)
                return eTcpError.BlockNull;
            if (!(pkg is byte[] || pkg is ArraySegment<byte>))
                return eTcpError.PkgTypeNoMatch;
            var source = (ArraySegment<byte>)pkg;
            if (source.Count == 0)
                return eTcpError.BlockSizeIsZero;
            if (target == null)
                return eTcpError.ArgNull;
            if (cache == null)
                return eTcpError.ArgNull;

            return cache.AllocSendBuf(source.Count, target);
        }
    }
}
