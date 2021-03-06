﻿// Copyright 2017 Gizeta
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using ShadowWatcher.Socket;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;

namespace ShadowWatcher
{
    public class SettingModel : INotifyPropertyChanged
    {
        private IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly();

        public SettingModel()
        {
            if (!storage.FileExists("setting"))
            {
                Save();
            }
            else
            {
                var reader = new StreamReader(new IsolatedStorageFileStream("setting", FileMode.Open, storage));
                Settings.Parse(reader.ReadLine());
            }
        }

        public bool RecordEnemyCard
        {
            get => Settings.RecordEnemyCard;
            set
            {
                if (Settings.RecordEnemyCard != value)
                {
                    Settings.RecordEnemyCard = value;
                    NotifyPropertyChanged("RecordEnemyCard");
                }
            }
        }

        public bool RecordPlayerCard
        {
            get => Settings.RecordPlayerCard;
            set
            {
                if (Settings.RecordPlayerCard != value)
                {
                    Settings.RecordPlayerCard = value;
                    NotifyPropertyChanged("RecordPlayerCard");
                }
            }
        }

        public bool EnhanceReplay
        {
            get => Settings.EnhanceReplay;
            set
            {
                if (Settings.EnhanceReplay != value)
                {
                    Settings.EnhanceReplay = value;
                    NotifyPropertyChanged("EnhanceReplay");
                }
            }
        }

        public bool ShowSummonCard
        {
            get => Settings.ShowSummonCard;
            set
            {
                if (Settings.ShowSummonCard != value)
                {
                    Settings.ShowSummonCard = value;
                    NotifyPropertyChanged("ShowSummonCard");
                }
            }
        }

        public bool CopyAnimatedCardFirst
        {
            get => Settings.CopyAnimatedCardFirst;
            set
            {
                if (Settings.CopyAnimatedCardFirst != value)
                {
                    Settings.CopyAnimatedCardFirst = value;
                    NotifyPropertyChanged("CopyAnimatedCardFirst");
                }
            }
        }

        public bool ShowCountdown
        {
            get => Settings.ShowCountdown;
            set
            {
                if (Settings.ShowCountdown != value)
                {
                    Settings.ShowCountdown = value;
                    NotifyPropertyChanged("ShowCountdown");
                }
            }
        }

        public bool KeyboardFilterShortcut
        {
            get => Settings.KeyboardFilterShortcut;
            set
            {
                if (Settings.KeyboardFilterShortcut != value)
                {
                    Settings.KeyboardFilterShortcut = value;
                    NotifyPropertyChanged("KeyboardFilterShortcut");
                }
            }
        }

        public void Save()
        {
            var writer = new StreamWriter(new IsolatedStorageFileStream("setting", FileMode.Create, storage));
            writer.Write(Settings.ToString());
            writer.Close();

            Sender.Send("Setting", Settings.ToString());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Save();
        }
    }
}
