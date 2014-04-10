using System;
using System.Collections.Generic;

namespace InteractionUtil.Common
{
    public class ShortcutDefinition : IComparable
    {
        public Guid Id { get; set; }

        public int Idx { get; set; }

        public bool Active { get; set; }

        public String Name { get; set; }

        public String OldName { get; set; }

        public String ProcessName { get; set; }

        public Dictionary<InteractionGesture, String> GestureMap { get; set; }

        public ShortcutDefinition()
        {
            Idx = 0;
            Active = true;
            ProcessName = String.Empty;

            GestureMap = new Dictionary<InteractionGesture, string>();
            foreach (InteractionGesture gesture in Enum.GetValues(typeof(InteractionGesture)))
            {
                GestureMap.Add(gesture, String.Empty);
            }
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            ShortcutDefinition item = (ShortcutDefinition)obj;
            return this.Idx.CompareTo(item.Idx);
        }
    }
}