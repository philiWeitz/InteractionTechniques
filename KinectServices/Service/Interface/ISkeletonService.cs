using System.Collections.Generic;
using KinectServices.Common;
using Microsoft.Kinect;

namespace KinectServices.Service.Interface
{
    public interface ISkeletonService
    {
        void enableSkeleton(KinectSensor sensor);

        KinectDataPoint getDataPoint(JointType type, KinectUser user);

        List<KinectUser> userInRange();

        int getUserBodyAngle(KinectUser user);
    }
}