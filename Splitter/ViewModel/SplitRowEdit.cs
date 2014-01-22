using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using Fizzi.Applications.Splitter.Common;

namespace Fizzi.Applications.Splitter.ViewModel
{
    class SplitRowEdit : INotifyPropertyChanged
    {
        public string Name { get; set; }

        private TimeSpan _personalBestTimeAtSplit;
        public TimeSpan PersonalBestTimeAtSplit { get { return _personalBestTimeAtSplit; } set { this.RaiseAndSetIfChanged("PersonalBestTimeAtSplit", ref _personalBestTimeAtSplit, value, PropertyChanged); } }

        private TimeSpan _goldSplitLength;
        public TimeSpan GoldSplitLength { get { return _goldSplitLength; } set { this.RaiseAndSetIfChanged("GoldSplitLength", ref _goldSplitLength, value, PropertyChanged); } }
        
        private bool _isPbTimeUnknown;
        public bool IsPbTimeUnknown { get { return _isPbTimeUnknown; } set { this.RaiseAndSetIfChanged("IsPbTimeUnknown", ref _isPbTimeUnknown, value, PropertyChanged); } }

        private bool _isGoldTimeUnknown;
        public bool IsGoldTimeUnknown { get { return _isGoldTimeUnknown; } set { this.RaiseAndSetIfChanged("IsGoldTimeUnknown", ref _isGoldTimeUnknown, value, PropertyChanged); } }

        public ICommand Divide { get; private set; }
        public ICommand Delete { get; private set; }

        public SplitRowEdit(SplitManagementViewModel collectionClass)
        {
            Name = "Unnamed Split";

            PersonalBestTimeAtSplit = TimeSpan.MaxValue;
            GoldSplitLength = TimeSpan.MaxValue;

            IsPbTimeUnknown = true;
            IsGoldTimeUnknown = true;

            Divide = Command.Create(() => true, () => collectionClass.Divide(this));
            Delete = Command.Create(() => true, () => collectionClass.Delete(this));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
