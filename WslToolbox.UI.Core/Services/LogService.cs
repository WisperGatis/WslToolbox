using System.Diagnostics;
using WslToolbox.UI.Core.EventArguments;
using WslToolbox.UI.Core.Helpers;

namespace WslToolbox.UI.Core.Services;

public class LogService
{
    public static event EventHandler<LogFileChangedEventArgs> LogFileChanged;

    public LogService()
    {
    }

    public static Task ReadLog()
    {
        return ReadLog(null);
    }

    public static Task ReadLog(CancellationTokenSource cancellationToken)
    {
        if (cancellationToken == null) throw new ArgumentNullException(nameof(cancellationToken));
        var watcher = new FileSystemWatcher();
        watcher.Path = Toolbox.AppData;
        watcher.Filter = "blaat.txt";
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.EnableRaisingEvents = true; 

        // ReSharper disable once HeapView.ObjectAllocation.Possible
        watcher.Changed += async (_, _) => await OnLogFileChanged(cancellationToken);
        return Task.CompletedTask;
    }

    private static async Task OnLogFileChanged(CancellationTokenSource cancellationToken)
    {
        await using var fileStream = File.Open("data\\blaat.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(fileStream);
        var line = await reader.ReadLineAsync(cancellationToken.Token);
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        Debug.WriteLine(line);
        LogFileChanged?.Invoke(nameof(LogService), new LogFileChangedEventArgs(line));
    }
}