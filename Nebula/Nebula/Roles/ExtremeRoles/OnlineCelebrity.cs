namespace Nebula.Roles.CrewmateRoles;

public class OnlineCelebrity : Role
{
    public static Color RoleColor = new Color(255f / 255f, 72f / 255f, 72f / 255f);

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
    }

    public override void OnDied(byte playerId)
    {
        if(PlayerControl.LocalPlayer.GetModData().role.side == Side.Crewmate) Helpers.PlayQuickFlash(RoleColor);
    }

    public OnlineCelebrity()
        : base("OnlineCelebrity", "onlinecelebrity", RoleColor, RoleCategory.Crewmate, Side.Crewmate, Side.Crewmate,
             Crewmate.crewmateSideSet, Crewmate.crewmateSideSet, Crewmate.crewmateEndSet,
             false, VentPermission.CanNotUse, false, true, false)
    {
    }
}
