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
        private ISensorService sensorService;
        private IProcessService processService;
        private IShortcutService shortcutService;
        private IGestureService gestureService;

        private int sensorIdx;

        public bool Enabled { get; set; }

        public String LastGesture { get; private set; }

        public KinectInteractionControl(int sensorIdx)
        {
            initialize(sensorIdx);
        }

        private void initialize(int sensorIdx)
        {
            Enabled = true;
            LastGesture = String.Empty;

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
            LastGesture = String.Empty;

            if (Enabled)
            {
                InteractionGesture gesture = gestureService.checkAllGestures();

                if (InteractionGesture.None != gesture)
                {
                    int timeOut = IConsts.GestureTimeOut;

                    if (InteractionGesture.CircleClock == gesture ||
                            InteractionGesture.CircleCounterClock == gesture)
                    {
                        timeOut = IConsts.GestureTimeOutContinuous;
                    }

                    String shortCut = shortcutService.GetShortcut(gesture);
                    processService.SendKeyToProcess(shortcutService.GetProcessName(), shortCut);

                    System.Media.SystemSounds.Exclamation.Play();
                    gestureService.setGestureTimeOut(timeOut);
                    LastGesture = gesture.ToString();
                }

                gestureService.focuseCurrentUser();
            }
        }
    }
}