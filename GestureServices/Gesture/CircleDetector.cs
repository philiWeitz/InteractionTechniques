using System;
using System.Collections.Generic;
using System.Linq;
using InteractionUtil.Util;
using KinectServices.Common;

namespace GestureServices.Gesture
{
    internal enum InteractionCircle
    {
        CLOCK_WISE = 0,
        COUNTER_CLOCK_WISE = 1
    }

    internal class CircleDetector
    {
        private int maxCircleTime;

        public CircleDetector(int maxCircleTime)
        {
            this.maxCircleTime = maxCircleTime;
        }

        public bool CheckCircleCounterClockGesture(List<KinectDataPoint> queue)
        {
            return checkCircleGesture(queue, InteractionCircle.COUNTER_CLOCK_WISE);
        }

        public bool CheckCircleClockGesture(List<KinectDataPoint> queue)
        {
            return checkCircleGesture(queue, InteractionCircle.CLOCK_WISE);
        }

        private bool checkCircleGesture(List<KinectDataPoint> queue, InteractionCircle direction)
        {
            for (int i = 0; i < queue.Count; ++i)
            {
                if (checkCircleGesture(queue, i, direction))
                {
                    return true;
                }
            }

            return false;
        }

        private bool checkCircleGesture(List<KinectDataPoint> queue, int stepSize, InteractionCircle direction)
        {
            int minPoints = stepSize * 4 + 1;

            for (int i = 0; i < queue.Count - minPoints; ++i)
            {
                KinectDataPoint p1 = queue.ElementAt(i);
                KinectDataPoint p4 = queue.ElementAt(i + (3 * stepSize));

                if (p1.TimeStamp.AddMilliseconds(maxCircleTime) < p4.TimeStamp)
                {
                    break;
                }

                KinectDataPoint p2 = queue.ElementAt(i + stepSize);
                KinectDataPoint p3 = queue.ElementAt(i + (2 * stepSize));

                double diag1 = p1.CalcScreenDistance(p3);
                double diag2 = p2.CalcScreenDistance(p4);
                double dist1 = p1.CalcScreenDistance(p2);
                double dist2 = p2.CalcScreenDistance(p3);
                double dist3 = p3.CalcScreenDistance(p4);
                double dist4 = p4.CalcScreenDistance(p1);

                if (diag1 > IConsts.GCircleDiameter && diag2 > IConsts.GCircleDiameter)
                {
                    if (Math.Abs(dist1 - dist2) < IConsts.GCircleGitterXY && Math.Abs(dist2 - dist3) < IConsts.GCircleGitterXY &&
                        Math.Abs(dist3 - dist4) < IConsts.GCircleGitterXY && Math.Abs(dist4 - dist1) < IConsts.GCircleGitterXY)
                    {
                        // TODO: improve circle direction detection
                        if (getDirection(new KinectDataPoint[] { p1, p2, p3, p4 }) == direction)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private InteractionCircle getDirection(KinectDataPoint[] points)
        {
            int refIdx = 0;

            for (int i = 1; i < points.Length; ++i)
            {
                if (points[i].ScreenY < points[refIdx].ScreenY)
                {
                    refIdx = i;
                }
            }

            KinectDataPoint point = points[refIdx];
            KinectDataPoint nexPoint = points[(refIdx + 1) % points.Length];

            if (point.ScreenX < nexPoint.ScreenX)
            {
                return InteractionCircle.CLOCK_WISE;
            }
            return InteractionCircle.COUNTER_CLOCK_WISE;
        }
    }
}