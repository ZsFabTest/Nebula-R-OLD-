using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ExtremeSkins.Core;

public abstract record InfoBase(string Name, string Author)
{
    public const string JsonName = "info.json";

    public static JsonSerializerOptions JsonSerializeOption => new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
    };

    public static void ExportToJson<T>(T info, string targetPath) where T : InfoBase
    {
        File.WriteAllText(
            Path.Combine(targetPath, JsonName),
            JsonSerializer.Serialize(info, JsonSerializeOption));
    }
}
