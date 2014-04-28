using InteractionUtil.Service.Impl;

namespace InteractionUtil.Service.Interface
{
    public interface IConfigService
    {
        int VolumeStrength { get; set; }

        VolumeSpec VolumeEnabled { get; set; }

        void ReadConfigFromFile();

        void WriteConfigToFile();
    }
}