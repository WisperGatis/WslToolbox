﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;
using WslToolbox.Core;
using WslToolbox.Gui.Collections;
using WslToolbox.Gui.Commands;
using WslToolbox.Gui.Handlers;
using WslToolbox.Gui.Views;

namespace WslToolbox.Gui.ViewModels
{
    public class MainViewModel
    {
        public readonly ConfigurationHandler Config = new();
        private readonly MainView View;
        public ICommand ShowApplicationCommand => new RelayCommand(ShowApplication, o => View.WindowState == System.Windows.WindowState.Minimized);
        public ICommand SaveConfigurationCommand => new RelayCommand(o => { Config.Save(); }, o => true);
        public ICommand ExitApplicationCommand => new RelayCommand(o => { Environment.Exit(-1); }, o => true);
        public ICommand StartWslServiceCommand => new RelayCommand(StartWslService, CanExecuteStart);
        public ICommand StopWslServiceCommand => new RelayCommand(StopWslService, CanExecuteStop);
        public ICommand RestartWslServiceCommand => new RelayCommand(RestartWslService, CanExecuteStop);
        public ICommand ShowSettingsCommand => new RelayCommand(ShowSettings, o => true);
        public Func<object, bool> CanExecuteStart = o => true;
        public Func<object, bool> CanExecuteStop = o => true;

        public MainViewModel(MainView view)
        {
            View = view;

            PollTimerInitializer();
        }

        public CompositeCollection SystemTrayMenuItems()
        {
            return SystemTrayMenuCollection.Items(this);
        }

        public void ShowSettings(object parameter)
        {
            SettingsView settingsWindow = new(Config.Configuration, Config);
            settingsWindow.ShowDialog();

            if ((bool)settingsWindow.DialogResult && SaveConfigurationCommand.CanExecute(null))
            {
                SaveConfigurationCommand.Execute(null);
            }
        }

        public async void StartWslService(object parameter)
        {
            _ = await ToolboxClass.StartWsl().ConfigureAwait(true);
        }

        public async void StopWslService(object parameter)
        {
            _ = await ToolboxClass.StopWsl().ConfigureAwait(true);
        }

        public void RestartWslService(object parameter)
        {
            StopWslServiceCommand.Execute(null);
            StartWslServiceCommand.Execute(null);
        }

        public async void CanStart(object parameter)
        {
            bool isRunning = await ToolboxClass.ServiceIsRunning();

            CanExecuteStart = o => Config.Configuration.PollServiceStatus ? !isRunning : true;
        }

        public async void CanStop(object parameter)
        {
            bool isRunning = await ToolboxClass.ServiceIsRunning();

            CanExecuteStop = o => Config.Configuration.PollServiceStatus ? isRunning : true;
        }

        public void PollTimerInitializer()
        {
            Timer poller = Config.Configuration.PollServiceStatus ? new(PollCallBack, null, 0, 2000) : null;
        }

        private async void PollCallBack(object o)
        {
            bool isRunning = await ToolboxClass.ServiceIsRunning();
            CanExecuteStop = o => isRunning;
            CanExecuteStart = o => !isRunning;
        }

        public void ShowApplication(object o)
        {
            View.WindowState = System.Windows.WindowState.Normal;
            View.Show();
        }
    }
}