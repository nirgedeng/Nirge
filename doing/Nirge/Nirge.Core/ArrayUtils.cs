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
        public static eTcpError Copy(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
        {
            if (src == null)
                return eTcpError.BlockNull;
            if (srcOffset < 0)
                return eTcpError.BlockSizeOutOfRange;
            if (dst == null)
                return eTcpError.BlockNull;
            if (dstOffset < 0)
                return eTcpError.BlockSizeOutOfRange;
            if (count < 0)
                return eTcpError.BlockSizeOutOfRange;

            if (count == 0)
                return eTcpError.Success;
            else if (count > 8)
                Buffer.BlockCopy(src, srcOffset, dst, dstOffset, count);
            else
            {
                for (var i = 0; i < count; ++i)
                    dst[dstOffset + i] = src[srcOffset + i];
            }

            return eTcpError.Success;
        }
    }
}
