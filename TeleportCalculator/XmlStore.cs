using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using TeleportCalculator.Properties;
using System.Xml.Schema;
using System.Windows.Forms;
using System.Globalization;

namespace TeleportCalculator
{
    internal class XmlStore : Singleton<XmlStore>, IDatabase
    {
        private static bool valdationOk = true;

        private XmlStore()
        {

        }

        private Dictionary<ushort, TeleportNode> CreatePoints(XmlDocument doc)
        {
            Dictionary<ushort, TeleportNode> points = new Dictionary<ushort, TeleportNode>();

            XmlNode teleportPointsNode = doc.DocumentElement;

            // Version check
            //
            if (teleportPointsNode.Attributes != null)
            {
                Console.WriteLine("Attributes");
                foreach (XmlAttribute attr in teleportPointsNode.Attributes)
                {
                    Console.WriteLine("{0}: {1}", attr.Name, attr.Value);
                    if (attr.Name == "Version")
                        if (!CheckCostsVersion(attr.Value))
                            return null;
                }
                Console.WriteLine();
            }
            else
                throw new XmlException("File version is absent");

            Console.WriteLine("{0}: {1}", teleportPointsNode.Name, teleportPointsNode.Value);


            // TeleportPoint enumeration
            //
            foreach (XmlNode teleportPointNode in teleportPointsNode.ChildNodes)
            {
                TeleportNode point = new TeleportNode();

                // Inner nodes
                //
                foreach (XmlNode teleportPointElementNode in teleportPointNode.ChildNodes)
                {
                    if (teleportPointElementNode.ChildNodes.Count == 1)
                    {
                        switch (teleportPointElementNode.Name)
                        {
                            case "ID":
                                point.ID = ushort.Parse(teleportPointElementNode.InnerText);
                                break;
                            case "X":
                                point.X = int.Parse(teleportPointElementNode.InnerText);
                                break;
                            case "Y":
                                point.Y = int.Parse(teleportPointElementNode.InnerText);
                                break;
                            case "AbsoluteX":
                                point.AbsoluteX = float.Parse(teleportPointElementNode.InnerText, CultureInfo.InvariantCulture.NumberFormat);
                                break;
                            case "AbsoluteY":
                                point.AbsoluteY = float.Parse(teleportPointElementNode.InnerText, CultureInfo.InvariantCulture.NumberFormat);
                                break;
                            default:
                                break;
                        }
                        Console.WriteLine("{0}: {1}", teleportPointElementNode.Name, teleportPointElementNode.InnerText);
                    }

                    // Processed number of subelements of TeleportCost
                    byte countedElements = 0;
                    ushort id = 0;
                    uint cost = 0;

                    // TeleportCost
                    //
                    if (teleportPointElementNode.Name == "TeleportCost")
                        foreach (XmlNode teleportCostNode in teleportPointElementNode.ChildNodes)
                        {
                            if (teleportCostNode.Name == "ID")
                            {
                                id = ushort.Parse(teleportCostNode.InnerText);
                                countedElements++;
                            }

                            if (teleportCostNode.Name == "Cost")
                            {
                                cost = uint.Parse(teleportCostNode.InnerText);
                                countedElements++;
                            }

                            if (countedElements == Settings.Default.TeleportCostsElementCount)
                            {
                                point.SetCost(id, cost);
                            }

                            Console.WriteLine("{0}: {1}", teleportCostNode.Name, teleportCostNode.InnerText);
                        }
                }

                points.Add(point.ID, point);
            }

            return points;
        }

        internal static bool CheckCostsVersion(string version)
        {
            if (version == null)
                return false;

            string[] compatibleVersions = Global.CostsCompatibility.Split(';');

            if (compatibleVersions.Contains<string>(version))
                return true;
            else
                return false;
        }

        private bool CheckTranslationVersion(string version)
        {
            if (version == null)
                return false;

            string[] compatibleVersions = Global.TranslationsCompatibility.Split(';');

            if (compatibleVersions.Contains<string>(version))
                return true;
            else
                return false;
        }

        private XmlDocument LoadPointsFile()
        {
            return ValidateXmlDoc("http://TeleportCalculator/TeleportSchema.xsd",
                                    Settings.Default.XsdSchemataFolderName + @"\TeleportSchema.xsd",
                                    Path.Combine(Application.StartupPath, Settings.Default.XmlCostsName));
        }

        internal static XmlDocument ValidateXmlDoc(string @namespace, string schemaUri, string docPath)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationEventHandler += new ValidationEventHandler(settings_ValidationEventHandler);
            settings.ValidationType = ValidationType.Schema;
            //settings.Schemas.Add("http://TeleportCalculator/TeleportSchema.xsd", "TeleportSchema.xsd");
            settings.Schemas.Add(@namespace, schemaUri);

            //string xmlCostsPath = Path.Combine(Application.StartupPath, Settings.Default.XmlCostsName);            

            XmlDocument doc = new XmlDocument();
            XmlReader reader = XmlReader.Create(docPath, settings);
            doc.Load(reader);

            valdationOk = true;
            doc.Validate(new ValidationEventHandler(settings_ValidationEventHandler), doc.DocumentElement);

            if (!valdationOk)
                return null;

            reader.Close();
            return doc;
        }

