using Microsoft.Windows.ApplicationModel.Resources;

namespace WslToolbox.UI.Helpers;

public static class ResourceExtensions
{
    private static readonly ResourceLoader ResourceLoader = new();

    public static string GetLocalized(this string resourceKey)
    {
        return ResourceLoader.GetString(resourceKey);
    }
}