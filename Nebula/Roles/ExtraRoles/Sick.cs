namespace Nebula.Roles.ExtraRoles;

public class Sick : Template.StandardExtraRole{

    public static Color RoleColor = new Color(127f / 255f, 127f / 255f, 127f / 255f);

    public int sickDataId { get; private set; }

    public override void LoadOptionData(){
        base.LoadOptionData();
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
                RoleColor, "&");
    }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        base.GlobalInitialize(__instance);
    }

    public override void OnDied(byte playerId){
        if (MeetingHud.Instance) return;
        byte target = Game.GameData.data.deadPlayers[playerId].MurderId;
        if (target != PlayerControl.LocalPlayer.PlayerId) return;

        RPCEventInvoker.AddExtraRole(PlayerControl.LocalPlayer, Roles.Sick, 0);
    }

    public override Module.CustomOption? RegisterAssignableOption(Role role)
    {
        Module.CustomOption option = role.CreateOption(new Color(0.8f, 0.95f, 1f), "option.canBeSick", role.CanHaveExtraAssignable(this), true).HiddenOnDisplay(true).SetIdentifier("role." + role.LocalizeName + ".canBeSick");
        option.AddPrerequisite(CustomOptionHolder.advanceRoleOptions);
        option.AddCustomPrerequisite(() => { return Roles.Sick.IsSpawnable(); });
        return option;
    }

    public Sick() : base("Sick","sick",RoleColor,0){
    }

}