namespace Nebula.Roles.ImpostorRoles;

public class Retarder : Template.TImpostor{
    public Module.CustomOption cooldownOption;
    public Module.CustomOption duringTimeOption;

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
        cooldownOption = CreateOption(Color.white,"cooldown",30f,2.5f,60f,2.5f);
        cooldownOption.suffix = "second";
        duringTimeOption = CreateOption(Color.white,"duringTime",5f,1f,15f,1f);
        duringTimeOption.suffix = "second";
    }

    public static SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.TrackEvilButton.png",115f);

    private CustomButton tardiness;
    public override void ButtonInitialize(HudManager __instance)
    {
        if(tardiness != null) tardiness.Destroy();
        tardiness = new CustomButton(
            () => {
                foreach(PlayerControl p in PlayerControl.AllPlayerControls){
                    if(p == PlayerControl.LocalPlayer) continue;
                    RPCEventInvoker.EmitSpeedFactor(p,new Game.SpeedFactor(114,duringTimeOption.getFloat(),0.1f,false));
                }
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && !Helpers.SabotageIsActive(); },
            () => { tardiness.Timer = tardiness.MaxTimer; },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            true,
            duringTimeOption.getFloat(),
            () => { tardiness.Timer = tardiness.MaxTimer; },
            "button.label.tardiness"
        ).SetTimer(CustomOptionHolder.InitialAbilityCoolDownOption.getFloat());
        tardiness.MaxTimer = cooldownOption.getFloat();
    }

    public override void CleanUp()
    {
        if(tardiness != null){
            tardiness.Destroy();
            tardiness = null;
        }
    }

    public override void AfterTeleport(float time)
    {
        if(tardiness.Timer < time) tardiness.Timer = time;
    }

    public Retarder() : base("Retarder","retarder",true){
        tardiness = null;
    }
}