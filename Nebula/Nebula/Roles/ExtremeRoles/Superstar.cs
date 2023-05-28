namespace Nebula.Roles.CrewmateRoles;

public class Superstar : Role{
    public static Color RoleColor = new Color(255f / 255f,242f / 255f,0f / 255f);

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
    }

    public override void EditDisplayNameForcely(byte playerId, ref string displayName)
    {
        displayName += Helpers.cs(RoleColor,"☆");
    }

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag) => EditDisplayRoleNameForcely(playerId,ref displayName);

    public override void EditDisplayNameColor(byte playerId, ref Color displayColor)
    {
        displayColor = RoleColor;
    }

    public Superstar()
         : base("Superstar","superstar",RoleColor,RoleCategory.Crewmate,Side.Crewmate,Side.Crewmate,
                Crewmate.crewmateSideSet,Crewmate.crewmateSideSet,Crewmate.crewmateEndSet,
                false,VentPermission.CanNotUse,false,false,false){
        IsGuessableRole = false;
    }
}