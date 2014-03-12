
using Common.Logging;
using KinectServices.Service.Interface;
using Microsoft.Kinect;

namespace KinectServices.Service.Impl
{
    public class SensorServiceImpl : ISensorService
    {
        private ILog logger;

        public SensorServiceImpl()
        {
            initialize();
        }

        public bool sensorAvailable(int idx)
        {
            return (KinectSensor.KinectSensors.Count > idx && 
                KinectSensor.KinectSensors[idx].Status == KinectStatus.Connected);
        }

        public void startSensor(int idx)
        {
            if (KinectSensor.KinectSensors.Count > idx)
            {
                logger.Info("Kinect connected to sensor " + idx);
                KinectSensor sensor = KinectSensor.KinectSensors[idx];

                if (sensor.Status == KinectStatus.Connected && !sensor.IsRunning)
                {
                    TransformSmoothParameters smooth = new TransformSmoothParameters();
                    smooth.Smoothing = 0.75f;
                    smooth.Correction = 0.0f;
                    smooth.Prediction = 0.0f;
                    smooth.JitterRadius = 0.05f;
                    smooth.MaxDeviationRadius = 0.04f;

                    sensor.ColorStream.Enable();
                    sensor.DepthStream.Enable();
                    sensor.SkeletonStream.Enable(smooth);

                    sensor.DepthStream.Range = DepthRange.Near;
                    sensor.SkeletonStream.EnableTrackingInNearRange = true;
                    sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;

                    sensor.Start();
                }
                else
                {
                    logger.Error("Couldn't connect to sensor " + idx);
                }
            }
            else
            {
                logger.Error("Couldn't connect to sensor " + idx);
            }
        }

        public KinectSensor getSensor(int idx)
        {
            if (KinectSensor.KinectSensors.Count > idx)
            {
                return KinectSensor.KinectSensors[idx];
            }

            logger.Error("Couldn't connect to sensor " + idx);
            return null;
        }

        public void stopSensor(int idx)
        {
            if (KinectSensor.KinectSensors.Count > idx)
            {
                KinectSensor.KinectSensors[idx].Stop();
                KinectSensor.KinectSensors[idx].AudioSource.Stop();
            }
            else
            {
                logger.Error("Couldn't connect to sensor " + idx);
            }
        }

        private void initialize()
        {
            logger = LogManager.GetLogger(this.GetType());
        }
    }
}
