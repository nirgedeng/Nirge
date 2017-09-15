/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using Google.Protobuf.Reflection;
using Google.Protobuf;
using OfficeOpenXml;
using System.Linq;
using log4net;
using System;

namespace Nirge.Core
{
    public class CData
    {
        protected int _uid;

        public int Uid
        {
            get => _uid;
        }

        public virtual string Uname
        {
            get => "";
        }
    }

    public class CDataAsset<T> where T : CData, IMessage, new()
    {
        enum eDataRow
        {
            Name = 1,
            Pre = 1,
            Data = 2,
        }

        class CXlsCol
        {
            int _col;
            string _name;

            public int Col
            {
                get
                {
                    return _col;
                }
            }

            public string Name
            {
                get
                {
                    return _name;
                }
            }

            public CXlsCol(int col, string name)
            {
                _col = col;
                _name = name;
            }
        }

        class CPrimitiveCol
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

        ILog _log;
        MessageDescriptor _descriptor;
        Dictionary<int, T> _datas;

        public CDataAsset(ILog log, MessageDescriptor descriptor)
        {
            _log = log;
            _descriptor = descriptor;
            _datas = new Dictionary<int, T>(128);
        }

        public bool Load(ExcelWorksheet sheet)
        {
            if (sheet == null)
                return false;

            if (sheet.Dimension.Rows < (int)eDataRow.Pre)
            {
                _log.ErrorFormat("[Data]CDataAsset.Load !rows, cls:\"{0}\", xls:\"{1}\", rows:\"{2}\", cols:\"{3}\""
                    , _descriptor.Name
                    , sheet.Name
                    , sheet.Dimension.Rows
                    , sheet.Dimension.Columns);
                return false;
            }

            var rows = sheet.Dimension.Rows - (int)eDataRow.Pre;
            if (rows == 0)
                return true;

            var clsCols = _descriptor.Fields.InFieldNumberOrder();
            var xlsCols = new List<CXlsCol>();
            for (int i = 1, len = sheet.Dimension.Columns; i <= len; ++i)
            {
                var name = "";

                try
                {
                    name = sheet.Cells[(int)eDataRow.Name, i].GetValue<string>();
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

                xlsCols.Add(new CXlsCol(i, name));
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

            var xlsPrimitiveCols = new Dictionary<int, CPrimitiveCol>();
            foreach (var i in clsPrimitiveCols)
            {
                var p = xlsCols.FindIndex(e => string.Compare(e.Name, i.Name) == 0);
                if (p == -1)
                    continue;
                xlsPrimitiveCols.Add(i.FieldNumber, new CPrimitiveCol(i, xlsCols[p]));
                xlsCols.RemoveAt(p);
            }

            return true;
        }
    }
}
