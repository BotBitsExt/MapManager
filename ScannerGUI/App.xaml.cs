﻿using System.Windows;
using BotBits;
using MapManager.Events;

namespace ScannerGUI
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private LoginWindow loginWindow;
        private ScannerWindow scannerWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var client = new BotBitsClient();
            EventLoader.Of(client).Load(this);

            loginWindow = new LoginWindow(client);
            loginWindow.Show();

            scannerWindow = new ScannerWindow(client);
        }

        [EventListener]
        private void OnLoggedIn(InitializationCompleteEvent e)
        {
            loginWindow.Close();
            scannerWindow.Show();
        }
    }

    public sealed class LoggedInEvent : Event<LoggedInEvent>
    {
    }
}