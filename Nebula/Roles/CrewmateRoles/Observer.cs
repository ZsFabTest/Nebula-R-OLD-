namespace Nebula.Roles.CrewmateRoles;

public class Observer : Role
{
    public static Color RoleColor = new Color(200f / 255f, 190f / 255f, 230 / 255f);

    public Observer()
        : base("Observer", "observer", RoleColor, RoleCategory.Crewmate, Side.Crewmate, Side.Crewmate,
             Crewmate.crewmateSideSet, Crewmate.crewmateSideSet, Crewmate.crewmateEndSet,
             true, VentPermission.CanUseUnlimittedVent, false, true, true)
    {
    }
}
