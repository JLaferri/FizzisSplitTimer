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
using System.ComponentModel;
using Fizzi.Applications.Splitter.Common;

namespace Fizzi.Applications.Splitter.View
{
    /// <summary>
    /// This class is the code behind for auto-fitting the splits into the given window size.
    /// I'm not overly happy with the method used to accomplish this. Certainly open to suggestions to simplify and improve.
    /// </summary>
    partial class SplitViewer : UserControl, INotifyPropertyChanged
    {
        public static DependencyProperty SplitsProperty =
            DependencyProperty.Register("Splits", typeof(ViewModel.SplitRowDisplay[]), typeof(SplitViewer),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSplitsProperty_PropertyChanged)));
        public ViewModel.SplitRowDisplay[] Splits { get { return (ViewModel.SplitRowDisplay[])GetValue(SplitsProperty); } set { SetValue(SplitsProperty, value); } }

        public static DependencyProperty CurrentSplitProperty =
            DependencyProperty.Register("CurrentSplit", typeof(ViewModel.SplitRowDisplay), typeof(SplitViewer),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCurrentSplitProperty_PropertyChanged)));
        public ViewModel.SplitRowDisplay CurrentSplit { get { return (ViewModel.SplitRowDisplay)GetValue(CurrentSplitProperty); } set { SetValue(CurrentSplitProperty, value); } }

        public static DependencyProperty SeparatorContentProperty =
            DependencyProperty.Register("SeparatorContent", typeof(object), typeof(SplitViewer));
        public object SeparatorContent { get { return (object)GetValue(SeparatorContentProperty); } set { SetValue(SeparatorContentProperty, value); } }

        public static DependencyProperty SplitTemplateProperty =
            DependencyProperty.Register("SplitTemplate", typeof(DataTemplate), typeof(SplitViewer));
        public DataTemplate SplitTemplate { get { return (DataTemplate)GetValue(SplitTemplateProperty); } set { SetValue(SplitTemplateProperty, value); } }

        private List<SplitRowDisplayContainer> _displaySplits;
        public List<SplitRowDisplayContainer> DisplaySplits { get { return _displaySplits; } set { this.RaiseAndSetIfChanged("DisplaySplits", ref _displaySplits, value, PropertyChanged); } }

        private ViewModel.SplitRowDisplay _finalSplit;
        public ViewModel.SplitRowDisplay FinalSplit { get { return _finalSplit; } set { this.RaiseAndSetIfChanged("FinalSplit", ref _finalSplit, value, PropertyChanged); } }

        private bool _isFinalSplitDisplayed;
        public bool IsFinalSplitDisplayed { get { return _isFinalSplitDisplayed; } set { this.RaiseAndSetIfChanged("IsFinalSplitDisplayed", ref _isFinalSplitDisplayed, value, PropertyChanged); } }

        private int _maxItemCount;
        public int MaxItemCount { get { return _maxItemCount; } set { this.RaiseAndSetIfChanged("MaxItemCount", ref _maxItemCount, value, PropertyChanged); } }
        
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            var oldMaxItemCount = MaxItemCount;
            computeMaxItemCount();

            if (oldMaxItemCount != MaxItemCount)
            {
                organizeDisplays();
            }

            base.OnRenderSizeChanged(sizeInfo);
        }

        private void computeMaxItemCount()
        {
            var firstItemContainer = splitsItemControl.ItemContainerGenerator.ContainerFromIndex(0);
            var presenter = (ContentPresenter)firstItemContainer;

            if (presenter == null) MaxItemCount = 0;
            else
            {
                presenter.UpdateLayout();

                var maxHeight = splitsItemControl.ActualHeight;
                var splitHeight = presenter.ActualHeight;

                MaxItemCount = (int)Math.Floor(maxHeight / splitHeight);
            }
        }

        private void organizeDisplays()
        {
            if (Splits == null || FinalSplit == null) 
            {
                DisplaySplits = null;
                IsFinalSplitDisplayed = false;
                return;
            }

            List<SplitRowDisplayContainer> displaySplits;

            var isRunCompleted = FinalSplit.CurrentRunSplit != null && FinalSplit.CurrentRunSplit.Owner.IsCompleted;

            //Determine if final split will be displayed
            IsFinalSplitDisplayed = Splits.Length <= MaxItemCount || CurrentSplit == FinalSplit || isRunCompleted;

            int amountToGrab = MaxItemCount;

            //If the final split is not displayed, grab one less split in order to display it
            if (!IsFinalSplitDisplayed) amountToGrab--;

            if (CurrentSplit == null)
            {
                if (isRunCompleted) displaySplits = Splits.Skip(Splits.Length - amountToGrab).Take(amountToGrab).Select(r => new SplitRowDisplayContainer(r)).ToList();
                else displaySplits = Splits.Take(amountToGrab).Select(r => new SplitRowDisplayContainer(r)).ToList();
            }
            else
            {
                var currentSplitPosition = Array.IndexOf(Splits, CurrentSplit);

                if (currentSplitPosition < amountToGrab) displaySplits = Splits.Take(amountToGrab).Select(r => new SplitRowDisplayContainer(r)).ToList();
                else displaySplits = Splits.Skip(currentSplitPosition - amountToGrab + 1).Take(amountToGrab).Select(r => new SplitRowDisplayContainer(r)).ToList();
            }

            if (!IsFinalSplitDisplayed)
            {
                displaySplits.Add(new SplitRowDisplayContainer(FinalSplit, true));
            }

            DisplaySplits = displaySplits;
        }

        private static void OnSplitsProperty_PropertyChanged(DependencyObject dsender, DependencyPropertyChangedEventArgs de)
        {
            var viewer = (SplitViewer)dsender;

            if (viewer.Splits == null)
            {
                viewer.DisplaySplits = null;
                viewer.FinalSplit = null;
            }
            else
            {
                viewer.DisplaySplits = viewer.Splits.Select(r => new SplitRowDisplayContainer(r)).ToList();
                viewer.FinalSplit = viewer.Splits.Last();
            }

            viewer.computeMaxItemCount();
            viewer.organizeDisplays();
        }

        private static void OnCurrentSplitProperty_PropertyChanged(DependencyObject dsender, DependencyPropertyChangedEventArgs de)
        {
            var viewer = (SplitViewer)dsender;

            viewer.computeMaxItemCount();
            viewer.organizeDisplays();
        }

        public SplitViewer()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
