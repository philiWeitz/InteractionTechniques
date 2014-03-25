using System.Collections.Generic;
using System.Linq;
using KinectServices.Common;
using Microsoft.Kinect;


namespace GestureServices.Gesture
{
    internal class PointQueue
    {
        private int maxTime;

        private Dictionary<JointType, Queue<KinectDataPoint>>
            jointPoints = new Dictionary<JointType, Queue<KinectDataPoint>>();


        // maxTime - max gesture time
        public PointQueue(int maxTime)
        {
            this.maxTime = maxTime;
        }

        public void AddPoint(KinectDataPoint point, JointType joint)
        {
            Queue<KinectDataPoint> queue = getQueue(joint);

            while (queue.Count > 0 && point.TimeStamp >= queue.First().TimeStamp.AddMilliseconds(maxTime))
            {
                queue.Dequeue();
            }
            queue.Enqueue(point);
        }

        public Queue<KinectDataPoint> GetQueue(JointType joint)
        {
            return getQueue(joint);
        }

        public void ClearQueue(JointType joint)
        {
            getQueue(joint).Clear();
        }

        public void ClearQueue()
        {
            foreach (Queue<KinectDataPoint> q in jointPoints.Values)
            {
                q.Clear();
            }
        }

        private Queue<KinectDataPoint> getQueue(JointType joint)
        {
            Queue<KinectDataPoint> queue;

            if (jointPoints.TryGetValue(joint, out queue) == false)
            {
                jointPoints[joint] = new Queue<KinectDataPoint>();
                queue = jointPoints[joint];
            }
            return queue;
        }
    }
}
