using InteractionUtil.Common;

namespace InteractionUtil.Service.Interface
{
    public interface IShortcutService
    {
        ShortcutItem GetShortcut(InteractionGesture key);

        string GetProcessName();

        void NextApplication();

        void PreviousApplication();
    }
}