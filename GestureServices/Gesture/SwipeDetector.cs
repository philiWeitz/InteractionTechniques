using System;
using System.Collections.Generic;
using System.Linq;
using InteractionUtil.Util;
using KinectServices.Common;

namespace GestureServices.Gesture
{
    internal enum InteractionDirection
    {
        TO_LEFT = 0,
        TO_RIGHT = 1,
        UP = 2,
        DOWN = 3
    }

    internal class SwipeDetector
    {
        private int maxSwipeTime;

        public SwipeDetector(int maxSwipeTime)
        {
            this.maxSwipeTime = maxSwipeTime;
        }

        public bool CheckToLeftSwipeGesture(List<KinectDataPoint> queue)
        {
            return checkSwipeGesture(queue, InteractionDirection.TO_LEFT);
        }

        public bool CheckToRightSwipeGesture(List<KinectDataPoint> queue)
        {
            return checkSwipeGesture(queue, InteractionDirection.TO_RIGHT);
        }

        public bool CheckUpSwipeGesture(List<KinectDataPoint> queue)
        {
            return checkSwipeGesture(queue, InteractionDirection.UP);
        }

        public bool CheckDownSwipeGesture(List<KinectDataPoint> queue)
        {
            return checkSwipeGesture(queue, InteractionDirection.DOWN);
        }

        private bool checkSwipeGesture(List<KinectDataPoint> queue, InteractionDirection direction)
        {
            for (int i = 0; i < queue.Count; ++i)
            {
                if (checkSwipeGesture(queue, i, direction))
                {
                    return true;
                }
            }
            return false;
        }

        private bool checkSwipeGesture(List<KinectDataPoint> queue, int stepSize, InteractionDirection direction)
        {
            int minPoints = stepSize + 1;

            for (int i = 0; i < queue.Count - minPoints; ++i)
            {
                KinectDataPoint p1 = queue.ElementAt(i);
                KinectDataPoint p2 = queue.ElementAt(i + stepSize);

                if (p1.TimeStamp.AddMilliseconds(maxSwipeTime) < p2.TimeStamp)
                {
                    break;
                }

                int minSwipeLength = (IConsts.GSwipeMinLength *
                    IConsts.KinectStdDistance) / queue.ElementAt(i).Z;

                double length = p1.CalcDistance(p2);

                if (length >= minSwipeLength &&
                    (getLeftRightDirection(p1, p2) == direction || getUpDownDirection(p1, p2) == direction))
                {
                    int depthDiff = Math.Abs(
                        queue.ElementAt(i).Z - queue.ElementAt(i + stepSize).Z);

                    if (maxXYDifference(p1, p2, direction) && depthDiff < IConsts.GSwipeGitterZ)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private InteractionDirection getLeftRightDirection(KinectDataPoint p1, KinectDataPoint p2)
        {
            if (p1.X < p2.X)
            {
                return InteractionDirection.TO_RIGHT;
            }
            return InteractionDirection.TO_LEFT;
        }

        private InteractionDirection getUpDownDirection(KinectDataPoint p1, KinectDataPoint p2)
        {
            if (p1.Y < p2.Y)
            {
                return InteractionDirection.DOWN;
            }
            return InteractionDirection.UP;
        }

        private bool maxXYDifference(KinectDataPoint p1, KinectDataPoint p2, InteractionDirection direction)
        {
            if (direction == InteractionDirection.TO_LEFT || direction == InteractionDirection.TO_RIGHT)
            {
                return (Math.Abs(p1.Y - p2.Y) < IConsts.GSwipeGitterXY);
            }
            else
            {
                return (Math.Abs(p1.X - p2.X) < IConsts.GSwipeGitterXY);
            }
        }
    }
}