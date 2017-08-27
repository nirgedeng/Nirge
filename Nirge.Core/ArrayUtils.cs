/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;

namespace Nirge.Core
{
    public static class ArrayUtils
    {
        public static void Copy(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
        {
            if (count == 0)
                return;
            if (src == null)
                throw new ArgumentNullException();
            if (srcOffset < 0)
                throw new ArgumentOutOfRangeException();
            if (dst == null)
                throw new ArgumentNullException();
            if (dstOffset < 0)
                throw new ArgumentOutOfRangeException();

            if (count > 12)
                Buffer.BlockCopy(src, srcOffset, dst, dstOffset, count);
            else
            {
                for (var i = 0; i < count; ++i)
                    dst[dstOffset + i] = src[srcOffset + i];
            }
        }
    }
}
