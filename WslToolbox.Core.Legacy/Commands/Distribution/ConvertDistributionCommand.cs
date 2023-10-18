using System;
using System.Threading.Tasks;

namespace WslToolbox.Core.Legacy.Commands.Distribution;

public static class ConvertDistributionCommand
{
    private const string Command = "wsl --set-version {0} 2";

    public static async Task<CommandClass> Execute(DistributionClass distribution)
    {
        if (distribution == null) throw new ArgumentNullException(nameof(distribution));
        return await Task.Run(() =>
        {
            return CommandClass.ExecuteCommand(string.Format(
                Command, distribution.Name
            ));
        }).ConfigureAwait(true);
    }
}