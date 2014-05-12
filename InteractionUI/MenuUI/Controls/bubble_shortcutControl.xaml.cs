﻿using InteractionUtil.Common;
using InteractionUtil.Service.Interface;
using InteractionUtil.Util;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace InteractionUI
{
    /// <summary>
    /// Interaction logic for bubble_shortcutControl.xaml
    /// </summary>
    public partial class bubble_shortcutControl : UserControl
    {
        private IShortcutReaderWriterService shortcutService;
        public bubble_shortcutControl()
        {
            InitializeComponent();
            initialize();

        }

        private void initialize()
        {
            shortcutService = SpringUtil.getService<IShortcutReaderWriterService>();

            foreach(ShortcutDefinition item in shortcutService.ReadDefinitionsFromDirectory()) {
                tabcontrol.Items.Add(item);
            }
        }


       
}


    }
