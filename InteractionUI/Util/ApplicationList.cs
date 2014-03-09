using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using InteractionUtil.Util;

namespace InteractionUI.Util
{
    class ApplicationList : List<String>
    {
        private static Object lockObject = new Object();
        private static ApplicationList appList;
        private static int currIdx = 0;


        public ApplicationList()
        {
            lock (lockObject)
            {
                _initialize();
            }
        }

        public static void NextProgram()
        {
            lock (lockObject)
            {
                currIdx = (currIdx + 1) % Instance.Count;
                ShortCutUtil.SetApplicationName(Instance.ElementAt(currIdx));
            }
        }

        public static void PreviousProgram()
        {
            lock (lockObject)
            {
                currIdx -= 1;
                if (currIdx < 0)
                {
                    currIdx = Instance.Count - 1;
                }
                ShortCutUtil.SetApplicationName(Instance.ElementAt(currIdx));
            }
        }

        private static ApplicationList Instance
        {
            get
            {
                if (null == appList)
                {
                    appList = new ApplicationList();
                }
                return appList;
            }
        }

        private void _initialize()
        {
            // add all shortcut files in the short cut directory
            String shortCutPath = Directory.GetCurrentDirectory() + InteractionConsts.SHORT_CUT_DIRECTORY;
            String[] files = Directory.GetFiles(shortCutPath);

            foreach (String file in files)
            {
                this.Add(Path.GetFileName(file));
            }
        }
    }
}
