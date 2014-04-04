using System;
using System.Collections.Generic;
using System.Linq;
using InteractionUtil.Util;
using KinectServices.Common;
using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace KinectServices.Service.Impl
{
    // enum for controlling the kinect elevation angle 
    internal enum UserCenter
    {
        up,
        down,
        center
    }

    internal class SkeletonServiceImpl : ISkeletonService
    {
        private static readonly int MAX_SKELETON_COUNT = 6;

        private UserCenter userCenter = UserCenter.center;
        private Skeleton[] skeletons = new Skeleton[MAX_SKELETON_COUNT];
        private HashSet<KinectSensor> sensorSet = new HashSet<KinectSensor>();

        private IDictionary<JointType, KinectDataPoint> jointDataPointMap =
            new Dictionary<JointType, KinectDataPoint>();

        public void enableSkeleton(KinectSensor sensor)
        {
            if (null != sensor && !sensorSet.Contains(sensor))
            {
                sensorSet.Add(sensor);
                sensor.AllFramesReady +=
                    new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);
            }
        }

        public KinectDataPoint getDataPoint(JointType type)
        {
            return jointDataPointMap[type];
        }

        public bool hasJoint(JointType type)
        {
            return jointDataPointMap.ContainsKey(type);
        }

        public bool userInRange()
        {
            if (hasJoint(JointType.ShoulderCenter))
            {
                KinectDataPoint shoulderCenter = getDataPoint(JointType.ShoulderCenter);
                if (shoulderCenter.Z > IConsts.KinectMinDistance)
                {
                    return true;
                }
            }
            return false;
        }

        private void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                // can happen that we have to drop frames
                if (null == skeletonFrame)
                {
                    return;
                }

                skeletonFrame.CopySkeletonDataTo(skeletons);

                Skeleton first = (
                    from s in skeletons
                    where s.TrackingState == SkeletonTrackingState.Tracked
                    select s).FirstOrDefault();

                // clear last joint points
                jointDataPointMap.Clear();

                if (null != first)
                {
                    using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
                    {
                        // can happen that we have to drop frames
                        if (null == depthFrame)
                        {
                            return;
                        }

                        KinectSensor sensor = (KinectSensor)sender;
                        CoordinateMapper coordinateMapper = new CoordinateMapper(sensor);

                        jointDataPointMap[JointType.HandLeft] =
                            getDataPoint(first.Joints, JointType.HandLeft, coordinateMapper, depthFrame);
                        jointDataPointMap[JointType.HandRight] =
                            getDataPoint(first.Joints, JointType.HandRight, coordinateMapper, depthFrame);
                        jointDataPointMap[JointType.ShoulderCenter] =
                            getDataPoint(first.Joints, JointType.ShoulderCenter, coordinateMapper, depthFrame);
                    }
                }
            }
        }

        private KinectDataPoint getDataPoint(JointCollection joints, JointType joint,
            CoordinateMapper coordinateMapper, DepthImageFrame depthFrame)
        {
            DepthImagePoint depthPoint = coordinateMapper.
                MapSkeletonPointToDepthPoint(joints[joint].Position, depthFrame.Format);
            ColorImagePoint colorPoint = coordinateMapper.MapDepthPointToColorPoint(
                depthFrame.Format, depthPoint, ColorImageFormat.RgbResolution640x480Fps30);

            return new KinectDataPoint(colorPoint, depthPoint);
        }

        public void centerUser(KinectSensor sensor)
        {
            if (userInRange())
            {
                KinectDataPoint point = getDataPoint(JointType.ShoulderCenter);

                // user stands outside of the upper limit -> move kinect up
                if (point.Y < IConsts.KinectCenterUpperLimit)
                {
                    userCenter = UserCenter.up;
                }
                // user stands outside of the lower limit -> move kinect dow
                else if (point.Y > IConsts.KinectCenterLowerLimit)
                {
                    userCenter = UserCenter.down;
                }

                // centeres the user based on inner borders
                if (userCenter == UserCenter.up && point.Y < IConsts.KinectCenterUpperLimitInner)
                {
                    // move kinect up
                    if (sensor.ElevationAngle < sensor.MaxElevationAngle - 5)
                    {
                        System.Media.SystemSounds.Asterisk.Play();

                        int newAngle = Math.Min(sensor.MaxElevationAngle, sensor.ElevationAngle + 5);
                        sensor.ElevationAngle = newAngle;
                    }
                }
                else if (userCenter == UserCenter.down && point.Y > IConsts.KinectCenterLowerLimitInner)
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
}