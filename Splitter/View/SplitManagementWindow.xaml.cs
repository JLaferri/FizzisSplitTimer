using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Fizzi.Applications.Splitter.ViewModel;

namespace Fizzi.Applications.Splitter.View
{
    /// <summary>
    /// Interaction logic for SplitManagementWindow.xaml
    /// </summary>
    public partial class SplitManagementWindow : Window
    {
        public SplitManagementWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var mvm = (MainViewModel)DataContext;

            mvm.CommitSplitManagementChanges();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
