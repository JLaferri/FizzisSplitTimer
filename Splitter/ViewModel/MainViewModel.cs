using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Fizzi.Applications.Splitter.Model;
using System.ComponentModel;
using Fizzi.Applications.Splitter.Common;
using System.Windows.Input;
using System.Reactive.Linq;
using System.Reactive;
using System.Windows;

namespace Fizzi.Applications.Splitter.ViewModel
{
    class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        public string Version { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        private SplitFile _currentFile;
        public SplitFile CurrentFile { get { return _currentFile; } set { this.RaiseAndSetIfChanged("CurrentFile", ref _currentFile, value, PropertyChanged); } }

        private Run _currentRun;
        public Run CurrentRun { get { return _currentRun; } set { this.RaiseAndSetIfChanged("CurrentRun", ref _currentRun, value, PropertyChanged); } }

        private SplitRowDisplay[] _splitRows;
        public SplitRowDisplay[] SplitRows { get { return _splitRows; } set { this.RaiseAndSetIfChanged("SplitRows", ref _splitRows, value, PropertyChanged); } }

        private SplitRowDisplay _previousSplitRow;
        public SplitRowDisplay PreviousSplitRow { get { return _previousSplitRow; } set { this.RaiseAndSetIfChanged("PreviousSplitRow", ref _previousSplitRow, value, PropertyChanged); } }

        public Timer LiveTimer { get; private set; }

        public ICommand SaveSplits { get; private set; }
        public ICommand SaveSplitsAs { get; private set; }
        public ICommand OpenFileCommand { get; private set; }
        public ICommand ImportFromWsplitCommand { get; private set; }

        private bool _resizeEnabled;
        public bool ResizeEnabled { get { return _resizeEnabled; } set { this.RaiseAndSetIfChanged("ResizeEnabled", ref _resizeEnabled, value, PropertyChanged); } }

        private KeyboardListener keyListener = new KeyboardListener();

        public MainViewModel()
        {
            //var splitList = new List<SplitInfo>();
            //splitList.Add(new SplitInfo() { Name = "Cure 1", PersonalBestSplit = new SplitTimeSpan(TimeSpan.FromSeconds(106)), SumOfBestSplit = new SplitTimeSpan(TimeSpan.FromSeconds(103.61)) });
            //splitList.Add(new SplitInfo() { Name = "Cure 2", PersonalBestSplit = new SplitTimeSpan(TimeSpan.FromSeconds(206.03 - 106)), SumOfBestSplit = new SplitTimeSpan(TimeSpan.FromSeconds(94.11)) });
            //splitList.Add(new SplitInfo() { Name = "Cure 3", PersonalBestSplit = new SplitTimeSpan(TimeSpan.FromSeconds(411.09 - 206.03)), SumOfBestSplit = new SplitTimeSpan(TimeSpan.FromSeconds(178.83)) });
            //splitList.Add(new SplitInfo() { Name = "Cure 4", PersonalBestSplit = new SplitTimeSpan(TimeSpan.FromSeconds(535.52 - 411.09)), SumOfBestSplit = new SplitTimeSpan(TimeSpan.FromSeconds(118.57)) });
            //splitList.Add(new SplitInfo() { Name = "Cure 5", PersonalBestSplit = new SplitTimeSpan(TimeSpan.FromSeconds(654.04 - 535.52)), SumOfBestSplit = new SplitTimeSpan(TimeSpan.FromSeconds(117.34)) });
            //splitList.Add(new SplitInfo() { Name = "Cure 6", PersonalBestSplit = new SplitTimeSpan(TimeSpan.FromSeconds(654.04 - 535.52)), SumOfBestSplit = new SplitTimeSpan(TimeSpan.FromSeconds(117.34)) });

            //var splitFile = new SplitFile("Cure Time Monitoring", splitList.ToArray());

            //splitFile.Path = @"C:\cure_time.fsp";
            //splitFile.Save();

            LiveTimer = new Timer(30);
            ResizeEnabled = false;

            //Subscribe to keyboard events
            keyListener.KeyDown += (sender, e) =>
            {
                switch (e.Key)
                {
                    case Key.Right:
                        if (CurrentRun != null) CurrentRun.Split();
                        break;
                    case Key.Left:
                        if (CurrentRun != null) CurrentRun.Unsplit();
                        break;
                    case Key.PageDown:
                        if (CurrentRun != null) CurrentRun.SkipSplit();
                        break;
                    case Key.End:
                        if (CurrentFile != null)
                        {
                            if (CurrentFile.CheckMergeSuggested(CurrentRun))
                            {
                                var result = MessageBox.Show("We have detected that your current run has unsaved gold splits or a new personal best. Would you like to save?",
                                    "Save Records?", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                                if (result == MessageBoxResult.Yes) SaveCurrentFile();
                            }

                            CurrentRun = new Run(CurrentFile.RunDefinition.Length);
                        }
                        break;
                }
            };

            OpenFileCommand = Command.Create(() => true, OpenFile);
            ImportFromWsplitCommand = Command.Create(() => true, ImportFromWsplit);
            SaveSplits = Command.Create(() => true, SaveCurrentFile);
            SaveSplitsAs = Command.Create(() => true, SaveCurrentFileAs);

            //Set up observable which monitors changes in the current run
            var runChangedObs = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => PropertyChanged += h, h => PropertyChanged -= h).Where(a => a.EventArgs.PropertyName == "CurrentRun")
                .Publish().RefCount();

            //Set up observable which monitors changes in the current file
            var fileChangedObs = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => PropertyChanged += h, h => PropertyChanged -= h).Where(a => a.EventArgs.PropertyName == "CurrentFile")
                .Publish().RefCount();

