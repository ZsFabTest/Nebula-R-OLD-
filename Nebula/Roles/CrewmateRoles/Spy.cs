namespace Nebula.Roles.CrewmateRoles;

public class Spy : Role
{
    public bool validSpyFlag;

    private Module.CustomOption impostorCanKillImpostorOption;
    private Module.CustomOption ventCoolDownOption;
    private Module.CustomOption ventDurationOption;
    private Module.CustomOption canUseKillButtomOption;
    private Module.CustomOption killCoolDownOption;
    private Module.CustomOption numberOfShotsOption;

    private SpriteLoader killButtonSprite = new SpriteLoader("Nebula.Resources.SheriffKillButton.png", 100f);

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color.yellow);
    }

    public bool CanKillImpostor()
    {
        return impostorCanKillImpostorOption.getBool() && validSpyFlag;
    }

    public override void LoadOptionData()
    {
        impostorCanKillImpostorOption = CreateOption(Color.white, "impostorCanKillImpostor", true);
        ventCoolDownOption = CreateOption(Color.white, "ventCoolDown", 20f, 5f, 60f, 2.5f);
        ventCoolDownOption.suffix = "second";
        ventDurationOption = CreateOption(Color.white, "ventDuration", 10f, 5f, 60f, 2.5f);
        ventDurationOption.suffix = "second";
        canUseKillButtomOption = CreateOption(Color.white, "canUseKillButton", true);
        killCoolDownOption = CreateOption(Color.white, "killCoolDown", 25f, 15f, 60f, 5f).AddPrerequisite(canUseKillButtomOption);
        killCoolDownOption.suffix = "second";
        numberOfShotsOption = CreateOption(Color.white, "numberOfShots", 1f, 1f, 3f, 1f).AddPrerequisite(canUseKillButtomOption);
    }

    public override void Initialize(PlayerControl __instance)
    {
        VentCoolDownMaxTimer = ventCoolDownOption.getFloat();
        VentDurationMaxTimer = ventDurationOption.getFloat();
    }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        validSpyFlag = true;
    }

    public override void StaticInitialize()
    {
        validSpyFlag = false;
    }

    public override void OnRoleRelationSetting()
    {
        RelatedRoles.Add(Roles.Seer);
        RelatedRoles.Add(Roles.Empiric);
        RelatedRoles.Add(Roles.Bait);
        RelatedRoles.Add(Roles.Provocateur);
    }

    static private CustomButton killButton;
    public override void ButtonInitialize(HudManager __instance)
    {
        int shots = (int)numberOfShotsOption.getFloat();

        if (killButton != null)
        {
            killButton.Destroy();
        }
        killButton = new CustomButton(
            () =>
            {
                PlayerControl target = Game.GameData.data.myData.currentTarget;

                var res = Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, target, (target == PlayerControl.LocalPlayer) ? Game.PlayerData.PlayerStatus.Misfire : Game.PlayerData.PlayerStatus.Dead, false, true);
                if (res != Helpers.MurderAttemptResult.SuppressKill)
                    killButton.Timer = killButton.MaxTimer;
                Game.GameData.data.myData.currentTarget = null;

                shots--;
                killButton.UsesText.text = shots.ToString();
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && shots > 0; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { killButton.Timer = killButton.MaxTimer; },
            killButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.LeftSideContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            "button.label.kill", ImageNames.AdminMapButton
        ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
        killButton.MaxTimer = killCoolDownOption.getFloat();
        killButton.UsesText.text = shots.ToString();
        killButton.LabelText.outlineColor = Palette.CrewmateBlue;
    }
    public override void CleanUp()
    {
        if (killButton != null)
        {
            killButton.Destroy();
            killButton = null;
        }
    }

    public override bool IsUnsuitable { get { return GameOptionsManager.Instance.CurrentGameOptions.NumImpostors <= 1 || PlayerControl.AllPlayerControls.Count < 7; } }

    public Spy()
            : base("Spy", "spy", Palette.ImpostorRed, RoleCategory.Crewmate, Side.Crewmate, Side.Crewmate,
                 Crewmate.crewmateSideSet, ImpostorRoles.Impostor.impostorSideSet, Crewmate.crewmateEndSet,
                 true, VentPermission.CanUseLimittedVent, false, false, true)
    {
        DeceiveImpostorInNameDisplay = true;
        killButton = null;
    }
}
