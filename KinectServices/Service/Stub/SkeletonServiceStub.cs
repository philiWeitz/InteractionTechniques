using System.Collections.Generic;
using System.Windows.Forms;
using KinectServices.Common;
using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace KinectServices.Service.Stub
{
    internal class SkeletonServiceStub : ISkeletonService
    {
        private IDictionary<JointType, KinectDataPoint> jointMap =
                    new Dictionary<JointType, KinectDataPoint>();

        public void enableSkeleton(KinectSensor sensor)
        {
            System.Timers.Timer timer = new System.Timers.Timer(100);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }

        public KinectDataPoint getDataPoint(JointType type, KinectUser user)
        {
            KinectDataPoint result = null;
            jointMap.TryGetValue(type, out result);

            return result;
        }

        public List<KinectUser> userInRange()
        {
            List<KinectUser> result = new List<KinectUser>();

            if (jointMap.ContainsKey(JointType.ShoulderCenter))
            {
                result.Add(KinectUser.User1);
            }
            return result;
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            jointMap.Clear();

            if (System.Windows.Forms.Control.MouseButtons.HasFlag(MouseButtons.Left))
            {
                KinectDataPoint shoulderCenter = new KinectDataPoint(0, 40, 2000, 320, 240);
                KinectDataPoint shoulderLeft = new KinectDataPoint(20, 40, 2000, 300, 240);
                KinectDataPoint shoulderRight = new KinectDataPoint(20, 40, 2000, 340, 240);

                KinectDataPoint handLeft = new KinectDataPoint(30, 10, 2000, 270, 210);
                KinectDataPoint handRight = new KinectDataPoint(30, 10, 2000, 350, 210);

                jointMap[JointType.ShoulderCenter] = shoulderCenter;
                jointMap[JointType.ShoulderLeft] = shoulderLeft;
                jointMap[JointType.ShoulderRight] = shoulderRight;
                jointMap[JointType.HandLeft] = handLeft;
                jointMap[JointType.HandRight] = handRight;
            }
            else if (System.Windows.Forms.Control.MouseButtons.HasFlag(MouseButtons.Right))
            {
                KinectDataPoint shoulderCenter = new KinectDataPoint(0, 40, 2000, 320, 240);
                KinectDataPoint shoulderLeft = new KinectDataPoint(20, 40, 2000, 300, 240);
                KinectDataPoint shoulderRight = new KinectDataPoint(20, 40, 2000, 340, 240);

                KinectDataPoint handLeft = new KinectDataPoint(30, 10, 2000, 270, 180);
                KinectDataPoint handRight = new KinectDataPoint(30, 10, 2000, 350, 260);

                jointMap[JointType.ShoulderCenter] = shoulderCenter;
                jointMap[JointType.ShoulderLeft] = shoulderLeft;
                jointMap[JointType.ShoulderRight] = shoulderRight;
                jointMap[JointType.HandLeft] = handLeft;
                jointMap[JointType.HandRight] = handRight;
            }
        }

        public int getUserBodyAngle(KinectUser user)
        {
            return 0;
        }
    }
}