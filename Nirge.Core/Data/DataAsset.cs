/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reflection;
using Google.Protobuf;
using OfficeOpenXml;
using System.Linq;
using System.Text;
using System.IO;
using System;
using log4net;

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
        Dictionary<int, T> _datas;

        public CDataAsset(ILog log)
        {
            _log = log;
            _datas = new Dictionary<int, T>(128);
        }

        public bool Load(ExcelWorksheet sheet)
        {
            if (sheet == null)
                return false;

            if (sheet.Dimension.Rows < (int)eDataRow.Pre)
            {
                return false;
            }

            var rows = sheet.Dimension.Rows - (int)eDataRow.Pre;
            if (rows == 0)
                return true;

            var cols = new List<CDataColumn>(sheet.Dimension.Columns);
            for (int i = 1; i <= sheet.Dimension.Columns; ++i)
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
                    continue;
                }

                cols.Add(new CDataColumn(i, name));
            }

            return true;
        }
    }
}
