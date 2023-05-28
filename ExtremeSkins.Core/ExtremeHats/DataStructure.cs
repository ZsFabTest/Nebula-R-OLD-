using System.IO;

namespace ExtremeSkins.Core.ExtremeHats;

public static class DataStructure
{
    public const string FolderName = "ExtremeHat";
    public const string FrontImageName     = "front.png";
    public const string FrontFlipImageName = "front_flip.png";
    public const string BackImageName      = "back.png";
    public const string BackFlipImageName  = "back_flip.png";
    public const string ClimbImageName     = "climb.png";

    public static string GetHatPath(string hatParentPath, string hatName)
        => Path.Combine(hatParentPath, hatName);

    public static string GetHatInfoPath(string hatParentPath, string hatName)
        => Path.Combine(hatParentPath, hatName, InfoBase.JsonName);
}
