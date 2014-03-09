using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Xml;
using System.IO;
using Common.Logging;
using InteractionUtil.Util;

namespace InteractionUI.Util
{
    class ShortCutUtil
    {
        private static ILog log = LogManager.GetLogger(typeof (ShortCutUtil));
        private static ReaderWriterLock readWriteLock = new ReaderWriterLock();

        private static ShortCutUtil instance;
        private static readonly string XML_NODE_DATA = "data";
        private static readonly string XML_ATTRIBUTE_NAME = "name";

        private String appName;
        private IDictionary<string, string> shortCuts = new Dictionary<string, string>();


        public static void SetApplicationName(String appName)
        {
            Instance.loadResourseFile(appName);
        }


        // gets the short cut string for the specified key
        public static string GetShortCut(String key)
        {
            return Instance._getShortCut(key);
        }

        // gets an instance of the short cut class
        private static ShortCutUtil Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new ShortCutUtil();
                }
                return instance;
            }
        }


        // internal function for getting a short cut
        private string _getShortCut(String key)
        {
            // set a reader lock to protect short cut dictionary
            readWriteLock.AcquireReaderLock(0);

            try
            {
                // if short cut contains the key -> get it
                if (shortCuts.ContainsKey(key))
                {
                    return Instance.shortCuts[key];
                }
                else
                {
                    log.Error("Couldn't find message with key \"" + key + "\"!");
                }
            }
            finally
            {
                // Ensure that the lock is released.
                readWriteLock.ReleaseReaderLock();
            }
            return String.Empty;
        }


        // internal function for initializing the short cut key to value map
        private void loadResourseFile(String appName)
        {
            // set a write lock to protect short cut dictionary
            readWriteLock.AcquireWriterLock(0);

            try
            {
                this.appName = appName;

                shortCuts.Clear();

                // gets the xml document for a specific language
                XmlDocument xmlDocument = new XmlDocument();

                try
                {
                    xmlDocument.Load(InteractionConsts.SHORT_CUT_DIRECTORY + appName);
                }
                catch (FileNotFoundException e)
                {
                    log.Error("Couldn't find language file \"" + InteractionConsts.SHORT_CUT_DIRECTORY + appName + "\"!", e);
                    return;
                }

                // reads all short cuts key value pairs from the xml document and stores them
                XmlNodeList nodes = xmlDocument.GetElementsByTagName(XML_NODE_DATA);
                foreach (XmlNode node in nodes)
                {
                    shortCuts.Add(node.Attributes.
                        GetNamedItem(XML_ATTRIBUTE_NAME).Value, node.InnerText.Trim());
                }
            }
            finally
            {
                // Ensure that the lock is released.
                readWriteLock.ReleaseWriterLock();
            }
        }
    }
}
