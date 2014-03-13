﻿using System;
using System.Collections.Generic;


namespace InteractionUtil.Common
{
    public class ShortcutDefinition
    {
        public String Name { get; set; }
        public String OldName { get; set; }
        public String ProcessName { get; set; }
        public Dictionary<InteractionGesture, String> GestureMap { get; set; }

        public ShortcutDefinition()
        {
            ProcessName = String.Empty;

            GestureMap = new Dictionary<InteractionGesture, string>();
            foreach (InteractionGesture gesture in Enum.GetValues(typeof(InteractionGesture)))
            {
                GestureMap.Add(gesture, String.Empty);
            }
        }
    }
}
