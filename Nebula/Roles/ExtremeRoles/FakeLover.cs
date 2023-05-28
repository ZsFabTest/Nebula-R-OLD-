using Il2CppSystem.Data;
using Nebula.Patches;

namespace Nebula.Roles.ExtraRoles;

public class FakeLover : ExtraRole
{
    private Module.CustomOption maxPairsOption;
    public Module.CustomOption loversModeOption;
    public Module.CustomOption chanceThatOneLoverIsImpostorOption;
    private Module.CustomOption canChangeTrilemmaOption;
    public Module.CustomOption loversAsIndependentSideOption;
    public Module.CustomOption allowExtraWinningOption;

    static public Color[] iconColor { get; } = new Color[] {
        (Color)new Color32(251, 3, 188, 255) ,
        (Color)new Color32(254, 132, 3, 255) ,
        (Color)new Color32(3, 254, 188, 255) ,
        (Color)new Color32(255, 255, 0, 255) ,
        (Color)new Color32(3, 183, 254, 255) };

    public override Assignable AssignableOnHelp => Roles.Lover;
    public override HelpSprite[] helpSprite => Roles.Lover.helpSprite;

    public override void EditOthersDisplayName(byte playerId, ref string displayName, bool hideFlag){
        if(playerId == Game.GameData.data.myData.getGlobalData().GetExtraRoleData(this)) displayName += Helpers.cs(iconColor[0],"♥");
    }

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag)
    {
        if(playerId == PlayerControl.LocalPlayer.PlayerId) displayName += Helpers.cs(iconColor[0],"♥");
    }

    public override void EditDescriptionString(ref string desctiption)
    {
        string partner = Helpers.cs(Color,Helpers.playerById((byte)Game.GameData.data.myData.getGlobalData().GetExtraRoleData(this)).name);
        desctiption += "\n" + Language.Language.GetString("role.lover.description").Replace("%NAME%", partner);
    }

    private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.InvolveButton.png", 115f, "ui.button.lover.involve");

    public virtual bool IsSpawnable()
    {
        return false;
    }

    public override bool HasCrewmateTask(byte playerId)
    {
        return false;
    }

    public FakeLover() : base("Lover", "lover", iconColor[0], 0)
    {
        FixedRoleCount = true;
        IsHideRole = true;
    }
    //
}