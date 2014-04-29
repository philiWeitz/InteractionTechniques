using System;
using System.Collections.Generic;
using InteractionUtil.Common;
using InteractionUtil.Util;
using KinectServices.Common;
using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace GestureServices.Gesture
{
    internal class GestureDetector
    {
        //private static readonly int KINECT_ANGLE_TIMEOUT = 2000;

        private KinectSensor sensor;
        private DateTime gestureTime;
        private ISkeletonService skeletonService = null;

        private UserDetector userDetector;
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
                        return new List<KinectDataPoint>(dataPointMap[user].GetQueue(JointType.HandLeft));
                    }
                    else if (JointType.HandRight == isGestureActive(user))
                    {
                        return new List<KinectDataPoint>(dataPointMap[user].GetQueue(JointType.HandRight));
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
            return userDetector.LastActiveUser;
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

            userDetector = new UserDetector(sensor);
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
            KinectDataPoint point = getSekeltonService().getDataPoint(joint, user);

            if (null != point)
            {
                dataPointMap[user].AddPoint(point, joint);
            }
        }

        private JointType isGestureActive(KinectUser user)
        {
            return userDetector.isGestureActive(user, dataPointMap[user]);
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
                //if (userDetector.focuseCurrentUser())
                //{
                //    setGestureTimeOut(KINECT_ANGLE_TIMEOUT);
                //}
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