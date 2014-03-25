using System.Collections.Generic;
using System.Linq;
using KinectServices.Common;

namespace GestureServices.Gesture
{
    internal enum InteractionPushPull
    {
        PUSH = 0,
        PULL = 1
    }

    internal class PushPullGestureDetector
    {
        private static readonly int MIN_DEPTH = 75;
        private static readonly int MAX_PUSH_PULL_RANGE = 50;
        private int maxPushPullTime;

        public PushPullGestureDetector(int maxPushPullTime)
        {
            this.maxPushPullTime = maxPushPullTime;
        }

        public bool CheckPushGesture(List<KinectDataPoint> queue)
        {
            return checkPushPullGesture(queue, InteractionPushPull.PUSH);
        }

        public bool CheckPullGesture(List<KinectDataPoint> queue)
        {
            return checkPushPullGesture(queue, InteractionPushPull.PULL);
        }

        private bool checkPushPullGesture(List<KinectDataPoint> queue, InteractionPushPull pushPull)
        {
            for (int i = 1; i < queue.Count; ++i)
            {
                if (chekPushPullGesture(queue, i, pushPull))
                {
                    return true;
                }
            }

            return false;
        }

        private bool chekPushPullGesture(List<KinectDataPoint> queue, int stepSize, InteractionPushPull pushPull)
        {
            int minPoints = 2 * stepSize + 1;

            for (int i = 0; i < queue.Count - minPoints; ++i)
            {
                if (queue.ElementAt(i).TimeStamp.AddMilliseconds(maxPushPullTime) < queue.ElementAt(i + (2 * stepSize)).TimeStamp)
                {
                    break;
                }

                int d1 = queue.ElementAt(i).DepthPoint.Depth - queue.ElementAt(i + stepSize).DepthPoint.Depth;
                int d2 = queue.ElementAt(i + stepSize).DepthPoint.Depth - queue.ElementAt(i + (2 * stepSize)).DepthPoint.Depth;

                if ((d1 >= MIN_DEPTH && d2 <= -MIN_DEPTH && pushPull == InteractionPushPull.PUSH)
                    || (d1 <= -MIN_DEPTH && d2 >= MIN_DEPTH && pushPull == InteractionPushPull.PULL))
                {
                    double dis1 = InteractionMath.CalcDistance(
                        queue.ElementAt(i).ColorPoint, queue.ElementAt(i + stepSize).ColorPoint);
                    double dis2 = InteractionMath.CalcDistance(
                        queue.ElementAt(i).ColorPoint, queue.ElementAt(i + (2 * stepSize)).ColorPoint);
                    double dis3 = InteractionMath.CalcDistance(
                        queue.ElementAt(i + stepSize).ColorPoint, queue.ElementAt(i + (2 * stepSize)).ColorPoint);

                    if (dis1 < MAX_PUSH_PULL_RANGE && dis2 < MAX_PUSH_PULL_RANGE && dis3 < MAX_PUSH_PULL_RANGE)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}