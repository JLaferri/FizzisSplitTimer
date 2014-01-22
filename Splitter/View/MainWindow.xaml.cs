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
                    ForceChangeWindowSize(mvm.CurrentFile.DisplayTemplate.WindowHeight, mvm.CurrentFile.DisplayTemplate.WindowWidth);
                }
            };

            this.Closing += (sender, e) =>
            {
                mvm.CheckMergeSuggested();      
            };
        }

        public void ForceChangeWindowSize(double height, double width)
        {
            var tempMinHeight = MinHeight;
            var tempMaxHeight = MaxHeight;

            MinHeight = height;
            MaxHeight = height;
            Height = height;

            MinHeight = tempMinHeight;
            MaxHeight = tempMaxHeight;

            var tempMinWidth = MinWidth;
            var tempMaxWidth = MaxWidth;

            MinWidth = width;
            MaxWidth = width;
            Width = width;

            MinWidth = tempMinWidth;
            MaxWidth = tempMaxWidth;
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
            ShowDisplaySettingsDialog();
        }

        public void ShowDisplaySettingsDialog()
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

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var mvm = (ViewModel.MainViewModel)DataContext;

            mvm.SettingsWindowOpen = true;
            mvm.PrepareSplitManagementWindow();

            var window = new SplitManagementWindow()
            {
                DataContext = mvm,
                Owner = this
            };

            window.ShowDialog();

            mvm.SettingsWindowOpen = false;
        }
    }
}
