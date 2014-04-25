using Microsoft.Kinect;

namespace KinectServices.Service.Interface
{
    public interface ISensorService
    {
        bool sensorAvailable(int idx);

        void startSensor(int idx);

        void stopSensor(int idx);

        KinectSensor getSensor(int idx);
    }
}