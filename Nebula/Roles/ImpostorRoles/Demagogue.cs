namespace Nebula.Roles.ImpostorRoles;

public class Demagogue : Role
{
    public class DemagogueEvent : Events.LocalEvent
    {
        PlayerControl target;
        Role targetRole;
        public DemagogueEvent(PlayerControl target,Role targetRole) : base(0.2f) { this.targetRole = targetRole; this.target = target; }
        public override void OnTerminal()
        {
            RPCEventInvoker.ImmediatelyUnsetExtraRole(target, Roles.SecondaryMadmate);
            RPCEventInvoker.ImmediatelyChangeRole(target, targetRole);
        }
    }

    public static Module.CustomOption demagogueMaxCreateCountOption;

    private SpriteLoader madmateButtonSprite = new SpriteLoader("Nebula.Resources.MadmateButton.png", 115f);

    public override void LoadOptionData()
    {
        demagogueMaxCreateCountOption = CreateOption(Color.white, "demagogueMaxCreateCount", 1f, 1f, 3f, 1f);
    }

    private CustomButton CreateImpostor;
    public int leftImpostorDataId { get; private set; }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        __instance.GetModData().SetRoleData(leftImpostorDataId, (int)demagogueMaxCreateCountOption.getFloat());
    }

    public override void ButtonInitialize(HudManager __instance)
    {
        if (CreateImpostor != null)
        {
            CreateImpostor.Destroy();
        }
        CreateImpostor = new CustomButton(
            () =>
            {
                PlayerControl target = Game.GameData.data.myData.currentTarget;
                //Madmate生成
                Role targetRole = getRole(target.GetModData().role);
                RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId, leftImpostorDataId, -1);

                Events.LocalEvent.Activate(new DemagogueEvent(target, targetRole));

                Game.GameData.data.myData.currentTarget = null;
                CreateImpostor.Timer = CreateImpostor.MaxTimer;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && Game.GameData.data.myData.getGlobalData().GetRoleData(leftImpostorDataId) > 0; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { CreateImpostor.Timer = CreateImpostor.MaxTimer; },
            madmateButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.LeftSideContent,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.madmate"
        );
        CreateImpostor.MaxTimer = 25f;
    }

    public override void CleanUp()
    {
        base.CleanUp();
        if(CreateImpostor != null){
            CreateImpostor.Destroy();
            CreateImpostor = null;
        }
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(1f, true);
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color.yellow);
    }

    private Role getRole(Role target){
        Role targetRole = Roles.Impostor;
        if (target == Roles.Mayor) targetRole = Roles.EvilAce;
        else if (target == Roles.Necromancer) targetRole = Roles.Reaper;
        else if (target == Roles.NiceGuesser) targetRole = Roles.EvilGuesser;
        else if (target == Roles.NiceTracker) targetRole = Roles.EvilTracker;
        else if (target == Roles.NiceTrapper) targetRole = Roles.EvilTrapper;
        else if (target == Roles.Guardian) targetRole = Roles.Jailer;
        else if (target == Roles.Observer) targetRole = Roles.Assassin;
        else if (target == Roles.Splicer) targetRole = Roles.Executioner;
        else if (target == Roles.Spy) targetRole = Roles.Spy;
        else if (target == Roles.Sheriff) targetRole = Roles.SerialKiller; //记得写好嗜血以后替换awa
        else if (target == Roles.OnlineCelebrity) targetRole = Roles.Morphing;
        else if (target == Roles.Holmes) targetRole = Roles.Painter;
        else if (target == Roles.Sanctifier) targetRole = Roles.Demagogue;
        else if (target == Roles.Doctor) targetRole = Roles.Cleaner;
        else if (target == Roles.Provocateur) targetRole = Roles.Vampire;
        else if (target == Roles.WhiteCat || target == Roles.BlueCat) targetRole = Roles.RedCat;
        else if (target == Roles.Observer) targetRole = Roles.Eraser;
        else if (target == Roles.OnlineCelebrity) targetRole = Roles.Camouflager;
        else if (target.side == Side.Impostor) targetRole = target;
        return targetRole.IsSpawnable() ? targetRole : Roles.Impostor;
    }

    public Demagogue()
            : base("Demagogue", "demagogue", Palette.ImpostorRed, RoleCategory.Impostor, Side.Impostor, Side.Impostor,
                 Impostor.impostorSideSet, Impostor.impostorSideSet, Impostor.impostorEndSet,
                 true, VentPermission.CanNotUse, true, true, true)
    {
        HideKillButtonEvenImpostor = true;
        CreateImpostor = null;

        leftImpostorDataId = Game.GameData.RegisterRoleDataId("demagogue.leftImpostor");
    }
}
