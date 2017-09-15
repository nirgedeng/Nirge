/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using Google.Protobuf.Collections;
using System.Collections.Generic;
using Google.Protobuf.Reflection;
using System.Collections;
using Google.Protobuf;
using OfficeOpenXml;
using System.Linq;
using log4net;
using System;

namespace Nirge.Core
{
    public abstract class CData
    {
        #region 

        public enum eXlsRow
        {
            Name = 1,
            Pre = 1,
            Data = 2,
        }

        public class CXlsCol
        {
            int _col;
            string _name;

            public int Col
            {
                get => _col;
            }

            public string Name
            {
                get => _name;
            }

            public CXlsCol(int col, string name)
            {
                _col = col;
                _name = name;
            }
        }

        public class CPrimitiveCol
        {
            FieldDescriptor _cls;
            CXlsCol _xls;

            public FieldDescriptor Cls
            {
                get => _cls;
            }

            public CXlsCol Xls
            {
                get => _xls;
            }

            public CPrimitiveCol(FieldDescriptor cls, CXlsCol xls)
            {
                _cls = cls;
                _xls = xls;
            }
        }

        public class CPrimitivesCol
        {
            FieldDescriptor _cls;
            IList<CXlsCol> _xls;

            public FieldDescriptor Cls
            {
                get => _cls;
            }

            public IList<CXlsCol> Xls
            {
                get => _xls;
            }

            public CPrimitivesCol(FieldDescriptor cls, IList<CXlsCol> xls)
            {
                _cls = cls;
                _xls = xls;
            }
        }

        #endregion

        int _uid;

        public int Uid
        {
            get => _uid;
            internal set => _uid = value;
        }

        public virtual string Uname
        {
            get => "";
        }

        internal protected abstract int CombineUid();

        T Read<T>(ILog log, MessageDescriptor descriptor, ExcelWorksheet sheet, int row, FieldDescriptor clsCol, CXlsCol xlsCol)
        {
            try
            {
                return sheet.Cells[row, xlsCol.Col].GetValue<T>();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("[Data]CData.Read !Cell, cls:\"{0}\", xls:\"{1}\", row:\"{2}\", col:\"{3},{4},{5},{6}\""
                    , descriptor.Name
                    , sheet.Name
                    , row
                    , clsCol.Name
                    , clsCol.FieldNumber
                    , xlsCol.Name
                    , xlsCol.Col), exception);
                return default(T);
            }
        }

        protected T ReadPrimitive<T>(ILog log, MessageDescriptor descriptor, ExcelWorksheet sheet, int row, int col, Dictionary<int, CPrimitiveCol> cols)
        {
            if (cols.TryGetValue(col, out var e))
                return Read<T>(log, descriptor, sheet, row, e.Cls, e.Xls);
            else
                return default(T);
        }

        internal protected virtual void ReadPrimitive(ILog log, MessageDescriptor descriptor, ExcelWorksheet sheet, int row, Dictionary<int, CPrimitiveCol> cols)
        {
        }

        protected void ReadPrimitives<T>(ILog log, MessageDescriptor descriptor, ExcelWorksheet sheet, int row, int col, Dictionary<int, CPrimitivesCol> cols, RepeatedField<T> primitives)
        {
            if (cols.TryGetValue(col, out var e))
            {
                foreach (var i in e.Xls)
                {
                    if (i == null)
                        primitives.Add(default(T));
                    else
                        primitives.Add(Read<T>(log, descriptor, sheet, row, e.Cls, i));
                }
            }
        }

