/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System;

namespace Nirge.Core
{
    public static class CArrayUtils
    {
        public static void Copy(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            if (srcOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(srcOffset));
            if (dst == null)
                throw new ArgumentNullException(nameof(dst));
            if (dstOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(dstOffset));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (count == 0)
                return;
            else if (count > 12)
                Buffer.BlockCopy(src, srcOffset, dst, dstOffset, count);
            else
            {
                for (var i = 0; i < count; ++i)
                    dst[dstOffset + i] = src[srcOffset + i];
            }
        }
    }
}
