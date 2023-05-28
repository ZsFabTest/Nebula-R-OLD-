namespace Nebula.Roles.ImpostorRoles;

public class Escapist : Role
{
    public Module.CustomOption escapeCoolDown;
    bool mark;
    Vector3 pos;

    private SpriteLoader MarkSprite = new SpriteLoader("Nebula.Resources.AssassinMarkButton.png", 115f);
    private SpriteLoader ButtonSprite = new SpriteLoader("Nebula.Resources.ChainShiftButton.png", 115f);

    public override void LoadOptionData()
    {
        escapeCoolDown = CreateOption(Color.white, "escapeCoolDown", 15f, 5f, 45f, 5f);
        escapeCoolDown.suffix = "second";
    }

    private CustomButton escape;

    public override void GlobalInitialize(PlayerControl __instance)
    {
        base.GlobalInitialize(__instance);
        mark = true;
    }

    public override void ButtonInitialize(HudManager __instance)
    {
        if(escape != null)
        {
            escape.Destroy();
        }
        escape = new CustomButton(
            () =>
            {
                if(mark)
                {
                    mark = !mark;
                    pos = PlayerControl.LocalPlayer.transform.position;
                    escape.Sprite = ButtonSprite.GetSprite();
                    return;
                }
                mark = !mark;
                RPCEventInvoker.ObjectInstantiate(CustomObject.Type.TeleportEvidence,PlayerControl.LocalPlayer.GetTruePosition());
                PlayerControl.LocalPlayer.transform.position = pos;
                escape.Timer = escape.MaxTimer;
                escape.Sprite = MarkSprite.GetSprite();
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () => { escape.Timer = escape.MaxTimer; mark = true; },
            MarkSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.escape"
        ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
        escape.MaxTimer = escapeCoolDown.getFloat();
    }

    public override void CleanUp()
    {
        if(escape != null)
        {
            escape.Destroy();
            escape = null;
        }
    }

    public Escapist()
        : base("Escapist", "escapist", Palette.ImpostorRed, RoleCategory.Impostor, Side.Impostor, Side.Impostor,
             Impostor.impostorSideSet, Impostor.impostorSideSet,
             Impostor.impostorEndSet,
             true, VentPermission.CanUseUnlimittedVent, true, true, true)
    {
        escape = null;
        mark = true;
    }
}
