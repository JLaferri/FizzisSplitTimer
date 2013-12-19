using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.IO;
using System.Xml;
using Fizzi.Applications.Splitter.Common;

namespace Fizzi.Applications.Splitter.Model
{
    [DataContract]
    class SplitFile : INotifyPropertyChanged
    {
        [DataMember(Name="PersonalBestDate")]
        private DateTime _personalBestDate;
        public DateTime PersonalBestDate { get { return _personalBestDate; } set { this.RaiseAndSetIfChanged("PersonalBestDate", ref _personalBestDate, value, PropertyChanged); } }

        [DataMember(Name = "Header")]
        private string _header;
        public string Header { get { return _header; } set { this.RaiseAndSetIfChanged("Header", ref _header, value, PropertyChanged); } }

        [DataMember(Name = "RunDefinition")]
        private SplitInfo[] _runDefinition;
        public SplitInfo[] RunDefinition { get { return _runDefinition; } private set { this.RaiseAndSetIfChanged("RunDefinition", ref _runDefinition, value, PropertyChanged); } }

        [DataMember(Name = "DisplaySettings")]
        private DisplaySettings _displaySettings;
        public DisplaySettings DisplaySettings { get { return _displaySettings; } private set { this.RaiseAndSetIfChanged("DisplaySettings", ref _displaySettings, value, PropertyChanged); } }

        private Run _personalBest;
        public Run PersonalBest { get { return _personalBest; } private set { this.RaiseAndSetIfChanged("PersonalBest", ref _personalBest, value, PropertyChanged); } }

        private Run _sumOfBest;
        public Run SumOfBest { get { return _sumOfBest; } private set { this.RaiseAndSetIfChanged("SumOfBest", ref _sumOfBest, value, PropertyChanged); } }

        private string _path;
        public string Path { get { return _path; } set { this.RaiseAndSetIfChanged("Path", ref _path, value, PropertyChanged); } }

        public bool IsPathSet { get { return !string.IsNullOrWhiteSpace(Path); } }

        public SplitFile(string header, SplitInfo[] runDefinition)
        {
            Header = header;
            RunDefinition = runDefinition;

            DisplaySettings = new Model.DisplaySettings();

            generateRunsFromDefinition();
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext c)
        {
            generateRunsFromDefinition();
        }

        private void generateRunsFromDefinition()
        {
            PersonalBest = new Run(RunDefinition.Select(si => si.PersonalBestSplit).ToArray());
            SumOfBest = new Run(RunDefinition.Select(si => si.SumOfBestSplit).ToArray());
        }

        public void Save()
        {
            if (Uri.IsWellFormedUriString(Path, UriKind.Absolute)) throw new Exception("Path is malformed.");

            DataContractSerializer dcs = new DataContractSerializer(typeof(SplitFile));

            using (Stream stream = new FileStream(Path, FileMode.Create, FileAccess.Write))
            {
                dcs.WriteObject(stream, this);
            }
        }

        public void MergeAndSave(Run latestRun)
        {
            var zippedSplits = latestRun.Splits.Zip(PersonalBest.Splits, (lr, pb) => new { Lr = lr, Pb = pb }).Zip(SumOfBest.Splits,
                (a, sb) => new { Lr = a.Lr, Pb = a.Pb, Sb = sb }).Zip(RunDefinition, (a, d) =>
                new { Latest = a.Lr, PersonalBest = a.Pb, SumOfBest = a.Sb, Definition = d }).ToArray();

            var isPersonalBest = IsPersonalBest(latestRun);
            var isGoldSplit = ContainsGoldSplit(latestRun);
            if (isPersonalBest || isGoldSplit)
            {
                if (isPersonalBest) PersonalBestDate = latestRun.StartTime;

                RunDefinition = zippedSplits.Select(a =>
                {
                    return new SplitInfo()
                    {
                        Name = a.Definition.Name,
                        PersonalBestSplit = isPersonalBest ? a.Latest.SplitInfo : a.Definition.PersonalBestSplit,
                        SumOfBestSplit = a.Latest != null && a.Latest.IsWellBounded && (a.Latest.Time < a.Definition.SumOfBestSplit.Time) ?
                            a.Latest.SplitInfo : a.Definition.SumOfBestSplit
                    };
                }).ToArray();

                generateRunsFromDefinition();
            };

            Save();
        }

