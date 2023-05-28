namespace Nebula.Roles.CrewmateRoles;

public class Observer : Role
{
    public static Color RoleColor = new Color(200f / 255f, 190f / 255f, 230 / 255f);

    private static Module.CustomOption canUseVent;
    private static Module.CustomOption hasImpostorLight;
    private static Module.CustomOption hideCoolDown;
    private static Module.CustomOption hideDuringTime;

    private CustomButton hideButton;
    private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.WarpButton.png", 115f);

    public override void LoadOptionData()
    {
        canUseVent = CreateOption(Color.white, "canUseVent", true);
        hasImpostorLight = CreateOption(Color.white, "hasImpostorLight", true);
        hideCoolDown = CreateOption(Color.white, "hideCoolDown", 15f, 5f, 30f, 2.5f);
        hideCoolDown.suffix = "second";
        hideDuringTime = CreateOption(Color.white, "hideDuringTime", 15f, 5f, 20f, 2.5f);
        hideDuringTime.suffix = "second";
    }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        VentPermission = canUseVent.getBool() ? VentPermission.CanUseUnlimittedVent : VentPermission.CanNotUse;
        UseImpostorLightRadius = hasImpostorLight.getBool();
    }

    public override void ButtonInitialize(HudManager __instance)
    {
        if(hideButton != null)
        {
            hideButton.Destroy();
        }
        hideButton = new CustomButton(
            () =>
            {
                RPCEventInvoker.EmitAttributeFactor(PlayerControl.LocalPlayer, new Game.PlayerAttributeFactor(Game.PlayerAttribute.Invisible, hideDuringTime.getFloat(), 0, false));
                RPCEventInvoker.UpdatePlayerVisibility(PlayerControl.LocalPlayer.PlayerId, false);
                Game.GameData.data.myData.Vision.Register(new Game.VisionFactor(hideDuringTime.getFloat(), 1.5f));
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () =>
            {
                hideButton.Timer = hideButton.MaxTimer;
                RPCEventInvoker.UpdatePlayerVisibility(PlayerControl.LocalPlayer.PlayerId, true);
            },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            true,
           hideDuringTime.getFloat(),
           () =>
           {
               hideButton.Timer = hideButton.MaxTimer;
               RPCEventInvoker.UpdatePlayerVisibility(PlayerControl.LocalPlayer.PlayerId, true);
           },
            "button.label.hide"
        ).SetTimer(CustomOptionHolder.InitialAbilityCoolDownOption.getFloat());
        hideButton.MaxTimer = hideCoolDown.getFloat();
    }

    public override void CleanUp()
    {
        if(hideButton != null)
        {
            hideButton.Destroy();
            hideButton = null;
        }
    }

    public override void MyPlayerControlUpdate(){
        foreach(var p in PlayerControl.AllPlayerControls){
            if(!p.Data.IsDead) Patches.PlayerControlPatch.SetPlayerOutline(p,RoleColor);
        }
        RPCEventInvoker.UpdatePlayerVisibility(PlayerControl.LocalPlayer.PlayerId,true);
    }

    public Observer()
        : base("Observer", "observer", RoleColor, RoleCategory.Crewmate, Side.Crewmate, Side.Crewmate,
             Crewmate.crewmateSideSet, Crewmate.crewmateSideSet, Crewmate.crewmateEndSet,
             true, VentPermission.CanUseUnlimittedVent, false, true, true)
    {
        hideButton = null;
    }
}
