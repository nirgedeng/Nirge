/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using Google.Protobuf.Reflection;
using Google.Protobuf;
using log4net.Config;
using OfficeOpenXml;
using Nirge.Core;
using System.IO;
using log4net;
using System;

namespace Data
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure(LogManager.CreateRepository("data"), new FileInfo("../../Data.log.xml"));

            using (var xls = new ExcelPackage(File.OpenRead(@"../../a.xlsx")))
            {
                var ds = new CDataAsset<a>(LogManager.Exists("data", "all"), a.Descriptor);
                ds.Load(xls.Workbook.Worksheets[1]);
            }
        }
    }
}
