using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.ObjectModel;

namespace ExcelConverter
{
    internal class TeleportPoint
    {
        private ushort id;
        private int x;
        private int y;
        private Dictionary<CultureInfo, string> names = new Dictionary<CultureInfo, string>();
        private Dictionary<ushort, uint> costs = new Dictionary<ushort, uint>();
        private uint tableRowPos;
        private string tableColumnPos;


        internal ushort ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        internal int X
        {
            get
            {
                return x;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("x", x.ToString());
                else
                    x = value;
            }
        }

        internal int Y
        {
            get
            {
                return y;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("y", y.ToString());
                else
                    y = value;
            }
        }

        internal uint GetCost(ushort id)
        {
            
            if (costs.ContainsKey(id))
                return costs[id];
            else
                throw new ArgumentOutOfRangeException("id", id.ToString());
        }

        internal void SetCost(ushort id, uint cost)
        {
            if (id < 0 || cost < 0)
                throw new ArgumentOutOfRangeException("id or cost", id.ToString() + " " + cost.ToString());

            if (costs.ContainsKey(id))
                costs[id] = cost;
            else
                costs.Add(id, cost);
        }

        internal string GetName(CultureInfo language)
        {
            if (language == null)
                throw new ArgumentException("Empty culture info");

            if (!names.ContainsKey(language))
                throw new ArgumentOutOfRangeException("Unknown culture info", language.DisplayName);
            else
                return names[language];
        }

        internal void SetName(CultureInfo language, string name)
        {
            if (language == null)
                throw new ArgumentException("Empty culture info");

            if (!names.ContainsKey(language))
                names.Add(language, name);
            else
                names[language] = name;
        }

        internal uint TableRowPos
        {
            get
            {
                return tableRowPos;
            }
            set
            {
                tableRowPos = value;
            }
        }

        internal string TableColumnPos
        {
            get
            {
                return tableColumnPos;
            }
            set
            {
                tableColumnPos = value;
            }
        }

        internal ReadOnlyCollection<KeyValuePair<ushort, uint>> Costs
        {
            get
            {
                return costs.ToList().AsReadOnly();
            }
        }

        internal ReadOnlyCollection<KeyValuePair<CultureInfo, string>> Names
        {
            get
            {
                return names.ToList().AsReadOnly();
            }
        }

        internal uint NamesCount
        {
            get
            {
                return (uint)names.Count;
            }
        }

        public TeleportPoint Clone()
        {
            TeleportPoint point = new TeleportPoint();
            point.costs = new Dictionary<ushort, uint>(this.costs);
            point.id = this.id;
            point.names = new Dictionary<CultureInfo, string>(this.names);
            point.tableColumnPos = this.tableColumnPos;
            point.tableRowPos = this.tableRowPos;
            point.x = this.x;
            point.y = this.y;
            
            return point;
        }
    }
}
