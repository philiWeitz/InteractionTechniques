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
        private List<TabItem> _tabItems;
        private TabItem _tabAdd;
        public bubble_shortcutControl()
        {


            try
            {
                InitializeComponent();

                // initialize tabItem array
                _tabItems = new List<TabItem>();

                // add a tabItem with + in header 
                TabItem tabAdd = new TabItem();
                tabAdd.Header = "+";

                _tabItems.Add(tabAdd);

                // add first tab
                this.AddTabItem();

                // bind tab control
                tabDynamic.DataContext = _tabItems;

                tabDynamic.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private TabItem AddTabItem()
        {
            //int count = _tabItems.Count;

            //// create new tab item
            TabItem tab = new TabItem();
            //tab.Header = string.Format("Tab {0}", count);
            //tab.Name = string.Format("tab{0}", count);
            //tab.HeaderTemplate = tabDynamic.FindResource("TabHeader") as DataTemplate;

            //// add controls to tab item, this case I added just a textbox
            //TextBox txt = new TextBox();
            //txt.Name = "txt";
            //tab.Content = txt;

            //// insert tab item right before the last (+) tab item
            //_tabItems.Insert(count - 1, tab);
            return tab;
        }

        private void tabDynamic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TabItem tab = tabDynamic.SelectedItem as TabItem;

            //if (tab != null && tab.Header != null)
            //{
            //    //if (tab.Header.Equals(_addTabHeader))
            //    //{
            //    // clear tab control binding
            //    //tabDynamic.DataContext = null;

            //    // add new tab
            //    TabItem newTab = this.AddTabItem();

            //    // bind tab control
            //    tabDynamic.DataContext = _tabItems;

            //    // select newly added tab item
            //    tabDynamic.SelectedItem = newTab;
            //    //}
            //    //else
            //    //{
            //    // your code here...
            //    //}
            //}
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //string tabName = (sender as Button).CommandParameter.ToString();

            //var item = tabDynamic.Items.Cast<tabitem>().Where(i => i.Name.Equals(tabName)).SingleOrDefault();

            //TabItem tab = item as TabItem;

            //if (tab != null)
            //{
            //    if (_tabItems.Count < 3)
            //    {
            //        MessageBox.Show("Cannot remove last tab.");
            //    }
            //    else if (MessageBox.Show(string.Format("Are you sure you want to remove the tab '{0}'?", tab.Header.ToString()),
            //        "Remove Tab", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //    {
            //        // get selected tab
            //        TabItem selectedTab = tabDynamic.SelectedItem as TabItem;

            //        // clear tab control binding
            //        tabDynamic.DataContext = null;

            //        _tabItems.Remove(tab);

            //        // bind tab control
            //        tabDynamic.DataContext = _tabItems;

            //        // select previously selected tab. if that is removed then select first tab
            //        if (selectedTab == null || selectedTab.Equals(tab))
            //        {
            //            selectedTab = _tabItems[0];
            //        }
            //        tabDynamic.SelectedItem = selectedTab;
            //    }
            //}
}


    }
}