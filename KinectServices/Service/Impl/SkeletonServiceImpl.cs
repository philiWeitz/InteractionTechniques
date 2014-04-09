using System;
using System.Collections.Generic;
using System.Linq;
using InteractionUtil.Util;
using KinectServices.Common;
using KinectServices.Service.Interface;
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
                if (shoulderCenter.Z > IConsts.KinectMinDistance)
                {
                    return true;
                }
            }
            return false;
        }

        public int getUserBodyAngle()
        {
            KinectDataPoint sLeft = jointDataPointMap[JointType.ShoulderLeft];
            KinectDataPoint sRight = jointDataPointMap[JointType.ShoulderRight];

            return sRight.CalcDepthAngle(sLeft);
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
                    KinectSensor sensor = (KinectSensor)sender;
                    CoordinateMapper coordinateMapper = new CoordinateMapper(sensor);

                    jointDataPointMap[JointType.HandLeft] =
                        getDataPointRelativeToBody(first.Joints, JointType.HandLeft, coordinateMapper);
                    jointDataPointMap[JointType.HandRight] =
                        getDataPointRelativeToBody(first.Joints, JointType.HandRight, coordinateMapper);

                    jointDataPointMap[JointType.ShoulderCenter] =
                        getDataPointAbsolut(first.Joints, JointType.ShoulderCenter, coordinateMapper);
                    jointDataPointMap[JointType.ShoulderLeft] =
                        getDataPointAbsolut(first.Joints, JointType.ShoulderLeft, coordinateMapper);
                    jointDataPointMap[JointType.ShoulderRight] =
                        getDataPointAbsolut(first.Joints, JointType.ShoulderRight, coordinateMapper);
                }
            }
        }

        private KinectDataPoint getDataPointAbsolut(JointCollection joints,
            JointType joint, CoordinateMapper coordinateMapper)
        {
            ColorImagePoint colorPoint = coordinateMapper.MapSkeletonPointToColorPoint(
                joints[joint].Position, ColorImageFormat.RgbResolution640x480Fps30);

            return new KinectDataPoint(colorPoint, joints[joint].Position);
        }

        private KinectDataPoint getDataPointRelativeToBody(JointCollection joints,
            JointType joint, CoordinateMapper coordinateMapper)
        {
            ColorImagePoint colorPoint = coordinateMapper.MapSkeletonPointToColorPoint(
                joints[joint].Position, ColorImageFormat.RgbResolution640x480Fps30);

            return new KinectDataPoint(colorPoint, joints[joint].Position, joints[JointType.ShoulderCenter].Position);
        }
    }
}