        internal protected virtual void ReadPrimitives(ILog log, MessageDescriptor descriptor, ExcelWorksheet sheet, int row, Dictionary<int, CPrimitivesCol> cols)
        {
        }
    }

    public class CDataAsset<T> : IEnumerable<T>, IEnumerable where T : CData, IMessage, new()
    {
        ILog _log;
        MessageDescriptor _descriptor;
        Dictionary<int, T> _vals;

        public int Count
        {
            get => _vals.Count;
        }

        public CDataAsset(ILog log, MessageDescriptor descriptor)
        {
            _log = log;
            _descriptor = descriptor;
            _vals = new Dictionary<int, T>(128);
        }

        public T Get(int id)
        {
            _vals.TryGetValue(id, out var e);
            return e;
        }

        public bool Load(ExcelWorksheet sheet)
        {
            if (sheet == null)
                return false;

            if (sheet.Dimension.Rows < (int)CData.eXlsRow.Pre)
            {
                _log.ErrorFormat("[Data]CDataAsset.Load !Rows, cls:\"{0}\", xls:\"{1}\", rows:\"{2}\", cols:\"{3}\""
                    , _descriptor.Name
                    , sheet.Name
                    , sheet.Dimension.Rows
                    , sheet.Dimension.Columns);
                return false;
            }

            var rows = sheet.Dimension.Rows - (int)CData.eXlsRow.Pre;
            if (rows == 0)
                return true;

            var clsCols = _descriptor.Fields.InFieldNumberOrder();
            var xlsCols = new List<CData.CXlsCol>();
            for (int i = 1, len = sheet.Dimension.Columns; i <= len; ++i)
            {
                var name = "";

                try
                {
                    name = sheet.Cells[(int)CData.eXlsRow.Name, i].GetValue<string>();
                }
                catch
                {
                }

                if (string.IsNullOrEmpty(name))
                {
                    _log.ErrorFormat("[Data]CDataAsset.Load !Name, cls:\"{0}\", xls:\"{1}\", col:\"{2}\""
                        , _descriptor.Name
                        , sheet.Name
                        , i);
                    continue;
                }

                xlsCols.Add(new CData.CXlsCol(i, name));
            }

            if (xlsCols.Count < 1)
            {
                _log.ErrorFormat("[Data]CDataAsset.Load !Cols, cls:\"{0}\", xls:\"{1}\", cols:\"{2}\""
                    , _descriptor.Name
                    , sheet.Name
                    , xlsCols.Count);
                return false;
            }

            var clsPrimitiveCols = new List<FieldDescriptor>();
            var clsUidCols = new List<Tuple<FieldDescriptor, DataIdOption>>();
            var clsPrimitivesCols = new List<FieldDescriptor>();
            var clsObjCols = new List<FieldDescriptor>();
            var clsObjsCols = new List<FieldDescriptor>();
            foreach (var i in clsCols)
            {
                if (i.IsMap)
                    continue;
                switch (i.FieldType)
                {
                case FieldType.Bool:
                case FieldType.Int32:
                case FieldType.Float:
                case FieldType.Int64:
                case FieldType.String:
                    if (i.IsRepeated)
                        clsPrimitivesCols.Add(i);
                    else
                    {
                        clsPrimitiveCols.Add(i);
                        if (i.CustomOptions.TryGetMessage<DataIdOption>(60102, out var e))
                            clsUidCols.Add(Tuple.Create(i, e));
                    }
                    break;
                case FieldType.Message:
                    if (i.IsRepeated)
                        clsObjsCols.Add(i);
                    else
                        clsObjCols.Add(i);
                    break;
                }
            }

            var uidBits = 0;
            foreach (var i in clsUidCols)
            {
                if (i.Item2.Bits < 0)
                {
                    _log.ErrorFormat("[Data]CDataAsset.Load !Uid, cls:\"{0}\", xls:\"{1}\", bits:\"{2},{3},{4}\""
                        , _descriptor.Name
                        , sheet.Name
                        , i.Item1.Name
                        , i.Item2.Order
                        , i.Item2.Bits);
                    return false;
                }
                uidBits += i.Item2.Bits;
            }
            if (uidBits != 32)
            {
                _log.ErrorFormat("[Data]CDataAsset.Load !Uid, cls:\"{0}\", xls:\"{1}\", bits:\"{2}\""
                    , _descriptor.Name
                    , sheet.Name
                    , uidBits);
                return false;
            }

            var xlsPrimitiveCols = new Dictionary<int, CData.CPrimitiveCol>();
            foreach (var i in clsPrimitiveCols)
            {
                var p = xlsCols.FindIndex(e => string.Compare(e.Name, i.Name) == 0);
                if (p == -1)
                    continue;
                xlsPrimitiveCols.Add(i.FieldNumber, new CData.CPrimitiveCol(i, xlsCols[p]));
                xlsCols.RemoveAt(p);
            }

            var xlsPrimitivesCols = new Dictionary<int, CData.CPrimitivesCol>();
            foreach (var i in clsPrimitivesCols)
            {
                var x = new List<Tuple<int, CData.CXlsCol>>();
                for (var j = xlsCols.Count - 1; j >= 0; --j)
                {
                    var xlsCol = xlsCols[j];
                    var pre = $"{i.Name}.";
                    if (xlsCol.Name.StartsWith(pre))
                    {
                        if (int.TryParse(xlsCol.Name.Substring(pre.Length), out var slot))
                        {
                            x.Add(Tuple.Create(slot, xlsCol));
                            xlsCols.RemoveAt(j);
                        }
                        else
                        {
                            _log.ErrorFormat("[Data]CDataAsset.Load !Col, cls:\"{0}\", xls:\"{1}\", col:\"{2},{3},{4},{5}\""
                                , _descriptor.Name
                                , sheet.Name
                                , i.Name
                                , i.FieldNumber
                                , xlsCol.Name
                                , xlsCol.Col);
                        }
                    }
                }
                var y = new List<CData.CXlsCol>();
                var p = 1;
                foreach (var j in x.OrderBy(e => e.Item1))
                {
                    for (int q = p, len = j.Item1; q < len; ++q)
                        y.Add(null);
                    y.Add(j.Item2);
                    p = j.Item1 + 1;
                }
                xlsPrimitivesCols.Add(i.FieldNumber, new CData.CPrimitivesCol(i, y));
            }

            for (int i = (int)CData.eXlsRow.Data, len = (int)CData.eXlsRow.Data + rows; i < len; ++i)
            {
                var e = new T();
                e.ReadPrimitive(_log, _descriptor, sheet, i, xlsPrimitiveCols);
                e.Uid = e.CombineUid();
                if (_vals.ContainsKey(e.Uid))
                {
                    _log.ErrorFormat("[Data]CDataAsset.Load !Uid, cls:\"{0}\", xls:\"{1}\", row:\"{2}\", uid:\"{3}\""
                        , _descriptor.Name
                        , sheet.Name
                        , i
                        , e.Uid);
                    continue;
                }
                e.ReadPrimitives(_log, _descriptor, sheet, i, xlsPrimitivesCols);
                _vals.Add(e.Uid, e);
            }

            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _vals.Values.GetEnumerator();
        }
    }
}
