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
                throw new ArgumentNullException();
            if (srcOffset < 0)
                throw new ArgumentOutOfRangeException();
            if (dst == null)
                throw new ArgumentNullException();
            if (dstOffset < 0)
                throw new ArgumentOutOfRangeException();
            if (count < 0)
                throw new ArgumentOutOfRangeException();

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
