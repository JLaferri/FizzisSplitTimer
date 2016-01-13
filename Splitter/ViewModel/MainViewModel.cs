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
using Fizzi.Applications.Splitter.Properties;
using System.Collections.ObjectModel;
using System.Configuration;
using BondTech.HotKeyManagement.WPF._4;
using System.Xml.Linq;

namespace Fizzi.Applications.Splitter.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
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
        public ICommand CreateNewFileCommand { get; private set; }
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

        public SettingsViewModel SettingsViewModel { get; private set; }
        public DisplayTemplatesViewModel DisplaySettingsViewModel { get; private set; }
        public SplitManagementViewModel SplitManagementViewModel { get; private set; }

        public HotKeyManager HotKeyManager { get; private set; }

        public View.MainWindow MainWindow { get; private set; }

        //private KeyboardListener keyListener = new KeyboardListener();

        public MainViewModel()
        {
            MainWindow = (View.MainWindow)Application.Current.MainWindow;

            if (Settings.Default.IsNewVersion)
            {
                Settings.Default.Upgrade();
                var targetConfigPath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;

                //Attempt to upgrade display templates
                if (!string.IsNullOrWhiteSpace(Settings.Default.ConfigPath))
                {
                    //Forcefully upgrade old DisplayTemplates by copying the old settings file xml node over
                    //I know that the Application Settings can be upgraded using the Upgrade method but I
                    //can't seem to find how to do the same with configuration sections
                    var previousVersion = XDocument.Load(Settings.Default.ConfigPath);
                    var newVersion = XDocument.Load(targetConfigPath);

                    var previousDisplayTemplates = previousVersion.Descendants("displayTemplates").FirstOrDefault();

                    if (previousDisplayTemplates != null)
                    {
                        newVersion.Descendants("configuration").First().AddFirst(new XElement(previousDisplayTemplates));
                        newVersion.Save(targetConfigPath);
                    }
                }

                Settings.Default.ConfigPath = targetConfigPath;
                Settings.Default.IsNewVersion = false;

                Settings.Default.Save();
            }

            LiveTimer = new Timer(30);
            SettingsViewModel = new SettingsViewModel(this);
            DisplaySettingsViewModel = new DisplayTemplatesViewModel(this);
            SplitManagementViewModel = new SplitManagementViewModel();

            ResizeEnabled = false;
            ShowGoldSplits = true;
            
            MainWindow.Loaded += (sender, e) =>
            {
                //HotKeyManager must be initialized after the window has loaded
                HotKeyManager = new HotKeyManager(MainWindow);
                HotKeyManager.KeyBoardHook();

                var keyPressedObs = Observable.FromEventPattern<KeyboardHookEventHandler, KeyboardHookEventArgs>(
                    h => HotKeyManager.KeyBoardKeyEvent += h, h => HotKeyManager.KeyBoardKeyEvent -= h)
                    .Where(_ => !SettingsWindowOpen && CurrentFile != null && CurrentRun != null)
                    .Publish().RefCount();

                TimeSpan cooldown = TimeSpan.FromMilliseconds(Settings.Default.HotkeyCooldownTime);

                keyPressedObs.Where(ep => ep.EventArgs.Key == Settings.Default.SplitKey).Cooldown(cooldown).SubscribeSafeLog(_ => CurrentRun.Split());
                keyPressedObs.Where(ep => ep.EventArgs.Key == Settings.Default.UnsplitKey).Cooldown(cooldown).SubscribeSafeLog(_ => CurrentRun.Unsplit());
                keyPressedObs.Where(ep => ep.EventArgs.Key == Settings.Default.SkipKey).Cooldown(cooldown).SubscribeSafeLog(_ => CurrentRun.SkipSplit());
                keyPressedObs.Where(ep => ep.EventArgs.Key == Settings.Default.PauseKey).Cooldown(cooldown).SubscribeSafeLog(_ => CurrentRun.Pause());
                keyPressedObs.Where(ep => ep.EventArgs.Key == Settings.Default.ResetKey).Cooldown(cooldown).SubscribeSafeLog(_ =>
                {
                    //This call to CheckMergeSuggested is done like this in order to free up the key pressed event immediately.
                    //Blocking the event with a popup causes application responsiveness issues.
                    Observable.Return(System.Reactive.Unit.Default).ObserveOnDispatcher().SubscribeSafeLog(_2 =>
                    {
                        CheckMergeSuggested();
                        CurrentRun = new Run(CurrentFile.RunDefinition.Length);
                    });
                });
            };

            CreateNewFileCommand = Command.Create(() => true, CreateNewFile);
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
            runStatusObs.Switch().SubscribeSafeLog(_ =>
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
            splitObs.Switch().SubscribeSafeLog(args =>
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
            fileChangedObs.SubscribeSafeLog(_ =>
            {
                CurrentRun = new Run(CurrentFile.RunDefinition.Length);
            });

            //Force window to resize correctly when a new template is loaded
            windowSizeObs.Switch().Throttle(TimeSpan.FromMilliseconds(50)).ObserveOnDispatcher().SubscribeSafeLog(_ =>
            {
                if (DisplaySettingsViewModel == null || DisplaySettingsViewModel.SelectedDisplayTemplate == null) return;

                MainWindow.ForceChangeWindowSize(DisplaySettingsViewModel.SelectedDisplayTemplate.WindowHeight, 
                    DisplaySettingsViewModel.SelectedDisplayTemplate.WindowWidth);
            });

            //Keep DisplayTemplateViewModel synchronized with these changes.
            fileDisplayTemplateObs.Switch().SubscribeSafeLog(_ =>
            {
                if (CurrentFile == null || CurrentFile.DisplayTemplate == null) return;
                
                DisplaySettingsViewModel.SelectedDisplayTemplate = CurrentFile.DisplayTemplate;
            });
        }

        public void BeginResize()
        {
            IsResizing = true;
        }

        public void AcceptResize()
        {
            IsResizing = false;

            DisplaySettingsViewModel.SelectedDisplayTemplate.WindowHeight = MainWindow.Height;
            DisplaySettingsViewModel.SelectedDisplayTemplate.WindowWidth = MainWindow.Width;

            MainWindow.ShowDisplaySettingsDialog();
        }

        public void CancelResize()
        {
            IsResizing = false;

            MainWindow.ForceChangeWindowSize(DisplaySettingsViewModel.SelectedDisplayTemplate.WindowHeight,
                    DisplaySettingsViewModel.SelectedDisplayTemplate.WindowWidth);

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

        public void ImportFromLlanfair()
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
                    CurrentFile = SplitFile.ImportFromLlanfair(ofd.FileName);
                }
                catch (Exception)
                {
                    MessageBox.Show(MainWindow, "Error importing splits from Llanfair. The file may be malformed.", "Error Importing", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void CreateNewFile()
        {
            var emptySplit = new SplitInfo()
            {
                Name = "Unnamed Split",
                PersonalBestSplit = SplitTimeSpan.Unknown,
                SumOfBestSplit = SplitTimeSpan.Unknown
            };

            CurrentFile = new SplitFile("Unnamed Run", Enumerable.Repeat(emptySplit, 1).ToArray());
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
            if (!CurrentFile.IsPathSet)
            {
                SaveCurrentFileAs();
                return;
            }

            try
            {
                CurrentFile.MergeAndSave(CurrentRun);
                CurrentRun = new Run(CurrentFile.RunDefinition.Length);
            }
            catch (Exception)
            {
                MessageBox.Show(MainWindow, "Error saving file. Perhaps you don't have access to that folder.", "Error Saving", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        public void PrepareSplitManagementWindow()
        {
            SplitManagementViewModel.LoadFromFile(CurrentFile);
        }

        public void CommitSplitManagementChanges()
        {
            CurrentFile.Header = SplitManagementViewModel.RunTitle;
            CurrentFile.PersonalBestDate = SplitManagementViewModel.PersonalBestDate;
            CurrentFile.ChangeRunDefinition(SplitManagementViewModel.ConvertToSplitInfo());

            //Trigger file reload
            this.Raise("CurrentFile", PropertyChanged);
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
    }
}
