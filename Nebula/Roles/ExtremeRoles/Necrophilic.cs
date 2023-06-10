namespace Nebula.Roles.CrewmateRoles;

public class Necrophilic : Role
{
    public static Color RoleColor = new Color(221f / 255f,20f / 255f,243f / 255f);

    public Module.CustomOption snapCoolDownOption;

    // HadarHideButton
    public SpriteLoader ButtonSprite = new SpriteLoader("Nebula.Resources.HadarHideButton.png", 115f);

    public override void LoadOptionData()
    {
        snapCoolDownOption = CreateOption(Color.white, "snapCoolDown", 15f, 5f, 35f, 2.5f);
        snapCoolDownOption.suffix = "second";
    }

    private CustomButton SnapToBody;
    public override void ButtonInitialize(HudManager __instance)
    {
        if(SnapToBody != null){
            SnapToBody.Destroy();
        }
        SnapToBody = new CustomButton(
            () => {
                if(Helpers.AllDeadBodies().Count() == 0)
                {
                    SnapToBody.Timer = SnapToBody.MaxTimer;
                    return;
                }
                DeadBody body = Helpers.AllDeadBodies()[NebulaPlugin.rnd.Next(Helpers.AllDeadBodies().Count())];
                RPCEventInvoker.ObjectInstantiate(CustomObject.Type.TeleportEvidence,PlayerControl.LocalPlayer.GetTruePosition());
                Vector3 pos = body.transform.position;
                PlayerControl.LocalPlayer.transform.position = pos;
                SnapToBody.Timer = SnapToBody.MaxTimer;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () => { SnapToBody.Timer = SnapToBody.MaxTimer; },
            ButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.snap"
        ).SetTimer(CustomOptionHolder.InitialAbilityCoolDownOption.getFloat());
        SnapToBody.MaxTimer = snapCoolDownOption.getFloat();
    }

    public override void CleanUp()
    {
        if(SnapToBody != null){
            SnapToBody.Destroy();
            SnapToBody = null;
        }
    }

    public Necrophilic()
        : base("Necrophilic", "necrophilic", RoleColor, RoleCategory.Crewmate, Side.Crewmate, Side.Crewmate,
             CrewmateRoles.Crewmate.crewmateSideSet, CrewmateRoles.Crewmate.crewmateSideSet,
             CrewmateRoles.Crewmate.crewmateEndSet,
             false, VentPermission.CanNotUse, false, false, false)
    {
        SnapToBody = null;
    }
}