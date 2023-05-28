namespace Nebula.Roles.ImpostorRoles;
using Nebula.Events;

public class Moda : Role{
    public class ModaEvent : LocalEvent{
        public ModaEvent() : base(0.1f) {}
        public override void OnActivate()
        {
            if(Helpers.AllDeadBodies().Count() == 1) return;
            RPCEventInvoker.CleanDeadBody(Helpers.AllDeadBodies()[0].ParentId);
        }
    }

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
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
                killButton.Timer = killButton.MaxTimer;
                if(r != Helpers.MurderAttemptResult.PerformKill) return;
                Game.GameData.data.myData.currentTarget = null;
                LocalEvent.Activate(new ModaEvent());
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
        killButton.MaxTimer = GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown);;
        killButton.SetButtonCoolDownOption(true);
    }

    public override void EditCoolDown(CoolDownType type, float count)
    {
        killButton.Timer -= count;
        killButton.actionButton.ShowButtonText("+" + count + "s");
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(true);
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
    }

    public override void CleanUp(){
        if(killButton != null){
            killButton.Destroy();
            killButton = null;
        }
    }

    public Moda()
        : base("Moda","moda",Palette.ImpostorRed,RoleCategory.Impostor,Side.Impostor,Side.Impostor,
        Impostor.impostorSideSet,Impostor.impostorSideSet,Impostor.impostorEndSet,
        true,VentPermission.CanUseUnlimittedVent,true,true,true){
        HideKillButtonEvenImpostor = true;
    }
}