using System.IO;

namespace ExtremeSkins.Core.ExtremeNamePlate;

public static class DataStructure
{
    public const string FolderName = "ExtremeNamePlate";
    public static string GetNamePlatePath(string npParentPath, string autherName, string namePlateName)
        => Path.Combine(npParentPath, autherName, $"{namePlateName}.png");
}
