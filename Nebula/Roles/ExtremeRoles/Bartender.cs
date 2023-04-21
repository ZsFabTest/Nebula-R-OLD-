namespace Nebula.Roles.CrewmateRoles;

public class Bartender : Role{
    public static Color RoleColor = new Color(241f / 255f,158f / 255f,33f / 255f);

    public override void LoadOptionData(){
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
    }

    public Bartender()
        : base("Bartender","bartender",RoleColor,RoleCategory.Crewmate,Side.Crewmate,Side.Crewmate,
        Crewmate.crewmateSideSet,Crewmate.crewmateSideSet,Crewmate.crewmateEndSet,
        false,VentPermission.CanNotUse,false,false,false){
    }
}