namespace Nebula.Roles.ExtraRoles;

public class Flash : Template.StandardExtraRole
{
    public static Color RoleColor = new Color(247f / 255f, 207f / 255f, 19f / 255f);

    public static Module.CustomOption CustomSpeed;

    public override void LoadOptionData()
    {
        CustomSpeed = CreateOption(Color.white, "customSpeed", 2f, 5f, 1f, 0.5f);
        CustomSpeed.suffix = "cross";
    }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        base.GlobalInitialize(__instance);
        RPCEvents.EmitSpeedFactor(__instance.PlayerId, new Game.SpeedFactor(0, 114514f, CustomSpeed.getFloat(), true));
    }

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag)
    {
        bool showFlag = false;
        if (playerId == PlayerControl.LocalPlayer.PlayerId || Game.GameData.data.myData.CanSeeEveryoneInfo) showFlag = true;

        if (showFlag) EditDisplayNameForcely(playerId, ref displayName);
    }

    public override void EditDisplayNameForcely(byte playerId, ref string displayName)
    {
        displayName += Helpers.cs(
                RoleColor, "έ");
    }

    public Flash() : base("Flash", "flash", RoleColor, 0)
    {
    }
}