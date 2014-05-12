using System;

namespace InteractionUtil.Common
{
    public class ShortcutItem : AbstractUiItem
    {
        public InteractionGesture ShortcutType { get; set; }

        public String ShortcutString { get; set; }

        public int Strength { get; set; }

        public ShortcutItem()
        {
            ShortcutType = InteractionGesture.None;
            ShortcutString = String.Empty;
            Strength = 1;
        }

        public ShortcutItem(String shortcutString, int strength)
        {
            ShortcutString = shortcutString;
            Strength = strength;
        }
    }
}