using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using WslToolbox.UI.Contracts.Services;
using WslToolbox.UI.ViewModels;
using WslToolbox.UI.Views.Pages;

namespace WslToolbox.UI.Services;

public class PageService : IPageService
{
    private readonly Dictionary<string, Type> _pages = new();

    public PageService()
    {
        Configure<DashboardViewModel, DashboardPage>();
        Configure<SettingsViewModel, SettingsPage>();
        Configure<DeveloperViewModel, DeveloperPage>();
        Configure<LogViewModel, LogPage>();
        Configure<WslSettingsViewModel, WslSettingsPage>();
    }

    public Type GetPageType(string key)
    {
        Type? pageType;
        lock (_pages)
        {
            if (!_pages.TryGetValue(key, out pageType))
            {
                throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
            }
        }

        return pageType;
    }

    private void Configure<TVm, TV>()
        where TVm : ObservableObject
        where TV : Page
    {
        lock (_pages)
        {
            var key = typeof(TVm).FullName!;
            if (_pages.ContainsKey(key))
            {
                throw new ArgumentException($"The key {key} is already configured in PageService");
            }

            var type = typeof(TV);
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (_pages.Any(p => p.Value == type))
            {
                throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == type).Key}");
            }

            _pages.Add(key, type);
        }
    }
}