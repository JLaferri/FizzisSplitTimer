using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Fizzi.Applications.Splitter.Common;
using System.Reactive.Linq;
using System.Globalization;
using Fizzi.Applications.Splitter.Properties;

namespace Fizzi.Applications.Splitter.Model
{
    class Timer : INotifyPropertyChanged
    {
        private string _displayTimer;
        public string DisplayTimer { get { return _displayTimer; } set { this.RaiseAndSetIfChanged("DisplayTimer", ref _displayTimer, value, PropertyChanged); } }

        private string _overrideTimeDisplay;
        public string OverrideTimeDisplay { get { return _overrideTimeDisplay; } set { this.RaiseAndSetIfChanged("OverrideTimeDisplay", ref _overrideTimeDisplay, value, PropertyChanged); } }

        public TimeSpan Time { get; private set; }
        public TimeSpan OverrideTime { get; private set; }

        private IObservable<long> frequencyObservable;
        private IDisposable subscription;

        public Timer(double frequencyHertz)
        {
            frequencyObservable = Observable.Interval(TimeSpan.FromSeconds(1 / frequencyHertz)).Publish().RefCount();
        }

        public void Clear()
        {
            if (subscription != null) subscription.Dispose();
            clearOverrideTimer();
            setTimer(TimeSpan.Zero);
        }

        public void Stop(TimeSpan stopTime)
        {
            if (subscription != null) subscription.Dispose();
            setOverrideTimer(stopTime);
        }

        public void Start(Run run)
        {
            if (subscription != null) subscription.Dispose();
            clearOverrideTimer();
            subscription = frequencyObservable.Subscribe(_ => setTimer(run.TimeSinceStart));
        }

        private void setTimer(TimeSpan time)
        {
            Time = time;
            DisplayTimer = FormatElapsedTimeSpan(time);
        }

        private void setOverrideTimer(TimeSpan time)
        {
            OverrideTime = time;
            OverrideTimeDisplay = FormatElapsedTimeSpan(time);
        }

        private void clearOverrideTimer()
        {
            OverrideTime = TimeSpan.Zero;
            OverrideTimeDisplay = null;
        }

        public static string FormatElapsedTimeSpan(TimeSpan time) { return FormatElapsedTimeSpan(time, Settings.Default.MsDecimalCount); }
        public static string FormatElapsedTimeSpan(TimeSpan time, int msDecimalCount)
        {
            int hours = (int)Math.Floor(time.TotalHours);
            int minutes = time.Minutes;

            var ccDecimalSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            var ccTimeSeparator = System.Globalization.DateTimeFormatInfo.CurrentInfo.TimeSeparator;

            var sb = (new StringBuilder(@"{0:ss\")).Append(ccDecimalSeparator);
            foreach (var _ in Enumerable.Range(0, msDecimalCount)) sb.Append("f");
            sb.Append("}");
            var secondsFormatter = sb.ToString();

            string timeString;
            if (hours > 0) timeString = string.Format("{2}:{1:00}:" + secondsFormatter, time, minutes, hours);
            else if (minutes > 0) timeString = string.Format("{1}:" + secondsFormatter, time, minutes);
            else timeString = string.Format(sb.Replace(":s", ":", 2, 2).ToString(), time);

            return timeString;
        }

        public static string FormatTimeDifferential(TimeSpan time) { return FormatTimeDifferential(time, Settings.Default.MsDecimalCount); }
        public static string FormatTimeDifferential(TimeSpan time, int msDecimalCount)
        {
            bool isNegative = false;
            if (time < TimeSpan.Zero)
            {
                isNegative = true;
                time = time.Negate();
            }

            var elapsedTimeString = FormatElapsedTimeSpan(time, msDecimalCount);
            string signCharacter = isNegative ? NumberFormatInfo.CurrentInfo.NegativeSign : NumberFormatInfo.CurrentInfo.PositiveSign;

            return signCharacter + elapsedTimeString;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
