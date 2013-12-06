using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using Fizzi.Applications.Splitter.Common;

namespace Fizzi.Applications.Splitter.Model
{
    [DataContract]
    class SplitInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public SplitTimeSpan PersonalBestSplit { get; set; }

        [DataMember]
        public SplitTimeSpan SumOfBestSplit { get; set; }
    }
}
