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

namespace Fizzi.Applications.Splitter.View
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            this.Closed += (sender, e) =>
            {
                var mvm = (ViewModel.MainViewModel)DataContext;
                mvm.SettingsViewModel.Refresh();
            };
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var mvm = (ViewModel.MainViewModel)DataContext;
            mvm.SettingsViewModel.Save();

            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
