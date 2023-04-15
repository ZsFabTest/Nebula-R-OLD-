namespace Nebula.Roles.NeutralRoles;

public class Pavlov : Role
{
    public class PavlovEvent : Events.LocalEvent
    {
        PlayerControl target;
        public PavlovEvent(PlayerControl target) : base(0.1f) { this.target = target; }
        public override void OnActivate()
        {
            RPCEventInvoker.ImmediatelyChangeRole(target,Roles.Dog);
        }
    }

    static public Color RoleColor = new Color(236f / 255f, 182f / 255f, 91f / 255f);

    static public Module.CustomOption createDogsCooldownOption;
    static public Module.CustomOption dogKillCooldownOption;

    private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.AppointButton.png", 115f);

    public bool hasDog;
    public static byte myDog { get; private set; }
    public static int leftDogDataId { get; private set; }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        RPCEventInvoker.UpdateRoleData(__instance.PlayerId, myDog, -1);
        RPCEventInvoker.UpdateRoleData(__instance.PlayerId, leftDogDataId, 3);
        hasDog = false;
    }

    public override void GlobalIntroInitialize(PlayerControl __instance) => GlobalInitialize(__instance);

    private CustomButton feed;
    public override void ButtonInitialize(HudManager __instance)
    {
        if (feed != null)
        {
            feed.Destroy();
        }
        feed = new CustomButton(
        () =>
            {
                PlayerControl target = Game.GameData.data.myData.currentTarget;
                Events.LocalEvent.Activate(new PavlovEvent(target));
                RPCEventInvoker.UpdateRoleData(PlayerControl.LocalPlayer.PlayerId, myDog, target.PlayerId);
                RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId, leftDogDataId, -1);
                hasDog = true;
                feed.Timer = feed.MaxTimer;
                feed.UsesText.text = Game.GameData.data.myData.getGlobalData().GetRoleData(leftDogDataId).ToString();
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove && !hasDog && Game.GameData.data.myData.getGlobalData().GetRoleData(leftDogDataId) > 0; },
            () => { feed.Timer = feed.MaxTimer; },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.LeftSideContent,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.feed"
        ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
        feed.MaxTimer = createDogsCooldownOption.getFloat();
        feed.UsesText.text = Game.GameData.data.myData.getGlobalData().GetRoleData(leftDogDataId).ToString();
        feed.LabelText.outlineColor = RoleColor;
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, RoleColor);
        if (hasDog)
        {
            PlayerControl Dog = Helpers.playerById((byte)Game.GameData.data.myData.getGlobalData().GetRoleData(myDog));
            if(Dog.Data.IsDead) hasDog = false;
            feed.Timer = feed.MaxTimer;
        }
    }

    public override void CleanUp()
    {
        hasDog = false;
        if(feed != null)
        {
            feed.Destroy();
            feed = null;
        }
    }

    public override void LoadOptionData()
    {
        createDogsCooldownOption = CreateOption(Color.white, "createDogsCooldown", 30f, 15f, 35f, 5f);
        createDogsCooldownOption.suffix = "second";
        dogKillCooldownOption = CreateOption(Color.white, "dogKillCooldown", 25f, 5f, 60f, 5f);
        dogKillCooldownOption.suffix = "second";
    }

    public override void EditDisplayNameColor(byte playerId, ref Color displayColor)
    {
        if (PlayerControl.LocalPlayer.GetModData().role.side == Side.Pavlov)
        {
                displayColor = RoleColor;
        }
    }

    public Pavlov()
        : base("Pavlov", "pavlov", RoleColor, RoleCategory.Neutral, Side.Pavlov, Side.Pavlov,
             new HashSet<Side>() { Side.Pavlov }, new HashSet<Side>() { Side.Pavlov },
             new HashSet<Patches.EndCondition>() { Patches.EndCondition.PavlovWin },
             true, VentPermission.CanNotUse, false, false, false)
    {
        feed = null;
    }
}

public class Dog : Role
{
    public override bool IsSpawnable()
    {
        return false;
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
                Game.GameData.data.myData.currentTarget = null;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { killButton.Timer = killButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode
        ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
        killButton.MaxTimer = Pavlov.dogKillCooldownOption.getFloat();
        killButton.SetButtonCoolDownOption(true);
    }

    public override void EditDisplayNameColor(byte playerId, ref Color displayColor)
    {
        if (PlayerControl.LocalPlayer.GetModData().role.side == Side.Pavlov)
        {
            displayColor = Pavlov.RoleColor;
        }
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(
            (player) =>
            {
                if (player.Object.inVent) return false;
                if (player.GetModData().role == Roles.Pavlov || player.GetModData().role == Roles.Dog)
                {
                    return false;
                }
                return true;
            });

        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color);
    }

    public override void CleanUp()
    {
        if(killButton != null)
        {
            killButton.Destroy();
            killButton = null;
        }
    }

    public Dog()
        : base("Dog", "dog", Pavlov.RoleColor, RoleCategory.Neutral, Side.Pavlov, Side.Pavlov,
             new HashSet<Side>() { Side.Pavlov }, new HashSet<Side>() { Side.Pavlov },
             new HashSet<Patches.EndCondition>() { Patches.EndCondition.PavlovWin },
             true, VentPermission.CanNotUse, true, true, true)
    {
        IsHideRole = true;
    }
}