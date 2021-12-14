﻿using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WslToolbox.Gui.Configurations;
using WslToolbox.Gui.Handlers;
using WslToolbox.Gui.Helpers.Ui;
using WslToolbox.Gui.ViewModels;
using ksc = WslToolbox.Gui.Collections.Settings.KeyboardShortcutSettingsGenericCollection;

namespace WslToolbox.Gui.Collections.Settings
{
    public class KeyboardShortcutSettingsGenericCollection : GenericCollection
    {
        private readonly List<KeyboardShortcut> _shortcuts;

        public KeyboardShortcutSettingsGenericCollection(object source) : base(source)
        {
            var settingsViewModel = (SettingsViewModel) source;
            _shortcuts = settingsViewModel.KeyboardShortcutHandler.KeyboardShortcuts;
        }

        public CompositeCollection Items()
        {
            return new CompositeCollection
            {
                ElementHelper.AddToggleSwitch(nameof(DefaultConfiguration.KeyboardShortcutConfiguration.Enabled),
                    "Enable keyboard shortcuts", "Configuration.KeyboardShortcutConfiguration.Enabled", Source,
                    header: null),
                new Separator(),
                ElementHelper.ItemsControlGroup(ShortcutControls(), source: Source,
                    requires: "Configuration.KeyboardShortcutConfiguration.Enabled")
            };
        }

        private CompositeCollection ShortcutControls()
        {
            var keyboardChecks = new CompositeCollection();

            foreach (var shortcut in _shortcuts)
            {
                var shortCutKey = string.Empty;

                if (shortcut.Modifier != ModifierKeys.None)
                    shortCutKey = $"{shortcut.Modifier.ToString()} + ";

                shortCutKey = $"{shortCutKey}{shortcut.Key}";

                var shortcutLine = new StackPanel {Orientation = Orientation.Horizontal};
                shortcutLine.Children.Add(ElementHelper.AddToggleSwitch(shortcut.Configuration,
                    $"{shortcut.Name}",
                    $"Configuration.KeyboardShortcutConfiguration.{shortcut.Configuration}", Source,
                    header: null));
                shortcutLine.Children.Add(ElementHelper.Separator(marginLeft: 10));
                shortcutLine.Children.Add(new TextBox
                {
                    Text = shortCutKey,
                    IsReadOnly = true,
                    IsEnabled = false
                });

                keyboardChecks.Add(shortcutLine);
            }

            return keyboardChecks;
        }
    }
}