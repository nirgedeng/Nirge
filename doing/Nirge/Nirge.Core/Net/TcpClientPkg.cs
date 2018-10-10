/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Net;
using System;
using log4net;
using System.Runtime.CompilerServices;
using Google.Protobuf;
using System.Reflection;

namespace Nirge.Core
{
    #region 

    public class CTcpClientPkgHead : ITcpClientPkgHead
    {
        const int gPkgHeadSize = 5;

        int _sendPkgType;
        int _sendPkgSize;
        byte[] _recvPkgHeadBuf;
        int _recvPkgType;
        int _recvPkgSize;

        public int PkgHeadSize
        {
            get
            {
                return gPkgHeadSize;
            }
        }

        public int SendPkgType
        {
            get
            {
                return _sendPkgType;
            }
            set
            {
                _sendPkgType = value;
            }
        }

        public int SendPkgSize
        {
            get
            {
                return _sendPkgSize;
            }
            set
            {
                _sendPkgSize = value;
            }
        }

        public byte[] RecvPkgHeadBuf
        {
            get
            {
                return _recvPkgHeadBuf;
            }
        }

        public int RecvPkgType
        {
            get
            {
                return _recvPkgType;
            }
            set
            {
                _recvPkgType = value;
            }
        }

        public int RecvPkgSize
        {
            get
            {
                return _recvPkgSize;
            }
            set
            {
                _recvPkgSize = value;
            }
        }

        public CTcpClientPkgHead()
        {
            _recvPkgHeadBuf = new byte[PkgHeadSize];
            Clear();
        }

        public void Clear()
        {
            _sendPkgType = 0;
            _sendPkgSize = 0;
            _recvPkgType = 0;
            _recvPkgSize = 0;
        }

        public void Fill(byte[] buf)
        {
            if (buf == null)
                throw new ArgumentNullException(nameof(buf));
            if (buf.Length < PkgHeadSize)
                throw new ArgumentOutOfRangeException(nameof(buf));

            var i = BitConverter.GetBytes(_sendPkgSize);
            buf[0] = i[0];
            buf[1] = i[1];
            buf[2] = i[2];
            buf[3] = i[3];
            buf[4] = (byte)_sendPkgType;
        }

        public void UnFill()
        {
            _recvPkgSize = BitConverter.ToInt32(_recvPkgHeadBuf, 0);
            _recvPkgType = _recvPkgHeadBuf[4];
        }
    }

    #endregion

    #region 

    public class CTcpClientArraySegment : ITcpClientPkg
    {
        public ArraySegment<byte> Fill(ITcpClientPkgHead pkgHead, int gPkgSize, object pkg, ITcpClientCache cache)
        {
            if (pkgHead == null)
                throw new ArgumentNullException(nameof(pkgHead));
            if (pkgHead.PkgHeadSize == 0)
                throw new ArgumentOutOfRangeException(nameof(pkgHead));
            if (gPkgSize == 0)
                throw new ArgumentOutOfRangeException(nameof(gPkgSize));
            if (pkg == null)
                throw new ArgumentNullException(nameof(pkg));
            if (!(pkg is byte[] || pkg is ArraySegment<byte>))
                throw new ArgumentOutOfRangeException(nameof(pkg));
            var seg = (ArraySegment<byte>)pkg;
            if (seg.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(pkg));
            var pkgSize = pkgHead.PkgHeadSize + seg.Count;
            if (pkgSize > gPkgSize)
                throw new ArgumentOutOfRangeException(nameof(pkg));
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

            var pkgSeg = cache.AllocSendBuf(pkgSize);
            CArrayUtils.Copy(seg.Array, seg.Offset, pkgSeg, pkgHead.PkgHeadSize, seg.Count);
            pkgHead.SendPkgSize = seg.Count;
            pkgHead.SendPkgType = (int)eTcpClientPkgType.ArraySegment;
            return new ArraySegment<byte>(pkgSeg, 0, pkgSize);
        }

        public object UnFill(ArraySegment<byte> pkgSeg, ITcpClientCache cache)
        {
            if (pkgSeg == null)
                throw new ArgumentNullException(nameof(pkgSeg));
            if (pkgSeg.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(pkgSeg));
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

