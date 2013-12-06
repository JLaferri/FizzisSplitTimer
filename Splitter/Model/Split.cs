using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Fizzi.Applications.Splitter.Model
{
    class Split
    {
        public Run Owner { get; private set; }

        public SplitTimeSpan SplitInfo { get; private set; }

        public bool IsPrecise { get { return SplitInfo.IsPrecise; } }
        public bool IsPreviousPrecise
        {
            get
            {
                var indexOfCurrent = Array.IndexOf(Owner.Splits, this);
                if (indexOfCurrent < 0) throw new Exception("This split no longer belongs to the run marked as its owner.");

                if (indexOfCurrent == 0) return true;
                else return Owner.Splits[indexOfCurrent - 1].IsPrecise;
            }
        }
        public bool IsWellBounded { get { return IsPrecise && IsPreviousPrecise; } }

        public TimeSpan Time { get { return SplitInfo.Time; } }
        public TimeSpan TimeFromRunStart
        {
            get
            {
                //Sum up all splits prior to this split
                var timePrior = Owner.Splits.TakeWhile(s => s != this).Aggregate(new TimeSpan(), (accu, s) =>
                {
                    return safeTimeAddition(accu, s.Time);
                });
                return safeTimeAddition(timePrior, Time);
            }
        }

        private TimeSpan safeTimeAddition(TimeSpan one, TimeSpan two)
        {
            if (one == TimeSpan.MaxValue || two == TimeSpan.MaxValue) return TimeSpan.MaxValue;

            try { return one.Add(two); }
            catch (OverflowException) { return TimeSpan.MaxValue; }
        }

        public Split(TimeSpan splitTime, Run owner) : this(new SplitTimeSpan(splitTime, true), owner) { }
        public Split(TimeSpan splitTime, Run owner, bool isPreciseSplit) : this(new SplitTimeSpan(splitTime, isPreciseSplit), owner) { }
        public Split(SplitTimeSpan splitTimeSpan, Run owner)
        {
            if (owner == null) throw new ArgumentNullException();

            Owner = owner;
            SplitInfo = splitTimeSpan;
        }

        public static bool IsGoldSplit(Split currentSplit, Split referenceSplit)
        {
            if (currentSplit == null || referenceSplit == null) return false;

            return currentSplit.IsWellBounded && currentSplit.Time < referenceSplit.Time;
        }

        public static bool? IsFasterTotalTime(Split currentSplit, Split referenceSplit)
        {
            if (currentSplit == null || referenceSplit == null) return null;
            if (!currentSplit.IsPrecise || !referenceSplit.IsPrecise) return null;

            return currentSplit.TimeFromRunStart < referenceSplit.TimeFromRunStart;
        }
    }
}
