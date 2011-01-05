using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.ObjectModel;

namespace TeleportCalculator
{
    internal class TeleportNode
    {
        private ushort id;
        private int x;
        private int y;
        private float absoluteX;
        private float absoluteY;
        private Dictionary<CultureInfo, string> names = new Dictionary<CultureInfo, string>();
        private Dictionary<ushort, uint> costs = new Dictionary<ushort, uint>();

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
                    throw new ArgumentOutOfRangeException("x", value.ToString());
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
                    throw new ArgumentOutOfRangeException("y", value.ToString());
                else
                    y = value;
            }
        }

        internal float AbsoluteX
        {
            get
            {
                return absoluteX;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("relativeX", value.ToString());
                else
                    absoluteX = value;
            }
        }

        internal float AbsoluteY
        {
            get
            {
                return absoluteY;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("relativeY", value.ToString());
                else
                    absoluteY = value;
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

        internal ReadOnlyDictionary<ushort, uint> Costs
        {
            get
            {
                return new ReadOnlyDictionary<ushort, uint>(costs);
            }
        }

        internal ReadOnlyCollection<KeyValuePair<ushort, uint>> SortedCosts
        {
            get
            {
                return new ReadOnlyCollection<KeyValuePair<ushort, uint>>(SortDictionary<ushort, uint>(costs));
            }
        }

        //internal ReadOnlyCollection<KeyValuePair<ushort, uint>> Costs
        //{
        //    get
        //    {
        //        List<KeyValuePair<ushort, uint>> list = costs.ToList();
        //        list.Sort();
        //        return list.AsReadOnly();
        //    }
        //}

        internal ReadOnlyDictionary<CultureInfo, string> Names
        {
            get
            {
                return new ReadOnlyDictionary<CultureInfo, string>(names);
            }
        }

        private static List<KeyValuePair<T1, T2>> SortDictionary<T1, T2>(Dictionary<T1, T2> data)
            where T1 : IComparable
            where T2 : IComparable
        {
            List<KeyValuePair<T1, T2>> result = new List<KeyValuePair<T1, T2>>(data);

            result.Sort(delegate(KeyValuePair<T1, T2> first, KeyValuePair<T1, T2> second)
              {
                  return first.Value.CompareTo(second.Value);
              }
              );
            return result;
        }
    }
}
