namespace Nebula.Roles.ImpostorRoles;

public class Marksman : Role{
    private Module.CustomOption maxShootOption;

    private CustomButton saveShootButton,getShootButton,killButton;
    private SpriteLoader saveShootButtonSprite = new SpriteLoader("Nebula.Resources.SaveShootButton.png", 115f, "ui.button.marksman.saveShoot");
    private SpriteLoader getShootButtonSprite = new SpriteLoader("Nebula.Resources.GetShootButton.png", 115f, "ui.button.marksman.getShoot");

    public int shootId { get; private set; }

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
        maxShootOption = CreateOption(Color.white,"maxShoot",3f,1f,10f,1f);
    }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        __instance.GetModData().SetRoleData(shootId,0);
    }

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

        if (saveShootButton != null)
        {
            saveShootButton.Destroy();
        }
        saveShootButton = new CustomButton(
            () =>
            {
                RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId,shootId,1);
                killButton.Timer = killButton.MaxTimer;
                getShootButton.UsesText.text = PlayerControl.LocalPlayer.GetModData().GetRoleData(shootId).ToString();
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return killButton.Timer <= 0 && PlayerControl.LocalPlayer.GetModData().GetRoleData(shootId) < maxShootOption.getFloat() && PlayerControl.LocalPlayer.CanMove; },
            () => { saveShootButton.Timer = 0; },
            saveShootButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.saveShoot"
        );
        saveShootButton.Timer = saveShootButton.MaxTimer = 0;

        if (getShootButton != null)
        {
            getShootButton.Destroy();
        }
        getShootButton = new CustomButton(
            () =>
            {
                RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId,shootId,-1);
                killButton.Timer = 0;
                getShootButton.UsesText.text = PlayerControl.LocalPlayer.GetModData().GetRoleData(shootId).ToString();
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.GetModData().GetRoleData(shootId) > 0 && killButton.Timer > 0 && PlayerControl.LocalPlayer.CanMove; },
            () => { getShootButton.Timer = 0; },
            getShootButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.secondaryAbilityInput.keyCode,
            "button.label.getShoot"
        );
        getShootButton.Timer = getShootButton.MaxTimer = 0;

        getShootButton.UsesText.text = "0";
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
        if(saveShootButton != null){
            saveShootButton.Destroy();
            saveShootButton = null;
        }
        if(getShootButton != null){
            getShootButton.Destroy();
            getShootButton = null;
        }
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(true);
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
    }

    public Marksman()
        : base("Marksman","marksman",Palette.ImpostorRed,RoleCategory.Impostor,Side.Impostor,Side.Impostor,
        Impostor.impostorSideSet,Impostor.impostorSideSet,Impostor.impostorEndSet,
        true,VentPermission.CanUseUnlimittedVent,true,true,true){
        saveShootButton = null;
        getShootButton = null;
        killButton = null;
        HideKillButtonEvenImpostor = true;
    }
}