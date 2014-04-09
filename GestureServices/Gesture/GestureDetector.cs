using System;
using System.Collections.Generic;
using InteractionUtil.Common;
using InteractionUtil.Util;
using KinectServices.Common;
using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace GestureServices.Gesture
{
    // enum for controlling the kinect elevation angle
    internal enum UserCenter
    {
        up,
        down,
        center
    }

    internal class GestureDetector
    {
        private ISkeletonService skeletonService = null;
        private UserCenter userCenter = UserCenter.center;

        private KinectSensor sensor;
        private DateTime gestureTime;
        private PointQueue dataPointQueue;
        private SwipeDetector swipeDetector;
        private CircleDetector circleDetector;
        private PushPullGestureDetector pushPullDetector;

        public GestureDetector(KinectSensor sensor)
        {
            initialize(sensor);
        }

        public void Update()
        {
            if (getSekeltonService().userInRange())
            {
                addDataPoint(JointType.HandLeft);
                addDataPoint(JointType.HandRight);
            }
        }

        public List<KinectDataPoint> getActiveUserDataPointQueue()
        {
            if (getSekeltonService().userInRange() && !isTimeOut())
            {
                if (JointType.HandLeft == isGestureActive())
                {
                    return dataPointQueue.GetQueue(JointType.HandRight);
                }
                else if (JointType.HandRight == isGestureActive())
                {
                    return dataPointQueue.GetQueue(JointType.HandLeft);
                }
            }
            return new List<KinectDataPoint>();
        }

        public void setGestureTimeOut(int gestureTimeOut)
        {
            gestureTime = DateTime.Now.AddMilliseconds(gestureTimeOut);
        }

        public InteractionGesture checkAllGestures()
        {
            if (getSekeltonService().userInRange() && !isTimeOut())
            {
                int bodyAngle = getSekeltonService().getUserBodyAngle();

                /*----- two handed gestures -----*/
                InteractionGesture twoHandedGesture = checkTwoHandedGestures(bodyAngle);

                if (InteractionGesture.None != twoHandedGesture)
                {
                    return twoHandedGesture;
                }
                else if (JointType.HandLeft == isGestureActive())
                {
                    return checkOneHandedGestures(JointType.HandRight, bodyAngle);
                }
                else if (JointType.HandRight == isGestureActive())
                {
                    return checkOneHandedGestures(JointType.HandLeft, bodyAngle);
                }
            }

            return InteractionGesture.None;
        }

        private InteractionGesture checkTwoHandedGestures(int bodyAngle)
        {
            bool gestureFound = false;

            /*----- push / pull gestures -----*/
            gestureFound = pushPullDetector.CheckPushGesture(bodyAngle, dataPointQueue.GetQueue(JointType.HandLeft))
                && pushPullDetector.CheckPushGesture(bodyAngle, dataPointQueue.GetQueue(JointType.HandRight));
            if (gestureFound) return InteractionGesture.PushTwoHanded;

            return InteractionGesture.None;
        }

        private InteractionGesture checkOneHandedGestures(JointType joint, int bodyAngle)
        {
            bool gestureFound = false;

            /*----- circle gestures -----*/
            gestureFound = circleDetector.CheckCircleClockGesture(dataPointQueue.GetQueue(joint));
            if (gestureFound) return InteractionGesture.CircleClock;

            gestureFound = circleDetector.CheckCircleCounterClockGesture(dataPointQueue.GetQueue(joint));
            if (gestureFound) return InteractionGesture.CircleCounterClock;

            /*----- push / pull gestures -----*/
            gestureFound = pushPullDetector.CheckPushGesture(bodyAngle, dataPointQueue.GetQueue(joint));
            if (gestureFound) return InteractionGesture.PushOneHanded;

            gestureFound = pushPullDetector.CheckPullGesture(bodyAngle, dataPointQueue.GetQueue(joint));
            if (gestureFound) return InteractionGesture.PullOneHanded;

            /*----- swipe gestures -----*/
            gestureFound = swipeDetector.CheckToLeftSwipeGesture(bodyAngle, dataPointQueue.GetQueue(joint));
            if (gestureFound) return InteractionGesture.SwipeToLeft;

            gestureFound = swipeDetector.CheckToRightSwipeGesture(bodyAngle, dataPointQueue.GetQueue(joint));
            if (gestureFound) return InteractionGesture.SwipeToRight;

            gestureFound = swipeDetector.CheckUpSwipeGesture(bodyAngle, dataPointQueue.GetQueue(joint));
            if (gestureFound) return InteractionGesture.SwipeUp;

            gestureFound = swipeDetector.CheckDownSwipeGesture(bodyAngle, dataPointQueue.GetQueue(joint));
            if (gestureFound) return InteractionGesture.SwipeDown;

            return InteractionGesture.None;
        }

        private void initialize(KinectSensor sensor)
        {
            this.sensor = sensor;
            dataPointQueue = new PointQueue(IConsts.GestureQueueSize);

            swipeDetector = new SwipeDetector(IConsts.GestureQueueSizeSwipe);
            circleDetector = new CircleDetector(IConsts.GestureQueueSizeCycle);
            pushPullDetector = new PushPullGestureDetector(IConsts.GestureQueueSizePush);
        }

        private void addDataPoint(JointType joint)
        {
            if (getSekeltonService().hasJoint(joint))
            {
                dataPointQueue.AddPoint(getSekeltonService().getDataPoint(joint), joint);
            }
        }

        private JointType isGestureActive()
        {
            if (getSekeltonService().hasJoint(JointType.HandLeft)
                && getSekeltonService().hasJoint(JointType.HandRight)
                && getSekeltonService().hasJoint(JointType.ShoulderCenter))
            {
                KinectDataPoint handLeft = getSekeltonService().getDataPoint(JointType.HandLeft);
                KinectDataPoint handRight = getSekeltonService().getDataPoint(JointType.HandRight);
                KinectDataPoint shoulder = getSekeltonService().getDataPoint(JointType.ShoulderCenter);

                if (handLeft.ScreenY < shoulder.ScreenY && handLeft.ScreenY < handRight.ScreenY)
                {
                    return JointType.HandLeft;
                }
                else if (handRight.ScreenY < shoulder.ScreenY && handRight.ScreenY < handLeft.ScreenY)
                {
                    return JointType.HandRight;
                }
            }

            return default(JointType);
        }

        private bool isTimeOut()
        {
            if (gestureTime > DateTime.Now)
            {
                dataPointQueue.ClearQueue();
                return true;
            }
            return false;
        }

        public void focuseCurrentUser()
        {
            if (getSekeltonService().userInRange() && !isTimeOut())
            {
                if (isGestureActive() != default(JointType))
                {
                    KinectDataPoint point = getSekeltonService().getDataPoint(JointType.ShoulderCenter);

                    // user stands outside of the upper limit -> move kinect up
                    if (point.ScreenY < IConsts.KinectCenterUpperLimit)
                    {
                        userCenter = UserCenter.up;
                    }
                    // user stands outside of the lower limit -> move kinect dow
                    else if (point.ScreenY > IConsts.KinectCenterLowerLimit)
                    {
                        userCenter = UserCenter.down;
                    }

                    // centeres the user based on inner borders
                    if (userCenter == UserCenter.up && point.ScreenY < IConsts.KinectCenterUpperLimitInner)
                    {
                        // move kinect up
                        if (sensor.ElevationAngle < sensor.MaxElevationAngle - 5)
                        {
                            System.Media.SystemSounds.Asterisk.Play();

                            int newAngle = Math.Min(sensor.MaxElevationAngle, sensor.ElevationAngle + 5);
                            sensor.ElevationAngle = newAngle;
                        }
                    }
                    else if (userCenter == UserCenter.down && point.ScreenY > IConsts.KinectCenterLowerLimitInner)
                    {
                        // move kinect down
                        if (sensor.ElevationAngle > sensor.MinElevationAngle + 5)
                        {
                            System.Media.SystemSounds.Asterisk.Play();

                            int newAngle = Math.Max(sensor.MinElevationAngle, sensor.ElevationAngle - 5);
                            sensor.ElevationAngle = newAngle;
                        }
                    }
                    else
                    {
                        // user is centered
                        userCenter = UserCenter.center;
                    }
                }
                else
                {
                    // is user out of range -> user is concidered to be centered
                    userCenter = UserCenter.center;
                }
            }
        }

        private ISkeletonService getSekeltonService()
        {
            if (null == skeletonService)
            {
                skeletonService = SpringUtil.getService<ISkeletonService>();
                skeletonService.enableSkeleton(sensor);
            }
            return skeletonService;
        }
    }
}