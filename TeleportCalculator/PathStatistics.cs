using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeleportCalculator
{
    internal struct PathStatistics : ICloneable
    {
        internal uint cost;
        internal float walkTime;
        internal float flyTime;
        internal float rideTime;
        internal float teleportTime;
        internal float efficiency;

        internal List<TeleportNode> path;

        public PathStatistics(TeleportNode point)
        {
            cost = 0;
            efficiency = 0;
            flyTime = 0;
            path = new List<TeleportNode>();
            path.Add(point);
            rideTime = 0;
            teleportTime = 0;
            walkTime = 0;
        }

        #region ICloneable Members

        public object Clone()
        {
            PathStatistics clone = new PathStatistics();
            clone.cost = this.cost;
            clone.efficiency = this.efficiency;
            clone.flyTime = this.flyTime;
            clone.path = new List<TeleportNode>(this.path);
            clone.rideTime = this.rideTime;
            clone.teleportTime = this.teleportTime;
            clone.walkTime = this.walkTime;
            return clone;
        }

        #endregion
    }
}
