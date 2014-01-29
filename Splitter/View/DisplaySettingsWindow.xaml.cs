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
using System.ComponentModel;
using Fizzi.Applications.Splitter.ViewModel;
using log4net;

namespace Fizzi.Applications.Splitter.View
{
    /// <summary>
    /// Interaction logic for DisplaySettingsWindow.xaml
    /// </summary>
    public partial class DisplaySettingsWindow : Window
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DisplaySettingsWindow));

        public DisplaySettingsWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var currentSelection = templateSelection.SelectedItem as IEditableObject;

            if (currentSelection != null)
            {
                try
                {
                    currentSelection.EndEdit();
                    MessageBox.Show(this, "Template was successfully saved.", "Success", MessageBoxButton.OK);
                }
                catch (Exception ex)
                {
                    logger.Error("Template saving failed.", ex);
                    MessageBox.Show(this, "Template save failed - see log in My Documents for details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var currentSelection = templateSelection.SelectedItem as IEditableObject;

            if (currentSelection != null) currentSelection.CancelEdit();

            Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IEditableObject oldSelection = null, newSelection = null; ;

            if (e.RemovedItems.Count > 0) oldSelection = e.RemovedItems[0] as IEditableObject;
            if (e.AddedItems.Count > 0) newSelection = e.AddedItems[0] as IEditableObject;

            if (oldSelection != null) oldSelection.CancelEdit();
            if (newSelection != null) newSelection.BeginEdit();
        }

        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {
            var mvm = (MainViewModel)DataContext;

            mvm.BeginResize();
            Close();
        }
    }
}
