﻿/*------------------------------------------------------------------
    Copyright ? : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System;

namespace Nirge.Core
{
    public class CRingBuf
    {
        byte[] _buf;
        int _head;
        int _tail;
        int _used;

        public bool IsEmpty
        {
            get
            {
                return _used == 0;
            }
        }

        public bool IsFull
        {
            get
            {
                return _used == _buf.Length;
            }
        }

        public int UsedCapacity
        {
            get
            {
                return _used;
            }
        }

        public int UnusedCapacity
        {
            get
            {
                return _buf.Length - _used;
            }
        }

        public CRingBuf(int capacity)
        {
            _buf = new byte[capacity];
            Clear();
        }

        public void Clear()
        {
            _head = 0;
            _tail = 0;
            _used = 0;
        }

        public bool Write(byte[] buf, int offset, int count)
        {
            if (buf == null)
                return false;
            if (count == 0)
                return false;
            if (UnusedCapacity < count)
                return false;

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

            _used += count;

            return true;
        }

        public bool Read(byte[] buf, int offset, int count)
        {
            if (buf == null)
                return false;
            if (count == 0)
                return false;
            if (count > UsedCapacity)
                return false;

            Read(ref _tail, buf, offset, count);
            _used -= count;

            return true;
        }

        public bool Peek(byte[] buf, int offset, int count)
        {
            if (buf == null)
                return false;
            if (count == 0)
                return false;
            if (count > UsedCapacity)
                return false;

            var tail = _tail;
            Read(ref tail, buf, offset, count);

            return true;
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
