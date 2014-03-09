using KinectServices.Common;
using Microsoft.Kinect;

namespace KinectServices.Service.Interface
{
    public interface IInteractionService
    {
        void enableInteractionService(KinectSensor sensor);
        void clearInteractionQueue();
        bool checkGesture(InteractionGesture gesture);
    }
}
