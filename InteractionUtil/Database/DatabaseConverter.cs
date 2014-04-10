using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            foreach(KeyValuePair<InteractionGesture,String> item in definition.GestureMap)
            {
                result.GestureMap.Add(item.Key.ToString(), item.Value);
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

            foreach (KeyValuePair<String,String> item in dao.GestureMap)
            {
                InteractionGesture gesture;
                if (Enum.TryParse<InteractionGesture>(item.Key, out gesture))
                {
                    result.GestureMap[gesture] = item.Value;
                }
            }
            return result;
        }
    }
}
