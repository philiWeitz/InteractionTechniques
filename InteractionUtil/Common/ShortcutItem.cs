using System;

namespace InteractionUtil.Common
{
    public class ShortcutItem : AbstractUiItem
    {
        public String Name { get; set; }

        public int Strength { get; set; }

        public ShortcutItem()
        {
            Name = String.Empty;
            Strength = 1;
        }

        public ShortcutItem(String name, int strenght)
        {
            Name = name;
            Strength = strenght;
        }
    }
}