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

            var a = cols.Select(x =>
            {
                var v = x.Name.IndexOf('.');
                if (v == -1)
                    return x.Name;
                else
                {
                    return x.Name.Substring(0, x.Name.Length - v);
                }
            }).Distinct().ToArray();

            if (!CheckFields(a, _descriptor.Fields.InFieldNumberOrder()))
            {
                return false;
            }

            return true;
        }

        bool CheckFields(string[] cols, IList<FieldDescriptor> fields)
        {
            foreach (var i in fields.Where(x => cols.FirstOrDefault(y => string.Compare(y, x.Name, true) == 0) != null))
            {
                if (i.IsMap)
                    return false;

                switch (i.FieldType)
                {
                case FieldType.Message:
                    if (!CheckFields(cols, i.MessageType.Fields.InFieldNumberOrder()))
                        return false;
                    break;
                case FieldType.Bool:
                case FieldType.Int32:
                case FieldType.Float:
                case FieldType.Int64:
                case FieldType.String:
                    break;
                default:
                    return false;
                }
            }
            return true;
        }
    }
}
