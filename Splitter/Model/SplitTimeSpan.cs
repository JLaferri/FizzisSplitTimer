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
    }
}
