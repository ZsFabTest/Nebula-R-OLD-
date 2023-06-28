namespace Nebula.Roles.ImpostorRoles;

public class SerialKiller : Role
{
    public static Module.CustomOption killCooldownOption;
    public static Module.CustomOption suicideMaxTimeOption;

    private static CustomButton killButton;
    public static CustomButton suicideButton;

    private bool hasKilled = false;

    private SpriteLoader SuicideButtonSprite = new SpriteLoader("Nebula.Resources.SuicideButton.png", 115f);

    public override void LoadOptionData()
    {
        killCooldownOption = CreateOption(Color.white, "killCooldown", 20f, 10f, 30f, 2.5f);
        suicideMaxTimeOption = CreateOption(Color.white, "suicideMaxTime", 40f, 20f, 60f, 5f);
    }

    public override void GlobalInitialize(PlayerControl __instance){
        hasKilled = false;
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
                PlayerControl target = Game.GameData.data.myData.currentTarget;
                var res = Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, target, Game.PlayerData.PlayerStatus.Dead, false, true);
                killButton.Timer = killButton.MaxTimer;
                Game.GameData.data.myData.currentTarget = null;
                hasKilled = true;
                suicideButton.Timer = suicideButton.MaxTimer;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && Game.GameData.data.myData.currentTarget != null; },
            () => { killButton.Timer = killButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            "button.label.kill"
        ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
        killButton.MaxTimer = killCooldownOption.getFloat();
        killButton.SetButtonCoolDownOption(true);

        if (suicideButton != null)
        {
            suicideButton.Destroy();
        }
        suicideButton = new CustomButton(
            () => { },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && hasKilled; },
            () => { return true; },
            () => { 
                if(hasKilled) suicideButton.Timer = suicideButton.MaxTimer;
                else suicideButton.Timer = 114514f;
            },
            SuicideButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.noGameInput.keyCode,
            "button.label.suicide"
        ).SetTimer(114514f);
        suicideButton.MaxTimer = suicideMaxTimeOption.getFloat();
        suicideButton.timeInVent = true;
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
        if(suicideButton.Timer <= 0)
        {
            RPCEventInvoker.UncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId, PlayerControl.LocalPlayer.PlayerId, Game.PlayerData.PlayerStatus.Suicide.Id,true);
            suicideButton.Timer = 114514f;
            hasKilled = false;
        }
    }

    public override void CleanUp()
    {
        base.CleanUp();
        if(killButton != null)
        {
            killButton.Destroy();
            killButton = null;
        }
        if(suicideButton != null)
        {
            suicideButton.Destroy();
            suicideButton = null;
        }
        hasKilled = false;
    }

    public SerialKiller()
    : base("SerialKiller", "serialkiller", Palette.ImpostorRed, RoleCategory.Impostor, Side.Impostor, Side.Impostor,
                 Impostor.impostorSideSet, Impostor.impostorSideSet, Impostor.impostorEndSet,
                 true, VentPermission.CanUseUnlimittedVent, true, true, true)
    {
        killButton = null;
        suicideButton = null;
        HideKillButtonEvenImpostor = true;
        hasKilled = false;
    }
}