        private static void settings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            valdationOk = false;
            MessageBox.Show("File: " + e.Exception.SourceUri + Environment.NewLine +
                "Line number: " + e.Exception.LineNumber.ToString() + Environment.NewLine +
                            "Line position: " + e.Exception.LinePosition.ToString() + Environment.NewLine +
                            e.Message, "XML validation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #region IDatabase Members

        public Dictionary<ushort, TeleportNode> GetPoints()
        {
            XmlDocument doc = LoadPointsFile();
            return CreatePoints(doc);
        }

        public List<Server> GetServers()
        {
            XmlDocument doc = LoadSettingsFile();
            return CreateServers(doc);
        }

        public Dictionary<ushort, TeleportNode> LoadTranslation(Dictionary<ushort, TeleportNode> points, CultureInfo culture)
        {
            string tranlationFilePath = Path.Combine(Application.StartupPath, Settings.Default.TranslationsFolder);
            if (culture.Name == "")
            tranlationFilePath = Path.Combine(tranlationFilePath, "International.xml");
            else
                tranlationFilePath = Path.Combine(tranlationFilePath, culture.Name + ".xml");
            XmlDocument doc = LoadTranlationFile(tranlationFilePath);
            return AddTranslation(points, doc, culture);
        }

        private Dictionary<ushort, TeleportNode> AddTranslation(Dictionary<ushort, TeleportNode> points, XmlDocument doc, CultureInfo culture)
        {
            XmlNode translationsNode = doc.DocumentElement;

            // Version check
            //
            if (translationsNode.Attributes != null)
            {
                Console.WriteLine("Attributes");
                foreach (XmlAttribute attr in translationsNode.Attributes)
                {
                    Console.WriteLine("{0}: {1}", attr.Name, attr.Value);
                    if (attr.Name == "Version")
                        if (!CheckTranslationVersion(attr.Value))
                            return null;

                    if (attr.Name == "Culture")
                        if (culture.Name != attr.Value)
                        {
                            Logger.WriteLine ("Desired culture: " + culture.Name + ", actual culture: " + attr.Value);

                            MessageBox.Show("Culture in file does not match desired culture!",
                                            "Culture mismatch",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);
                        }
                }
                Console.WriteLine();
            }
            else
                throw new XmlException("File version is absent");

            Console.WriteLine("{0}: {1}", translationsNode.Name, translationsNode.Value);


            // Translation enumeration
            //
            foreach (XmlNode translationNode in translationsNode.ChildNodes)
            {
                foreach (XmlNode translationElementNode in translationNode.ChildNodes)
                {
                    if (translationElementNode.Name == "ID")
                    {
                        ushort id = ushort.Parse(translationElementNode.InnerText);
                        if (!points.ContainsKey(id))
                        {
                            MessageBox.Show("Costs collection does not match translation!",
                                            "Data consistency error",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Stop);

                            return null;
                        }
                        else
                        {
                            XmlNode nameNode = translationElementNode.NextSibling;

                            TeleportNode point = points[id];
                            point.SetName(culture, nameNode.InnerText);
                            points[id] = point;
                        }
                    }
                }
            }

            return points;
        }

        private XmlDocument LoadTranlationFile(string tranlationFilePath)
        {
            return ValidateXmlDoc("http://TeleportCalculator/TranslationSchema.xsd",
                                Settings.Default.XsdSchemataFolderName + @"\TranslationSchema.xsd",
                                tranlationFilePath);
        }

        #endregion

        private XmlDocument LoadSettingsFile()
        {
            return ValidateXmlDoc("http://TeleportCalculator/SettingsSchema.xsd",
                                                Settings.Default.XsdSchemataFolderName + @"\SettingsSchema.xsd",
                                                Path.Combine(Application.StartupPath, Settings.Default.SetingsFileName));
        }

        private List<Server> CreateServers(XmlDocument doc)
        {
            XmlNode settingsNode = doc.DocumentElement;

            // Version check
            //
            if (settingsNode.Attributes != null)
            {
                Console.WriteLine("Attributes");
                foreach (XmlAttribute attr in settingsNode.Attributes)
                {
                    Console.WriteLine("{0}: {1}", attr.Name, attr.Value);
                    if (attr.Name == "Version")
                        if (!CheckSettingsVersion(attr.Value))
                            return null;
                }
                Console.WriteLine();
            }
            else
                throw new XmlException("File version is absent");

            Console.WriteLine("{0}: {1}", settingsNode.Name, settingsNode.Value);


            List<Server> servers = new List<Server>();

            // Server enumeration
            //
            foreach (XmlNode serverNode in settingsNode.ChildNodes)
            {
                // Flag enumerstion
                //
                foreach (XmlNode flagNode in serverNode.ChildNodes)
                {
                    Server server = new Server();
                    server.image = flagNode.Attributes["Image"].Value;

                    // Inner nodes
                    //
                    foreach (XmlNode flagElementNode in flagNode.ChildNodes)
                    {
                        if (flagElementNode.Name == "Name")
                            server.name = flagElementNode.InnerText;

                        if (flagElementNode.Name == "Culture")
                            server.culture = new System.Globalization.CultureInfo(flagElementNode.InnerText);
                    }

                    servers.Add(server);
                }
            }

            return servers;
        }

        private bool CheckSettingsVersion(string version)
        {
            if (version == null)
                return false;

            string[] compatibleVersions = Global.SettingsCompatibility.Split(';');

            if (compatibleVersions.Contains<string>(version))
                return true;
            else
                return false;
        }       
    }
}
