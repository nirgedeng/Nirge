/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Threading;
using log4net;
using System;

namespace Nirge.Core
{
    #region 

    public class CClientRpcCommunicator : CRpcCommunicator
    {
        ILog _log;
        CTcpClient _cli;

        public CClientRpcCommunicator(ILog log, CTcpClient cli)
        {
            _log = log;
            _cli = cli;
        }

        public override bool Send(int channel, byte[] buf, int offset, int count)
        {
            var ret = _cli.Send(buf, offset, count);

            switch (ret)
            {
            case eTcpError.None:
                return true;
            default:
                _log.ErrorFormat("[Rpc]ClientRpcCommunicator.Send err, code:\"{0}\", channel:\"{1}\", buf:\"{2},{3}\""
                    , ret
                    , channel
                    , offset
                    , count);
                return false;
            }
        }
    }

    #endregion

    #region 

    public class CServerRpcCommunicator : CRpcCommunicator
    {
        ILog _log;
        CTcpServer _ser;

        public CServerRpcCommunicator(ILog log, CTcpServer ser)
        {
            _log = log;
            _ser = ser;
        }

        public override bool Send(int channel, byte[] buf, int offset, int count)
        {
            var ret = _ser.Send(channel, buf, offset, count);

            switch (ret)
            {
            case eTcpError.None:
                return true;
            default:
                _log.ErrorFormat("[Rpc]ServerRpcCommunicator.Send err, code:\"{0}\", channel:\"{1}\", buf:\"{2},{3}\""
                    , ret
                    , channel
                    , offset
                    , count);
                return false;
            }
        }
    }

    #endregion
}
