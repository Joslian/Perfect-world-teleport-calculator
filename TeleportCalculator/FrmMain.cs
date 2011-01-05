using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using TeleportCalculator.Properties;
using System.Globalization;
using System.IO;
using System.Xml;

namespace TeleportCalculator
{
    public partial class FrmMain : Form
    {
        private bool firstRun = true;
        private bool firstClick = true;

        private float sizeRatio = 1;
        private float yOffset = -229.825f;
        private float xOffset = -16.32f;
        private float mapWidth;
        private float mapHeight;
        private int imageWidth;
        private int imageHeight;

        private int firstX1 = 114;
        private int firstY1 = 314;

        private int firstX2 = 663;
        private int firstY2 = 968;

        private float absX1 = 0;
        private float absX2 = 0;
        private float absY1 = 0;
        private float absY2 = 0;

        private InputLayer mapInputlayer = null;
        private Renderer renderer = null;
        //private List<Server> servers = null;
        private Database database = Database.Instance();
        CultureInfo currentCulture = null;
        private Dictionary<ulong, PathStatistics> paths = null;

        private ToolTip imageToolTip = new ToolTip();
        private StringBuilder imageTooltipBuilder = new StringBuilder();
        delegate int AddItemsCallback(string[] items);
        private SortColumn sortColumn = SortColumn.Cost;

        //const Int32 COMBOBOX_HEIGHTCONST = 0X153;

        //[DllImport("user32.dll")]

        //private static extern int SendMessage(IntPtr hwnd, Int32 wMsg, Int32 wParam, Int32 lParam);

        //private void AdjustComboBoxHeight(ComboBox control, Int32 height)
        //{
        //    SendMessage(control.Handle, COMBOBOX_HEIGHTCONST, -1, height);
        //    control.Refresh();
        //}

        public FrmMain()
        {
            InitializeComponent();

            mapWidth = 752.4f;
            mapHeight = 789.3f;

            imageWidth = pictureBox1.Width;
            imageHeight = pictureBox1.Height;

            if (firstRun)
                Logger.WriteLine("First run");

            renderer = new Renderer(pictureBox1.CreateGraphics(), pictureBox1.Image);
            mapInputlayer = new InputLayer(renderer);
            mapInputlayer.PathCalculation += new InputLayer.PathCalculationEventHandler(mapInputlayer_PathCalculation);
            mapInputlayer.GetNodeName += new InputLayer.GetNodeNameEventHandler(mapInputlayer_GetNodeName);

            LoadPoints();
            LoadServers();

            imageToolTip.SetToolTip(pictureBox1, "");
            imageToolTip.ShowAlways = true;
        }

        void mapInputlayer_GetNodeName(string nodeName)
        {
            imageToolTip.SetToolTip(pictureBox1, nodeName);
        }

        void mapInputlayer_PathCalculation(Dictionary<ulong, PathStatistics> paths)
        {
            this.paths = paths;

            List<ulong> idsSortedByField = SortPaths(paths);

            listView1.Items.Clear();

            for (int i = 0; i < Math.Min((int)Settings.Default.MaxPathCount, idsSortedByField.Count); i++)
            {
                ulong currID = idsSortedByField[i];

                SetText(new string[] { currID.ToString(),
                                        paths[currID].cost.ToString(),
                                        paths[currID].teleportTime.ToString(),
                                        paths[currID].efficiency.ToString(),
                                        paths[currID].walkTime.ToString(),
                                        paths[currID].flyTime.ToString (),
                                        paths[currID].rideTime.ToString () });
            }

            //listView1.SetSortIcon(1, SortOrder.Ascending);
        }

        private List<ulong> SortPaths(Dictionary<ulong, PathStatistics> paths)
        {
            Stack<ulong> smallValues = new Stack<ulong>();
            Queue<ulong> largeValues = new Queue<ulong>();
            List<ulong> sortedPathIDs = new List<ulong>();

            switch (sortColumn)
            {
                case SortColumn.Cost:
                    uint minCost = paths[0].cost;
                    uint maxCost = minCost;

                    foreach (KeyValuePair<ulong, PathStatistics> pathInfo in paths)
                    {
                        if (pathInfo.Value.cost <= minCost)
                        {
                            minCost = pathInfo.Value.cost;
                            smallValues.Push(pathInfo.Key);
                        }
                        else
                        {
                            maxCost = pathInfo.Value.cost;
                            largeValues.Enqueue(pathInfo.Key);
                        }
                    }

                    while (smallValues.Count > 0)
                    {
                        sortedPathIDs.Add(smallValues.Pop());
                    }
                    while (largeValues.Count > 0)
                    {
                        sortedPathIDs.Add(largeValues.Dequeue());
                    }

                    return sortedPathIDs;
                    break;
                case SortColumn.Time:
                    break;
                case SortColumn.Efficiency:
                    break;
                case SortColumn.WalkTime:
                    break;
                case SortColumn.FlyTime:
                    break;
                case SortColumn.RideTime:
                    break;
                default:
                    break;
            }

            return null;
        }

        private void LoadServers()
        {
            database.LoadServers();
            cmbServer.Items.Clear();

            foreach (Server server in database.Servers)
            {
                cmbServer.Items.Add(server.name);
            }

            cmbServer.Text = "International";
            //cmbServer.SelectedIndex = 0;

            Logger.WriteLine("Flags loaded OK");
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            mapInputlayer.UpdateMouseButtons(e.Location, imageHeight, imageWidth);
        }

