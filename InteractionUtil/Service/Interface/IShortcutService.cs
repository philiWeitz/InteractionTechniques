using InteractionUtil.Common;

namespace InteractionUtil.Service.Interface
{
    public interface IShortcutService
    {
        string GetShortcut(InteractionGesture key);
        string GetProcessName();
        void NextApplication();
        void PreviousApplication();
    }
}
