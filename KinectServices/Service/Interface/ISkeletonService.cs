using KinectServices.Common;
using Microsoft.Kinect;

namespace KinectServices.Service.Interface
{
    public interface ISkeletonService
    {
        void enableSkeleton(KinectSensor sensor);

        KinectDataPoint getDataPoint(JointType type);

        bool hasJoint(JointType type);

        bool userInRange();

        int getUserBodyAngle();
    }
}