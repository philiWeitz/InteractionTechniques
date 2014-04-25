using System;
using GestureServices.Service.Interface;
using InteractionUtil.Common;
using InteractionUtil.Service.Interface;
using InteractionUtil.Util;
using KinectServices.Service.Interface;

namespace InteractionUI.BusinessLogic
{
    public class KinectInteractionControl
    {
        private static readonly int GESTURE_DISPLAY_TIME_IN_SEC = 3;

        private ISensorService sensorService;
        private IProcessService processService;
        private IShortcutService shortcutService;
        private IGestureService gestureService;

        private int sensorIdx;
        private DateTime lastGestureTime;

        public bool Enabled { get; set; }

        public String LastGesture { get; set; }

        public KinectInteractionControl(int sensorIdx)
        {
            initialize(sensorIdx);
        }

        private void initialize(int sensorIdx)
        {
            Enabled = true;
            LastGesture = String.Empty;
            lastGestureTime = DateTime.Now;

            processService = SpringUtil.getService<IProcessService>();
            sensorService = SpringUtil.getService<ISensorService>();
            shortcutService = SpringUtil.getService<IShortcutService>();
            gestureService = SpringUtil.getService<IGestureService>();

            this.sensorIdx = sensorIdx;
            sensorService.startSensor(sensorIdx);
            gestureService.enableGestureService(sensorService.getSensor(sensorIdx));
        }

        public void checkGesture()
        {
            if (Enabled)
            {
                InteractionGesture gesture = gestureService.checkAllGestures();

                if (InteractionGesture.None != gesture)
                {
                    int timeOut = IConsts.GestureTimeOut;

                    if (InteractionGesture.PushTwoHanded == gesture)
                    {
                        shortcutService.NextApplication();
                    }
                    else if (InteractionGesture.CircleClock == gesture ||
                            InteractionGesture.CircleCounterClock == gesture)
                    {
                        timeOut = IConsts.GestureTimeOutContinuous;
                    }

                    String shortCut = shortcutService.GetShortcut(gesture);
                    processService.SendKeyToProcess(shortcutService.GetProcessName(), shortCut);

                    MediaManager.PlayTrack(gesture);
                    gestureService.setGestureTimeOut(timeOut);

                    LastGesture = gesture.ToString();
                    lastGestureTime = DateTime.Now.AddSeconds(GESTURE_DISPLAY_TIME_IN_SEC);
                }

                if (LastGesture != String.Empty && lastGestureTime < DateTime.Now)
                {
                    LastGesture = String.Empty;
                }
                gestureService.focuseCurrentUser();
            }
        }
    }
}