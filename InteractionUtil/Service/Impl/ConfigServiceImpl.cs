using System;
using System.IO;
using System.Xml;
using InteractionUtil.Properties;
using InteractionUtil.Service.Interface;

namespace InteractionUtil.Service.Impl
{
    public enum VolumeSpec
    {
        Enabled = 0,
        Disabled = 1,
        EnabledOnlyLocale = 2,
        EnabledOnlyRemote = 3
    }

    internal class ConfigServiceImpl : IConfigService
    {
        private static readonly String ROOT = "root";

        private String fileName = Directory.GetCurrentDirectory() + @"\potatoConf.xml";

        public int GestureTimeOut { get; set; }

        public int VolumeStrength { get; set; }

        public VolumeSpec VolumeEnabled { get; set; }

        public ConfigServiceImpl()
        {
            GestureTimeOut = int.Parse(Resource.ConfGestureTimeOut);
            VolumeStrength = int.Parse(Resource.ConfVolumeStrength);
        }

        public void ReadConfigFromFile()
        {
            if (File.Exists(fileName))
            {
                xmlToConf();
            }
        }

        public void WriteConfigToFile()
        {
            using (StreamWriter file = new StreamWriter(fileName))
            {
                file.Write(confToXml());
            }
        }

        private String confToXml()
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlNode root = xmlDoc.CreateElement(ROOT);
            xmlDoc.AppendChild(root);

            addNode(xmlDoc, root, "GestureTimeOut", GestureTimeOut);
            addNode(xmlDoc, root, "VolumeStrength", VolumeStrength);
            addNode(xmlDoc, root, "VolumeEnabled", VolumeEnabled);

            return xmlDoc.InnerXml;
        }

        private void xmlToConf()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);

            GestureTimeOut = getFirstNodeItem<int>(xmlDoc, "GestureTimeOut");
            VolumeStrength = getFirstNodeItem<int>(xmlDoc, "VolumeStrength");
            VolumeEnabled = getFirstNodeItem<VolumeSpec>(xmlDoc, "VolumeEnabled");
        }

        private T getFirstNodeItem<T>(XmlDocument xmlDoc, String nodeName)
        {
            XmlNodeList processNodes = xmlDoc.GetElementsByTagName(nodeName);

            if (processNodes.Count > 0)
            {
                Type type = typeof(T);

                if (type.IsEnum)
                {
                    return (T)Enum.Parse(type, processNodes.Item(0).InnerText, true);
                }
                else
                {
                    return (T)Convert.ChangeType(processNodes.Item(0).InnerText, type);
                }
            }
            return default(T);
        }

        private void addNode(XmlDocument xmlDoc, XmlNode parent, String nodeName, Object innerText)
        {
            XmlNode node = xmlDoc.CreateElement(nodeName);
            node.InnerText = innerText.ToString();
            parent.AppendChild(node);
        }
    }
}