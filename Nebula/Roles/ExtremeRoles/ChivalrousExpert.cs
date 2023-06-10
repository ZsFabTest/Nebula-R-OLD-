namespace Nebula.Roles.CrewmateRoles;

public class ChivalrousExpert : Role{
    public static Color RoleColor = new Color(230f / 255f,213f / 255f,130f / 255f);

    public int ceId { get; private set; }

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
    }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        __instance.GetModData().SetRoleData(ceId,0);
    }

    private CustomButton killButton;
    public override void ButtonInitialize(HudManager __instance)
    {
        if (killButton != null)
        {
            killButton.Destroy();
        }
        killButton = new CustomButton(
            () =>
            {
                var r = Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, Game.GameData.data.myData.currentTarget, Game.PlayerData.PlayerStatus.Dead, true);
                if(r == Helpers.MurderAttemptResult.PerformKill) RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId,ceId,1);
                killButton.Timer = killButton.MaxTimer;
                Game.GameData.data.myData.currentTarget = null;
                killButton.UsesText.text = "0";
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.GetModData().GetRoleData(ceId) <= 0; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { killButton.Timer = killButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            "button.label.kill"
        );
        killButton.MaxTimer = killButton.Timer = 0;

        killButton.UsesText.text = "1";
    }

    public override void CleanUp(){
        if(killButton != null){
            killButton.Destroy();
            killButton = null;
        }
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, RoleColor);
    }

    public ChivalrousExpert()
         : base("ChivalrousExpert","chivalrousExpert",RoleColor,RoleCategory.Crewmate,Side.Crewmate,Side.Crewmate,
                Crewmate.crewmateSideSet,Crewmate.crewmateSideSet,Crewmate.crewmateEndSet,
                false,VentPermission.CanNotUse,false,false,false){
        killButton = null;
    }
}