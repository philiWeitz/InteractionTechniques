using System;
using System.IO;
using System.Xml;
using InteractionUtil.Properties;
using InteractionUtil.Service.Interface;

namespace InteractionUtil.Service.Impl
{
    public delegate void EventHandler();

    internal class ConfigServiceImpl : IConfigService
    {
        private static readonly String ROOT = "root";

        public event EventHandler ConfigChanged;

        private String fileName = Directory.GetCurrentDirectory() + @"\potatoConf.xml";

        public int GestureTimeOut { get; set; }

        public bool VolumeEnabled { get; set; }

        public bool ActiveUserFeedbackEnabled { get; set; }

        public ConfigServiceImpl()
        {
            VolumeEnabled = true;
            ActiveUserFeedbackEnabled = true;
            GestureTimeOut = int.Parse(Resource.ConfGestureTimeOut);
        }

        public void ReadConfigFromFile()
        {
            if (File.Exists(fileName))
            {
                xmlToConf();
            }

            PopulateChanges();
        }

        public void WriteConfigToFile()
        {
            using (StreamWriter file = new StreamWriter(fileName))
            {
                file.Write(confToXml());
            }

            PopulateChanges();
        }

        public void PopulateChanges()
        {
            if (null != ConfigChanged)
            {
                ConfigChanged.Invoke();
            }
        }

        private String confToXml()
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlNode root = xmlDoc.CreateElement(ROOT);
            xmlDoc.AppendChild(root);

            addNode(xmlDoc, root, "GestureTimeOut", GestureTimeOut);
            addNode(xmlDoc, root, "VolumeEnabled", VolumeEnabled);
            addNode(xmlDoc, root, "ActiveUserFeedbackEnabled", ActiveUserFeedbackEnabled);

            return xmlDoc.InnerXml;
        }

        private void xmlToConf()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);

            GestureTimeOut = getFirstNodeItem<int>(xmlDoc, "GestureTimeOut");
            VolumeEnabled = getFirstNodeItem<bool>(xmlDoc, "VolumeEnabled");
            ActiveUserFeedbackEnabled = getFirstNodeItem<bool>(xmlDoc, "ActiveUserFeedbackEnabled");
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