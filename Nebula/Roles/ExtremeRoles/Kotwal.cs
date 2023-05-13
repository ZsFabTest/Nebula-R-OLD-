namespace Nebula.Roles.CrewmateRoles;

public class Kotwal : Role
{
    public class KotwalEvent : Events.LocalEvent
    {
        PlayerControl target;
        public KotwalEvent(PlayerControl target) : base(0.1f) { this.target = target; }
        public override void OnActivate()
        {
            if(target.GetModData().role.category == RoleCategory.Impostor)
            {
                PlayerControl player = PlayerControl.LocalPlayer;
                RPCEventInvoker.UncheckedMurderPlayer(player.PlayerId, player.PlayerId, Game.PlayerData.PlayerStatus.Suicide.Id, true);
            }else{
                RPCEventInvoker.ImmediatelyChangeRole(target, Roles.Sheriff);
                if(PlayerControl.LocalPlayer.IsMadmate() && !target.IsMadmate()){
                    RPCEventInvoker.SetExtraRole(target,Roles.SecondaryMadmate,0);
                }
                if((PlayerControl.LocalPlayer.GetModData().extraRole.Contains(Roles.SecondaryJackal) || PlayerControl.LocalPlayer.GetModData().extraRole.Contains(Roles.SecondarySidekick))
                && !(target.GetModData().extraRole.Contains(Roles.SecondarySidekick) || target.GetModData().extraRole.Contains(Roles.SecondaryJackal))){
                    RPCEventInvoker.SetExtraRole(target,Roles.SecondaryJackal,0);
                }
            }
        }
    }

    static public Color RoleColor = Sheriff.RoleColor;

    static public Module.CustomOption createSheriffCooldownOption;

    private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.AppointButton.png", 115f);

    public static int appointDataId { get; private set; }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color.yellow);
    }

    // just for update

    public override void GlobalInitialize(PlayerControl __instance)
    {
        RPCEventInvoker.UpdateRoleData(PlayerControl.LocalPlayer.PlayerId, appointDataId, 1);
    }

    public override void LoadOptionData()
    {
        createSheriffCooldownOption = CreateOption(Color.white, "createSheriffCooldown", 30f, 15f, 45f, 2.5f);
        createSheriffCooldownOption.suffix = "second";
    }

    private CustomButton appoint;
    public override void ButtonInitialize(HudManager __instance)
    {
        if(appoint != null)
        {
            appoint.Destroy();
        }
        appoint = new CustomButton(
            () =>
            {
                PlayerControl target = Game.GameData.data.myData.currentTarget;
                Events.LocalEvent.Activate(new KotwalEvent(target));
                RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId, appointDataId, -1);
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && Game.GameData.data.myData.getGlobalData().GetRoleData(appointDataId) > 0; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { appoint.Timer = appoint.MaxTimer; },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.appoint"
        ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
        appoint.MaxTimer = createSheriffCooldownOption.getFloat();
    }

    public override void CleanUp()
    {
        if(appoint != null)
        {
            appoint.Destroy();
            appoint = null;
        }
    }

    public Kotwal()
        : base("Kotwal", "kotwal", RoleColor, RoleCategory.Crewmate, Side.Crewmate, Side.Crewmate,
             Crewmate.crewmateSideSet, Crewmate.crewmateSideSet, Crewmate.crewmateEndSet,
             false, VentPermission.CanNotUse, false, false, false)
    {
        appoint = null;
    }
}
