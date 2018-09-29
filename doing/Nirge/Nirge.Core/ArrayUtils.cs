/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System;

namespace Nirge.Core
{
    public static class ArrayUtils
    {
        public static void Copy(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
        {
            if (src == null)
                throw new ArgumentNullException("src");
            if (srcOffset < 0)
                throw new ArgumentOutOfRangeException("srcOffset");
            if (dst == null)
                throw new ArgumentNullException("dst");
            if (dstOffset < 0)
                throw new ArgumentOutOfRangeException("dstOffset");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            if (count == 0)
                return;
            else if (count > 8)
                Buffer.BlockCopy(src, srcOffset, dst, dstOffset, count);
            else
            {
                for (var i = 0; i < count; ++i)
                    dst[dstOffset + i] = src[srcOffset + i];
            }
        }
    }
}
