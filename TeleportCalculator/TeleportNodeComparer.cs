using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeleportCalculator
{
    class TeleportNodeComparer : IEqualityComparer <TeleportNode>
    {
        #region IEqualityComparer<TeleportPoint> Members

        public bool Equals(TeleportNode x, TeleportNode y)
        {
            // Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            // Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.ID == y.ID;
        }

        public int GetHashCode(TeleportNode obj)
        {
            // Check whether the object is null.
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return obj.GetHashCode();
        }

        #endregion
    }
}
