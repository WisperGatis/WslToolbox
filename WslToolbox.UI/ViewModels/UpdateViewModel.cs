﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace WslToolbox.UI.ViewModels;

public partial class UpdateViewModel : ObservableRecipient
{
    [ObservableProperty]
    private bool _enableInstallUpdate;

    [ObservableProperty]
    private Version _latestVersion;
    
    [ObservableProperty]
    private Version _currentVersion;
}