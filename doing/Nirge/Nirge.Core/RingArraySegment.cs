﻿/*------------------------------------------------------------------
    Copyright ? : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System;

namespace Nirge.Core
{
    public class CRingArraySegment
    {
        byte[] _buf;
        int _head;
        int _tail;
        int _usedSize;

        public bool IsEmpty
        {
            get
            {
                return _usedSize == 0;
            }
        }

        public bool IsFull
        {
            get
            {
                return _usedSize == _buf.Length;
            }
        }

        public int UsedSize
        {
            get
            {
                return _usedSize;
            }
        }

        public int UnusedSize
        {
            get
            {
                return _buf.Length - _usedSize;
            }
        }

        public CRingArraySegment(int count)
        {
            if (count == 0)
                throw new ArgumentOutOfRangeException("count");

            _buf = new byte[count];
            Clear();
        }

        public void Clear()
        {
            _head = 0;
            _tail = 0;
            _usedSize = 0;
        }

        public void Write(byte[] buf, int offset, int count)
        {
            if (buf == null)
                throw new ArgumentNullException("buf");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");
            if (count == 0)
                throw new ArgumentOutOfRangeException("count");
            if (UnusedSize < count)
                throw new ArgumentOutOfRangeException("count");

            if (_head < _tail)
            {
                ArrayUtils.Copy(buf, offset, _buf, _head, count);
                _head += count;
            }
            else
            {
                var p = _buf.Length - _head;
                if (p < count)
                {
                    var q = count - p;
                    ArrayUtils.Copy(buf, offset, _buf, _head, p);
                    ArrayUtils.Copy(buf, offset + p, _buf, 0, q);
                    _head = q;
                }
                else
                {
                    ArrayUtils.Copy(buf, offset, _buf, _head, count);
                    _head += count;
                }
            }

            _usedSize += count;
        }

        public void Read(byte[] buf, int offset, int count)
        {
            if (buf == null)
                throw new ArgumentNullException("buf");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");
            if (count == 0)
                throw new ArgumentOutOfRangeException("count");
            if (count > UsedSize)
                throw new ArgumentOutOfRangeException("count");

            Read(ref _tail, buf, offset, count);
            _usedSize -= count;
        }

        public void Peek(byte[] buf, int offset, int count)
        {
            if (buf == null)
                throw new ArgumentNullException("buf");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");
            if (count == 0)
                throw new ArgumentOutOfRangeException("count");
            if (count > UsedSize)
                throw new ArgumentOutOfRangeException("count");

            var tail = _tail;
            Read(ref tail, buf, offset, count);
        }

        void Read(ref int tail, byte[] buf, int offset, int count)
        {
            if (_head > tail)
            {
                ArrayUtils.Copy(_buf, tail, buf, offset, count);
                tail += count;
            }
            else
            {
                var p = _buf.Length - tail;
                if (p < count)
                {
                    var q = count - p;
                    ArrayUtils.Copy(_buf, tail, buf, offset, p);
                    ArrayUtils.Copy(_buf, 0, buf, offset + p, q);
                    tail = q;
                }
                else
                {
                    ArrayUtils.Copy(_buf, tail, buf, offset, count);
                    tail += count;
                }
            }
        }
    }
}
