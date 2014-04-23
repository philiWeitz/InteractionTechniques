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

        public bool CheckToLeftSwipeGesture(int bodyAngle, List<KinectDataPoint> queue)
        {
            return checkSwipeGesture(bodyAngle, queue, InteractionDirection.TO_LEFT);
        }

        public bool CheckToRightSwipeGesture(int bodyAngle, List<KinectDataPoint> queue)
        {
            return checkSwipeGesture(bodyAngle, queue, InteractionDirection.TO_RIGHT);
        }

        public bool CheckUpSwipeGesture(int bodyAngle, List<KinectDataPoint> queue)
        {
            return checkSwipeGesture(bodyAngle, queue, InteractionDirection.UP);
        }

        public bool CheckDownSwipeGesture(int bodyAngle, List<KinectDataPoint> queue)
        {
            return checkSwipeGesture(bodyAngle, queue, InteractionDirection.DOWN);
        }

        private bool checkSwipeGesture(int bodyAngle, List<KinectDataPoint> queue, InteractionDirection direction)
        {
            for (int i = 0; i < queue.Count; ++i)
            {
                if (checkSwipeGesture(bodyAngle, queue, i, direction))
                {
                    return true;
                }
            }
            return false;
        }

        private bool checkSwipeGesture(int bodyAngle, List<KinectDataPoint> queue, int stepSize, InteractionDirection direction)
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

                double length = p1.CalcDistance3D(p2);

                if (length >= IConsts.GSwipeMinLength)
                {
                    int horizontalAngle = p1.CalcHorizontalAngle(p2);
                    int depthAngle = p1.CalcDepthAngle(p2);

                    if (checkUpDown(p1, p2, depthAngle, horizontalAngle, direction) ||
                        checkLeftRight(p1, p2, depthAngle, horizontalAngle, bodyAngle, direction))
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
            return false;
        }

        private bool checkUpDown(KinectDataPoint p1, KinectDataPoint p2, int depthAngle,
            int horizontalAngle, InteractionDirection direction)
        {
            if (direction == InteractionDirection.UP || direction == InteractionDirection.DOWN)
            {
                if (depthAngle < IConsts.GSwipeDepthAngle)
                {
                    bool directionOk;

                    if (direction == InteractionDirection.UP)
                    {
                        directionOk = (p1.ScreenY > p2.ScreenY);
                    }
                    else
                    {
                        directionOk = (p1.ScreenY < p2.ScreenY);
                    }
                    return ((90 - horizontalAngle) < IConsts.GSwipeHorizontalAngle) && directionOk;
                }
            }
            return false;
        }

        private bool checkLeftRight(KinectDataPoint p1, KinectDataPoint p2, int depthAngle,
            int horizontalAngle, int bodyAngle, InteractionDirection direction)
        {
            if (direction == InteractionDirection.TO_LEFT || direction == InteractionDirection.TO_RIGHT)
            {
                if (Math.Abs(bodyAngle - depthAngle) < IConsts.GSwipeDepthAngle)
                {
                    bool directionOk;

                    if (direction == InteractionDirection.TO_LEFT)
                    {
                        directionOk = (p1.ScreenX > p2.ScreenX);
                    }
                    else
                    {
                        directionOk = (p1.ScreenX < p2.ScreenX);
                    }
                    return (horizontalAngle < IConsts.GSwipeHorizontalAngle) && directionOk;
                }
            }
            return false;
        }
    }
}