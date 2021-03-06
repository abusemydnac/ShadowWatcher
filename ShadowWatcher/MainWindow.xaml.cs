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

using Microsoft.Win32;
using ShadowWatcher.Contract;
using ShadowWatcher.Socket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace ShadowWatcher
{
    public partial class MainWindow : Window
    {
        private bool isAttached = false;

        public CardList EnemyDeckList { get; set; } = new CardList();
        public CardList PlayerDeckList { get; set; } = new CardList();
        public SettingModel Setting { get; set; } = new SettingModel();
        public Countdown Countdown { get; set; } = new Countdown();

        public MainWindow()
        {
            InitializeComponent();

            Receiver.Initialize();
            MainTab.IsEnabled = false;

            DataContext = this;
            CountdownGrid.DataContext = Countdown;
        }

        private void attachObserver()
        {
            Receiver.OnReceived = Receiver_OnReceived;

            var result = Injector.Attach();
            if (result != 0)
            {
                MessageBox.Show($"Error: {result}");
                return;
            }

            isAttached = true;
            MainTab.IsEnabled = true;
        }

        private void detachObserver(bool closing = false)
        {
            var result = Injector.Detach();
            if (result != 0 && !closing)
            {
                MessageBox.Show($"Error: {result}");
                return;
            }

            Receiver.OnReceived = null;
            isAttached = false;
            MainTab.IsEnabled = false;
        }

        private void Receiver_OnReceived(string action, string data)
        {
            switch (action)
            {
                case "BattleReady":
                    Dispatcher.Invoke(() =>
                    {
                        EnemyDeckList.Clear();
                    });
                    break;
                case "Load":
                    Sender.Initialize(int.Parse(data));
                    Sender.Send("Setting", $"{Settings.ToString()}");
                    break;
                case "EnemyPlay":
                case "EnemyAdd":
                    Dispatcher.Invoke(() =>
                    {
                        EnemyDeckList.Add(CardData.Parse(data, action == "EnemyPlay" ? 1 : -1));
                    });
                    break;
                case "PlayerDeck":
                    var cardList = new List<CardData>();
                    var cards = data.Split('\n');
                    foreach (var card in cards)
                    {
                        cardList.Add(CardData.Parse(card));
                    }                    

                    Dispatcher.Invoke(() =>
                    {
                        PlayerDeckList.Clear();
                        PlayerDeckList.Add(cardList);
                    });
                    break;
                case "PlayerDraw":
                    Dispatcher.Invoke(() =>
                    {
                        PlayerDeckList.Add(CardData.Parse(data, -1));
                    });
                    break;
                case "ReplayDetail":
                    Dispatcher.Invoke(() =>
                    {
                        ReplayGrid.DataContext = ReplayData.Parse(data);
                    });
                    break;
                case "Countdown":
                    Countdown.Start();
                    break;
                case "PlayerTurnEnd":
                    Countdown.Stop();
                    break;
            }
            Dispatcher.Invoke(() =>
            {
                LogText.Text = $"{action}:{data}\n{LogText.Text}";
            });
        }

        private void AttachButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isAttached)
                attachObserver();
            else
                detachObserver();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (isAttached)
                detachObserver(true);
        }

        private void RepSaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.DefaultExt = ".json";
            dialog.Filter = "对战数据 (.json)|*.json";
            if (dialog.ShowDialog() == true)
            {
                var stream = new StreamWriter(dialog.FileName);
                stream.Write((ReplayGrid.DataContext as ReplayData).ToString());
                stream.Close();
            }
        }

        private void RepLoadButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.DefaultExt = ".json";
            dialog.Filter = "对战数据 (.json)|*.json";
            if (dialog.ShowDialog() == true)
            {
                var stream = new StreamReader(dialog.FileName);
                var json = stream.ReadToEnd();
                stream.Close();

                ReplayGrid.DataContext = ReplayData.Parse(json);

                Sender.Send("ReplayRequest", $"{json}");
            }
        }
    }
}
