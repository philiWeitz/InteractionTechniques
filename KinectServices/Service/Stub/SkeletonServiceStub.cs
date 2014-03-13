using System.Collections.Generic;
using KinectServices.Common;
using KinectServices.Service.Interface;
using KinectServices.Util;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Input;

namespace KinectServices.Service.Stub
{
    internal class SkeletonServiceStub : ISkeletonService
    {
        private IDictionary<JointType, ColorImagePoint> jointMap =
                    new Dictionary<JointType, ColorImagePoint>();


        public void enableSkeleton(KinectSensor sensor)
        {
            System.Timers.Timer timer = new System.Timers.Timer(10);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        public KinectDataPoint getDataPoint(JointType type)
        {
            return null;
        }

        public ColorImagePoint getColorPointJoint(JointType type)
        {
            return jointMap[type];
        }

        public DepthImagePoint getDepthPointJoint(JointType type)
        {
            DepthImagePoint point = new DepthImagePoint();
            point.Depth = KinectConsts.MIN_DISTANCE + 1;

            return point;
        }

        public bool hasJoint(JointType type)
        {
            return jointMap.ContainsKey(type);
        }

        public bool userInRange()
        {
            return true;
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            MouseState current_mouse = Mouse.GetState();
 
            ColorImagePoint headImagePoint = new ColorImagePoint();
            headImagePoint.X = current_mouse.X;
            headImagePoint.Y = current_mouse.Y;

            jointMap.Clear();
            if (ButtonState.Pressed == current_mouse.LeftButton)
            {
                this.jointMap[JointType.HandLeft] = headImagePoint;
            }
            else if (ButtonState.Pressed == current_mouse.RightButton)
            {
                this.jointMap[JointType.HandRight] = headImagePoint;
            }
            else
            {
                this.jointMap[JointType.Head] = headImagePoint;
            }
        }
    }
}
