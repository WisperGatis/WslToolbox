﻿using WslToolbox.Gui.Views;

namespace WslToolbox.Gui.Commands.Distribution
{
    public class RefreshDistributionsCommand : GenericCommand
    {
        private MainView _mainView;

        public RefreshDistributionsCommand(MainView mainView)
        {
            _mainView = mainView;
            IsExecutableDefault = o => true;
            IsExecutable = IsExecutableDefault;
        }

        public override void Execute(object parameter)
        {
            _mainView ??= (MainView) parameter;
            _mainView.PopulateWsl();
        }
    }
}