﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using WslToolbox.Gui.Handlers;
using WslToolbox.Gui.Helpers.Ui;
using WslToolbox.Gui.Validators;
using WslToolbox.Gui.ViewModels;

namespace WslToolbox.Gui.Collections.Dialogs
{
    public sealed class ImportDistributionDialogCollection : INotifyPropertyChanged
    {
        private string _distributionName;

        private bool _distributionNameIsValid;
        private bool _runAfterImport;

        private string _selectedBasePath;

        private string _selectedFilePath;

        public string DistributionName
        {
            get => _distributionName;
            set
            {
                if (value == _distributionName) return;
                _distributionName = value;
                OnPropertyChanged(nameof(DistributionName));
            }
        }

        public bool DistributionNameIsValid
        {
            get => _distributionNameIsValid;
            set
            {
                if (value == _distributionNameIsValid) return;
                _distributionNameIsValid = value;
                OnPropertyChanged(nameof(DistributionNameIsValid));
            }
        }

        public string SelectedBasePath
        {
            get => _selectedBasePath;
            set
            {
                if (value == _selectedBasePath) return;
                _selectedBasePath = value;
                OnPropertyChanged(nameof(SelectedBasePath));
            }
        }

        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set
            {
                if (value == _selectedFilePath) return;
                _selectedFilePath = value;
                OnPropertyChanged(nameof(SelectedFilePath));
            }
        }

        public bool RunAfterImport
        {
            get => _runAfterImport;
            set
            {
                if (value == _runAfterImport) return;
                _runAfterImport = value;
                OnPropertyChanged(nameof(RunAfterImport));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<Control> Items(MainViewModel viewModel)
        {
            var userBasePath = viewModel.Config.Configuration.UserBasePath;
            var distributionFileBrowse = new Button {Content = "Browse..."};
            var distributionBasePathBrowse = new Button {Content = "Browse..."};

            SelectedBasePath = Directory.Exists(userBasePath) ? userBasePath : null;
            distributionFileBrowse.Click += (_, _) => { SelectDistributionFile(); };
            distributionBasePathBrowse.Click += (_, _) => { SelectDistributionBasePath(); };

            Control[] items =
            {
                new Label {Content = "Name:", Margin = new Thickness(0, 0, 0, 2), FontWeight = FontWeights.Bold},
                new Label
                {
                    Content = "- Only alphanumeric characters are allowed.\n" +
                              "- Name must contain at least 3 characters.",
                    Margin = new Thickness(0, 0, 0, 10)
                },
                ElementHelper.AddTextBox(nameof(DistributionName), bind: "DistributionName", width: 400, source: this,
                    isReadonly: false, isEnabled: true, updateSourceTrigger: UpdateSourceTrigger.PropertyChanged,
                    placeholder: "Name your distribution"),
                ElementHelper.Separator(),

                new Label {Content = "Filename:", Margin = new Thickness(0, 0, 0, 2), FontWeight = FontWeights.Bold},
                ElementHelper.AddTextBox(nameof(SelectedFilePath),
                    null, "SelectedFilePath", this, width: 400,
                    bindingMode: BindingMode.TwoWay, updateSourceTrigger: UpdateSourceTrigger.PropertyChanged,
                    placeholder: "Select an exported distribution file."),
                ElementHelper.Separator(0),
                distributionFileBrowse,

                ElementHelper.Separator(),
                new Label {Content = "Base path:", Margin = new Thickness(0, 0, 0, 2), FontWeight = FontWeights.Bold},
                ElementHelper.AddTextBox(nameof(SelectedBasePath),
                    bind: "SelectedBasePath", source: this, width: 400, bindingMode: BindingMode.TwoWay,
                    updateSourceTrigger: UpdateSourceTrigger.PropertyChanged,
                    placeholder: "Select an installation directory"),
                ElementHelper.Separator(0),
                distributionBasePathBrowse
            };

            return items;
        }

        private void SelectDistributionFile()
        {
            var distributionFilePathDialog = FileDialogHandler.OpenFileDialog();

            SelectedFilePath = distributionFilePathDialog.ShowDialog() == null
                ? null
                : distributionFilePathDialog.FileName;
        }

        private void SelectDistributionBasePath()
        {
            OpenFileDialog openLocation = new()
            {
                Title = "Select distribution base path",
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Select folder",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            SelectedBasePath = openLocation.ShowDialog() == null ? null : Path.GetDirectoryName(openLocation.FileName);
        }

        private bool ValidateImportValues()
        {
            return DistributionName != null
                   && DistributionNameValidator.ValidateName(DistributionName)
                   && SelectedBasePath is {Length: > 1}
                   && SelectedFilePath is {Length: > 1}
                   && Directory.Exists(SelectedBasePath)
                   && File.Exists(SelectedFilePath);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Debug.WriteLine($"{propertyName} changed.");
            try
            {
                DistributionNameIsValid = ValidateImportValues();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}