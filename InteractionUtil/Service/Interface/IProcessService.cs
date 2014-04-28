using System;
using InteractionUtil.Common;

namespace InteractionUtil.Service.Interface
{
    public interface IProcessService
    {
        void SendKeyToProcess(String name, ShortcutItem item);
    }
}