            var pkg = new byte[pkgSeg.Count];
            CArrayUtils.Copy(pkgSeg.Array, pkgSeg.Offset, pkg, 0, pkgSeg.Count);

            return pkg;
        }
    }

    public interface ITcpClientProtobufCode
    {
        uint GetCode(Type pkgType);
        MessageParser GetParser(uint pkgCode);
    }

    public class CTcpClientProtobufCode : ITcpClientProtobufCode
    {
        HashSet<Assembly> _assemblys;
        Dictionary<int, uint> _codes;
        Dictionary<uint, MessageParser> _parsers;

        public CTcpClientProtobufCode()
        {
            _assemblys = new HashSet<Assembly>();
            _codes = new Dictionary<int, uint>();
            _parsers = new Dictionary<uint, MessageParser>();

            Collect(Assembly.GetExecutingAssembly());
        }

        public void Collect(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            if (_assemblys.Contains(assembly))
                throw new ArgumentOutOfRangeException(nameof(assembly));

            _assemblys.Add(assembly);

            foreach (var i in assembly.GetExportedTypes())
            {
                if (i.IsInterface)
                    continue;
                if (i.IsAbstract)
                    continue;
                if (i.GetInterface(typeof(IMessage<>).FullName) == null)
                    continue;
                var pkgCode = CHashUtils.BKDRHash(i.FullName);
                if (_parsers.ContainsKey(pkgCode))
                    throw new InvalidOperationException(nameof(pkgCode));
                var pkgParser = (MessageParser)i.GetProperty("Parser")?.GetValue(null);
                if (pkgParser == null)
                    throw new InvalidOperationException(nameof(pkgParser));

                _codes.Add(i.GetHashCode(), pkgCode);
                _parsers.Add(pkgCode, pkgParser);
            }
        }

        public uint GetCode(Type pkgType)
        {
            if (pkgType == null)
                throw new ArgumentNullException(nameof(pkgType));
            if (_codes.TryGetValue(pkgType.GetHashCode(), out var pkgCode))
                return pkgCode;
            else
                throw new ArgumentOutOfRangeException(nameof(pkgType));
        }

        public MessageParser GetParser(uint pkgCode)
        {
            if (_parsers.TryGetValue(pkgCode, out var parser))
                return parser;
            else
                throw new ArgumentOutOfRangeException(nameof(pkgCode));
        }
    }

    public class CTcpClientProtobuf : ITcpClientPkg
    {
        const int gPkgCodeSize = 4;

        ITcpClientProtobufCode _code;
        CArrayStream _stream;
        CodedInputStream _input;
        CodedOutputStream _output;

        public CTcpClientProtobuf(ITcpClientProtobufCode code)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            _code = code;
            _stream = new CArrayStream(0);
            _input = new CodedInputStream(_stream, true);
            _output = new CodedOutputStream(_stream, true);
        }

        public ArraySegment<byte> Fill(ITcpClientPkgHead pkgHead, int gPkgSize, object pkg, ITcpClientCache cache)
        {
            if (pkgHead == null)
                throw new ArgumentNullException(nameof(pkgHead));
            if (pkgHead.PkgHeadSize == 0)
                throw new ArgumentOutOfRangeException(nameof(pkgHead));
            if (gPkgSize == 0)
                throw new ArgumentOutOfRangeException(nameof(gPkgSize));
            if (pkg == null)
                throw new ArgumentNullException(nameof(pkg));
            if (!(pkg is IMessage))
                throw new ArgumentOutOfRangeException(nameof(pkg));
            var seg = (IMessage)pkg;
            var pbSize = seg.CalculateSize();
            var pkgSize = pkgHead.PkgHeadSize + gPkgCodeSize + pbSize;
            if (pkgSize > gPkgSize)
                throw new ArgumentOutOfRangeException(nameof(pkg));
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

            var pkgSeg = cache.AllocSendBuf(pkgSize);
            {
                var pkgCode = _code.GetCode(pkg.GetType());
                var codeSeg = BitConverter.GetBytes(pkgCode);
                pkgSeg[pkgHead.PkgHeadSize + 0] = codeSeg[0];
                pkgSeg[pkgHead.PkgHeadSize + 1] = codeSeg[1];
                pkgSeg[pkgHead.PkgHeadSize + 2] = codeSeg[2];
                pkgSeg[pkgHead.PkgHeadSize + 3] = codeSeg[3];
            }
            _stream.SetBuf(pkgSeg, pkgHead.PkgHeadSize + gPkgCodeSize, pbSize);
            seg.WriteTo(_output);
            _output.Flush();
            pkgHead.SendPkgSize = gPkgCodeSize + pbSize;
            pkgHead.SendPkgType = (int)eTcpClientPkgType.Protobuf;
            return new ArraySegment<byte>(pkgSeg, 0, pkgSize);
        }