            //Set up an observable of an observable which will be used to monitor changes in run status
            var runStatusObs = runChangedObs.Select(_ =>
            {
                if (CurrentRun == null) return Observable.Never<Unit>();

                var startObs = Observable.FromEventPattern(h => CurrentRun.RunStarted += h, h => CurrentRun.RunStarted -= h).Select(_2 => Unit.Default);
                var completeObs = Observable.FromEventPattern(h => CurrentRun.RunCompleted += h, h => CurrentRun.RunCompleted -= h).Select(_2 => Unit.Default);

                //Propagate the start and complete events but also fire an event immediately which will be used
                //to reset the timer as soon as the run changes
                return startObs.Merge(completeObs).StartWith(Unit.Default);
            });

            //Set up an observable of an observable which will be used to monitor changes in splits
            var splitObs = runChangedObs.Select(_ =>
            {
                if (CurrentRun == null) return Observable.Never<EventPattern<SplitChange>>();

                var splitChangedObs = Observable.FromEventPattern<SplitChange>(h => CurrentRun.SplitChanged += h, h => CurrentRun.SplitChanged -= h);

                return splitChangedObs.StartWith(new EventPattern<SplitChange>(this, new SplitChange(SplitChange.ActionEnum.Reset, null, -1)));
            });

            //Monitor when run changes and changes state in order to control live timer
            runStatusObs.Switch().Subscribe(_ =>
            {
                if (CurrentRun == null) LiveTimer.Clear();
                else if (!CurrentRun.IsStarted) LiveTimer.Clear();
                else if (CurrentRun.IsCompleted) LiveTimer.Stop(CurrentRun.Splits.Last().TimeFromRunStart);
                else if (CurrentRun.IsStarted) LiveTimer.Start(CurrentRun.StartTime);
            });

            //Monitor when run changes and a split is detected. This is used to update the split view
            splitObs.Switch().Subscribe(args =>
            {
                switch(args.EventArgs.Action)
                {
                    case SplitChange.ActionEnum.Removed:
                        SplitRows[args.EventArgs.Index].CurrentRunSplit = null;
                        PreviousSplitRow = args.EventArgs.Index > 0 ? SplitRows[args.EventArgs.Index - 1] : null;
                        break;
                    case SplitChange.ActionEnum.Added:
                        SplitRows[args.EventArgs.Index].CurrentRunSplit = args.EventArgs.Item;
                        PreviousSplitRow = SplitRows[args.EventArgs.Index];
                        break;
                    case SplitChange.ActionEnum.Reset:
                        var pbSplits = CurrentFile.PersonalBest.Splits;
                        var goldSplits = CurrentFile.SumOfBest.Splits;
                        var splitInfo = CurrentFile.RunDefinition;
                        var merged = pbSplits.Zip(splitInfo, (pbs, si) => new { PbSplit = pbs, Info = si }).Zip(goldSplits,
                            (a, gs) => new { PbSplit = a.PbSplit, GoldSplit = gs, Info = a.Info });

                        SplitRows = merged.Select(a => new SplitRowDisplay(a.Info.Name, a.PbSplit, a.GoldSplit)).ToArray();
                        PreviousSplitRow = null;
                        break;
                }
            });

            //Change display when a new file is loaded
            fileChangedObs.Subscribe(_ =>
            {
                CurrentRun = new Run(CurrentFile.RunDefinition.Length);
            });
        }

        public void ImportFromWsplit()
        {
            var ofd = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "All files (*.*)|*.*",
                RestoreDirectory = true,
                Title = "Select Split File"
            };

            var result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                try
                {
                    CurrentFile = SplitFile.ImportFromWsplit(ofd.FileName);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error importing splits from WSplit. The file may be malformed.", "Error Importing", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void OpenFile()
        {
            var ofd = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Fizzi Split Files (*.fsp)|*.fsp|All files (*.*)|*.*",
                RestoreDirectory = true,
                Title = "Select Split File"
            };

            var result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                CurrentFile = SplitFile.Load(ofd.FileName);
            }
        }

        public void SaveCurrentFile()
        {
            if (!CurrentFile.IsPathSet) SaveCurrentFileAs();
            else CurrentFile.MergeAndSave(CurrentRun);

            CurrentRun = new Run(CurrentFile.RunDefinition.Length);
        }

        public void SaveCurrentFileAs()
        {
            var ofd = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "Fizzi Split Files (*.fsp)|*.fsp|All files (*.*)|*.*",
                RestoreDirectory = true,
                Title = "Choose Save Location"
            };

            var result = ofd.ShowDialog();
            if (result.HasValue && result.Value)
            {
                CurrentFile.Path = ofd.FileName;
                SaveCurrentFile();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            if (keyListener != null) keyListener.Dispose();
        }
    }
}
