using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using TeleportCalculator.Properties;
using System.Globalization;

namespace TeleportCalculator
{
    internal class Renderer
    {
        public delegate void GetNodeNameEventHandler(string nodeName);
        public event GetNodeNameEventHandler GetNodeName;            

        private GraphLayer graphLayer = GraphLayer.Instance();
        //private object lockObj = new object();
        private DateTime lastDrawDateTime;
        private Graphics g = null;
        private Brush brush = new SolidBrush(Color.Red);
        private Pen pen = null;
        private Image offScreenInitialImage;
        private Image offScreenCurrentImage;
        private Graphics offScreenDC;

        private Queue<MouseEventArgs> mouseEventArgs = new Queue<MouseEventArgs>();


        public Renderer(Graphics g, Image image)
        {
            this.g = g;

            offScreenInitialImage = image;
            offScreenCurrentImage = (Image)offScreenInitialImage.Clone();
            offScreenDC = Graphics.FromImage(offScreenCurrentImage);
            pen = new Pen(brush, 3);
            lastDrawDateTime = DateTime.Now;
        }

        internal void ProcessMouseCoordinates(object stateInfo)
        {
            KeyValuePair<MouseEventArgs, CultureInfo> kvp = (KeyValuePair<MouseEventArgs, CultureInfo>)stateInfo;
            MouseEventArgs e = kvp.Key;
            CultureInfo currentCulture = kvp.Value;

            try
            {
                float absoluteX = (float)e.X / g.VisibleClipBounds.Width;
                float absoluteY = 1 - (float)e.Y / g.VisibleClipBounds.Height;

                float pointX;
                float pointY;
                float distance;

                List<TeleportNodeDistance> points = graphLayer.GetClosestPoint(absoluteX, absoluteY, out pointX, out pointY, out distance);

                if (distance < Settings.Default.MinimalDistanceToTeleportNode)
                {
                    Console.WriteLine("Hit mouse {0} {1}", absoluteX, absoluteY);
                    Console.WriteLine("Hit node at {0} {1}", pointX, pointY);

                    offScreenDC.DrawImage(offScreenInitialImage, 0, 0);

                    offScreenDC.DrawEllipse(pen, pointX * g.VisibleClipBounds.Width - 2, (1 - pointY) * g.VisibleClipBounds.Height - 2, 4, 4);
                    //offScreenDC.DrawEllipse(pen, kvp.Value.AbsoluteX * g.VisibleClipBounds.Width - 2, (1 - kvp.Value.AbsoluteY) * g.VisibleClipBounds.Height - 2, 4, 4);

                    //unsortedPoints.Sort(new TeleportPointDistanceComparer());
                    TeleportNode point = points[0].point;

                    DrawConnectedPaths(pen, point);

                    g.DrawImage(offScreenCurrentImage, 0, 0, g.VisibleClipBounds.Width, g.VisibleClipBounds.Height);

                    GetNodeNameEventHandler temp = GetNodeName;
                    if (temp != null)
                        temp(point.Names[currentCulture]);
                }
            }
            catch (InvalidOperationException ex)
            {
                InvalidOperationException ex2 = ex;
                //throw;
            }
        }


        private void DrawConnectedPaths(Pen pen, TeleportNode point)
        {
            foreach (ushort connectedPointID in point.Costs.Keys)
            {
                DrawPath(point.ID, connectedPointID);
                DrawPoint(connectedPointID);
            }
        }

        private void DrawPath(ushort startPointID, ushort connectedPointID)
        {
            float startAbsoluteX;
            float startAbsoluteY;
            float endAbsoluteX;
            float endAbsoluteY;

            graphLayer.GetPointAbsoluteCoordinates(startPointID, out startAbsoluteX, out startAbsoluteY);
            graphLayer.GetPointAbsoluteCoordinates(connectedPointID, out endAbsoluteX, out endAbsoluteY);

            offScreenDC.DrawLine(pen,
                startAbsoluteX * g.VisibleClipBounds.Width,
                (1 - startAbsoluteY) * g.VisibleClipBounds.Height,
                endAbsoluteX * g.VisibleClipBounds.Width,
                (1 - endAbsoluteY) * g.VisibleClipBounds.Height);
        }

        private void DrawPoint(ushort pointID)
        {
            float absoluteX;
            float absoluteY;

            graphLayer.GetPointAbsoluteCoordinates(pointID, out absoluteX, out absoluteY);

            offScreenDC.DrawEllipse(pen, absoluteX * g.VisibleClipBounds.Width - 2, (1 - absoluteY) * g.VisibleClipBounds.Height - 2, 4, 4);
        }

        internal void DrawPathSequence(List<TeleportNode> path)
        {
            offScreenDC.DrawImage(offScreenInitialImage, 0, 0);

            for (int i = 0; i < path.Count - 1; i++)
            {
                ushort currentNodeID = path[i].ID;
                ushort nextNodeID = path[i + 1].ID;

                DrawPath(currentNodeID, nextNodeID);
            }

            g.DrawImage(offScreenCurrentImage, 0, 0, g.VisibleClipBounds.Width, g.VisibleClipBounds.Height);
        }
    }
}
