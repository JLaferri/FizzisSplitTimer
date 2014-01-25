using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;

namespace Fizzi.Applications.Splitter.Common
{
    public static class ObservableEx
    {
        public static IObservable<T> Cooldown<T>(this IObservable<T> source, TimeSpan cooldownTime)
        {
            return source.TimeInterval().Scan(new { TimeSinceLastSuccess = cooldownTime, Value = default(T) }, (c, t) =>
            {
                var combinedTimes = c.TimeSinceLastSuccess.Add(t.Interval);

                //If total time since reset is still less than cooldown, keep the sum in scan, else reset cooldown
                if (combinedTimes < cooldownTime) return new { TimeSinceLastSuccess = combinedTimes, Value = t.Value };
                else return new { TimeSinceLastSuccess = TimeSpan.Zero, Value = t.Value };
            }).Where(a => a.TimeSinceLastSuccess == TimeSpan.Zero).Select(a => a.Value);
        }
    }
}
