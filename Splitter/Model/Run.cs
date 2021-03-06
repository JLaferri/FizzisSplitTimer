﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Fizzi.Applications.Splitter.Model
{
    class Run
    {
        public event EventHandler RunStatusChanged;
        public event EventHandler<SplitChange> SplitChanged;

        public int CurrentSplit { get; private set; }
        public Split[] Splits { get; private set; }

        public bool IsStarted { get; private set; }
        public bool IsPaused { get { return !stopwatch.IsRunning; } }
        public bool IsCompleted { get { return CurrentSplit >= Splits.Length; } }

        //public DateTime? PauseTime { get; private set; }
        public DateTime StartTime { get; private set; }

        public TimeSpan CompletedRunTime
        {
            get
            {
                if (Splits == null) return TimeSpan.MaxValue;

                var finalSplit = Splits.LastOrDefault();
                if (finalSplit == null || !finalSplit.IsPrecise) return TimeSpan.MaxValue;

                return finalSplit.TimeFromRunStart;
            }
        }

        public TimeSpan TimeSinceStart { get { return stopwatch.Elapsed; } }

        public System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        public Run(int splitCount)
        {
            if (splitCount < 1) throw new ArgumentOutOfRangeException("Runs must contain at least one split.");

            Splits = new Split[splitCount];
            CurrentSplit = 0;

            IsStarted = false;
        }

        public Run(SplitTimeSpan[] splits)
        {
            Splits = splits.Select(sts => new Split(sts, this)).ToArray();

            //By setting current split to the length of the amount of splits, this run is marked as completed
            //The reason this constructor starts runs as completed immediately is that these runs are supposed to be
            //runs to compare to such as the personal best or the sum-of-best run
            CurrentSplit = Splits.Length;
            IsStarted = true;
        }

        public void Split()
        {
            if (!IsStarted)
            {
                //If run has not yet started - start it now
                stopwatch.Start();

                StartTime = DateTime.Now;
                IsStarted = true;
                OnRunStatusChanged();
            }
            else if (!IsCompleted)
            {
                //If first split already exists, add this split to the end
                var timeSinceStart = TimeSinceStart; //fetch the time for this split before doing any other computing

                var timeToLastSplit = Splits.Take(CurrentSplit).Aggregate(new TimeSpan(), (accu, s) => accu.Add(s.Time));
                var timeSinceLastSplit = timeSinceStart.Subtract(timeToLastSplit);

                var split = new Split(timeSinceLastSplit, this);
                Splits[CurrentSplit] = split;
                CurrentSplit++;

                OnSplitChanged(new SplitChange(SplitChange.ActionEnum.Added, split, CurrentSplit - 1));
                if (IsCompleted) OnRunStatusChanged();
            }
        }

        public void SkipSplit()
        {
            //A skip split request will not start the run
            //A skip split request will not work when trying to end the run

            if (IsStarted && CurrentSplit < (Splits.Length - 1))
            {
                var timeSinceStart = TimeSinceStart; //fetch the time before doing any other computing

                var timeToLastSplit = Splits.Take(CurrentSplit).Aggregate(new TimeSpan(), (accu, s) => accu.Add(s.Time));
                var timeSinceLastSplit = timeSinceStart.Subtract(timeToLastSplit);

                var split = new Split(timeSinceLastSplit, this, false);
                Splits[CurrentSplit] = split;
                CurrentSplit++;

                OnSplitChanged(new SplitChange(SplitChange.ActionEnum.Added, split, CurrentSplit - 1));
                if (IsCompleted) OnRunStatusChanged();
            }
        }

        public void Unsplit()
        {
            //An unsplit request will not work if the run is complete
            //An unsplit request will do nothing if the current split is the first split

            if (CurrentSplit > 0)
            {
                var wasRunPreviouslyComplete = IsCompleted;

                CurrentSplit--;

                var split = Splits[CurrentSplit];
                Splits[CurrentSplit] = null;

                OnSplitChanged(new SplitChange(SplitChange.ActionEnum.Removed, split, CurrentSplit));
                if (wasRunPreviouslyComplete) OnRunStatusChanged();
            }
        }

        public void Pause()
        {
            //Pausing is only valid if the run has already started and if it is not over
            if (IsStarted && !IsCompleted)
            {
                if (!IsPaused) stopwatch.Stop();
                else stopwatch.Start();
            }
        }

        protected virtual void OnRunStatusChanged()
        {
            if (RunStatusChanged != null) RunStatusChanged(this, EventArgs.Empty);
        }

        protected virtual void OnSplitChanged(SplitChange change)
        {
            if (SplitChanged != null) SplitChanged(this, change);
        }
    }
}
