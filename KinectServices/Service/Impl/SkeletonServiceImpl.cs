using System;
using System.Collections.Generic;
using System.Linq;
using KinectServices.Common;
using KinectServices.Service.Interface;
using Microsoft.Kinect;


namespace KinectServices.Service.Impl
{
    public class SkeletonServiceImpl : ISkeletonService
    {
        private static readonly int MAX_SKELETON_COUNT = 6;

        private DateTime timeStamp;
        private DepthImageFormat depthImageFormat;
        private Skeleton[] skeletons = new Skeleton[MAX_SKELETON_COUNT];

        private IDictionary<JointType, ColorImagePoint> colorPointJointMap =
            new Dictionary<JointType, ColorImagePoint>();
        private IDictionary<JointType, DepthImagePoint> depthPointJointMap =
            new Dictionary<JointType, DepthImagePoint>();


        public void enableSkeleton(KinectSensor sensor)
        {
            if (null != sensor)
            {
                sensor.AllFramesReady +=
                    new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);
            }
        }

        public KinectDataPoint getDataPoint(JointType type)
        {
            return new KinectDataPoint(colorPointJointMap[type], depthPointJointMap[type]);
        }

        public ColorImagePoint getColorPointJoint(JointType type)
        {
            return colorPointJointMap[type];
        }


        public DepthImagePoint getDepthPointJoint(JointType type)
        {
            return depthPointJointMap[type];
        }


        public bool hasJoint(JointType type)
        {
            return colorPointJointMap.ContainsKey(type);
        }

        private void printJointSpeed(JointType joint , ColorImagePoint newColorPoint)
        {
            if (colorPointJointMap.ContainsKey(joint))
            {
                ColorImagePoint oldColorPoint = colorPointJointMap[joint];

                int a = oldColorPoint.X - newColorPoint.X;
                int b = oldColorPoint.Y - newColorPoint.Y;

                double c = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
                double ticks = (DateTime.Now.Ticks - timeStamp.Ticks) / 100000;
                double pixelPerTicks = c / ticks;

                Console.WriteLine("Speed " + joint.ToString() + ": " + pixelPerTicks);
            }
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

                if (null != first)
                {
                    using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
                    {
                        // can happen that we have to drop frames
                        if (null == depthFrame)
                        {
                            return;
                        }

                        KinectSensor sensor = (KinectSensor) sender;
                        CoordinateMapper coordinateMapper = new CoordinateMapper(sensor);

                        depthImageFormat = depthFrame.Format;

                        // get the depth point out of the skeleton tracking
                        DepthImagePoint headDepthPoint = coordinateMapper.
                            MapSkeletonPointToDepthPoint(first.Joints[JointType.Head].Position, depthFrame.Format);
                        DepthImagePoint leftHandDepthPoint = coordinateMapper.
                            MapSkeletonPointToDepthPoint(first.Joints[JointType.HandLeft].Position, depthFrame.Format);
                        DepthImagePoint rightHandDepthPoint = coordinateMapper.
                            MapSkeletonPointToDepthPoint(first.Joints[JointType.HandRight].Position, depthFrame.Format);

                        depthPointJointMap.Clear();
                        depthPointJointMap[JointType.Head] = headDepthPoint;
                        depthPointJointMap[JointType.HandLeft] = leftHandDepthPoint;
                        depthPointJointMap[JointType.HandRight] = rightHandDepthPoint;

                        // gets the color points out of the skeleton tracking
                        ColorImagePoint headImagePoint = coordinateMapper.MapDepthPointToColorPoint(
                            depthFrame.Format, headDepthPoint, ColorImageFormat.RgbResolution640x480Fps30);
                        ColorImagePoint leftHandImagePoint = coordinateMapper.MapDepthPointToColorPoint(
                            depthFrame.Format, leftHandDepthPoint, ColorImageFormat.RgbResolution640x480Fps30);
                        ColorImagePoint rightHandImagePoint = coordinateMapper.MapDepthPointToColorPoint(
                            depthFrame.Format, rightHandDepthPoint, ColorImageFormat.RgbResolution640x480Fps30);

                        // debug - print left hand speed
                        //printJointSpeed(JointType.HandLeft, leftHandImagePoint);

                        colorPointJointMap.Clear();
                        colorPointJointMap[JointType.Head] = headImagePoint;
                        colorPointJointMap[JointType.HandLeft] = leftHandImagePoint;
                        colorPointJointMap[JointType.HandRight] = rightHandImagePoint;

                        this.timeStamp = DateTime.Now;
                    }
                }
            }
        }
    }
}
