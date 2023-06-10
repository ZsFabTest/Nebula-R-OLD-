namespace Nebula.Roles.ComplexRoles;

public class FTransporter : Template.HasBilateralness
{
    static public Color RoleColor = new Color(0f / 255f, 162f / 255f, 232f / 255f);

    public Module.CustomOption teleportCooldownOption;
    public Module.CustomOption specialSetCooldownAfterTeleportOption;
    public Module.CustomOption leastCooldownOption;

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
        base.LoadOptionData();
        teleportCooldownOption = CreateOption(Color.white, "teleportCooldown", 25f, 15f, 45f, 2.5f);
        teleportCooldownOption.suffix = "second";
        specialSetCooldownAfterTeleportOption = CreateOption(Color.white,"specialSetCooldownAfterTeleport",true);
        leastCooldownOption = CreateOption(Color.white,"leastCooldown",7.5f,1.5f,15f,1.5f).AddPrerequisite(specialSetCooldownAfterTeleportOption);
        leastCooldownOption.suffix = "second";

        FirstRole = Roles.NiceTransporter;
        SecondaryRole = Roles.EvilTransporter;
    }

    public FTransporter()
            : base("Transporter","transporter",RoleColor)
    {
    }

    public override List<Role> GetImplicateRoles() { return new List<Role>() { Roles.EvilTransporter, Roles.NiceTransporter }; }


}

public class Transporter : Template.BilateralnessRole{
    private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.ChainShiftButton.png", 115f);
    private SpriteLoader markSprite = new SpriteLoader("Nebula.Resources.AssassinMarkButton.png", 115f);

    public override Assignable AssignableOnHelp => Roles.F_Mover;
    public override HelpSprite[] helpSprite => Roles.F_Mover.helpSprite;

    PlayerControl target;

    private CustomButton teleport;
    public override void ButtonInitialize(HudManager __instance)
    {
        if(teleport != null)
        {
            teleport.Destroy();
        }
        teleport = new CustomButton(
            () =>
            {
                if (target == null)
                {
                    target = Game.GameData.data.myData.currentTarget;
                    teleport.Sprite = buttonSprite.GetSprite();
                    Game.GameData.data.myData.currentTarget.ShowFailedMurder();
                    teleport.SetLabel("button.label.teleport");
                }
                else
                {
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    {
                        player.transform.position = target.transform.position;
                        if(Roles.F_Transporter.specialSetCooldownAfterTeleportOption.getBool()){
                            RPCEventInvoker.AfterTeleportEvent(Roles.F_Transporter.leastCooldownOption.getFloat());
                        }
                    }
                    teleport.Timer = teleport.MaxTimer;
                    teleport.Sprite = markSprite.GetSprite();
                    target = null;
                    teleport.SetLabel("button.label.mark");
                }
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return (Game.GameData.data.myData.currentTarget || target != null) && PlayerControl.LocalPlayer.CanMove; },
            () => { teleport.Timer = teleport.MaxTimer; target = null; },
            markSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.mark"
        ).SetTimer(CustomOptionHolder.InitialAbilityCoolDownOption.getFloat());
        teleport.MaxTimer = Roles.F_Transporter.teleportCooldownOption.getFloat();
    }

    public override void CleanUp()
    {
        if(teleport != null)
        {
            teleport.Destroy();
            teleport = null;
        }
    }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        target = null;
    }

    public override void AfterTeleport(float time)
    {
        if(teleport.Timer < time) teleport.Timer = time;
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color.yellow);
        if(target != null && target.Data.IsDead){
            teleport.Sprite = markSprite.GetSprite();
            target = null;
            teleport.SetLabel("button.label.mark");
        }
    }

    public Transporter(string name, string localizeName, bool isImpostor)
            : base(name, localizeName,
                 isImpostor ? Palette.ImpostorRed : FTransporter.RoleColor,
                 isImpostor ? RoleCategory.Impostor : RoleCategory.Crewmate,
                 isImpostor ? Side.Impostor : Side.Crewmate, isImpostor ? Side.Impostor : Side.Crewmate,
                 isImpostor ? ImpostorRoles.Impostor.impostorSideSet : CrewmateRoles.Crewmate.crewmateSideSet,
                 isImpostor ? ImpostorRoles.Impostor.impostorSideSet : CrewmateRoles.Crewmate.crewmateSideSet,
                 isImpostor ? ImpostorRoles.Impostor.impostorEndSet : CrewmateRoles.Crewmate.crewmateEndSet,
                 false, isImpostor ? VentPermission.CanUseUnlimittedVent : VentPermission.CanNotUse,
                 isImpostor, isImpostor, isImpostor, () => { return Roles.F_Transporter; }, isImpostor)
    {
        target = null;
        teleport = null;
        IsHideRole = true;
    }
}
