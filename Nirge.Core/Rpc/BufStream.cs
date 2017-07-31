/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Reflection;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using System;

namespace Nirge.Core
{
    public class CBufStream : Stream
    {
        //------------------------------------------------------------------

        byte[] _buf;
        int _l;
        int _len;
        int _r;
        int _pos;

        public CBufStream(byte[] buf, int offset, int count)
        {
            SetBuf(buf, offset, count);
        }

        public CBufStream(byte[] buf)
            :
            this(buf, 0, buf.Length)
        {
        }

        public void SetBuf(byte[] buf, int offset, int count)
        {
            _buf = buf;
            _l = offset;
            _len = count;
            _r = _l + count;
            _pos = _l;
        }

        public void SetBuf(byte[] buf)
        {
            SetBuf(buf, 0, buf.Length);
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        //------------------------------------------------------------------

        public override long Position
        {
            get
            {
                return _pos;
            }
            set
            {
                if (value < 0 || value >= _len)
                {
                    throw new ArgumentOutOfRangeException("_pos");
                }
                _pos = (int)value;
            }
        }

        public byte[] Array
        {
            get
            {
                return _buf;
            }
        }

        public int Offset
        {
            get
            {
                return _l;
            }
        }

        public override long Length
        {
            get
            {
                return _pos - _l;
            }
        }

        public int Count
        {
            get
            {
                return (int)Length;
            }
        }

        public byte[] ToArray()
        {
            var buf = new byte[Count];
            Buffer.BlockCopy(_buf, _l, buf, 0, Count);
            return buf;
        }

        public override int Read(byte[] buf, int offset, int count)
        {
            var r = _pos + count;
            if (r > _r)
            {
                return 0;
            }
            else
            {
                Buffer.BlockCopy(_buf, _pos, buf, offset, count);
                _pos = r;
                return count;
            }
        }

        public override int ReadByte()
        {
            if (_pos < _r)
            {
                return _buf[_pos++];
            }
            else
            {
                return -1;
            }
        }

        public override void Write(byte[] buf, int offset, int len)
        {
            var r = _pos + len;
            if (r > _r)
            {
                throw new ArgumentOutOfRangeException("_pos");
            }
            else
            {
                Buffer.BlockCopy(buf, offset, _buf, _pos, len);
                _pos = r;
            }
        }

        public override void WriteByte(byte val)
        {
            if (_pos < _r)
            {
                _buf[_pos++] = val;
            }
            else
            {
                throw new ArgumentOutOfRangeException("_pos");
            }
        }

        //------------------------------------------------------------------
    }
}
