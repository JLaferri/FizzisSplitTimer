using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fizzi.Applications.Splitter.Common;
using System.Windows.Input;
using System.Reactive.Linq;
using System.ComponentModel;
using Fizzi.Applications.Splitter.Properties;

namespace Fizzi.Applications.Splitter.ViewModel
{
    class SettingsViewModel : INotifyPropertyChanged
    {
        private bool _isPendingHotkeyChange;
        public bool IsPendingHotkeyChange { get { return _isPendingHotkeyChange; } set { this.RaiseAndSetIfChanged("IsPendingHotkeyChange", ref _isPendingHotkeyChange, value, PropertyChanged); } }

        public Dictionary<HotkeyAction, Notifiable<string>> KeyDisplayStrings { get; private set; }

        public ICommand ChangeHotkey { get; private set; }
        public ICommand ClearHotkey { get; private set; }

        private Dictionary<HotkeyAction, Key?> selectedKeyStorage = new Dictionary<HotkeyAction, Key?>();

        public SettingsViewModel(KeyboardListener keyListener)
        {
            KeyDisplayStrings = new Dictionary<HotkeyAction, Common.Notifiable<string>>();

            KeyDisplayStrings[HotkeyAction.Split] = new Common.Notifiable<string>(Settings.Default.SplitKey == null ? "Unset" : Settings.Default.SplitKey.ToString());
            KeyDisplayStrings[HotkeyAction.Unsplit] = new Common.Notifiable<string>(Settings.Default.UnsplitKey == null ? "Unset" : Settings.Default.UnsplitKey.ToString());
            KeyDisplayStrings[HotkeyAction.Reset] = new Common.Notifiable<string>(Settings.Default.ResetKey == null ? "Unset" : Settings.Default.ResetKey.ToString());
            KeyDisplayStrings[HotkeyAction.Skip] = new Common.Notifiable<string>(Settings.Default.SkipKey == null ? "Unset" : Settings.Default.SkipKey.ToString());
            KeyDisplayStrings[HotkeyAction.Pause] = new Common.Notifiable<string>(Settings.Default.PauseKey == null ? "Unset" : Settings.Default.PauseKey.ToString());

            var keyObservable = Observable.FromEventPattern<Common.RawKeyEventHandler, Common.RawKeyEventArgs>(
                h => keyListener.KeyUp += h, h => keyListener.KeyUp -= h);

            ChangeHotkey = Command.Create<HotkeyAction>(_ => true, ha =>
            {
                IsPendingHotkeyChange = true;
                KeyDisplayStrings[ha].Value = "<< Press Key To Set Hotkey >>";

                keyObservable.Take(1).LastAsync().Subscribe(key =>
                {
                    selectedKeyStorage[ha] = key.EventArgs.Key;
                    KeyDisplayStrings[ha].Value = key.EventArgs.Key.ToString();
                    IsPendingHotkeyChange = false;
                });
            });

            ClearHotkey = Command.Create<HotkeyAction>(_ => true, ha =>
            {
                KeyDisplayStrings[ha].Value = "Unset";
                selectedKeyStorage[ha] = null;
            });
        }

        public void Refresh()
        {
            Settings.Default.Reload();

            KeyDisplayStrings[HotkeyAction.Split].Value = Settings.Default.SplitKey == null ? "Unset" : Settings.Default.SplitKey.ToString();
            KeyDisplayStrings[HotkeyAction.Unsplit].Value = Settings.Default.UnsplitKey == null ? "Unset" : Settings.Default.UnsplitKey.ToString();
            KeyDisplayStrings[HotkeyAction.Reset].Value = Settings.Default.ResetKey == null ? "Unset" : Settings.Default.ResetKey.ToString();
            KeyDisplayStrings[HotkeyAction.Skip].Value = Settings.Default.SkipKey == null ? "Unset" : Settings.Default.SkipKey.ToString();
            KeyDisplayStrings[HotkeyAction.Pause].Value = Settings.Default.PauseKey == null ? "Unset" : Settings.Default.PauseKey.ToString();
        }

        public void Save()
        {
            foreach (var kvp in selectedKeyStorage)
            {
                switch (kvp.Key)
                {
                    case HotkeyAction.Split:
                        Settings.Default.SplitKey = kvp.Value;
                        break;
                    case HotkeyAction.Unsplit:
                        Settings.Default.UnsplitKey = kvp.Value;
                        break;
                    case HotkeyAction.Reset:
                        Settings.Default.ResetKey = kvp.Value;
                        break;
                    case HotkeyAction.Skip:
                        Settings.Default.SkipKey = kvp.Value;
                        break;
                    case HotkeyAction.Pause:
                        Settings.Default.PauseKey = kvp.Value;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            Settings.Default.Save();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
