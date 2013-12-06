using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fizzi.Applications.Splitter.Model
{
    class SplitChange : EventArgs
    {
        public ActionEnum Action { get; private set; }
        public Split Item { get; private set; }
        public int Index { get; private set; }

        public SplitChange(ActionEnum action, Split item, int index)
        {
            Action = action;
            Item = item;
            Index = index;
        }

        public enum ActionEnum { Added, Removed, Reset }
    }
}
