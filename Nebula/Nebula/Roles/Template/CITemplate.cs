namespace Nebula.Roles.Template;

public class TCrewmate : Role{
    public bool hasRoleUpdate { get; protected set; }

    public override void MyPlayerControlUpdate()
    {
        if(!hasRoleUpdate) return;
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
    }

    public TCrewmate(string name,string localizeName,Color RoleColor,bool canUsev)
        : base(name,localizeName,RoleColor,RoleCategory.Crewmate,Side.Crewmate,Side.Crewmate,
        CrewmateRoles.Crewmate.crewmateSideSet,CrewmateRoles.Crewmate.crewmateSideSet,CrewmateRoles.Crewmate.crewmateEndSet,
        false,canUsev ? VentPermission.CanUseUnlimittedVent : VentPermission.CanNotUse,canUsev,false,false){
        this.hasRoleUpdate = false;
    }
}

public class TImpostor : Role{

    public bool hasRoleUpdate { get; protected set; }

    public override void MyPlayerControlUpdate()
    {
        if(!hasRoleUpdate) return;
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
    }

    public TImpostor(string name,string localizeName,bool canUsev)
        : base(name,localizeName,Palette.ImpostorRed,RoleCategory.Impostor,Side.Impostor,Side.Impostor,
        ImpostorRoles.Impostor.impostorSideSet,ImpostorRoles.Impostor.impostorSideSet,ImpostorRoles.Impostor.impostorEndSet,
        true,canUsev ? VentPermission.CanUseUnlimittedVent : VentPermission.CanNotUse,canUsev,false,false){
        this.hasRoleUpdate = false;
    }
}