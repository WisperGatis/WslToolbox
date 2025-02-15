﻿using System.Diagnostics;
using System.Net.Http.Headers;
using Windows.ApplicationModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.UI.Xaml;
using Serilog;
using WslToolbox.UI.Activation;
using WslToolbox.UI.Contracts.Services;
using WslToolbox.UI.Core.Configurations;
using WslToolbox.UI.Core.Contracts.Services;
using WslToolbox.UI.Core.Helpers;
using WslToolbox.UI.Core.Models;
using WslToolbox.UI.Core.Services;
using WslToolbox.UI.Extensions;
using WslToolbox.UI.Services;
using WslToolbox.UI.ViewModels;
using WslToolbox.UI.Views.Modals;
using WslToolbox.UI.Views.Pages;
using UnhandledExceptionEventArgs = Microsoft.UI.Xaml.UnhandledExceptionEventArgs;

namespace WslToolbox.UI;

public partial class App : Application
{
    public const string Name = "WSL Toolbox";
    public static readonly bool IsDeveloper = Debugger.IsAttached;
    private readonly ILogger<App> _logger;

    public App()
    {
        InitializeComponent();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Toolbox.UserConfiguration, true, true)
            .AddJsonFile(Toolbox.LogConfiguration, true, true)
            .AddEnvironmentVariables()
            .Build();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Error()
            .ReadFrom.Configuration(configuration)
            .WriteTo.File(Toolbox.LogFile)
            .CreateLogger();

        Log.Logger.Debug("Logger initialized");
        Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(builder => { builder.AddConfiguration(configuration); })
            .UseContentRoot(AppContext.BaseDirectory)
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                services.AddAutoMapper(typeof(AutoMapperProfiles));

                // Default Activation Handler
                services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                // Clients
                services.AddHttpClient<DownloadService>(c =>
                {
                    c.BaseAddress = Toolbox.GitHubDownloadUrl;
                    c.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
                });
                services.AddHttpClient<UpdateService>(c =>
                {
                    c.BaseAddress = Toolbox.GitHubManifestFile;
                    c.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
                });

                // Services
                services.AddSingleton<AppNotificationService>();
                services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
                services.AddTransient<INavigationViewService, NavigationViewService>();
                services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
                services.AddSingleton<LogService>();

                services.AddSingleton<IActivationService, ActivationService>();
                services.AddSingleton<IPageService, PageService>();
                services.AddSingleton<INavigationService, NavigationService>();

                // Core Services
                services.AddSingleton<IDistributionDataService, DistributionDataService>();
                services.AddSingleton<IFileService, FileService>();
                services.AddSingleton<IConfigurationService, ConfigurationService>();
                services.AddSingleton<DistributionService>();

                // Views and ViewModels
                services.AddPage<ShellViewModel, ShellPage>();
                services.AddPage<DashboardViewModel, DashboardPage>();
                services.AddPage<SettingsViewModel, SettingsPage>();
                services.AddPage<NotificationViewModel, NotificationModal>();
                services.AddPage<DeveloperViewModel, DeveloperPage>();
                services.AddPage<LogViewModel, LogPage>();
                services.AddPage<WslSettingsViewModel, WslSettingsPage>();

                services.AddSingleton<StartupDialogViewModel>();

                // Configuration
                services.Configure<UserOptions>(context.Configuration.GetSection(nameof(UserOptions)));
                services.Configure<NotificationOptions>(context.Configuration.GetSection(nameof(NotificationOptions)));
            }).Build();


        _logger = GetService<ILogger<App>>();
        GetService<AppNotificationService>().Initialize();

        UnhandledException += (o, args) => App_UnhandledException(o, args);
    }

    private IHost Host { get; }

    public static UserOptions GetUserOptions()
    {
        var optionsClass = GetService<IOptions<UserOptions>>();

        return optionsClass.Value;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static bool IsPackage()
    {
        try
        {
            return Package.Current.Id != null;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public static T GetService<T>()
        where T : class
    {
        if ((Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        try
        {
            _logger.LogError(e.Exception, "An UI exception has occurred: {Message}", e.Message);
        }
        catch (Exception)
        {
            throw new Exception($"Unexpected error {e.Message}");
        }

#if DEBUG
        throw new Exception($"Unexpected error {e.Message}");
#endif
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        await GetService<IActivationService>().ActivateAsync(args);
    }
}