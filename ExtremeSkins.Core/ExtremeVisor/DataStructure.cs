using System.IO;

namespace ExtremeSkins.Core.ExtremeVisor;

public static class DataStructure
{
    public const string FolderName = "ExtremeVisor";
    public const string IdleImageName = "idle.png";
    public const string FlipIdleImageName = "flip_idle.png";

    public static string GetVisorPath(string visorParentPath, string visorName)
        => Path.Combine(visorParentPath, visorName);

    public static string GetVisorInfoPath(string visorParentPath, string visorName)
        => Path.Combine(visorParentPath, visorName, InfoBase.JsonName);
}
