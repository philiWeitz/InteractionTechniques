using System.Collections.Generic;
using System.Media;
using System.Windows.Media;
using InteractionUI.Properties;
using InteractionUtil.Common;

namespace InteractionUI.BusinessLogic
{
    internal class MediaManager
    {
        private static MediaManager instance;

        private MediaPlayer player = new MediaPlayer();

        private Dictionary<InteractionGesture, SoundPlayer> gestureToSoundMap =
            new Dictionary<InteractionGesture, SoundPlayer>();

        public static bool Mute { get; set; }

        public static void PlayTrack(InteractionGesture gesture)
        {
            Instance()._playTrack(gesture);
        }

        private static MediaManager Instance()
        {
            if (null == instance)
            {
                instance = new MediaManager();
            }
            return instance;
        }

        /*********************************************************/

        private MediaManager()
        {
            initialize();
        }

        private void initialize()
        {
            gestureToSoundMap[InteractionGesture.SwipeToLeft] = new SoundPlayer(Sounds.ToLeft);
            gestureToSoundMap[InteractionGesture.SwipeToRight] = new SoundPlayer(Sounds.ToRight);
            gestureToSoundMap[InteractionGesture.PushOneHanded] = new SoundPlayer(Sounds.Push);
        }

        private void _playTrack(InteractionGesture gesture)
        {
            if (!Mute)
            {
                SoundPlayer player;

                if (gestureToSoundMap.TryGetValue(gesture, out player))
                {
                    player.Play();
                }
                else
                {
                    SystemSounds.Exclamation.Play();
                }
            }
        }
    }
}