using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace TeleportCalculator
{
    internal interface IDatabase
    {
        Dictionary<ushort, TeleportNode> GetPoints();
        List<Server> GetServers();
        Dictionary<ushort, TeleportNode> LoadTranslation(Dictionary<ushort, TeleportNode> points, CultureInfo culture);
    }
}