        private void ShowCalculations()
        {
            float width = (firstX1 - firstX2) / (absX1 - absX2);
            float height = (firstY1 - firstY2) / (absY1 - absY2);

            float XOffset = absX1 * width - firstX1;
            float YOffset = absY1 * height - firstY1;

            xOffset = XOffset;
            yOffset = YOffset;

            firstRun = false;
            firstClick = false;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            UpdateSizeRatio();
            //MessageBox.Show(((float)pictureBox1.Height / (float)pictureBox1.Width).ToString());
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            UpdateSizeRatio();
        }

        private void UpdateSizeRatio()
        {
            sizeRatio = (float)pictureBox1.Width / (float)pictureBox1.Height;
        }

        private void btnResetAdjust_Click(object sender, EventArgs e)
        {
            firstRun = true;
            firstClick = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            mapInputlayer.UpdateMouseCoordinates(e, currentCulture);
        }

        private void btnLoadPoints_Click(object sender, EventArgs e)
        {
            LoadPoints();
        }

        private void LoadPoints()
        {
            database.LoadPoints();

            if (database.Points != null)
                Logger.WriteLine("Points loaded OK");
        }

        private void cmbServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (database.Servers != null && database.Servers.Count != 0)
                foreach (Server server in database.Servers)
                {
                    if (server.name == (string)cmbServer.SelectedItem)
                    {
                        LoadFlag(server.image);
                        LoadTranslation(server.culture);
                        currentCulture = server.culture;
                    }
                }
        }

        private void LoadTranslation(CultureInfo cultureInfo)
        {
            if (database.Points == null)
                database.LoadPoints();

            database.LoadTranslation(cultureInfo);
        }

        private void LoadFlag(string flagImageFile)
        {
            picFlag.Load(Path.Combine(Settings.Default.ImagesFolder, flagImageFile));
            Logger.WriteLine("Flag changed OK");
        }
        private void btnWriteCoordsData_Click(object sender, EventArgs e)
        {
            XmlDocument doc = XmlStore.ValidateXmlDoc("http://TeleportCalculator/TeleportSchema.xsd",
                                    Settings.Default.XsdSchemataFolderName + @"\TeleportSchema.xsd",
                                    Path.Combine(Application.StartupPath, Settings.Default.XmlCostsName));

            if (doc != null)
                doc.Load(Path.Combine(Application.StartupPath, Settings.Default.XmlCostsName));

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
                        if (!XmlStore.CheckCostsVersion(attr.Value))
                            MessageBox.Show("Version check failed");
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
                TeleportNode point = null;

                // Inner nodes
                //
                foreach (XmlNode teleportPointElementNode in teleportPointNode.ChildNodes)
                {
                    if (teleportPointElementNode.ChildNodes.Count == 1)
                    {
                        switch (teleportPointElementNode.Name)
                        {
                            case "ID":
                                point = database.Points[ushort.Parse(teleportPointElementNode.InnerText)];
                                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                                nsmgr.AddNamespace("ab", teleportPointNode.NamespaceURI);
                                XmlNode absoluteXNode = teleportPointNode.SelectSingleNode("ab:AbsoluteX", nsmgr);
                                XmlNode absoluteYNode = teleportPointNode.SelectSingleNode("ab:AbsoluteY", nsmgr);
                                absoluteXNode.LastChild.Value = point.AbsoluteX.ToString("0.00000", CultureInfo.InvariantCulture.NumberFormat);
                                absoluteYNode.LastChild.Value = point.AbsoluteY.ToString("0.00000", CultureInfo.InvariantCulture.NumberFormat);

                                continue;

                                break;

                            default:
                                break;
                        }
                        Console.WriteLine("{0}: {1}", teleportPointElementNode.Name, teleportPointElementNode.InnerText);
                    }
                }
            }

            doc.Save(Path.Combine(Application.StartupPath, Settings.Default.XmlCostsName));
            MessageBox.Show("File successfully saved");
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            imageHeight = pictureBox1.Height;
            imageWidth = pictureBox1.Width;
        }

        private int SetText(string[] items)
        {
            if (this.listView1.InvokeRequired)
            {
                AddItemsCallback d = new AddItemsCallback(SetText);
                this.Invoke(d, new object[] { items });
            }
            else
            {
                listView1.Items.Add(items[0], items[1], -1);
                listView1.Items[items[0]].SubItems.Add(items[1]);
                listView1.Items[items[0]].SubItems.Add(items[2]);
                listView1.Items[items[0]].SubItems.Add(items[3]);
                listView1.Items[items[0]].SubItems.Add(items[4]);
                listView1.Items[items[0]].SubItems.Add(items[5]);
                listView1.Items[items[0]].SubItems.Add(items[6]);
                listView1.Update();
                return listView1.Items.Count - 1;
            }
            return -1;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ulong id = ulong.Parse(listView1.SelectedItems[0].Name);
                List<TeleportNode> path = paths[id].path;
                renderer.DrawPathSequence(path);
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem lvi = this.listView1.GetItemAt(e.X, e.Y);

            if (listView1.SelectedItems.Count > 0 && lvi.Selected)
            {
                ulong id = ulong.Parse(listView1.SelectedItems[0].Name);
                List<TeleportNode> path = paths[id].path;
                renderer.DrawPathSequence(path);
            }
        }
    }
}