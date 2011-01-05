using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Globalization;

namespace TeleportCalculator
{
    internal class InputLayer
    {
        public delegate void PathCalculationEventHandler(Dictionary<ulong, PathStatistics> paths);
        public event PathCalculationEventHandler PathCalculation;

        public delegate void GetNodeNameEventHandler(string nodeName);
        public event GetNodeNameEventHandler GetNodeName;            

        private GraphLayer graphLayer = GraphLayer.Instance();

        private bool firstClick = true;
        private float absX1 = 0;
        private float absY1 = 0;
        private float absX2 = 0;
        private float absY2 = 0;

        private float newAbsX1 = 0;
        private float newAbsY1 = 0;

        Renderer renderer = null;

        public InputLayer(Renderer renderer)
        {
            this.renderer = renderer;
            renderer.GetNodeName += new Renderer.GetNodeNameEventHandler(renderer_GetNodeName);
        }

        void renderer_GetNodeName(string nodeName)
        {
            GetNodeNameEventHandler temp = GetNodeName;

            if (temp != null)
                temp(nodeName);
        }

        internal void UpdateMouseCoordinates(MouseEventArgs e, CultureInfo currentCulture)
        {
            KeyValuePair<MouseEventArgs, CultureInfo> kvp = new KeyValuePair<MouseEventArgs, CultureInfo>(e, currentCulture);
            renderer.ProcessMouseCoordinates(kvp);
            //KeyValuePair<MouseEventArgs, DateTime> kvp = new KeyValuePair<MouseEventArgs, DateTime>(e, DateTime.Now);
            //ThreadPool.QueueUserWorkItem(renderer.ProcessMouseCoordinates, kvp);
            //ThreadPool.QueueUserWorkItem(renderer.ProcessMouseCoordinates, e);
            //ThreadPool.QueueUserWorkItem(renderer.QueueMouseCoords, e);
            //renderer.QueueMouseCoords(e);
        }
       
        internal void UpdateMouseButtons(Point point, int imageHeight, int imageWidth)
        {
            float absoluteX = (float)point.X / imageWidth;
            float absoluteY = 1 - (float)point.Y / imageHeight;

            if (firstClick)
            {
                //absX1 = absoluteX;
                //absY1 = absoluteY;

                newAbsX1 = absoluteX;
                newAbsY1 = absoluteY;

                firstClick = false;
            }
            else
            {
                absX1 = newAbsX1;
                absY1 = newAbsY1;

                absX2 = absoluteX;
                absY2 = absoluteY;
                firstClick = true;

                List<PathStatistics> paths = graphLayer.GetPaths(absX1, absY1, absX2, absY2);
                Dictionary<ulong, PathStatistics> pathsWithID = new Dictionary<ulong, PathStatistics>(paths.Count);

                ulong id = 0;
                foreach (PathStatistics path in paths)
                {
                    pathsWithID.Add(id++, path);
                }

                PathCalculationEventHandler temp = PathCalculation;
                if (temp != null)
                    temp(pathsWithID);
            }
        }
    }
}

