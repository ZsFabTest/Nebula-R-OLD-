namespace Nebula.Roles.ImpostorRoles;

public class Gambler : Role{
    private Module.CustomOption reduceKillCooldownOption;
    private Module.CustomOption addKillCooldownOption;

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
        reduceKillCooldownOption = CreateOption(Color.white,"reduceKillCooldown",15f,5f,25f,0.5f);
        reduceKillCooldownOption.suffix = "second";
        addKillCooldownOption = CreateOption(Color.white,"addKillCooldown",15f,5f,25f,0.5f);
        addKillCooldownOption.suffix = "second";
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
                int result = NebulaPlugin.rnd.Next(1,11);
                if(result <= 5) killButton.Timer = killButton.MaxTimer - reduceKillCooldownOption.getFloat();
                else killButton.Timer = killButton.MaxTimer + addKillCooldownOption.getFloat();
                if(killButton.Timer < 0) killButton.Timer = 0;
                Game.GameData.data.myData.currentTarget = null;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { killButton.Timer = killButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            "button.label.kill"
        ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
        killButton.MaxTimer = GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown);
        killButton.SetButtonCoolDownOption(true);
    }

    public override void EditCoolDown(CoolDownType type, float count)
    {
        killButton.Timer -= count;
        killButton.actionButton.ShowButtonText("+" + count + "s");
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
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(true);
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
    }

    public Gambler()
         : base("Gambler","gambler",Palette.ImpostorRed,RoleCategory.Impostor,Side.Impostor,Side.Impostor,
                Impostor.impostorSideSet,Impostor.impostorSideSet,Impostor.impostorEndSet,
                true,VentPermission.CanUseUnlimittedVent,true,true,true){
        HideKillButtonEvenImpostor = true;
        killButton = null;
    }
}