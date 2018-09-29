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
    public class CTcpClientPkgFill
    {
        Dictionary<Type, ITcpClientPkg> _pkgs;
        List<byte[]> _segs;

        public CTcpClientPkgFill()
        {
            _pkgs = new Dictionary<Type, ITcpClientPkg>();
            _segs = new List<byte[]>();
        }

        public void Register(Type pkgType, ITcpClientPkg pkg)
        {
            if (pkgType == null)
                throw new ArgumentNullException("pkgType");
            if (pkg == null)
                throw new ArgumentNullException("pkg");

            _pkgs.Add(pkgType, pkg);
        }

        public void Fill(object pkg, Queue<ArraySegment<byte>> target, ITcpClientCache cache)
        {
            if (pkg == null)
                throw new ArgumentNullException("pkg");
            if (target == null)
                throw new ArgumentNullException("target");
            if (cache == null)
                throw new ArgumentNullException("cache");

            ITcpClientPkg i;
            if (_pkgs.TryGetValue(pkg.GetType(), out i))
                i.Fill(pkg, target, cache, _segs);
            else
                throw new ArgumentOutOfRangeException("pkg");
        }

    }

    public interface ITcpClientPkg
    {
        void Fill(object pkg, Queue<ArraySegment<byte>> target, ITcpClientCache cache, IList<byte[]> segs);
    }

    public class CTcpClientArraySegment : ITcpClientPkg
    {
        public void Fill(object pkg, Queue<ArraySegment<byte>> target, ITcpClientCache cache, IList<byte[]> segs)
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
            cache.AllocSendBuf(pkgSize, segs);

            if (segs.Count == 0)
                throw new CNetException("zero alloc send buf");
            var pkgSizeTmp = 0;
            foreach (var i in segs)
                pkgSizeTmp += i.Length;
            if (pkgSizeTmp < pkgSize)
            {
                foreach (var i in segs)
                    cache.CollectSendBuf(i);
                segs.Clear();
                throw new CNetException("alloc send buf not encough");
            }

            var offset = source.Offset;
            foreach (var i in segs)
            {
                var count = pkgSize < i.Length
                    ? pkgSize : i.Length;
                ArrayUtils.Copy(source.Array, offset, i, 0, count);
                offset += count;
                pkgSize -= count;
                target.Enqueue(new ArraySegment<byte>(i, 0, count));
            }
            segs.Clear();
        }
    }
}
