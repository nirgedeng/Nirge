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
    public abstract class CRpcChannel
    {
        public abstract bool Send(int channel, byte[] buf, int offset, int count);
    }

    public class CClientRpcChannel
    {
        public virtual bool Send(int channel, byte[] buf, int offset, int count)
        {
            return true;
        }
    }

    public class CServerRpcChannel
    {
        public virtual bool Send(int channel, byte[] buf, int offset, int count)
        {
            return true;
        }
    }
}
