using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Collections.ObjectModel;
using ExcelConverter.Properties;

namespace ExcelConverter
{
    internal class ExcelParser
    {
        private readonly Encoding xmlEncoding = Encoding.UTF8;
        private const int pointsCount = 57;
        Dictionary<byte, TeleportPoint> leftPoints = new Dictionary<byte, TeleportPoint>(pointsCount);
        Dictionary<byte, TeleportPoint> topPoints = new Dictionary<byte, TeleportPoint>(pointsCount);

        /// <summary>
        /// ID
        /// </summary>
        private const string idColumn = "K";
        private const uint idColumnStart = 12;
        private const uint idRow = 10;
        private const string idRowStart = "M";

        /// <summary>
        /// Name PW-RU
        /// </summary>
        private const string ruNameColumn = "L";
        private const uint ruNameColumnStart = 12;
        private const uint ruNameRow = 11;
        private const string ruNameRowStart = "M";

        /// <summary>
        /// Coordinates
        /// </summary>
        private const string coordsColumn = "G";
        private const uint coordsColumnStart = 12;
        private const uint coordsRow = 6;
        private const string coordsRowStart = "M";

        /// <summary>
        /// Name PW-PH
        /// </summary>
        private const string phNameColumn = "H";
        private const uint phNameColumnStart = 12;
        private const uint phNameRow = 7;
        private const string phNameRowStart = "M";

        /// <summary>
        /// Name PW-MS/PW-MY
        /// </summary>
        private const string msMyNameColumn = "I";
        private const uint msMyNameStart = 12;
        private const uint msMyNameRow = 8;
        private const string msMyNameRowStart = "M";

        /// <summary>
        /// Name PWI
        /// </summary>
        private const string intNameColumn = "J";
        private const uint intNameStart = 12;
        private const uint intNameRow = 8;
        private const string intNameRowStart = "M";


        internal string GetNextColumnName(string columnName)
        {
            if (columnName.Length == 1)
                if (columnName.ToUpper().CompareTo("Z") < 0)
                {
                    int code = columnName[0];
                    return ((char)++code).ToString();
                }
                else
                    return "AA";
            else
            {
                char firstLetter = Char.ToUpper(columnName[0]);
                char secondLetter = Char.ToUpper(columnName[1]);

                if (secondLetter < 'Z')
                {
                    int nextCode = (int)secondLetter + 1;
                    return firstLetter.ToString() + ((char)nextCode).ToString();
                }
                else
                {
                    int firstNextCode = (int)firstLetter + 1;
                    if ((char)firstNextCode <= 'Z')
                        return ((char)firstNextCode).ToString() + "A";
                    else
                        throw new IndexOutOfRangeException("Cannot increment 'ZZ'");
                }
            }
        }

        internal string GetPreviousColumnName(string columnName)
        {
            if (columnName.Length == 1)
                if (columnName.ToUpper().CompareTo("A") > 0)
                {
                    int code = columnName[0];
                    return ((char)--code).ToString();
                }
                else
                    throw new IndexOutOfRangeException("Cannot decrement 'A'");
            else
            {
                char firstLetter = Char.ToUpper(columnName[0]);
                char secondLetter = Char.ToUpper(columnName[1]);

                if (columnName.ToUpper() == "AA")
                    return "Z";

                if (secondLetter > 'A')
                {
                    int previousCode = (int)secondLetter - 1;
                    return firstLetter.ToString() + ((char)previousCode).ToString();
                }
                else
                {
                    int firstPreviousCode = (int)firstLetter - 1;
                    return ((char)firstPreviousCode).ToString() + "Z";
                }
            }
        }

