using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Fizzi.Applications.Splitter.Model
{
    [DataContract]
    struct SplitTimeSpan
    {
        public static readonly SplitTimeSpan Unknown = new SplitTimeSpan(TimeSpan.MaxValue, false);

        [DataMember]
        public bool IsPrecise { get; set; }
        [DataMember]
        public TimeSpan Time { get; set; }

        public SplitTimeSpan(TimeSpan time) : this(time, true) { }
        public SplitTimeSpan(TimeSpan time, bool isPrecise) : this()
        {
            Time = time;
            IsPrecise = isPrecise;
        }

        public static bool operator !=(SplitTimeSpan x, SplitTimeSpan y)
        {
            return !(x == y);
        }

        public static bool operator ==(SplitTimeSpan x, SplitTimeSpan y)
        {
            return x.IsPrecise == y.IsPrecise && x.Time == y.Time;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SplitTimeSpan)) return base.Equals(obj);

            return this == (SplitTimeSpan)obj;
        }

        public override int GetHashCode()
        {
            return IsPrecise.GetHashCode() ^ Time.GetHashCode();
        }
    }
}
