using InteractionUtil.Service.Impl;

namespace InteractionUtil.Service.Interface
{
    public interface IConfigService
    {
        event EventHandler ConfigChanged;

        void PopulateChanges();

        double GestureTimeOut { get; set; }

        bool VolumeEnabled { get; set; }

        bool ActiveUserFeedbackEnabled { get; set; }

        bool NoUserInRangeFeedbackEnabled { get; set; }

        void ReadConfigFromFile();

        void WriteConfigToFile();
    }
}