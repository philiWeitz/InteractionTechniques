using System;
using System.Windows.Threading;
using InteractionUI.Util;
using InteractionUtil.Service.Interface;
using InteractionUtil.Util;
using KinectServices.Common;
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
        private IInteractionService interactionService;

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
            interactionService = SpringUtil.getService<IInteractionService>();

            sensorService.startSensor(sensorIdx);
            interactionService.enableInteractionService(sensorService.getSensor(sensorIdx));

            kinectTimer = new DispatcherTimer(DispatcherPriority.SystemIdle);
            kinectTimer.Tick += new EventHandler(kinectTimer_Tick);
            kinectTimer.Interval = TimeSpan.FromMilliseconds(INTERVAL);
        }

        private void kinectTimer_Tick(object sender, EventArgs e)
        {
            bool detected = false;

            foreach (InteractionGesture gesture in Enum.GetValues(typeof(InteractionGesture)))
            {
                if (interactionService.checkGesture(gesture))
                {
                    if (InteractionGesture.PushTwoHanded == gesture)
                    {
                        ApplicationList.NextProgram();
                    }
                    else if (InteractionGesture.PullTwoHanded == gesture)
                    {
                        ApplicationList.PreviousProgram();
                    }

                    if (null != GestureEvent)
                    {
                        GestureEvent.Invoke(this, new KinectInteractionArg(gesture.ToString()));
                    }

                    String shortCut = ShortCutUtil.GetShortCut(gesture.ToString());
                    processService.SendKeyToProcess(ShortCutUtil.GetProcessName(), shortCut);

                    detected = true;
                    break;
                }
            }

            if (detected)
            {
                System.Media.SystemSounds.Exclamation.Play();
                interactionService.setGestureTimeOut(800);
            }
        }
    }
}
