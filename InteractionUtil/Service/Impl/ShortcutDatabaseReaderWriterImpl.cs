using System;
using System.Collections.Generic;
using Datamodel.Model;
using Datamodel.Service.Impl;
using Datamodel.Service.Interface;
using InteractionUtil.Common;
using InteractionUtil.Service.Interface;

namespace InteractionUtil.Service.Impl
{
    internal class ShortcutDatabaseReaderWriterImpl : IShortcutReaderWriterService
    {
        private List<ShortcutDefinition> shortcutList = null;
        private List<ShortcutDefinition> activeShortcutList = null;

        private ShortcutDefDAOService shortCutService;

        public ShortcutDatabaseReaderWriterImpl()
        {
            shortCutService = new ShortcutDefDAOServiceImpl();
        }

        public List<ShortcutDefinition> GetShortCutList()
        {
            if (null == shortcutList)
            {
                ReadDefinitionsFromDirectory();
            }
            return shortcutList;
        }

        public List<ShortcutDefinition> GetActiveShortCutList()
        {
            if (null == activeShortcutList)
            {
                ReadDefinitionsFromDirectory();
            }
            return activeShortcutList;
        }

        public void AddShortcutDefinition(String name)
        {
            if (null == shortCutService.GetEntityByName(name))
            {
                ShortcutDefDAO item = new ShortcutDefDAO();
                item.Name = name;

                shortCutService.SaveEntity(item);
            }
        }

        public void SaveOrUpdateShortcutDefinition(ShortcutDefinition item)
        {
            ShortcutDefDAO databaseItem = DatabaseConverter.ShortcutDefinitionToDao(item);
            shortCutService.SaveOrUpdateEntity(databaseItem);
        }

        public void RemoveShortcutDefinition(ShortcutDefinition item)
        {
            shortCutService.DeleteEntityById(item.Id);
        }

        public List<ShortcutDefinition> ReadDefinitionsFromDirectory()
        {
            shortcutList = new List<ShortcutDefinition>();
            activeShortcutList = new List<ShortcutDefinition>();

            foreach (ShortcutDefDAO databaseItem in shortCutService.GetAll())
            {
                ShortcutDefinition def = DatabaseConverter.DaoToShortcutDefinition(databaseItem);
                shortcutList.Add(def);

                if (def.Active)
                {
                    activeShortcutList.Add(def);
                }
            }

            shortcutList.Sort();
            activeShortcutList.Sort();

            return shortcutList;
        }
    }
}