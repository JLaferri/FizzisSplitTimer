using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Fizzi.Applications.Splitter.Common;
using System.Reactive.Linq;

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

        public DateTime LatestReferenceTime { get; private set; }

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

        public void Start(DateTime referenceTime)
        {
            LatestReferenceTime = referenceTime;

            if (subscription != null) subscription.Dispose();
            clearOverrideTimer();
            subscription = frequencyObservable.Subscribe(_ => setTimer(DateTime.Now.Subtract(LatestReferenceTime)));
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

        public static string FormatElapsedTimeSpan(TimeSpan time)
        {
            int hours = (int)Math.Floor(time.TotalHours);
            int minutes = time.Minutes;
            int seconds = time.Seconds;
            int fractional = (int)(time.Milliseconds / 10);

            string timeString;
            if (hours > 0) timeString = string.Format("{3}:{2:00}:{1:00}.{0:00}", fractional, seconds, minutes, hours);
            else if (minutes > 0) timeString = string.Format("{2}:{1:00}.{0:00}", fractional, seconds, minutes);
            else timeString = string.Format("{1}.{0:00}", fractional, seconds);

            return timeString;
        }

        public static string FormatTimeDifferential(TimeSpan time)
        {
            bool isNegative = false;
            if (time < TimeSpan.Zero)
            {
                isNegative = true;
                time = time.Negate();
            }

            var elapsedTimeString = FormatElapsedTimeSpan(time);
            string signCharacter = isNegative ? "-" : "+";

            return signCharacter + elapsedTimeString;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
