namespace Nebula.Roles.ExtraRoles;

public class Flash : Template.StandardExtraRole
{
    public static Color RoleColor = new Color(239f / 255f, 136f / 255f, 22f / 255f);

    private Module.CustomOption CustomSpeed;

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
        CustomSpeed = CreateOption(Color.white, "customSpeed", 2f, 1.25f, 3f, 0.25f);
        CustomSpeed.suffix = "cross";
    }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        base.GlobalInitialize(__instance);
        RPCEvents.EmitSpeedFactor(__instance.PlayerId, new Game.SpeedFactor(114, 99999f, CustomSpeed.getFloat(), true));
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
                RoleColor, "©");
    }

    public override void EditSpawnableRoleShower(ref string suffix, Role role)
    {
        if (IsSpawnable() && role.CanHaveExtraAssignable(this)) suffix += Helpers.cs(Color, "©");
    }

    public override void EditDescriptionString(ref string description)
    {
        description += "\n" + Language.Language.GetString("role.flash.description");
    }

    public override Module.CustomOption? RegisterAssignableOption(Role role)
    {
        Module.CustomOption option = role.CreateOption(new Color(0.8f, 0.95f, 1f), "option.canBeFlash", role.DefaultExtraAssignableFlag(this), true).HiddenOnDisplay(true).SetIdentifier("role." + role.LocalizeName + ".canBeFlash");
        option.AddPrerequisite(CustomOptionHolder.advanceRoleOptions);
        option.AddCustomPrerequisite(() => { return IsSpawnable(); });
        return option;
    }

    public Flash() : base("Flash", "flash", RoleColor, 1)
    {
    }
}