        internal void GetIDs(_Worksheet worksheet)
        {
            uint currentRow = idColumnStart;
            string cellName = idColumn + currentRow.ToString();
            Range range = null;
            byte id;
            int x;
            int y;

            for (int i = 0; i < pointsCount; i++)
            {
                range = worksheet.get_Range(cellName, Missing.Value);

                if (range == null)
                    Console.WriteLine("ERROR: range == null, Cell = {0}", cellName);

                id = Convert.ToByte(range.Value2);

                KeyValuePair<int, int> coords = GetCoords(worksheet, currentRow);
                x = coords.Key;
                y = coords.Value;
                string ruName = GetRuName(worksheet, currentRow);
                string phName = GetPhName(worksheet, currentRow);
                string intName = GetIntName(worksheet, currentRow);
                string msMyName = GetMsMyName(worksheet, currentRow);

                if (!leftPoints.ContainsKey(id))
                {
                    TeleportPoint point = new TeleportPoint();
                    point.ID = id;
                    point.X = x;
                    point.Y = y;
                    point.SetName(new System.Globalization.CultureInfo("ru"), ruName);
                    point.SetName(new System.Globalization.CultureInfo("en-PH"), phName);
                    point.SetName(new System.Globalization.CultureInfo(""), intName);
                    point.SetName(new System.Globalization.CultureInfo("ms"), msMyName);
                    point.TableRowPos = currentRow;
                    point.TableColumnPos = idColumn;
                    leftPoints.Add(id, point);
                }
                else
                {
                    TeleportPoint point = leftPoints[id];
                    point.ID = id;
                    point.X = x;
                    point.Y = y;
                    point.SetName(new System.Globalization.CultureInfo("ru"), ruName);
                    point.SetName(new System.Globalization.CultureInfo("en-PH"), phName);
                    point.SetName(new System.Globalization.CultureInfo(""), intName);
                    point.SetName(new System.Globalization.CultureInfo("ms"), msMyName);
                    point.TableRowPos = currentRow;
                    point.TableColumnPos = idColumn;
                    leftPoints[id] = point;
                }

                currentRow++;
                cellName = idColumn + currentRow.ToString();
            }
        }

        internal void GetHorizontalIDs(_Worksheet worksheet)
        {
            string currentColumn = idRowStart;
            string cellName = currentColumn + idRow.ToString();
            Range range = null;
            byte id;

            for (int i = 0; i < pointsCount; i++)
            {
                range = worksheet.get_Range(cellName, Missing.Value);

                if (range == null)
                    Console.WriteLine("ERROR: range == null, Cell = {0}", cellName);

                id = Convert.ToByte(range.Value2);

                TeleportPoint point = leftPoints[id].Clone();
                point.TableColumnPos = currentColumn;
                point.TableRowPos = idRow;

                if (!topPoints.ContainsKey(id))
                {
                    topPoints.Add(id, point);
                }
                else
                {
                    topPoints[id] = point;
                }

                currentColumn = GetNextColumnName(currentColumn);
                cellName = currentColumn + idRow.ToString();
            }
        }

        private string GetRuName(_Worksheet worksheet, uint currentRow)
        {
            string cellName = ruNameColumn + currentRow.ToString();

            return GetPointName(worksheet, cellName);
        }

        private static string GetPointName(_Worksheet worksheet, string cellName)
        {
            Range range = worksheet.get_Range(cellName, Missing.Value);

            if (range == null)
                Console.WriteLine("ERROR: range == null, Cell = {0}", cellName);

            return range.Value2.ToString();
        }

        private string GetPhName(_Worksheet worksheet, uint currentRow)
        {
            string cellName = phNameColumn + currentRow.ToString();

            return GetPointName(worksheet, cellName);
        }

        private string GetMsMyName(_Worksheet worksheet, uint currentRow)
        {
            string cellName = msMyNameColumn + currentRow.ToString();

            return GetPointName(worksheet, cellName);
        }

        private string GetIntName(_Worksheet worksheet, uint currentRow)
        {
            string cellName = intNameColumn + currentRow.ToString();

            return GetPointName(worksheet, cellName);
        }

