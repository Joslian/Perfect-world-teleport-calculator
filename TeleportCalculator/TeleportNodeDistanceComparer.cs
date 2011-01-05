using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeleportCalculator
{
    class TeleportNodeDistanceComparer : IComparer<TeleportNodeDistance>
    {
        #region IComparer<TeleportPointDistance> Members

        public int Compare(TeleportNodeDistance x, TeleportNodeDistance y)
        {
            return x.distance.CompareTo(y.distance);
        }

        #endregion
    }
}
