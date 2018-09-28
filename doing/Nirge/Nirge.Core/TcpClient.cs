/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Net;
using System;
using log4net;

namespace Nirge.Core
{
    public class CTcpClient : CTcpClientBase
    {
        public CTcpClient(CTcpClientArgs args, ILog log, ITcpClientCache cache)
            :
            base(args, log, cache)
        {
        }

        public CTcpClient(ILog log)
            :
            base(log)
        {
        }

        public CTcpClient()
            :
            base()
        {
        }

        protected override eTcpError Fill(object pkg, Queue<ArraySegment<byte>> target, ITcpClientCache cache)
        {
            if (pkg is byte[])
            {
                ArraySegment<byte> source = (byte[])pkg;
                if (source.Count == 0)
                    return eTcpError.BlockSizeIsZero;

                return cache.AllocSendBuf(source.Count, target);
            }
            else if (pkg is ArraySegment<byte>)
            {
                var source = (ArraySegment<byte>)pkg;
                if (source.Count == 0)
                    return eTcpError.BlockSizeIsZero;

                return cache.AllocSendBuf(source.Count, target);
            }

            return eTcpError.BlockTypeNotSupported;
        }
    }
}
