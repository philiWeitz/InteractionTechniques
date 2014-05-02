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

        public bool CheckPushGesture(int bodyAngle, List<KinectDataPoint> queue)
        {
            return checkPushPullGesture(bodyAngle, queue, InteractionPushPull.PUSH);
        }

        public bool CheckPullGesture(int bodyAngle, List<KinectDataPoint> queue)
        {
            return checkPushPullGesture(bodyAngle, queue, InteractionPushPull.PULL);
        }

        private bool checkPushPullGesture(int bodyAngle, List<KinectDataPoint> queue, InteractionPushPull pushPull)
        {
            for (int i = 1; i < queue.Count; ++i)
            {
                if (chekPushPullGesture(bodyAngle, queue, i, pushPull))
                {
                    return true;
                }
            }
            return false;
        }

        private bool chekPushPullGesture(int bodyAngle, List<KinectDataPoint> queue, int stepSize, InteractionPushPull pushPull)
        {
            int minPoints = (2 * stepSize) + 1;

            for (int i = 0; i < queue.Count - minPoints; ++i)
            {
                KinectDataPoint p1 = queue.ElementAt(i);
                KinectDataPoint p3 = queue.ElementAt(i + (2 * stepSize));

                if (p1.TimeStamp.AddMilliseconds(maxPushPullTime) < p3.TimeStamp)
                {
                    break;
                }

                KinectDataPoint p2 = queue.ElementAt(i + stepSize);

                bool isPush = (p1.Z < p2.Z);
                double distance = p1.CalcDistance3D(p2);

                int angleDepth = 90 - p1.CalcDepthAngle(p2);
                int angleHorizont = p1.CalcHorizontalAngle(p2);

                if (distance > IConsts.GPushPullMinDepth
                    && angleHorizont < IConsts.GesturePushPullAngle
                    && angleDepth < IConsts.GesturePushPullAngle
                    && (pushPull == InteractionPushPull.PUSH && isPush
                      || pushPull == InteractionPushPull.PULL && !isPush))
                {
                    double keepDist = p2.CalcDistance3D(p3);

                    // the hand has to stay in a radius of 10 cm
                    if (keepDist < 10)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}