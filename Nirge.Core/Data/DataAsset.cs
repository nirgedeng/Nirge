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
        int _id;
        string _name;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
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

        class CDataColumn
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

            public CDataColumn(int col, string name)
            {
                _col = col;
                _name = name;
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
                _log.ErrorFormat("[Data]CDataAsset.Load err, rows:\"{0}\", cols:\"{0}\"", sheet.Dimension.Rows, sheet.Dimension.Columns);
                return false;
            }

            var rows = sheet.Dimension.Rows - (int)eDataRow.Pre;
            if (rows == 0)
                return true;

            var cols = new List<CDataColumn>(sheet.Dimension.Columns);
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
                    _log.ErrorFormat("[Data]CDataAsset.Load !name, col:\"{0}\"", i);
                    continue;
                }

                cols.Add(new CDataColumn(i, name));
            }

            return true;
        }
    }
}
