using static UnityEngine.GraphicsBuffer;

namespace Nebula.Roles.ImpostorRoles;

public class SerialKiller : Role
{
    public static Module.CustomOption killCooldownOption;
    public static Module.CustomOption suicideMaxTimeOption;

    private static CustomButton killButton;
    private static CustomButton suicideButton;

    private double lasttime;
    private bool firstKill;

    private SpriteLoader SuicideButtonSprite = new SpriteLoader("Nebula.Resources.SuicideButton.png", 115f);

    private System.Timers.Timer Mytimer;

    public override void LoadOptionData()
    {
        killCooldownOption = CreateOption(Color.white, "killCooldown", 20f, 10f, 30f, 2.5f);
        suicideMaxTimeOption = CreateOption(Color.white, "suicideMaxTime", 40f, 20f, 60f, 5f);
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
                if (res != Helpers.MurderAttemptResult.SuppressKill)
                    killButton.Timer = killButton.MaxTimer;
                Game.GameData.data.myData.currentTarget = null;
                lasttime = suicideMaxTimeOption.getFloat();
                firstKill = true;
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

        firstKill = false;
        lasttime = 114514f;
        int interval = 1000;
        Mytimer = new System.Timers.Timer(interval);
        Mytimer.AutoReset = true;
        Mytimer.Elapsed += new System.Timers.ElapsedEventHandler(TimeCheck);
        Mytimer.Start();
        if (suicideButton != null)
        {
            suicideButton.Destroy();
        }
        suicideButton = new CustomButton(
            () => { Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, Game.PlayerData.PlayerStatus.Suicide, false, true); },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return true; },
            () => { suicideButton.Timer = suicideButton.MaxTimer; },
            SuicideButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.suicide"
        ).SetTimer(114514f);
        suicideButton.MaxTimer = suicideMaxTimeOption.getFloat();
        suicideButton.isEffectActive = true;
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(true);
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
        if(suicideButton.Timer <= 0)
        {
            Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, Game.PlayerData.PlayerStatus.Suicide, false, true);
            suicideButton.Timer = 114514f;
        }
    }

    private void TimeCheck(object sender, System.Timers.ElapsedEventArgs e)
    {
        lasttime -= 1f;
        suicideButton.Timer = lasttime >= 0f ? (float)lasttime : 1f;
        if (lasttime <= 0 && PlayerControl.LocalPlayer.CanMove && firstKill && !PlayerControl.LocalPlayer.Data.IsDead)
        {
            Mytimer.Stop();
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
        Mytimer.Stop();
    }

    public SerialKiller()
    : base("SerialKiller", "serialkiller", Palette.ImpostorRed, RoleCategory.Impostor, Side.Impostor, Side.Impostor,
                 Impostor.impostorSideSet, Impostor.impostorSideSet, Impostor.impostorEndSet,
                 true, VentPermission.CanUseUnlimittedVent, true, true, true)
    {
        killButton = null;
        suicideButton = null;
        HideKillButtonEvenImpostor = true;
    }
}
