/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System;

namespace Nirge.Core
{
    public static class CHashUtils
    {
        public static uint BKDRHash(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentOutOfRangeException("s");

            const uint gSeed = 131;

            uint hash = 0;
            foreach (var i in s)
                hash = hash * gSeed + i;
            return hash;
        }
    }
}
