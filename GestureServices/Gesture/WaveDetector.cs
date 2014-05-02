using System.Collections.Generic;
using System.Linq;
using InteractionUtil.Util;
using KinectServices.Common;

namespace GestureServices.Gesture
{
    internal class WaveDetector
    {
        private int maxWaveTime;

        public WaveDetector(int maxWaveTime)
        {
            this.maxWaveTime = maxWaveTime;
        }

        public bool CheckWaveGesture(List<KinectDataPoint> queue)
        {
            for (int i = 0; i < queue.Count; ++i)
            {
                if (checkWaveGesture(queue, i))
                {
                    return true;
                }
            }

            return false;
        }

        private bool checkWaveGesture(List<KinectDataPoint> queue, int stepSize)
        {
            int minPoints = stepSize * 4 + 1;

            for (int i = 0; i < queue.Count - minPoints; ++i)
            {
                KinectDataPoint p1 = queue.ElementAt(i);
                KinectDataPoint p4 = queue.ElementAt(i + (3 * stepSize));

                if (p1.TimeStamp.AddMilliseconds(maxWaveTime) < p4.TimeStamp)
                {
                    break;
                }

                KinectDataPoint p2 = queue.ElementAt(i + stepSize);
                KinectDataPoint p3 = queue.ElementAt(i + (2 * stepSize));

                double dist1 = p1.CalcScreenDistance(p2);
                double dist2 = p2.CalcScreenDistance(p3);
                double dist3 = p3.CalcScreenDistance(p4);

                double gitterPosA = p1.CalcDistance3D(p3);
                double gitterPosB = p2.CalcDistance3D(p4);

                if (dist1 > IConsts.GWaveMinLength && dist2 > IConsts.GWaveMinLength
                    && dist3 > IConsts.GWaveMinLength && gitterPosA < IConsts.GWaveMaxGitter
                    && gitterPosB < IConsts.GWaveMaxGitter)
                {
                    return true;
                }
            }

            return false;
        }
    }
}