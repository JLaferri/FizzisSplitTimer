using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fizzi.Applications.Splitter.Model;
using System.ComponentModel;
using Fizzi.Applications.Splitter.Common;

namespace Fizzi.Applications.Splitter.ViewModel
{
    class SplitRowDisplay : INotifyPropertyChanged
    {
        public string Name { get; private set; }

        private string _display;
        public string Display { get { return _display; } private set { this.RaiseAndSetIfChanged("Display", ref _display, value, PropertyChanged); } }

        private string _goldDisplay;
        public string GoldDisplay { get { return _goldDisplay; } private set { this.RaiseAndSetIfChanged("GoldDisplay", ref _goldDisplay, value, PropertyChanged); } }

        private Split _personalBestSplit;
        public Split PersonalBestSplit { get { return _personalBestSplit; } private set { this.RaiseAndSetIfChanged("PersonalBestSplit", ref _personalBestSplit, value, PropertyChanged); } }

        private Split _goldSplit;
        public Split GoldSplit { get { return _goldSplit; } private set { this.RaiseAndSetIfChanged("GoldSplit", ref _goldSplit, value, PropertyChanged); } }

        private Split _currentRunSplit;
        public Split CurrentRunSplit { get { return _currentRunSplit; } set { this.RaiseAndSetIfChanged("CurrentRunSplit", ref _currentRunSplit, value, PropertyChanged); } }

        private string _pbOffsetDisplay;
        public string PbOffsetDisplay { get { return _pbOffsetDisplay; } private set { this.RaiseAndSetIfChanged("PbOffsetDisplay", ref _pbOffsetDisplay, value, PropertyChanged); } }

        private string _goldOffsetDisplay;
        public string GoldOffsetDisplay { get { return _goldOffsetDisplay; } private set { this.RaiseAndSetIfChanged("GoldOffsetDisplay", ref _goldOffsetDisplay, value, PropertyChanged); } }

        private bool _isNewGoldSplit;
        public bool IsNewGoldSplit { get { return _isNewGoldSplit; } private set { this.RaiseAndSetIfChanged("IsNewGoldSplit", ref _isNewGoldSplit, value, PropertyChanged); } }

        private bool? _isAheadOfPb;
        public bool? IsAheadOfPb { get { return _isAheadOfPb; } private set { this.RaiseAndSetIfChanged("IsAheadOfPb", ref _isAheadOfPb, value, PropertyChanged); } }

        public SplitRowDisplay(string name, Split pbSplit, Split goldSplit)
        {
            Name = name;
            GoldSplit = goldSplit;

            this.PropertyChanged += (sender, e) =>
            {
                //Only adjust display based on changing splits
                if (e.PropertyName == "PersonalBestSplit" || e.PropertyName == "GoldSplit" || e.PropertyName == "CurrentRunSplit")
                {
                    IsNewGoldSplit = Split.IsGoldSplit(CurrentRunSplit, GoldSplit);
                    IsAheadOfPb = Split.IsFasterTotalTime(CurrentRunSplit, PersonalBestSplit);

                    var displaySetter = "[??]";
                    var goldDisplaySetter = "[??]";
                    var pbOffsetSetter = "[??]";
                    var goldOffsetSetter = "[??]";

                    //If splits have changed, set Display accordingly
                    if (CurrentRunSplit == null)
                    {
                        //When current split is null, only the default pb times can be displayed
                        if (PersonalBestSplit.IsPrecise)
                        {
                            displaySetter = Timer.FormatElapsedTimeSpan(PersonalBestSplit.TimeFromRunStart);
                        }

                        if (GoldSplit.IsWellBounded)
                        {
                            goldDisplaySetter = Timer.FormatElapsedTimeSpan(GoldSplit.TimeFromRunStart);
                        }
                    }
                    else
                    {
                        if (CurrentRunSplit.IsPrecise && PersonalBestSplit.IsPrecise)
                        {
                            //If both splits are precise, we can display a time differential
                            var pbTime = PersonalBestSplit.TimeFromRunStart;
                            var curTime = CurrentRunSplit.TimeFromRunStart;

                            displaySetter = Timer.FormatTimeDifferential(curTime.Subtract(pbTime));
                        }
                        else if (CurrentRunSplit.IsPrecise)
                        {
                            //If only the current display is precise, we can at least show the current run time
                            displaySetter = Timer.FormatElapsedTimeSpan(CurrentRunSplit.TimeFromRunStart);
                        }

                        if (CurrentRunSplit.IsWellBounded && GoldSplit.SplitInfo != SplitTimeSpan.Unknown)
                        {
                            //If both current split and gold split are well bounded, we can show gold split differential
                            goldOffsetSetter = Timer.FormatTimeDifferential(CurrentRunSplit.Time.Subtract(GoldSplit.Time));
                        }

                        if (CurrentRunSplit.IsWellBounded && PersonalBestSplit.IsWellBounded)
                        {
                            //If both current split and pb split are well bounded, we can show pb split differential
                            pbOffsetSetter = Timer.FormatTimeDifferential(CurrentRunSplit.Time.Subtract(PersonalBestSplit.Time));
                        }
                    }

                    Display = displaySetter;
                    GoldDisplay = goldDisplaySetter;
                    PbOffsetDisplay = pbOffsetSetter;
                    GoldOffsetDisplay = goldOffsetSetter;
                }
            };

            //Write pbSplit after setting up PropertyChanged handler in order to trigger it once
            PersonalBestSplit = pbSplit;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
