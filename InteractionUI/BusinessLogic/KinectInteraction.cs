using System;
using System.Windows.Threading;
using GestureServices.Service.Interface;
using InteractionUtil.Common;
using InteractionUtil.Service.Interface;
using InteractionUtil.Util;
using KinectServices.Service.Interface;

namespace InteractionUI.BusinessLogic
{
    public class KinectInteractionArg : EventArgs
    {
        public KinectInteractionArg(String gestureName)
        {
            GestureName = gestureName;
        }

        public String GestureName { get; set; }
    }

    public class KinectInteraction
    {
        private static readonly int INTERVAL = 100;
        private static readonly int GESTURE_TIMEOUT = 800;
        private static readonly int CIRCLE_GESTURE_TIMEOUT = 150;

        public event EventHandler GestureEvent;

        private ISensorService sensorService;
        private IProcessService processService;
        private IShortcutService shortcutService;
        private IGestureService gestureService;
        private ISkeletonService skeletonService;

        private int sensorIdx;
        private DispatcherTimer kinectTimer;

        public String LastGesture { get; private set; }

        public KinectInteraction(int sensorIdx)
        {
            initialize(sensorIdx);
        }

        public void Start()
        {
            kinectTimer.Start();
        }

        public void Stop()
        {
            kinectTimer.Stop();
        }

        public bool isEnabled()
        {
            return kinectTimer.IsEnabled;
        }

        private void initialize(int sensorIdx)
        {
            LastGesture = String.Empty;

            processService = SpringUtil.getService<IProcessService>();
            sensorService = SpringUtil.getService<ISensorService>();
            shortcutService = SpringUtil.getService<IShortcutService>();
            gestureService = SpringUtil.getService<IGestureService>();
            skeletonService = SpringUtil.getService<ISkeletonService>();

            this.sensorIdx = sensorIdx;
            sensorService.startSensor(sensorIdx);
            gestureService.enableGestureService(sensorService.getSensor(sensorIdx));
            skeletonService.enableSkeleton(sensorService.getSensor(sensorIdx));

            kinectTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);
            kinectTimer.Tick += new EventHandler(kinectTimer_Tick);
            kinectTimer.Interval = TimeSpan.FromMilliseconds(INTERVAL);
        }

        private void kinectTimer_Tick(object sender, EventArgs e)
        {
            int timeOut = GESTURE_TIMEOUT;
            bool detected = false;

            if (skeletonService.userInRange())
            {
                foreach (InteractionGesture gesture in Enum.GetValues(typeof(InteractionGesture)))
                {
                    if (gestureService.checkGesture(gesture))
                    {
                        if (InteractionGesture.PushTwoHanded == gesture)
                        {
                            shortcutService.NextApplication();
                        }
                        else if (InteractionGesture.PullTwoHanded == gesture)
                        {
                            shortcutService.PreviousApplication();
                        }
                        else if (InteractionGesture.CircleClock == gesture ||
                            InteractionGesture.CircleCounterClock == gesture)
                        {
                            timeOut = CIRCLE_GESTURE_TIMEOUT;
                        }

                        if (null != GestureEvent)
                        {
                            GestureEvent.Invoke(this, new KinectInteractionArg(gesture.ToString()));
                        }

                        String shortCut = shortcutService.GetShortcut(gesture);
                        processService.SendKeyToProcess(shortcutService.GetProcessName(), shortCut);

                        detected = true;
                        break;
                    }
                }

                if (detected)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    gestureService.setGestureTimeOut(timeOut);
                }

                skeletonService.centerUser(sensorService.getSensor(sensorIdx));
            }
        }
    }
}