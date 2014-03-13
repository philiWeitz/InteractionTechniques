using System;
using System.Collections.Generic;
using InteractionUtil.Common;
using InteractionUtil.Service.Interface;
using InteractionUtil.Util;

namespace InteractionUtil.Service.Impl
{
    internal class ShortcutServiceImpl : IShortcutService
    {
        int currIdx = 0;
        IShortcutReaderWriterService readerWriterService = null;


        public string GetShortcut(InteractionGesture key)
        {
            List<ShortcutDefinition> list = getReaderWriterService().GetShortCutList();
            
            if (list.Count > 0)
            {
                if (currIdx >= list.Count)
                {
                    currIdx = 0;
                }
                return list[currIdx].GestureMap[key];
            }

            return String.Empty;
        }


        public string GetProcessName()
        {
            List<ShortcutDefinition> list = getReaderWriterService().GetShortCutList();

            if (list.Count > 0)
            {
                if (currIdx >= list.Count)
                {
                    currIdx = 0;
                }
                return list[currIdx].ProcessName;
            }

            return String.Empty;
        }


        public void NextApplication()
        {
            currIdx = (currIdx + 1) % getReaderWriterService().GetShortCutList().Count;
        }


        public void PreviousApplication()
        {
            currIdx -= 1;
            if (currIdx < 0)
            {
                currIdx = getReaderWriterService().GetShortCutList().Count - 1;
            }
        }


        private IShortcutReaderWriterService getReaderWriterService()
        {
            if (null == readerWriterService)
            {
                readerWriterService = SpringUtil.getService<IShortcutReaderWriterService>();
            }
            return readerWriterService;
        }

    }
}
