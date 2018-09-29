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
        ITcpClientPkg _segPkg;

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

        public override void Alloc(CTcpClientArgs args, ILog log, ITcpClientCache cache)
        {
            base.Alloc(args, log, cache);

            _segPkg = new CTcpClientArraySegment();
        }

        protected override void Clear()
        {
            base.Clear();
        }

        public override void Collect()
        {
            switch (_state)
            {
            case eTcpClientState.Closed:
                _segPkg = null;
                break;
            case eTcpClientState.Connecting:
            case eTcpClientState.Connected:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
                break;
            }

            base.Collect();
        }

        protected override void Fill(object pkg, Queue<ArraySegment<byte>> target, ITcpClientCache cache)
        {
            if (pkg is byte[])
                _segPkg.Fill(pkg, target, cache);
            else if (pkg is ArraySegment<byte>)
                _segPkg.Fill(pkg, target, cache);

            throw new CNetException("pkg type not supported");
        }
    }
}
