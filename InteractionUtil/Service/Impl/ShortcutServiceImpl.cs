using System;
using System.Collections.Generic;
using InteractionUtil.Common;
using InteractionUtil.Service.Interface;
using InteractionUtil.Util;

namespace InteractionUtil.Service.Impl
{
    internal class ShortcutServiceImpl : IShortcutService
    {
        private int currIdx = 0;
        private IShortcutReaderWriterService readerWriterService = null;

        public string GetShortcut(InteractionGesture key)
        {
            List<ShortcutDefinition> list = getReaderWriterService().GetActiveShortCutList();

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
            List<ShortcutDefinition> list = getReaderWriterService().GetActiveShortCutList();

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
            if (getReaderWriterService().GetActiveShortCutList().Count > 0)
            {
                currIdx = (currIdx + 1) % getReaderWriterService().GetActiveShortCutList().Count;
            }
            else
            {
                currIdx = 0;
            }
        }

        public void PreviousApplication()
        {
            if (getReaderWriterService().GetActiveShortCutList().Count > 0)
            {
                currIdx -= 1;
                if (currIdx < 0)
                {
                    currIdx = getReaderWriterService().GetActiveShortCutList().Count - 1;
                }
            }
            else
            {
                currIdx = 0;
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