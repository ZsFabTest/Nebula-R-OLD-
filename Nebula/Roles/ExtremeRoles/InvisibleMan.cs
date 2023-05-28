namespace Nebula.Roles.ImpostorRoles;

public class InvisibleMan : Role{
    private Module.CustomOption InvisibleCooldown;
    private Module.CustomOption InvisibleDuringTime;
    private Module.CustomOption canUseVentOption;
    //ֻ��Ϊ���ع�awa
    private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.SpectreButton.png", 115f, "ui.button.invisibleMan.invisible");

    public override void LoadOptionData()
    {
        InvisibleCooldown = CreateOption(Color.white,"invisibleCooldown",30f,15f,45f,2.5f);
        InvisibleCooldown.suffix = "second";
        InvisibleDuringTime = CreateOption(Color.white,"invisibleDuringTime",7.5f,2.5f,15f,2.5f);
        InvisibleDuringTime.suffix = "second";
        canUseVentOption = CreateOption(Color.white,"canUseVent",false);
    }

    private CustomButton invisible;
    public override void ButtonInitialize(HudManager __instance)
    {
        if (invisible != null)
        {
            invisible.Destroy();
        }
        invisible = new CustomButton(
            () =>
            {
                RPCEventInvoker.EmitAttributeFactor(PlayerControl.LocalPlayer, new Game.PlayerAttributeFactor(Game.PlayerAttribute.Invisible, InvisibleDuringTime.getFloat(), 0, false));
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove;
            },
            () => { invisible.Timer = invisible.MaxTimer; },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode, true,
            InvisibleDuringTime.getFloat(),
           () =>
           {
               invisible.Timer = invisible.MaxTimer;
               RPCEventInvoker.UpdatePlayerVisibility(PlayerControl.LocalPlayer.PlayerId, true);
           },
            "button.label.clarify"
        ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
        invisible.MaxTimer = InvisibleCooldown.getFloat();
    }

    public override void CleanUp()
    {
        if(invisible != null){
            invisible.Destroy();
            invisible = null;
        }
    }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        VentPermission = canUseVentOption.getBool() ? VentPermission.CanUseUnlimittedVent : VentPermission.CanNotUse;
    }

    public InvisibleMan()
        : base("InvisibleMan","invisibleMan",Palette.ImpostorRed,RoleCategory.Impostor,Side.Impostor,Side.Impostor,
        Impostor.impostorSideSet,Impostor.impostorSideSet,Impostor.impostorEndSet,
        true,VentPermission.CanUseUnlimittedVent,true,true,true){
    }
}