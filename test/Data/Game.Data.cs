// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: game.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using log4net;
using OfficeOpenXml;
using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Nirge.Core
{
    public partial class a : CData
    {
        public override int CombineUid()
        {
            return F << 0 | G << 16;
        }

        public override string Uname => P;

        public override void ReadPrimitive(ILog log, pbr.MessageDescriptor descriptor, ExcelWorksheet sheet, int row, scg.Dictionary<int, CPrimitiveCol> cols)
        {
            F = ReadPrimitive<int>(log, descriptor, sheet, row, 1, cols);
            G = ReadPrimitive<int>(log, descriptor, sheet, row, 2, cols);
            H = ReadPrimitive<long>(log, descriptor, sheet, row, 3, cols);
            P = ReadPrimitive<string>(log, descriptor, sheet, row, 4, cols);
        }
    }

}

#endregion Designer generated code
