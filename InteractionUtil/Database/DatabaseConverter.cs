using System;
using System.Collections.Generic;
using Datamodel.Model;

namespace InteractionUtil.Common
{
    internal static class DatabaseConverter
    {
        public static ShortcutDefDAO ShortcutDefinitionToDao(ShortcutDefinition definition)
        {
            ShortcutDefDAO result = new ShortcutDefDAO();

            result.Id = definition.Id;
            result.Idx = definition.Idx;
            result.Name = definition.Name;
            result.Active = definition.Active;
            result.ProcessName = definition.ProcessName;

            foreach (KeyValuePair<InteractionGesture, ShortcutItem> item in definition.GestureMap)
            {
                result.GestureMap.Add(item.Key.ToString(), ShortcutItemToDao(item.Value));
            }

            return result;
        }

        public static ShortcutDefinition DaoToShortcutDefinition(ShortcutDefDAO dao)
        {
            ShortcutDefinition result = new ShortcutDefinition();

            result.Id = dao.Id;
            result.Idx = dao.Idx;
            result.Name = dao.Name;
            result.Active = dao.Active;
            result.ProcessName = dao.ProcessName;

            foreach (KeyValuePair<String, ShortcutItemDAO> item in dao.GestureMap)
            {
                InteractionGesture gesture;
                if (Enum.TryParse<InteractionGesture>(item.Key, out gesture))
                {
                    result.GestureMap[gesture] = DaoToShortcutItem(item.Value);
                }
            }
            return result;
        }

        public static ShortcutItemDAO ShortcutItemToDao(ShortcutItem item)
        {
            ShortcutItemDAO result = new ShortcutItemDAO();

            result.Id = item.Id;
            result.Name = item.Name;
            result.Strength = item.Strength;

            return result;
        }

        public static ShortcutItem DaoToShortcutItem(ShortcutItemDAO dao)
        {
            ShortcutItem result = new ShortcutItem();

            result.Id = dao.Id;
            result.Name = dao.Name;
            result.Strength = dao.Strength;

            return result;
        }
    }
}