        private KeyValuePair<int, int> GetCoords(_Worksheet worksheet, uint currentRow)
        {
            string cellName = coordsColumn + currentRow.ToString();

            Range range = worksheet.get_Range(cellName, Missing.Value);

            if (range == null)
                Console.WriteLine("ERROR: range == null, Cell = {0}", cellName);

            string coord = range.Value2.ToString();
            string[] coords = coord.Split(new char[] { ' ' });
            return new KeyValuePair<int, int>(Convert.ToInt32(coords[0]), Convert.ToInt32(coords[1]));
        }

        internal void MakeCostMatrix(_Worksheet worksheet)
        {
            Range range = null;
            string cellName = null;
            uint cost = 0;

            foreach (TeleportPoint leftPoint in leftPoints.Values)
            {
                foreach (TeleportPoint  topPoint in topPoints.Values)
                {
                    cellName = topPoint.TableColumnPos + leftPoint.TableRowPos.ToString();

                    range = worksheet.get_Range(cellName, Missing.Value);

                    if (range == null)
                        Console.WriteLine("ERROR: range == null, Cell = {0}", cellName);

                    if (range.Value2 != null)
                    {
                        cost = Convert.ToUInt32(range.Value2);
                        leftPoint.SetCost(topPoint.ID, cost);
                    }
                }
            }
        }

        internal void WriteCostsXml(string xmlCostsPath)
        {
            XmlTextWriter writer = null;
            try
            {
                writer = new XmlTextWriter(xmlCostsPath, xmlEncoding);
                writer.Formatting = Formatting.Indented;

                writer.WriteStartDocument();
                writer.WriteStartElement("Teleports");
                writer.WriteAttributeString("xmlns", Settings.Default.TeleportSchemaXmlns);
                writer.WriteAttributeString("Version", "1.0");

                foreach (TeleportPoint point in leftPoints.Values)
                {
                    writer.WriteStartElement("TeleportPoint");
                    writer.WriteElementString("ID", point.ID.ToString());
                    writer.WriteElementString("X", point.X.ToString());
                    writer.WriteElementString("Y", point.Y.ToString());
                    writer.WriteElementString("RelativeX", "0");
                    writer.WriteElementString("RelativeY", "0");
                    
                    foreach (KeyValuePair <ushort, uint> cost in point.Costs)
                    {
                        writer.WriteStartElement("TeleportCost");
                        writer.WriteElementString("ID", cost.Key.ToString());
                        writer.WriteElementString("Cost", cost.Value.ToString());
                        writer.WriteEndElement();     
                    }
                    
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        internal void WriteTranlationsXml(string xmlTranslationsPath)
        {
            if (!Directory.Exists(xmlTranslationsPath))
                Directory.CreateDirectory(xmlTranslationsPath);

            uint namesCount = leftPoints[0].NamesCount;
            ReadOnlyCollection<KeyValuePair<CultureInfo, string>> names0 = leftPoints[0].Names;

            foreach (KeyValuePair<CultureInfo, string> kvp in names0)
            {
                CultureInfo currentCulture = kvp.Key;

                string cultureName = null;

                if (currentCulture.Name == "")
                    cultureName = "International";
                else
                    cultureName = currentCulture.Name;

                cultureName += ".xml";

                string filePath = Path.Combine (xmlTranslationsPath, cultureName);
                XmlTextWriter writer = null;

                try
                {
                    writer = new XmlTextWriter(filePath, xmlEncoding);
                    writer.Formatting = Formatting.Indented;

                    writer.WriteStartDocument();
                    writer.WriteStartElement("Translations");
                    writer.WriteAttributeString("xmlns", Settings.Default.TranslationSchemaXmlns);
                    writer.WriteAttributeString("Version", "1.0");
                    writer.WriteAttributeString("Culture", currentCulture.Name);

                    foreach (TeleportPoint point in leftPoints.Values)
                    {
                        writer.WriteStartElement("Translation");
                        writer.WriteElementString("ID", point.ID.ToString());
                        writer.WriteElementString("Name", point.GetName(currentCulture));
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }
                finally
                {
                    if (writer != null)
                        writer.Close();
                }
            }
        }
    }
}