        public object UnFill(ArraySegment<byte> pkgSeg, ITcpClientCache cache)
        {
            if (pkgSeg == null)
                throw new ArgumentNullException(nameof(pkgSeg));
            if (pkgSeg.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(pkgSeg));
            if (pkgSeg.Count < gPkgCodeSize)
                throw new ArgumentOutOfRangeException(nameof(pkgSeg));
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

            var pkgCode = BitConverter.ToUInt32(pkgSeg.Array, pkgSeg.Offset);
            var pkgParser = _code.GetParser(pkgCode);
            _stream.SetBuf(pkgSeg.Array, pkgSeg.Offset + gPkgCodeSize, pkgSeg.Count - gPkgCodeSize);
            var pkg = pkgParser.ParseFrom(_input);

            return pkg;
        }
    }

    #endregion

    #region 

    public class CTcpClientPkgFill : ITcpClientPkgFill
    {
        List<Tuple<Type, int, ITcpClientPkg>> _pkgs;

        public CTcpClientPkgFill()
        {
            _pkgs = new List<Tuple<Type, int, ITcpClientPkg>>();
        }

        public void Register(Type pkgType, int ePkgType, ITcpClientPkg pkg)
        {
            if (pkgType == null)
                throw new ArgumentNullException(nameof(pkgType));
            if (ePkgType == 0)
                throw new ArgumentOutOfRangeException(nameof(ePkgType));
            if (pkg == null)
                throw new ArgumentNullException(nameof(pkg));

            _pkgs.Add(Tuple.Create(pkgType, ePkgType, pkg));
        }

        public ArraySegment<byte> Fill(ITcpClientPkgHead pkgHead, int gPkgSize, object pkg, ITcpClientCache cache)
        {
            if (pkgHead == null)
                throw new ArgumentNullException(nameof(pkgHead));
            if (pkgHead.PkgHeadSize == 0)
                throw new ArgumentOutOfRangeException(nameof(pkgHead));
            if (gPkgSize == 0)
                throw new ArgumentOutOfRangeException(nameof(gPkgSize));
            if (pkg == null)
                throw new ArgumentNullException(nameof(pkg));
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

            var pkgType = pkg.GetType();
            foreach (var i in _pkgs)
            {
                if (i.Item1 == pkgType
                    || pkgType.GetInterface(i.Item1.FullName) != null
                    || i.Item1.IsInstanceOfType(pkg))
                    return i.Item3.Fill(pkgHead, gPkgSize, pkg, cache);
            }

            throw new ArgumentOutOfRangeException(nameof(pkg));
        }

        public object UnFill(int pkgType, ArraySegment<byte> pkgSeg, ITcpClientCache cache)
        {
            if (pkgType == 0)
                throw new ArgumentOutOfRangeException(nameof(pkgType));
            if (pkgSeg == null)
                throw new ArgumentNullException(nameof(pkgSeg));
            if (pkgSeg.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(pkgSeg));
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

            foreach (var i in _pkgs)
            {
                if (i.Item2 == pkgType)
                    return i.Item3.UnFill(pkgSeg, cache);
            }

            throw new ArgumentOutOfRangeException(nameof(pkgType));
        }
    }

    #endregion
}
