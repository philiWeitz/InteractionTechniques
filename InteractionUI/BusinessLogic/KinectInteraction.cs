﻿using System;
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

        public event EventHandler GestureEvent;

        private ISensorService sensorService;
        private IProcessService processService;
        private IShortcutService shortcutService;
        private IGestureService gestureService;

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

            this.sensorIdx = sensorIdx;
            sensorService.startSensor(sensorIdx);
            gestureService.enableGestureService(sensorService.getSensor(sensorIdx));

            kinectTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);
            kinectTimer.Tick += new EventHandler(kinectTimer_Tick);
            kinectTimer.Interval = TimeSpan.FromMilliseconds(INTERVAL);
        }

        private void kinectTimer_Tick(object sender, EventArgs e)
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

                if (null != GestureEvent)
                {
                    GestureEvent.Invoke(this, new KinectInteractionArg(gesture.ToString()));
                }

                String shortCut = shortcutService.GetShortcut(gesture);
                processService.SendKeyToProcess(shortcutService.GetProcessName(), shortCut);

                System.Media.SystemSounds.Exclamation.Play();
                gestureService.setGestureTimeOut(timeOut);
            }

            gestureService.focuseCurrentUser();
        }
    }
}