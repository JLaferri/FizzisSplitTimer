using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Fizzi.Applications.Splitter.ViewModel
{
    class DisplaySettingsViewModel
    {
        public string[] AvailableFonts { get; private set; }

        public DisplaySettingsViewModel()
        {
            AvailableFonts = Fonts.SystemFontFamilies.Select(ff => ff.ToString()).OrderBy(s => s).ToArray();
        }
    }
}
