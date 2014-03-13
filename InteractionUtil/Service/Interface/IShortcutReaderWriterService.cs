using System;
using System.Collections.Generic;
using InteractionUtil.Common;

namespace InteractionUtil.Service.Interface
{
    public interface IShortcutReaderWriterService
    {
        void AddShortcutDefinition(String name);
        void UpdateShortcutDefinition(ShortcutDefinition item);
        void RemoveShortcutDefinition(ShortcutDefinition item);
        List<ShortcutDefinition> ReadDefinitionsFromDirectory();
        List<ShortcutDefinition> GetShortCutList();
    }
}
