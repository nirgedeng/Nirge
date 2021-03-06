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
        public static int CombineUid(int f, int g)
        {
            return f << 0 | g << 16;
        }

        public static void ResolveUid(int uid, out int f, out int g)
        {
            f = uid << 16 >> 16;
            g = (uid & ~f) << 0 >> 16;
        }

        protected override int CombineUid()
        {
            return CombineUid(F, G);
        }

        public override string Uname => P;

        protected override void ReadPrimitive(ILog log, pbr.MessageDescriptor descriptor, ExcelWorksheet sheet, int row, scg.Dictionary<int, CPrimitiveCol> cols)
        {
            F = ReadPrimitive<int>(log, descriptor, sheet, row, 1, cols);
            G = ReadPrimitive<int>(log, descriptor, sheet, row, 2, cols);
            H = ReadPrimitive<long>(log, descriptor, sheet, row, 3, cols);
            P = ReadPrimitive<string>(log, descriptor, sheet, row, 4, cols);
        }

        protected override void ReadPrimitives(ILog log, pbr.MessageDescriptor descriptor, ExcelWorksheet sheet, int row, scg.Dictionary<int, CPrimitivesCol> cols)
        {
            ReadPrimitives<int>(log, descriptor, sheet, row, 5, cols, Q);
        }
    }

}

#endregion Designer generated code
