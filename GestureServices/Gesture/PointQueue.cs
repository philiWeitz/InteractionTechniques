using System.Collections.Generic;
using System.Linq;
using KinectServices.Common;
using Microsoft.Kinect;

namespace GestureServices.Gesture
{
    internal class PointQueue
    {
        private int maxTime;

        private Dictionary<JointType, List<KinectDataPoint>>
            jointPoints = new Dictionary<JointType, List<KinectDataPoint>>();

        // maxTime - max gesture time
        public PointQueue(int maxTime)
        {
            this.maxTime = maxTime;
        }

        public void AddPoint(KinectDataPoint point, JointType joint)
        {
            List<KinectDataPoint> queue = getQueue(joint);

            while (queue.Count > 0 && point.TimeStamp >= queue.First().TimeStamp.AddMilliseconds(maxTime))
            {
                queue.RemoveAt(0);
            }
            queue.Add(point);
        }

        public List<KinectDataPoint> GetQueue(JointType joint)
        {
            return getQueue(joint);
        }

        public void ClearQueue(JointType joint)
        {
            getQueue(joint).Clear();
        }

        public void ClearQueue()
        {
            foreach (List<KinectDataPoint> q in jointPoints.Values)
            {
                q.Clear();
            }
        }

        private List<KinectDataPoint> getQueue(JointType joint)
        {
            List<KinectDataPoint> queue;

            if (jointPoints.TryGetValue(joint, out queue) == false)
            {
                jointPoints[joint] = new List<KinectDataPoint>();
                queue = jointPoints[joint];
            }
            return queue;
        }
    }
}