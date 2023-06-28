namespace Nebula.Roles.AllSideRoles;

public class GameMaster : Role
{
    public class GMEvent : Events.LocalEvent
    {
        PlayerControl target;
        byte targetId;
        Role targetRole;
        byte targetRoleId;
        public GMEvent(PlayerControl target) : base(0.2f) { this.target = target; }
        public override void OnTerminal()
        {
            RPCEventInvoker.SetExtraRole(target,Roles.Flash,0);
            RPCEventInvoker.SetExtraRole(target,Roles.SecondaryJackal,0);
        }
    }

    public override void Initialize(PlayerControl __instance)
    {
        //RPCEventInvoker.AddExtraRole(PlayerControl.LocalPlayer, Roles.SecondaryGuesser, 0);
        //RPCEventInvoker.AddExtraRole(PlayerControl.LocalPlayer, Roles.Cheater, 0);
    }

    private CustomButton testButton;
    public override void ButtonInitialize(HudManager __instance)
    {
        if(testButton != null) testButton.Destroy();
        testButton = new CustomButton(
        () =>
            {
                PlayerControl target = Game.GameData.data.myData.currentTarget;
                Events.LocalEvent.Activate(new GMEvent(target));
            },
            () => { return true; },
            () => { return true; },
            () => { },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.LeftSideContent,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode
        ).SetTimer(0f);
        testButton.MaxTimer = 0f;
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(1f, true);
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color.yellow);
    }

    public override void CleanUp()
    {
        if(testButton != null)
        {
            testButton.Destroy();
            testButton = null;
        }
    }

    public GameMaster()
        : base("GM", "gm", CrewmateRoles.OnlineCelebrity.RoleColor, RoleCategory.Neutral, Side.ChainShifter, Side.ChainShifter,
             new HashSet<Side>() { Side.ChainShifter }, new HashSet<Side>() { Side.ChainShifter },
             new HashSet<Patches.EndCondition>() { },
             false, VentPermission.CanUseUnlimittedVent, true, true, true)
    {
        IsHideRole = true;
        IsGuessableRole = false;
        testButton = null;
    }
}
