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
    static public Module.CustomOption dogCanUseVentOption;

    private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.AppointButton.png", 115f);

    public bool hasDog;
    public static byte leftDogDataId;
    public static byte myDog;

    public override void GlobalInitialize(PlayerControl __instance)
    {
        myDog = 255;
        leftDogDataId = 3;
        hasDog = false;
    }

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
                myDog = target.PlayerId;
                leftDogDataId--;
                hasDog = true;
                feed.Timer = feed.MaxTimer;
                feed.UsesText.text = leftDogDataId.ToString();
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && !hasDog && leftDogDataId > 0; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { feed.Timer = feed.MaxTimer; },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.LeftSideContent,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.feed"
        ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
        feed.MaxTimer = createDogsCooldownOption.getFloat();
        feed.UsesText.text = "3";
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(
            (player) => {
                if (player.Object.inVent) return false;
                if(player.GetModData().role.side == Side.Pavlov) return false;
                return true;
            }
        );
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color.yellow);
        if (hasDog && myDog != 255)
        {
            PlayerControl Dog = Helpers.playerById(myDog);
            if(Dog.Data.IsDead || Dog.GetModData().role != Roles.Dog) hasDog = false;
            feed.Timer = feed.MaxTimer;
        }
    }

    public override void CleanUp()
    {
        hasDog = false;
        myDog = 255;
        if(feed != null)
        {
            feed.Destroy();
            feed = null;
        }
    }

    public override void LoadOptionData()
    {
<<<<<<< HEAD
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
=======
>>>>>>> newbranch
        createDogsCooldownOption = CreateOption(Color.white, "createDogsCooldown", 30f, 15f, 35f, 5f);
        createDogsCooldownOption.suffix = "second";
        dogKillCooldownOption = CreateOption(Color.white, "dogKillCooldown", 25f, 5f, 60f, 5f);
        dogKillCooldownOption.suffix = "second";
        dogCanUseVentOption = CreateOption(Color.white,"dogCanUseVent",true);
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
        myDog = 255;
        leftDogDataId = 3;
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
        VentPermission = Pavlov.dogCanUseVentOption.getBool() ? VentPermission.CanUseUnlimittedVent : VentPermission.CanNotUse;
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
                if (player.GetModData().role.side == Side.Pavlov)
                {
                    return false;
                }
                return true;
            });

        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
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
             true, VentPermission.CanUseUnlimittedVent, true, true, true)
    {
        IsHideRole = true;
    }
}