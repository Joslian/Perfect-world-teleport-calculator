using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeleportCalculator
{
    internal struct TeleportNodeDistance
    {
        internal float distance;
        internal TeleportNode point;

        public TeleportNodeDistance(float distance, TeleportNode point)
        {
            this.distance = distance;
            this.point = point;
        }
    }
}
