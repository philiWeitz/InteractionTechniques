﻿using System;
using System.Collections.Generic;
using System.Linq;
using KinectServices.Common;
using KinectServices.Service.Interface;
using KinectServices.Util;
using Microsoft.Kinect;

namespace KinectServices.Service.Impl
{
    internal class SkeletonServiceImpl : ISkeletonService
    {
        private static readonly int MAX_SKELETON_COUNT = 6;

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
                if (shoulderCenter.Z > KinectConsts.MIN_DISTANCE)
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
    }
}