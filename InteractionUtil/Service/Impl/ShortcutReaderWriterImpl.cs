using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using InteractionUtil.Common;
using InteractionUtil.Service.Interface;
using InteractionUtil.Util;


namespace InteractionUtil.Service.Impl
{
    internal class ShortcutReaderWriterImpl : IShortcutReaderWriterService
    {
        private static readonly String ROOT = "root";
        private static readonly String NODE_IDX = "Index"; 
        private static readonly String NODE_ACTIVE = "Active";       
        private static readonly String NODE_PROCESS = "ProcessName";        
        
        private String shortCutPath;
        private List<ShortcutDefinition> shortcutList = null;
        private List<ShortcutDefinition> activeShortcutList = null;


        public ShortcutReaderWriterImpl()
        {
            shortCutPath = Directory.GetCurrentDirectory() + @"\" + IConsts.SHORT_CUT_DIRECTORY;
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
            using (StreamWriter file = new StreamWriter(shortCutPath + name + ".xml"))
            {
                file.Write(shortcutDefinitionToXml(new ShortcutDefinition()));
            }
        }

        public void UpdateShortcutDefinition(ShortcutDefinition item)
        {
            if (item.OldName != null && item.OldName != item.Name)
            {
                if (File.Exists(shortCutPath + item.OldName + ".xml"))
                {
                    File.Delete(shortCutPath + item.OldName + ".xml");
                }
            }

            using (StreamWriter file = new StreamWriter(shortCutPath + item.Name + ".xml"))
            {
                file.Write(shortcutDefinitionToXml(item));
            }
        }

        public void RemoveShortcutDefinition(ShortcutDefinition item)
        {
            if (File.Exists(shortCutPath + item.Name + ".xml"))
            {
                File.Delete(shortCutPath + item.Name + ".xml");
                shortcutList.Remove(item);
                activeShortcutList.Remove(item);
            }
        }

        public List<ShortcutDefinition> ReadDefinitionsFromDirectory()
        {
            shortcutList = new List<ShortcutDefinition>();
            activeShortcutList = new List<ShortcutDefinition>();

            foreach (String file in Directory.GetFiles(shortCutPath,"*.xml"))
            {
                ShortcutDefinition def = xmlToShortcutDefinition(file);
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

        private String shortcutDefinitionToXml(ShortcutDefinition item)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlNode root = xmlDoc.CreateElement(ROOT);
            xmlDoc.AppendChild(root);

            addNode(xmlDoc, root, NODE_PROCESS, item.ProcessName);
            addNode(xmlDoc, root, NODE_ACTIVE, item.Active);
            addNode(xmlDoc, root, NODE_IDX, item.Idx);

            foreach (KeyValuePair<InteractionGesture,String> mapItem in item.GestureMap)
            {
                addNode(xmlDoc, root, mapItem.Key.ToString(), mapItem.Value);
            }
           
            return xmlDoc.InnerXml;
        }

        private ShortcutDefinition xmlToShortcutDefinition(String path)
        {
            ShortcutDefinition result = new ShortcutDefinition();
            result.Name = Path.GetFileNameWithoutExtension(path);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            XmlNodeList processNodes = xmlDoc.GetElementsByTagName(NODE_PROCESS);
            if (processNodes.Count > 0)
            {
                result.ProcessName = processNodes.Item(0).InnerText;
            }

            XmlNodeList activeNodes = xmlDoc.GetElementsByTagName(NODE_ACTIVE);
            if (activeNodes.Count > 0)
            {
                result.Active = Boolean.Parse(activeNodes.Item(0).InnerText);
            }

            XmlNodeList idxNodes = xmlDoc.GetElementsByTagName(NODE_IDX);
            if (idxNodes.Count > 0)
            {
                result.Idx = int.Parse(idxNodes.Item(0).InnerText);
            }

            foreach (InteractionGesture gesture in Enum.GetValues(typeof(InteractionGesture)))
            {
                XmlNodeList nodes = xmlDoc.GetElementsByTagName(gesture.ToString());
                if (nodes.Count > 0)
                {
                    result.GestureMap[gesture] = nodes.Item(0).InnerText;
                }
            }

            return result;
        }

        private void addNode(XmlDocument xmlDoc, XmlNode parent, String nodeName, Object innerText)
        {
            XmlNode node = xmlDoc.CreateElement(nodeName);
            node.InnerText = innerText.ToString();
            parent.AppendChild(node);
        }
    }
}
