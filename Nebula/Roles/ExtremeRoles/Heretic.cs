namespace Nebula.Roles.ImpostorRoles;

public class Heretic : Role{
    public class HereticEvent : Events.LocalEvent{
        PlayerControl target;
        bool mode = false;
        public HereticEvent(PlayerControl target,bool mode = false) : base(0.1f) { this.target = target; this.mode = mode; }
        public override void OnActivate()
        {
            if(!mode) RPCEventInvoker.ImmediatelyChangeRole(target,Roles.Madmate);
            else{
                if(target.GetModData().role.side == Side.Crewmate) RPCEventInvoker.AddExtraRole(target,Roles.SecondaryMadmate,0);
                else
                {
                    target.ShowFailedMurder();
                    return;
                }
            }
            PlayerControl.LocalPlayer.GetModData().AddRoleData(Heretic.leftMadmateDataId, -1);
        }
    }

    private Module.CustomOption maxPreachCountOption;
    private Module.CustomOption preachCooldownOption;
    private Module.CustomOption secondaryMadmateOption;

    private SpriteLoader ButtonSprite = new SpriteLoader("Nebula.Resources.MadmateButton.png", 115f,"ui.button.heretic.preach");

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
        maxPreachCountOption = CreateOption(Color.white,"maxPreachCount",1f,1f,3f,1f);
        preachCooldownOption = CreateOption(Color.white,"preachCooldown",20f,10f,30f,5f);
        preachCooldownOption.suffix = "second";
        secondaryMadmateOption = CreateOption(Color.white,"secondaryMadmate",true);
    }

    public static int leftMadmateDataId { get; set; } = 0;

    public override void GlobalInitialize(PlayerControl __instance)
    {
        __instance.GetModData().SetRoleData(leftMadmateDataId,(int)maxPreachCountOption.getFloat());
    }

    private CustomButton preach;
    public override void ButtonInitialize(HudManager __instance)
    {
        if(preach != null){
            preach.Destroy();
        }
        preach = new CustomButton(
            () =>
            {
                Events.LocalEvent.Activate(new HereticEvent(Game.GameData.data.myData.currentTarget,secondaryMadmateOption.getBool()));
                Game.GameData.data.myData.currentTarget = null;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && Game.GameData.data.myData.getGlobalData().GetRoleData(leftMadmateDataId) > 0; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { preach.Timer = preach.MaxTimer; },
            ButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.LeftSideContent,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.preach"
        ).SetTimer(CustomOptionHolder.InitialAbilityCoolDownOption.getFloat());
        preach.MaxTimer = preachCooldownOption.getFloat();
    }

    public override void EditOthersDisplayNameColor(byte playerId, ref Color displayColor)
    {
        PlayerControl target = Helpers.playerById(playerId);
        if (target.GetModData().role == Roles.Madmate) displayColor = Palette.ImpostorRed;
        else if(target.GetModData().extraRole.Contains(Roles.SecondaryMadmate)) displayColor = Palette.ImpostorRed;
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;

        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(
            (player) =>
            {
                if (player.Object.inVent) return false;
                if (player.GetModData().role.side == Side.Impostor)
                {
                    return false;
                }
                return true;
            });

        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
    }

    public override void CleanUp()
    {
        base.CleanUp();
        if(preach != null){
            preach.Destroy();
            preach = null;
        }
    }

    public Heretic()
        : base("Heretic","heretic",Palette.ImpostorRed,RoleCategory.Impostor,Side.Impostor,Side.Impostor,
        Impostor.impostorSideSet,Impostor.impostorSideSet,Impostor.impostorEndSet,
        true,VentPermission.CanUseUnlimittedVent,true,true,true){
        preach = null;
    }
}