        public bool CheckMergeSuggested(Run latestRun)
        {
            return ContainsGoldSplit(latestRun) || IsPersonalBest(latestRun);
        }

        public bool ContainsGoldSplit(Run latestRun)
        {
            //Check if any splits from the current run beat any gold split
            return latestRun.Splits.Zip(SumOfBest.Splits, (lr, sb) => new { Latest = lr, Best = sb })
                .Any(a => Split.IsGoldSplit(a.Latest, a.Best));
        }

        public bool IsPersonalBest(Run latestRun)
        {
            //Check if this run was better than personal best
            var lastLatestSplit = latestRun.Splits.Last();
            var lastPersonalBestSplit = PersonalBest.Splits.Last();

            if (lastLatestSplit == null) return false;

            return lastLatestSplit.TimeFromRunStart < lastPersonalBestSplit.TimeFromRunStart;
        }

        public static SplitFile Load(string path)
        {
            if (Uri.IsWellFormedUriString(path, UriKind.Absolute)) throw new Exception("Path is malformed.");

            DataContractSerializer dcs = new DataContractSerializer(typeof(SplitFile));

            SplitFile file;
            using (Stream stream = new FileStream(path, FileMode.Open))
            {
                file = (SplitFile)dcs.ReadObject(stream);
            }

            //Set path to be what we loaded
            file.Path = path;

            if (file.DisplaySettings == null) file.DisplaySettings = new DisplaySettings();

            return file;
        }

        public static SplitFile ImportFromWsplit(string path)
        {
            var firstLine = File.ReadLines(path).First();
            if (!firstLine.Contains("Title=")) throw new Exception("Cannot find Title.");

            var linesOfText = File.ReadAllLines(path);
            var runName = firstLine.Replace("Title=", string.Empty);

            //var heightWidth = linesOfText[3].Replace("Size=", string.Empty).Split(',');
            //var height = double.Parse(heightWidth[0]);
            //var width = double.Parse(heightWidth[1]);

            var decodedStrings = linesOfText.Skip(4).Take(linesOfText.Length - 5).Select(s =>
            {
                var commaSplit = s.Split(',');

                return new
                {
                    Name = commaSplit[0],
                    PbTime = TimeSpan.FromSeconds(double.Parse(commaSplit[2])),
                    Gold = TimeSpan.FromSeconds(double.Parse(commaSplit[3]))
                };
            }).ToArray();

            var splits = Enumerable.Range(0, decodedStrings.Length).Select(i =>
            {
                var split = decodedStrings[i];

                SplitTimeSpan pbSplit;
                    
                if (split.PbTime == TimeSpan.Zero)
                {
                    var lastKnownTime = decodedStrings.Take(i).Where(a => a.PbTime != TimeSpan.Zero).Select(a => a.PbTime).FirstOrDefault();
                    pbSplit = new SplitTimeSpan(lastKnownTime, false);
                }
                else if (i == 0) pbSplit = new SplitTimeSpan(split.PbTime);
                else pbSplit = new SplitTimeSpan(split.PbTime.Subtract(decodedStrings[i - 1].PbTime));

                return new SplitInfo()
                {
                    Name = split.Name,
                    PersonalBestSplit = pbSplit,
                    SumOfBestSplit = split.Gold == TimeSpan.Zero ? SplitTimeSpan.Unknown : new SplitTimeSpan(split.Gold)
                };
            }).ToArray();

            var result = new SplitFile(runName, splits);

            //result.DisplaySettings.WindowHeight = height;
            //result.DisplaySettings.WindowWidth = width;

            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
