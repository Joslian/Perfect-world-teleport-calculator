using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection; // For Missing.Value and BindingFlags
using System.Runtime.InteropServices; // For COMException
using Microsoft.Office.Interop.Excel;
using System.IO;
using ExcelConverter.Properties;

namespace ExcelConverter
{
    class Program
    {
        private static string filePath = @"E:\Games\Perfect World\Teleport calculator\Teleport.xls";

        static void Main(string[] args)
        {
            Console.WriteLine("Creating new Excel.Application");
            Application app = new Application();
            if (app == null)
            {
                Console.WriteLine("ERROR: EXCEL couldn't be started!");
            }

            Console.WriteLine("Making application visible");
            app.Visible = true;

            Console.WriteLine("Getting the workbooks collection");
            Workbooks workbooks = app.Workbooks;

            Console.WriteLine("Openning " + @"E:\Games\Perfect World\Teleport calculator\Teleport.xls");
            _Workbook workbook = workbooks.Open(filePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            Console.WriteLine("Getting the worksheets collection");
            Sheets sheets = workbook.Worksheets;

            _Worksheet worksheet = (_Worksheet)sheets.get_Item(1);
            if (worksheet == null)
            {
                Console.WriteLine("ERROR: worksheet == null");
            }

            ExcelParser parser = new ExcelParser();
            Console.WriteLine("Getting IDs");
            parser.GetIDs(worksheet);

            Console.WriteLine("Getting horizontal IDs");
            parser.GetHorizontalIDs(worksheet);

            Console.WriteLine("Making horizontal cost matrix");
            parser.MakeCostMatrix(worksheet);

            Console.WriteLine("Writing cost XML");
            string xmlCostsPath = Path.Combine (System.Windows.Forms.Application.StartupPath, Settings.Default.XmlPath);
            xmlCostsPath = Path.Combine(xmlCostsPath, Settings.Default.XmlCostsName);
            parser.WriteCostsXml(xmlCostsPath);

            Console.WriteLine("Writing tranlations XML");
            string xmlTranslationsPath = Path.Combine(System.Windows.Forms.Application.StartupPath, Settings.Default.XmlPath);
            xmlTranslationsPath = Path.Combine(xmlTranslationsPath, Settings.Default.TranslationsFolder);
            parser.WriteTranlationsXml(xmlTranslationsPath);

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    }
}
