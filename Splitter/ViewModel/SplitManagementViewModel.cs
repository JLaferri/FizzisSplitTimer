using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Fizzi.Applications.Splitter.Model;
using System.Windows.Input;
using Fizzi.Applications.Splitter.Common;

namespace Fizzi.Applications.Splitter.ViewModel
{
    class SplitManagementViewModel
    {
        public string RunTitle { get; set; }
        public DateTime PersonalBestDate { get; set; }
        public ObservableCollection<SplitRowEdit> Splits { get; private set; }

        public ICommand AddSplit { get; private set; }

        public SplitManagementViewModel()
        {
            AddSplit = Command.Create(() => true, () => Add());
        }

        public void LoadFromFile(SplitFile file)
        {
            var pbAndGold = file.RunDefinition.Zip(file.PersonalBest.Splits, (d, pb) => new { D = d, Pb = pb })
                .Zip(file.SumOfBest.Splits, (a, gs) => new { Definition = a.D, PersonalBest = a.Pb, Gold = gs }).ToArray();

            Splits = new ObservableCollection<SplitRowEdit>(pbAndGold.Select(a =>
            {
                var editRow = new SplitRowEdit(this);
                editRow.GoldSplitLength = a.Definition.SumOfBestSplit.Time;
                editRow.IsGoldTimeUnknown = !a.Definition.SumOfBestSplit.IsPrecise;
                editRow.PersonalBestTimeAtSplit = a.PersonalBest.TimeFromRunStart;
                editRow.IsPbTimeUnknown = !a.PersonalBest.IsPrecise;
                editRow.Name = a.Definition.Name;

                return editRow;
            }));

            RunTitle = file.Header;
            PersonalBestDate = file.PersonalBestDate;
        }

        public SplitInfo[] ConvertToSplitInfo()
        {
            var times = Splits.ToArray();

            var splits = Enumerable.Range(0, times.Length).Select(i =>
            {
                var split = times[i];

                SplitTimeSpan pbSplit;

                if (split.IsPbTimeUnknown)
                {
                    //Default value for a timespan is zero. If the first entry was unknown it will be set to zero with IsPrecise = false
                    var lastKnownTime = times.Take(i).Where(a => !a.IsPbTimeUnknown).Select(a => a.PersonalBestTimeAtSplit).FirstOrDefault();
                    pbSplit = new SplitTimeSpan(lastKnownTime, false);
                }
                else if (i == 0) pbSplit = new SplitTimeSpan(split.PersonalBestTimeAtSplit);
                else pbSplit = new SplitTimeSpan(split.PersonalBestTimeAtSplit.Subtract(times[i - 1].PersonalBestTimeAtSplit));

                return new SplitInfo()
                {
                    Name = split.Name,
                    PersonalBestSplit = pbSplit,
                    SumOfBestSplit = split.IsGoldTimeUnknown ? SplitTimeSpan.Unknown : new SplitTimeSpan(split.GoldSplitLength)
                };
            }).ToArray();

            return splits;
        }

        public void Divide(SplitRowEdit toDivide)
        {
            var index = Splits.IndexOf(toDivide);

            Splits.Insert(index, new SplitRowEdit(this)
            {
                PersonalBestTimeAtSplit = toDivide.PersonalBestTimeAtSplit
            });

            toDivide.IsGoldTimeUnknown = true;
            toDivide.GoldSplitLength = TimeSpan.MaxValue;
        }

        public void Delete(SplitRowEdit toDelete)
        {
            //Do nothing if this is the last split
            if (Splits.Count <= 1) return;

            Splits.Remove(toDelete);
        }

        public void Add()
        {
            Splits.Add(new SplitRowEdit(this));
        }
    }
}
