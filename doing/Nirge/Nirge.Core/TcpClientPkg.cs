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
    //------------------------------------------------------------------

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
                throw new ArgumentNullException("buf");
            if (buf.Length < PkgHeadSize)
                throw new ArgumentOutOfRangeException("buf");

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

    //------------------------------------------------------------------

    public class CTcpClientArraySegment : ITcpClientPkg
    {
        public ArraySegment<byte> Fill(ITcpClientPkgHead pkgHead, int gPkgSize, object pkg, ITcpClientCache cache)
        {
            if (pkgHead == null)
                throw new ArgumentNullException("pkgHead");
            if (pkgHead.PkgHeadSize == 0)
                throw new ArgumentOutOfRangeException("pkgHead");
            if (gPkgSize == 0)
                throw new ArgumentOutOfRangeException("gPkgSize");
            if (pkg == null)
                throw new ArgumentNullException("pkg");
            if (!(pkg is byte[] || pkg is ArraySegment<byte>))
                throw new ArgumentOutOfRangeException("pkg");
            var o = (ArraySegment<byte>)pkg;
            if (o.Count == 0)
                throw new ArgumentOutOfRangeException("pkg");
            if (o.Count > gPkgSize)
                throw new ArgumentOutOfRangeException("pkg");
            if (cache == null)
                throw new ArgumentNullException("cache");

            var pkgSize = pkgHead.PkgHeadSize + o.Count;
            var pkgSeg = cache.AllocSendBuf(pkgSize);
            CArrayUtils.Copy(o.Array, o.Offset, pkgSeg, pkgHead.PkgHeadSize, o.Count);
            pkgHead.SendPkgSize = o.Count;
            pkgHead.SendPkgType = (int)eTcpClientPkgType.ArraySegment;
            return new ArraySegment<byte>(pkgSeg, 0, pkgSize);
        }

        public object UnFill(ArraySegment<byte> pkgSeg, ITcpClientCache cache)
        {
            if (pkgSeg == null)
                throw new ArgumentNullException("pkgSeg");
            if (pkgSeg.Count == 0)
                throw new ArgumentOutOfRangeException("pkgSeg");
            if (cache == null)
                throw new ArgumentNullException("cache");

            return pkgSeg;
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
                throw new ArgumentNullException("assembly");
            if (_assemblys.Contains(assembly))
                throw new ArgumentOutOfRangeException("assembly");

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
                    throw new InvalidOperationException("pkgCode");
                var pkgParser = (MessageParser)i.GetProperty("Parser").GetValue(null);
                if (pkgParser == null)
                    throw new InvalidOperationException("pkgParser");

                _codes.Add(i.GetHashCode(), pkgCode);
                _parsers.Add(pkgCode, pkgParser);
            }
        }

        public uint GetCode(Type pkgType)
        {
            if (pkgType == null)
                throw new ArgumentNullException("pkgType");
            uint pkgCode;
            if (_codes.TryGetValue(pkgType.GetHashCode(), out pkgCode))
                return pkgCode;
            else
                throw new ArgumentOutOfRangeException("pkgType");
        }

        public MessageParser GetParser(uint pkgCode)
        {
            MessageParser parser;
            if (_parsers.TryGetValue(pkgCode, out parser))
                return parser;
            else
                throw new ArgumentOutOfRangeException("pkgCode");
        }
    }

    public class CTcpClientProtobuf : ITcpClientPkg
    {
        const int gPkgCodeSize = 4;

        CArrayStream _stream;
        CodedInputStream _input;
        CodedOutputStream _output;
        ITcpClientProtobufCode _code;

        public CTcpClientProtobuf(ITcpClientProtobufCode code)
        {
            if (code == null)
                throw new ArgumentNullException("code");

            _stream = new CArrayStream(0);
            _input = new CodedInputStream(_stream, true);
            _output = new CodedOutputStream(_stream, true);
            _code = code;
        }

        public ArraySegment<byte> Fill(ITcpClientPkgHead pkgHead, int gPkgSize, object pkg, ITcpClientCache cache)
        {
            if (pkgHead == null)
                throw new ArgumentNullException("pkgHead");
            if (pkgHead.PkgHeadSize == 0)
                throw new ArgumentOutOfRangeException("pkgHead");
            if (gPkgSize == 0)
                throw new ArgumentOutOfRangeException("gPkgSize");
            if (pkg == null)
                throw new ArgumentNullException("pkg");
            if (!(pkg is IMessage))
                throw new ArgumentOutOfRangeException("pkg");
            var o = (IMessage)pkg;
            if (cache == null)
                throw new ArgumentNullException("cache");

            var pbSize = o.CalculateSize();
            var pkgSize = pkgHead.PkgHeadSize + gPkgCodeSize + pbSize;
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
            o.WriteTo(_output);
            _output.Flush();
            pkgHead.SendPkgSize = gPkgCodeSize + pbSize;
            pkgHead.SendPkgType = (int)eTcpClientPkgType.Protobuf;
            return new ArraySegment<byte>(pkgSeg, 0, pkgSize);
        }

        public object UnFill(ArraySegment<byte> pkgSeg, ITcpClientCache cache)
        {
            if (pkgSeg == null)
                throw new ArgumentNullException("pkgSeg");
            if (pkgSeg.Count == 0)
                throw new ArgumentOutOfRangeException("pkgSeg");
            if (pkgSeg.Count < gPkgCodeSize)
                throw new ArgumentOutOfRangeException("pkgSeg");
            if (cache == null)
                throw new ArgumentNullException("cache");

            var pkgCode = BitConverter.ToUInt32(pkgSeg.Array, pkgSeg.Offset);
            var pkgParser = _code.GetParser(pkgCode);
            _stream.SetBuf(pkgSeg.Array, pkgSeg.Offset + gPkgCodeSize, pkgSeg.Count - gPkgCodeSize);
            var pkg = pkgParser.ParseFrom(_input);

            return pkg;
        }
    }

    //------------------------------------------------------------------

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
                throw new ArgumentNullException("pkgType");
            if (ePkgType == 0)
                throw new ArgumentOutOfRangeException("ePkgType");
            if (pkg == null)
                throw new ArgumentNullException("pkg");

            _pkgs.Add(Tuple.Create(pkgType, ePkgType, pkg));
        }

        public ArraySegment<byte> Fill(ITcpClientPkgHead pkgHead, int gPkgSize, object pkg, ITcpClientCache cache)
        {
            if (pkgHead == null)
                throw new ArgumentNullException("pkgHead");
            if (pkgHead.PkgHeadSize == 0)
                throw new ArgumentOutOfRangeException("pkgHead");
            if (gPkgSize == 0)
                throw new ArgumentOutOfRangeException("gPkgSize");
            if (pkg == null)
                throw new ArgumentNullException("pkg");
            if (cache == null)
                throw new ArgumentNullException("cache");

            foreach (var i in _pkgs)
            {
                if (i.Item1 == pkg.GetType()
                    || i.Item1.IsInstanceOfType(pkg))
                    return i.Item3.Fill(pkgHead, gPkgSize, pkg, cache);
            }

            throw new ArgumentOutOfRangeException("pkg");
        }

        public object UnFill(int pkgType, ArraySegment<byte> pkgSeg, ITcpClientCache cache)
        {
            if (pkgType == 0)
                throw new ArgumentOutOfRangeException("pkgType");
            if (pkgSeg == null)
                throw new ArgumentNullException("pkgSeg");
            if (pkgSeg.Count == 0)
                throw new ArgumentOutOfRangeException("pkgSeg");
            if (cache == null)
                throw new ArgumentNullException("cache");

            foreach (var i in _pkgs)
            {
                if (i.Item2 == pkgType)
                    return i.Item3.UnFill(pkgSeg, cache);
            }

            throw new ArgumentOutOfRangeException("pkg");
        }
    }

    //------------------------------------------------------------------
}
