using System;
using System.Collections.Generic;
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

    internal class UserDetector
    {
        private ISkeletonService skeletonService = null;
        private UserCenter userCenter = UserCenter.center;
        private KinectSensor sensor;

        public KinectUser? LastActiveUser { get; private set; }

        public UserDetector(KinectSensor sensor)
        {
            this.sensor = sensor;
            LastActiveUser = null;
        }

        public JointType isGestureActive(KinectUser user, PointQueue queue)
        {
            KinectDataPoint handLeft = getSekeltonService().getDataPoint(JointType.HandLeft, user);
            KinectDataPoint handRight = getSekeltonService().getDataPoint(JointType.HandRight, user);
            KinectDataPoint shoulder = getSekeltonService().getDataPoint(JointType.ShoulderCenter, user);

            if (null != handLeft && null != handRight && null != shoulder)
            {
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
                    if (null == LastActiveUser || user != LastActiveUser)
                    {
                        LastActiveUser = user;
                        queue.GetQueue(result.Value);
                    }
                    return result.Value;
                }
            }

            // if the last user is not active anymore -> set lastActiveUser to null
            if (null != LastActiveUser && user == LastActiveUser.Value)
            {
                LastActiveUser = null;
            }

            return default(JointType);
        }

        public bool focuseCurrentUser(IDictionary<KinectUser, PointQueue> dataPointMap)
        {
            bool result = false;

            foreach (KinectUser user in getSekeltonService().userInRange())
            {
                if (default(JointType) != isGestureActive(user, dataPointMap[user]))
                {
                    KinectDataPoint point = getSekeltonService().getDataPoint(JointType.ShoulderCenter, user);

                    if (null != point)
                    {
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

                                result = true;
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

                                result = true;
                            }
                        }
                        else
                        {
                            // user is centered
                            userCenter = UserCenter.center;
                        }
                    }
                }
                else
                {
                    // is user out of range -> user is concidered to be centered
                    userCenter = UserCenter.center;
                }
            }

            return result;
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