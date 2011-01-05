using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeleportCalculator.Properties;

namespace TeleportCalculator
{
    internal class GraphLayer : Singleton<GraphLayer>
    {
        #region Old
        //private Dictionary<ushort, TeleportPoint> points = new Dictionary<ushort, TeleportPoint>(56);

        //internal void AddPoint(TeleportPoint point)
        //{
        //    CheckNullDictionary();

        //    if (!points.ContainsKey(point.ID))
        //        points.Add(point.ID, point);
        //    else
        //        points[point.ID] = point;
        //}

        //internal void RemovePoint(ushort id)
        //{
        //    if (points.ContainsKey(id))
        //        points.Remove(id);
        //}

        //private void CheckNullDictionary()
        //{
        //    if (points == null)
        //        points = new Dictionary<ushort, TeleportPoint>();
        //}
        #endregion

        private Database database = Database.Instance();
        private bool flag = true;

        private GraphLayer()
        { }

        internal List<TeleportNodeDistance> GetClosestPoint(float absoluteX, float absoluteY, out float pointX, out float pointY, out float distance)
        {
            List<TeleportNodeDistance> list = new List<TeleportNodeDistance>();
            //SortedList<float, TeleportPoint> sortedPoints = new SortedList<float, TeleportPoint>();
            float minDistance = float.MaxValue;
            ushort minDistanceID = ushort.MaxValue;
            float currDistance = 0;

            foreach (TeleportNode point in database.Points.Values)
            {
                currDistance = (float)Math.Sqrt(Math.Pow(absoluteX - point.AbsoluteX, 2) + Math.Pow(absoluteY - point.AbsoluteY, 2));
                list.Add(new TeleportNodeDistance(currDistance, point));
                if (currDistance < minDistance)
                {
                    minDistance = currDistance;
                    minDistanceID = point.ID;
                }
            }

            pointX = database.Points[minDistanceID].AbsoluteX;
            pointY = database.Points[minDistanceID].AbsoluteY;
            distance = minDistance;
            TeleportNodeDistanceComparer distanceComparer = new TeleportNodeDistanceComparer();
            list.Sort(distanceComparer);
            return list;
        }

        internal void GetPointAbsoluteCoordinates(ushort pointID, out float absoluteX, out float absoluteY)
        {
            //TeleportPoint point = points[pointID];            
            TeleportNode point = database.Points[pointID];
            absoluteX = point.AbsoluteX;
            absoluteY = point.AbsoluteY;
        }

        internal List<PathStatistics> GetPaths(float absX1, float absY1, float absX2, float absY2)
        {
            List<PathStatistics> statistics = new List<PathStatistics>();
            float dummy;
            List<TeleportNodeDistance> points = GetClosestPoint(absX1, absY1, out dummy, out dummy, out dummy);

            List<TeleportNodeDistance> possibleStartNodes = new List<TeleportNodeDistance>(points.GetRange(0, Settings.Default.ClosestTeleportNodesCount));
            TeleportNode endNode = GetClosestPoint(absX2, absY2, out dummy, out dummy, out dummy)[0].point;

            TeleportNode closestStartPoint = possibleStartNodes[0].point;
            PathStatistics closestStartPointStatistics = new PathStatistics(closestStartPoint);
            List<PathStatistics> sequences = new List<PathStatistics>();
            sequences.Add(closestStartPointStatistics);

            List<PathStatistics> newSequences = new List<PathStatistics>();

            while (flag)
            {
                foreach (PathStatistics sequence in sequences)
                {
                    List<PathStatistics> tmp = GetNewPaths(sequence, endNode);
                    if (tmp != null)
                    {
                        newSequences.AddRange(tmp);
                    }
                    else
                    {
                        flag = false;
                        newSequences.Add(sequence);
                    }
                }

                sequences = new List<PathStatistics>(newSequences);
                newSequences.Clear();
            }

            flag = true;

            foreach (PathStatistics sequence in sequences)
            {
                if (sequence.path.Last().ID == endNode.ID)
                    newSequences.Add(sequence);
            }

            return newSequences;
        }

        private List<PathStatistics> GetNewPaths(PathStatistics sequence, TeleportNode endNode)
        {
            TeleportNode lastNode = sequence.path.Last();

            if (lastNode.ID != endNode.ID)
            {
                List<PathStatistics> sequenses = new List<PathStatistics>();

                foreach (ushort nextNodeID in lastNode.Costs.Keys)
                {
                    TeleportNode nextNode = database.Points[nextNodeID];
                    TeleportNodeComparer comparer = new TeleportNodeComparer();

                    if (!sequence.path.Contains(nextNode, comparer))
                    {
                        //flag = true;
                        PathStatistics newSequence = (PathStatistics)sequence.Clone();
                        newSequence.path.Add(nextNode);

                        newSequence.cost += lastNode.GetCost(nextNodeID);
                        sequenses.Add(newSequence);
                    }
                }

                return sequenses;
            }
            else
                return null;
        }
    }
}
