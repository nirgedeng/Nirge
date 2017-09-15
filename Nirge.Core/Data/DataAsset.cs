﻿/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

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

        public enum eRow
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

        #endregion

        protected int _uid;

        public int Uid
        {
            get => _uid;
            internal set => _uid = value;
        }

        public virtual string Uname
        {
            get => "";
        }

        public abstract int CombineUid();

        T Read<T>(ILog log, MessageDescriptor descriptor, ExcelWorksheet sheet, int row, CPrimitiveCol col)
        {
            try
            {
                return sheet.Cells[row, col.Xls.Col].GetValue<T>();
            }
            catch (Exception exception)
            {
                log.Error(string.Format("[Data]CData.Read !, cls:\"{0}\", xls:\"{1}\", row:\"{2}\", col:\"{3},{4},{5}\""
                    , descriptor.Name
                    , sheet.Name
                    , row
                    , col.Cls.Name
                    , col.Xls.Col
                    , col.Xls.Name), exception);
                return default(T);
            }
        }

        protected T ReadPrimitive<T>(ILog log, MessageDescriptor descriptor, ExcelWorksheet sheet, int row, int col, Dictionary<int, CPrimitiveCol> cols)
        {
            if (cols.TryGetValue(col, out var e))
                return Read<T>(log, descriptor, sheet, row, e);
            else
                return default(T);
        }

        public virtual void ReadPrimitive(ILog log, MessageDescriptor descriptor, ExcelWorksheet sheet, int row, Dictionary<int, CPrimitiveCol> cols)
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

            if (sheet.Dimension.Rows < (int)CData.eRow.Pre)
            {
                _log.ErrorFormat("[Data]CDataAsset.Load !rows, cls:\"{0}\", xls:\"{1}\", rows:\"{2}\", cols:\"{3}\""
                    , _descriptor.Name
                    , sheet.Name
                    , sheet.Dimension.Rows
                    , sheet.Dimension.Columns);
                return false;
            }

            var rows = sheet.Dimension.Rows - (int)CData.eRow.Pre;
            if (rows == 0)
                return true;

            var clsCols = _descriptor.Fields.InFieldNumberOrder();
            var xlsCols = new List<CData.CXlsCol>();
            for (int i = 1, len = sheet.Dimension.Columns; i <= len; ++i)
            {
                var name = "";

                try
                {
                    name = sheet.Cells[(int)CData.eRow.Name, i].GetValue<string>();
                }
                catch
                {
                }

                if (string.IsNullOrEmpty(name))
                {
                    _log.ErrorFormat("[Data]CDataAsset.Load !name, cls:\"{0}\", xls:\"{1}\", col:\"{2}\""
                        , _descriptor.Name
                        , sheet.Name
                        , i);
                    continue;
                }

                xlsCols.Add(new CData.CXlsCol(i, name));
            }

            if (xlsCols.Count < 1)
            {
                _log.ErrorFormat("[Data]CDataAsset.Load !cols, cls:\"{0}\", xls:\"{1}\", cols:\"{2}\""
                    , _descriptor.Name
                    , sheet.Name
                    , xlsCols.Count);
                return false;
            }

            var clsPrimitiveCols = new List<FieldDescriptor>();
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
                        clsPrimitiveCols.Add(i);
                    break;
                case FieldType.Message:
                    if (i.IsRepeated)
                        clsObjsCols.Add(i);
                    else
                        clsObjCols.Add(i);
                    break;
                }
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

            for (int i = (int)CData.eRow.Data, len = (int)CData.eRow.Data + rows; i < len; ++i)
            {
                var e = new T();
                e.ReadPrimitive(_log, _descriptor, sheet, i, xlsPrimitiveCols);
                e.Uid = e.CombineUid();
                if (_vals.ContainsKey(e.Uid))
                {
                    _log.ErrorFormat("[Data]CDataAsset.Load !row, cls:\"{0}\", xls:\"{1}\", row:\"{2}\", id:\"{3}\""
                        , _descriptor.Name
                        , sheet.Name
                        , i
                        , e.Uid);
                    continue;
                }
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
