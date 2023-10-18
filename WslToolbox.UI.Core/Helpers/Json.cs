using Newtonsoft.Json;

namespace WslToolbox.UI.Core.Helpers;

public static class Json
{
    public static async Task<T> ToObjectAsync<T>(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        return await Task.Run(() => JsonConvert.DeserializeObject<T>(value));
    }

    public static async Task<string> StringifyAsync(object value)
    {
        return await Task.Run(() => JsonConvert.SerializeObject(value));
    }
}