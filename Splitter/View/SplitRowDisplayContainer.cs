using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fizzi.Applications.Splitter.View
{
    class SplitRowDisplayContainer
    {
        public bool ShowSeparator { get; private set; }
        public ViewModel.SplitRowDisplay Display { get; private set; }

        public SplitRowDisplayContainer(ViewModel.SplitRowDisplay display, bool showSeparator)
        {
            ShowSeparator = showSeparator;
            Display = display;
        }
        public SplitRowDisplayContainer(ViewModel.SplitRowDisplay display) : this(display, false) { }
    }
}
