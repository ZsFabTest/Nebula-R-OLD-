namespace Nebula.Roles.CrewmateRoles;

public class Locksmith : Template.TCrewmate{
    public static Color RoleColor = new Color(122f / 255f,146f / 255f,190f / 255f);
    public static SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.DoorButton.png",115f);

    private Module.CustomOption unlockDoorCooldownOption;
    PlainDoor target;

    public override void GlobalInitialize(PlayerControl __instance)
    {
        target = null;
    }

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
        unlockDoorCooldownOption = CreateOption(Color.white,"unlockDoorCooldown",5f,0f,30f,2.5f);
        unlockDoorCooldownOption.suffix = "second";
    }

    public override void MyPlayerControlUpdate()
    {
        target = GetDoor();
    }

    private CustomButton open;
    public override void ButtonInitialize(HudManager __instance)
    {
        if(open != null) open.Destroy();
        open = new CustomButton(
            () => {
                target.SetDoorway(true);
                open.Timer = open.MaxTimer;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return target && !target.Open && PlayerControl.LocalPlayer.CanMove; },
            () => { open.Timer = open.MaxTimer; },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.open"
        ).SetTimer(CustomOptionHolder.InitialAbilityCoolDownOption.getFloat());
        open.MaxTimer = unlockDoorCooldownOption.getFloat();
    }

    public override void CleanUp(){
        if(open != null){
            open.Destroy();
            open = null;
        }
        target = null;
    }

    private static PlainDoor GetDoor()
    {
        return GameObject.FindObjectsOfType<PlainDoor>().ToArray().FirstOrDefault(x =>
        {
            if (x == null) return false;
            float num = Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), x.transform.position);
            return num <= 1.8f;
        });
    }

    public Locksmith() : base("Locksmith","locksmith",RoleColor,false){
        target = null;
        open = null;
    }
}