﻿using System;
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
using Fizzi.Applications.Splitter.Properties;
using System.Collections.ObjectModel;
using System.Configuration;

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

        private SplitRowDisplay _currentSplitRow;
        public SplitRowDisplay CurrentSplitRow { get { return _currentSplitRow; } set { this.RaiseAndSetIfChanged("CurrentSplitRow", ref _currentSplitRow, value, PropertyChanged); } }

        public Timer LiveTimer { get; private set; }

        public ICommand SaveSplits { get; private set; }
        public ICommand SaveSplitsAs { get; private set; }
        public ICommand OpenFileCommand { get; private set; }
        public ICommand ImportFromWsplitCommand { get; private set; }

        public ICommand AcceptResizeCommand { get; private set; }
        public ICommand CancelResizeCommand { get; private set; }

        private bool _resizeEnabled;
        public bool ResizeEnabled { get { return _resizeEnabled; } set { this.RaiseAndSetIfChanged("ResizeEnabled", ref _resizeEnabled, value, PropertyChanged); } }

        private bool _isResizing;
        public bool IsResizing { get { return _isResizing; } private set { this.RaiseAndSetIfChanged("IsResizing", ref _isResizing, value, PropertyChanged); } }

        private bool _showGoldSplits;
        public bool ShowGoldSplits { get { return _showGoldSplits; } set { this.RaiseAndSetIfChanged("ShowGoldSplits", ref _showGoldSplits, value, PropertyChanged); } }

        private bool _isCurrentRunStarted;
        public bool IsCurrentRunStarted { get { return _isCurrentRunStarted; } set { this.RaiseAndSetIfChanged("IsCurrentRunStarted", ref _isCurrentRunStarted, value, PropertyChanged); } }

        public bool SettingsWindowOpen { get; set; }

        private double resizeStartWidth, resizeStartHeight;

        public SettingsViewModel SettingsViewModel { get; private set; }
        public DisplayTemplatesViewModel DisplaySettingsViewModel { get; private set; }

        public View.MainWindow MainWindow { get; private set; }

        private KeyboardListener keyListener = new KeyboardListener();

        public MainViewModel()
        {
            //var defaultDisplayTemplate2 = new DisplayTemplate();
            //defaultDisplayTemplate2.TemplateName = "<Default>";
            //PersistanceManager.Instance.DisplayTemplates.Add(defaultDisplayTemplate2);
            //PersistanceManager.Instance.DisplayTemplatesConfiguration.Save();
            MainWindow = (View.MainWindow)Application.Current.MainWindow;

            if (Settings.Default.IsNewVersion)
            {
                Settings.Default.Upgrade();
                Settings.Default.IsNewVersion = false;
                Settings.Default.Save();
            }

            if (Settings.Default.IsVeryFirstLoad)
            {
                //Load Key defaults on first load ever
                Settings.Default.SplitKey = Key.Right;
                Settings.Default.UnsplitKey = Key.Left;
                Settings.Default.SkipKey = Key.PageDown;
                Settings.Default.ResetKey = Key.End;
                Settings.Default.PauseKey = Key.Pause;

                Settings.Default.IsVeryFirstLoad = false;
                Settings.Default.Save();
            }

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
            SettingsViewModel = new SettingsViewModel(keyListener);
            DisplaySettingsViewModel = new DisplayTemplatesViewModel(this);

            ResizeEnabled = false;
            ShowGoldSplits = true;

            //Subscribe to keyboard events
            keyListener.KeyDown += (sender, e) =>
            {
                if (!SettingsWindowOpen && CurrentFile != null && CurrentRun != null)
                {
                    if (e.Key == Settings.Default.SplitKey) CurrentRun.Split();
                    if (e.Key == Settings.Default.UnsplitKey) CurrentRun.Unsplit();
                    if (e.Key == Settings.Default.SkipKey) CurrentRun.SkipSplit();
                    if (e.Key == Settings.Default.ResetKey)
                    {
                        CheckMergeSuggested();
                        CurrentRun = new Run(CurrentFile.RunDefinition.Length);
                    }
                }
            };

            OpenFileCommand = Command.Create(() => true, OpenFile);
            ImportFromWsplitCommand = Command.Create(() => true, ImportFromWsplit);
            SaveSplits = Command.Create(() => true, SaveCurrentFile);
            SaveSplitsAs = Command.Create(() => true, SaveCurrentFileAs);
            AcceptResizeCommand = Command.Create(() => true, AcceptResize);
            CancelResizeCommand = Command.Create(() => true, CancelResize);

            //Set up observable which monitors changes in the current run
            var runChangedObs = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => PropertyChanged += h, h => PropertyChanged -= h).Where(a => a.EventArgs.PropertyName == "CurrentRun")
                .Publish().RefCount();

            //Set up observable which monitors changes in the current file
            var fileChangedObs = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => PropertyChanged += h, h => PropertyChanged -= h).Where(a => a.EventArgs.PropertyName == "CurrentFile")
                .Publish().RefCount();

            //Set up observable which monitors changes in the display template
            var displayTemplateObs = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => DisplaySettingsViewModel.PropertyChanged += h, h => DisplaySettingsViewModel.PropertyChanged -= h)
                .Where(a => a.EventArgs.PropertyName == "SelectedDisplayTemplate").Publish().RefCount();

            //Set up an observable of an observable which will be used to monitor changes in run status
            var runStatusObs = runChangedObs.Select(_ =>
            {
                if (CurrentRun == null) return Observable.Never<Unit>();

                var startObs = Observable.FromEventPattern(h => CurrentRun.RunStatusChanged += h, h => CurrentRun.RunStatusChanged -= h).Select(_2 => Unit.Default);

                //Propagate the start and complete events but also fire an event immediately which will be used
                //to reset the timer as soon as the run changes
                return startObs.StartWith(Unit.Default);
            });

            //Set up an observable of an observable which will be used to monitor changes in splits
            var splitObs = runChangedObs.Select(_ =>
            {
                if (CurrentRun == null) return Observable.Never<EventPattern<SplitChange>>();

                var splitChangedObs = Observable.FromEventPattern<SplitChange>(h => CurrentRun.SplitChanged += h, h => CurrentRun.SplitChanged -= h);

                return splitChangedObs.StartWith(new EventPattern<SplitChange>(this, new SplitChange(SplitChange.ActionEnum.Reset, null, -1)));
            });

            //Set up an observable of an observable which will be used to monitor changes in display template
            var fileDisplayTemplateObs = fileChangedObs.Select(_ =>
            {
                if (CurrentFile == null) return Observable.Never<Unit>();

                var templateChangedObs = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => CurrentFile.PropertyChanged += h, h => CurrentFile.PropertyChanged -= h).Where(a => a.EventArgs.PropertyName == "DisplayTemplate");

                return templateChangedObs.Select(_2 => Unit.Default).StartWith(Unit.Default);
            });

            //Set up an observable of an observable which will be used to monitor changes in window size
            var windowSizeObs = displayTemplateObs.Select(_ => Unit.Default).StartWith(Unit.Default).Select(_ =>
            {
                if (DisplaySettingsViewModel == null || DisplaySettingsViewModel.SelectedDisplayTemplate == null) return Observable.Never<Unit>();

                var selectedDisplay = DisplaySettingsViewModel.SelectedDisplayTemplate;

                var sizeChangedObs = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => selectedDisplay.PropertyChanged += h, h => selectedDisplay.PropertyChanged -= h)
                    .Where(a => a.EventArgs.PropertyName == "WindowWidth" || a.EventArgs.PropertyName == "WindowHeight");

                return sizeChangedObs.Select(_2 => Unit.Default).StartWith(Unit.Default);
            });

            //Monitor when run changes and changes state in order to control live timer
            runStatusObs.Switch().Subscribe(_ =>
            {
                if (CurrentRun == null || !CurrentRun.IsStarted)
                {
                    IsCurrentRunStarted = false;
                    LiveTimer.Clear();
                }
                else if (CurrentRun.IsCompleted)
                {
                    LiveTimer.Stop(CurrentRun.CompletedRunTime);
                }
                else if (CurrentRun.IsStarted)
                {
                    if (CurrentRun.CurrentSplit == 0)
                    {
                        IsCurrentRunStarted = true;
                        CurrentSplitRow = SplitRows[0];
                    }

                    LiveTimer.Start(CurrentRun);
                }
            });

            //Monitor when run changes and a split is detected. This is used to update the split view
            splitObs.Switch().Subscribe(args =>
            {
                switch(args.EventArgs.Action)
                {
                    case SplitChange.ActionEnum.Removed:
                        SplitRows[args.EventArgs.Index].CurrentRunSplit = null;
                        PreviousSplitRow = args.EventArgs.Index > 0 ? SplitRows[args.EventArgs.Index - 1] : null;
                        CurrentSplitRow = SplitRows[args.EventArgs.Index];
                        break;
                    case SplitChange.ActionEnum.Added:
                        SplitRows[args.EventArgs.Index].CurrentRunSplit = args.EventArgs.Item;
                        PreviousSplitRow = SplitRows[args.EventArgs.Index];
                        CurrentSplitRow = SplitRows.Length > args.EventArgs.Index + 1 ? SplitRows[args.EventArgs.Index + 1] : null;
                        break;
                    case SplitChange.ActionEnum.Reset:
                        var pbSplits = CurrentFile.PersonalBest.Splits;
                        var goldSplits = CurrentFile.SumOfBest.Splits;
                        var splitInfo = CurrentFile.RunDefinition;
                        var merged = pbSplits.Zip(splitInfo, (pbs, si) => new { PbSplit = pbs, Info = si }).Zip(goldSplits,
                            (a, gs) => new { PbSplit = a.PbSplit, GoldSplit = gs, Info = a.Info });

                        SplitRows = merged.Select(a => new SplitRowDisplay(a.Info.Name, a.PbSplit, a.GoldSplit)).ToArray();
                        PreviousSplitRow = null;
                        CurrentSplitRow = null;
                        break;
                }
            });

            //Change current run when a new file is loaded
            fileChangedObs.Subscribe(_ =>
            {
                CurrentRun = new Run(CurrentFile.RunDefinition.Length);
            });

            //Force window to resize correctly when a new template is loaded
            windowSizeObs.Switch().Subscribe(_ =>
            {
                if (DisplaySettingsViewModel == null || DisplaySettingsViewModel.SelectedDisplayTemplate == null) return;

                MainWindow.ForceChangeWindowSize(DisplaySettingsViewModel.SelectedDisplayTemplate.WindowHeight, 
                    DisplaySettingsViewModel.SelectedDisplayTemplate.WindowWidth);
            });

            //Keep DisplayTemplateViewModel synchronized with these changes.
            fileDisplayTemplateObs.Switch().Subscribe(_ =>
            {
                if (CurrentFile == null || CurrentFile.DisplayTemplate == null) return;
                
                DisplaySettingsViewModel.SelectedDisplayTemplate = CurrentFile.DisplayTemplate;
            });
        }

        public void BeginResize()
        {
            IsResizing = true;

            resizeStartHeight = DisplaySettingsViewModel.SelectedDisplayTemplate.WindowHeight;
            resizeStartWidth = DisplaySettingsViewModel.SelectedDisplayTemplate.WindowWidth;
        }

        public void AcceptResize()
        {
            IsResizing = false;

            MainWindow.ShowDisplaySettingsDialog();
        }

        public void CancelResize()
        {
            IsResizing = false;

            DisplaySettingsViewModel.SelectedDisplayTemplate.WindowHeight = resizeStartHeight;
            DisplaySettingsViewModel.SelectedDisplayTemplate.WindowWidth = resizeStartWidth;

            MainWindow.ShowDisplaySettingsDialog();
        }

        public void ImportFromWsplit()
        {
            var ofd = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "All files (*.*)|*.*",
                RestoreDirectory = true,
                Title = "Select Split File"
            };

            var result = ofd.ShowDialog(MainWindow);
            if (result.HasValue && result.Value)
            {
                try
                {
                    CurrentFile = SplitFile.ImportFromWsplit(ofd.FileName);
                }
                catch (Exception)
                {
                    MessageBox.Show(MainWindow, "Error importing splits from WSplit. The file may be malformed.", "Error Importing", MessageBoxButton.OK, MessageBoxImage.Error);
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

            var result = ofd.ShowDialog(MainWindow);
            if (result.HasValue && result.Value)
            {
                try
                {
                    CurrentFile = SplitFile.Load(ofd.FileName);
                }
                catch (Exception)
                {
                    MessageBox.Show(MainWindow, "Error loading file. The file might be corrupt.", "Error Loading", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void SaveCurrentFile()
        {
            if (!CurrentFile.IsPathSet) SaveCurrentFileAs();
            else
            {
                try
                {
                    CurrentFile.MergeAndSave(CurrentRun);
                }
                catch (Exception)
                {
                    MessageBox.Show(MainWindow, "Error saving file. Perhaps you don't have access to that folder.", "Error Saving", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

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

            var result = ofd.ShowDialog(MainWindow);
            if (result.HasValue && result.Value)
            {
                var oldFilePath = CurrentFile.Path;

                try
                {
                    CurrentFile.Path = ofd.FileName;
                    SaveCurrentFile();
                }
                catch (Exception)
                {
                    CurrentFile.Path = oldFilePath;
                    MessageBox.Show(MainWindow, "Error saving file. Perhaps you don't have access to that folder.", "Error Importing", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void CheckMergeSuggested()
        {
            if (CurrentFile != null && CurrentRun != null && CurrentFile.CheckMergeSuggested(CurrentRun))
            {
                var result = MessageBox.Show(MainWindow, "We have detected that your current run has unsaved gold splits or a new personal best. Would you like to save?",
                    "Save Records?", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes) SaveCurrentFile();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            if (keyListener != null) keyListener.Dispose();
        }
    }
}
