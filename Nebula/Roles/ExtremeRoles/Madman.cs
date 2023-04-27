namespace Nebula.Roles.NeutralRoles;

public class Madman : Role
{
    static public Color RoleColor = new Color(191f / 255f, 0f / 255f, 32f / 255f);
    private Module.CustomOption isGuessableOption;
    private Module.CustomOption killCooldownOption;

    private static PlayerControl target;

    public override void LoadOptionData()
    {
        isGuessableOption = CreateOption(Color.white, "isGuessable", false);
        killCooldownOption = CreateOption(Color.white, "killCooldown", 25f, 15f, 60f, 5f);
    }

    public override bool IsGuessableRole { get => isGuessableOption.getBool(); protected set => base.IsGuessableRole = value; }

    public int killingDataId { get; private set; }
    public int madmanDataId { get; private set; }
    public int killedNumber { get; private set; }
    public override RelatedRoleData[] RelatedRoleDataInfo
    {
        get => new RelatedRoleData[] {
            new RelatedRoleData(madmanDataId, "Madman Identifier",0,15),
            new RelatedRoleData(killingDataId, "Madman Kill", 0, 15),
            new RelatedRoleData(killedNumber, "Madman Number of Kiiled",0,15)};
    }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        __instance.GetModData().SetRoleData(madmanDataId, __instance.PlayerId);
        __instance.GetModData().SetRoleData(killingDataId, 0);
        __instance.GetModData().SetRoleData(killedNumber, 0);
    }

    public override void MyPlayerControlUpdate()
    {
        int madmanId = Game.GameData.data.AllPlayers[PlayerControl.LocalPlayer.PlayerId].GetRoleData(madmanDataId);

        Game.MyPlayerData data = Game.GameData.data.myData;

        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(
            (player) =>
            {
                if (player.Object.inVent) return false;
                return true;
            });

        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
    }

    public override void OnMeetingStart()
    {
        if (target != null && !PlayerControl.LocalPlayer.Data.IsDead)
        {
            RPCEventInvoker.SwapExtraRole(PlayerControl.LocalPlayer, target, Roles.SecondaryGuesser, true);
            RPCEventInvoker.SwapExtraRole(PlayerControl.LocalPlayer, target, Roles.SecondaryMadmate, true);
            RPCEventInvoker.SwapRole(PlayerControl.LocalPlayer, target);

            target = null;
        }
    }

    static private CustomButton killButton;

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
                if (r == Helpers.MurderAttemptResult.PerformKill) RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId, killedNumber, 1);
                killButton.Timer = killButton.MaxTimer;
                target = Game.GameData.data.myData.currentTarget;
                Game.GameData.data.myData.currentTarget = null;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => {
                int killing = PlayerControl.LocalPlayer.GetModData().GetRoleData(killedNumber);
                return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove && killing == 0 ; 
            },
            () => { killButton.Timer = killButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.LeftSideContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            "button.label.kill"
        ).SetTimer(killCooldownOption.getFloat());
        killButton.MaxTimer = killCooldownOption.getFloat();
        killButton.SetButtonCoolDownOption(true);
    }

    public override void CleanUp()
    {
        if (killButton != null)
        {
            killButton.Destroy();
            killButton = null;
        }
    }

    public Madman()
        : base("Madman", "madman", RoleColor, RoleCategory.Neutral, Side.ChainShifter, Side.ChainShifter,
             new HashSet<Side>() { Side.ChainShifter }, new HashSet<Side>() { Side.ChainShifter },
             new HashSet<Patches.EndCondition>() { },
             true, VentPermission.CanNotUse, false, false, false)
    {
        killButton = null;
        killingDataId = Game.GameData.RegisterRoleDataId("madman.killing");
    }

}
