using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.ObjectModel;

namespace TeleportCalculator
{
    internal class Database : Singleton <Database>
    {
        private IDatabase db = null;
        private Dictionary<ushort, TeleportNode> points = null;
        private List<Server> servers = null;

        internal ReadOnlyDictionary<ushort, TeleportNode> Points
        {
            get
            {
                if (points == null)
                    return null;

                return new ReadOnlyDictionary<ushort, TeleportNode>(points);
            }
        }

        internal ReadOnlyCollection<Server> Servers
        {
            get
            {
                if (servers == null)
                    return null;

                return servers.AsReadOnly();
            }
        }

        protected Database()
        {
            db = XmlStore.Instance();
        }

        internal void LoadPoints()
        {
            //IDatabase db = XmlStore.Instance();
            points = db.GetPoints();

            if (points != null)
                Logger.WriteLine("Points loaded OK");
        }

        internal void LoadTranslation(CultureInfo cultureInfo)
        {
            if (points == null)
                LoadPoints();

            if (points == null)
                MessageBox.Show("Cannot load points!",
                                "Teleport points load error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Stop);

            points = db.LoadTranslation(points, cultureInfo);
        }

        internal void LoadServers()
        {            
            servers = db.GetServers();
        }
    }
}
