using System.Collections.Generic;
using System.Linq;
using InteractionUtil.Util;
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
                KinectDataPoint p1 = queue.ElementAt(i);
                KinectDataPoint p3 = queue.ElementAt(i + (2 * stepSize));

                if (p1.TimeStamp.AddMilliseconds(maxPushPullTime) < p3.TimeStamp)
                {
                    break;
                }

                KinectDataPoint p2 = queue.ElementAt(i + stepSize);

                int d1 = p1.Z - p2.Z;
                int d2 = p2.Z - p3.Z;

                if ((d1 >= IConsts.GPushPullMinDepth && d2 <= -IConsts.GPushPullMinDepth && pushPull == InteractionPushPull.PUSH)
                    || (d1 <= -IConsts.GPushPullMinDepth && d2 >= IConsts.GPushPullMinDepth && pushPull == InteractionPushPull.PULL))
                {
                    double dis1 = p1.CalcDistance(p2);
                    double dis2 = p1.CalcDistance(p3);
                    double dis3 = p2.CalcDistance(p3);

                    if (dis1 < IConsts.GPushPullGitterXY &&
                        dis2 < IConsts.GPushPullGitterXY && dis3 < IConsts.GPushPullGitterXY)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}