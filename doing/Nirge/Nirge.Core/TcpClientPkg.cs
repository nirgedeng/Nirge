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

            var pkgSize = source.Count;
            cache.AllocSendBuf(pkgSize, _segs);

            if (_segs.Count == 0)
                throw new CNetException("zero alloc send buf");
            var pkgSizeTmp = 0;
            foreach (var i in _segs)
                pkgSizeTmp += i.Length;
            if (pkgSizeTmp < pkgSize)
            {
                foreach (var i in _segs)
                    cache.CollectSendBuf(i);
                _segs.Clear();
                throw new CNetException("alloc send buf not encough");
            }

            var offset = source.Offset;
            foreach (var i in _segs)
            {
                var count = pkgSize < i.Length
                    ? pkgSize : i.Length;
                Buffer.BlockCopy(source.Array, offset, i, 0, count);
                offset += count;
                pkgSize -= count;
                target.Enqueue(new ArraySegment<byte>(i, 0, count));
            }
            _segs.Clear();
        }
    }
}
