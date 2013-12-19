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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fizzi.Applications.Splitter.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var mvm = (ViewModel.MainViewModel)DataContext;

            //Monitor MainViewModel CurrentFile changed event in order to set window size.
            //This is done in code behind because Min and Max sizes must be controlled very specifically for
            //the window to force a resize while maintaining the ability to resize manually
            mvm.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "CurrentFile")
                {
                    var tempMinHeight = MinHeight;
                    var tempMaxHeight = MaxHeight;

                    MinHeight = mvm.CurrentFile.DisplaySettings.WindowHeight;
                    MaxHeight = mvm.CurrentFile.DisplaySettings.WindowHeight;

                    MinHeight = tempMinHeight;
                    MaxHeight = tempMaxHeight;

                    var tempMinWidth = MinWidth;
                    var tempMaxWidth = MaxWidth;

                    MinWidth = mvm.CurrentFile.DisplaySettings.WindowWidth;
                    MaxWidth = mvm.CurrentFile.DisplaySettings.WindowWidth;

                    MinWidth = tempMinWidth;
                    MaxWidth = tempMaxWidth;
                }
            };

            this.Closing += (sender, e) =>
            {
                mvm.CheckMergeSuggested();      
            };
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var mvm = (ViewModel.MainViewModel)DataContext;

            mvm.SettingsWindowOpen = true;

            var window = new SettingsWindow()
            {
                DataContext = mvm,
                Owner = this
            };

            window.ShowDialog();

            mvm.SettingsWindowOpen = false;
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }

        private void DisplaySettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var mvm = (ViewModel.MainViewModel)DataContext;

            mvm.SettingsWindowOpen = true;

            var window = new DisplaySettingsWindow()
            {
                DataContext = mvm,
                Owner = this
            };

            window.ShowDialog();

            mvm.SettingsWindowOpen = false;
        }
    }
}
