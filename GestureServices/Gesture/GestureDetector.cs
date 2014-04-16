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
        private static readonly int KINECT_ANGLE_TIMEOUT = 2000;

        private ISkeletonService skeletonService = null;
        private UserCenter userCenter = UserCenter.center;
        private KinectUser? lastActiveUser = null;

        private KinectSensor sensor;
        private DateTime gestureTime;
        private SwipeDetector swipeDetector;
        private CircleDetector circleDetector;
        private PushPullGestureDetector pushPullDetector;

        private IDictionary<KinectUser, PointQueue> dataPointMap =
            new Dictionary<KinectUser, PointQueue>();

        public GestureDetector(KinectSensor sensor)
        {
            initialize(sensor);
        }

        public void Update()
        {
            foreach (KinectUser user in getSekeltonService().userInRange())
            {
                addDataPoint(JointType.HandLeft, user);
                addDataPoint(JointType.HandRight, user);
            }
        }

        public List<KinectDataPoint> getActiveUserDataPointQueue()
        {
            if (!isTimeOut())
            {
                foreach (KinectUser user in getSekeltonService().userInRange())
                {
                    if (JointType.HandLeft == isGestureActive(user))
                    {
                        return dataPointMap[user].GetQueue(JointType.HandLeft);
                    }
                    else if (JointType.HandRight == isGestureActive(user))
                    {
                        return dataPointMap[user].GetQueue(JointType.HandRight);
                    }
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
            if (!isTimeOut())
            {
                List<KinectUser> users = getSekeltonService().userInRange();

                foreach (KinectUser user in users)
                {
                    int bodyAngle = getSekeltonService().getUserBodyAngle(user);

                    /*----- two handed gestures -----*/
                    InteractionGesture twoHandedGesture = checkTwoHandedGestures(bodyAngle, user);

                    if (InteractionGesture.None != twoHandedGesture)
                    {
                        return twoHandedGesture;
                    }

                    JointType active = isGestureActive(user);
                    if (default(JointType) != active)
                    {
                        if (JointType.HandLeft == active)
                        {
                            return checkOneHandedGestures(JointType.HandLeft, bodyAngle, user);
                        }
                        else if (JointType.HandRight == active)
                        {
                            return checkOneHandedGestures(JointType.HandRight, bodyAngle, user);
                        }
                    }
                }
            }
            return InteractionGesture.None;
        }

        public KinectUser? getActiveKinectUser()
        {
            return lastActiveUser;
        }

        private InteractionGesture checkTwoHandedGestures(int bodyAngle, KinectUser user)
        {
            bool gestureFound = false;

            /*----- push / pull gestures -----*/
            gestureFound = pushPullDetector.CheckPushGesture(bodyAngle, dataPointMap[user].GetQueue(JointType.HandLeft))
                && pushPullDetector.CheckPushGesture(bodyAngle, dataPointMap[user].GetQueue(JointType.HandRight));
            if (gestureFound) return InteractionGesture.PushTwoHanded;

            return InteractionGesture.None;
        }

        private InteractionGesture checkOneHandedGestures(JointType joint, int bodyAngle, KinectUser user)
        {
            bool gestureFound = false;

            /*----- circle gestures -----*/
            gestureFound = circleDetector.CheckCircleClockGesture(dataPointMap[user].GetQueue(joint));
            if (gestureFound) return InteractionGesture.CircleClock;

            gestureFound = circleDetector.CheckCircleCounterClockGesture(dataPointMap[user].GetQueue(joint));
            if (gestureFound) return InteractionGesture.CircleCounterClock;

            /*----- push / pull gestures -----*/
            gestureFound = pushPullDetector.CheckPushGesture(bodyAngle, dataPointMap[user].GetQueue(joint));
            if (gestureFound) return InteractionGesture.PushOneHanded;

            gestureFound = pushPullDetector.CheckPullGesture(bodyAngle, dataPointMap[user].GetQueue(joint));
            if (gestureFound) return InteractionGesture.PullOneHanded;

            /*----- swipe gestures -----*/
            gestureFound = swipeDetector.CheckToLeftSwipeGesture(bodyAngle, dataPointMap[user].GetQueue(joint));
            if (gestureFound) return InteractionGesture.SwipeToLeft;

            gestureFound = swipeDetector.CheckToRightSwipeGesture(bodyAngle, dataPointMap[user].GetQueue(joint));
            if (gestureFound) return InteractionGesture.SwipeToRight;

            gestureFound = swipeDetector.CheckUpSwipeGesture(bodyAngle, dataPointMap[user].GetQueue(joint));
            if (gestureFound) return InteractionGesture.SwipeUp;

            gestureFound = swipeDetector.CheckDownSwipeGesture(bodyAngle, dataPointMap[user].GetQueue(joint));
            if (gestureFound) return InteractionGesture.SwipeDown;

            return InteractionGesture.None;
        }

        private void initialize(KinectSensor sensor)
        {
            this.sensor = sensor;

            swipeDetector = new SwipeDetector(IConsts.GestureQueueSizeSwipe);
            circleDetector = new CircleDetector(IConsts.GestureQueueSizeCycle);
            pushPullDetector = new PushPullGestureDetector(IConsts.GestureQueueSizePush);

            foreach (KinectUser user in Enum.GetValues(typeof(KinectUser)))
            {
                dataPointMap[user] = new PointQueue(IConsts.GestureQueueSize);
            }
        }

        private void addDataPoint(JointType joint, KinectUser user)
        {
            if (getSekeltonService().hasJoint(joint, user))
            {
                dataPointMap[user].AddPoint(getSekeltonService().getDataPoint(joint, user), joint);
            }
        }

        private JointType isGestureActive(KinectUser user)
        {
            if (getSekeltonService().hasJoint(JointType.HandLeft, user)
                && getSekeltonService().hasJoint(JointType.HandRight, user)
                && getSekeltonService().hasJoint(JointType.ShoulderCenter, user))
            {
                KinectDataPoint handLeft = getSekeltonService().getDataPoint(JointType.HandLeft, user);
                KinectDataPoint handRight = getSekeltonService().getDataPoint(JointType.HandRight, user);
                KinectDataPoint shoulder = getSekeltonService().getDataPoint(JointType.ShoulderCenter, user);

                JointType? result = null;

                if (handLeft.ScreenY < shoulder.ScreenY && handLeft.ScreenY < handRight.ScreenY)
                {
                    result = JointType.HandRight;
                }
                else if (handRight.ScreenY < shoulder.ScreenY && handRight.ScreenY < handLeft.ScreenY)
                {
                    result = JointType.HandLeft;
                }

                if (null != result)
                {
                    if (null == lastActiveUser || user != lastActiveUser)
                    {
                        lastActiveUser = user;
                        dataPointMap[user].GetQueue(result.Value).Clear();
                    }
                    return result.Value;
                }
            }

            // if the last user is not active anymore -> set lastActiveUser to null
            if (null != lastActiveUser && user == lastActiveUser.Value)
            {
                lastActiveUser = null;
            }

            return default(JointType);
        }

        private bool isTimeOut()
        {
            if (gestureTime > DateTime.Now)
            {
                foreach (PointQueue queue in dataPointMap.Values)
                {
                    queue.ClearQueue();
                }
                return true;
            }
            return false;
        }

        public void focuseCurrentUser()
        {
            if (!isTimeOut())
            {
                foreach (KinectUser user in Enum.GetValues(typeof(KinectUser)))
                {
                    if (default(JointType) != isGestureActive(user))
                    {
                        KinectDataPoint point = getSekeltonService().getDataPoint(JointType.ShoulderCenter, user);

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

                                setGestureTimeOut(KINECT_ANGLE_TIMEOUT);
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

                                setGestureTimeOut(KINECT_ANGLE_TIMEOUT);
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