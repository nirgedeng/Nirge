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
        void Fill(object pkg, Queue<ArraySegment<byte>> target, ITcpClientCache cache);
    }

    public class CTcpClientArraySegment : ITcpClientPkg
    {
        List<byte[]> _segs;

        public CTcpClientArraySegment()
        {
            _segs = new List<byte[]>();
        }

        public void Fill(object pkg, Queue<ArraySegment<byte>> target, ITcpClientCache cache)
        {
            if (pkg == null)
                throw new ArgumentNullException("pkg");
            if (!(pkg is byte[] || pkg is ArraySegment<byte>))
                throw new ArgumentOutOfRangeException("pkg");
            var source = (ArraySegment<byte>)pkg;
            if (source.Count == 0)
                throw new ArgumentOutOfRangeException("pkg");
            if (target == null)
                throw new ArgumentNullException("target");
            if (cache == null)
                throw new ArgumentNullException("cache");

            var count = source.Count;

            cache.AllocSendBuf(source.Count, _segs);

            if (_segs.Count == 0)
                throw new CNetException("zero alloc send buf");

            foreach (var i in _segs)
                target.Enqueue(i);
            _segs.Clear();
        }
    }
}
