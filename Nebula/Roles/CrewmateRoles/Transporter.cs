namespace Nebula.Roles.CrewmateRoles;

public class Transporter : Role
{
    static public Color RoleColor = new Color(0f / 255f, 162f / 255f, 232f / 255f);

    static public Module.CustomOption teleportCooldownOption;

    private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.ChainShiftButton.png", 115f);
    private SpriteLoader markSprite = new SpriteLoader("Nebula.Resources.AssassinMarkButton.png", 115f);

    PlayerControl target;

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
        teleportCooldownOption = CreateOption(Color.white, "teleportCooldown", 25f, 15f, 45f, 2.5f);
    }

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
                }
                else
                {
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    {
                        player.transform.position = target.transform.position;
                        teleport.Timer = teleport.MaxTimer;
                    }
                    teleport.Sprite = markSprite.GetSprite();
                    target = null;
                }
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return (Game.GameData.data.myData.currentTarget || target != null) && PlayerControl.LocalPlayer.CanMove; },
            () => { teleport.Timer = teleport.MaxTimer; target = null; },
            markSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.teleport"
        ).SetTimer(CustomOptionHolder.InitialAbilityCoolDownOption.getFloat());
        teleport.MaxTimer = teleportCooldownOption.getFloat();
    }

    public override void CleanUp()
    {
        if(teleport != null)
        {
            teleport.Destroy();
            teleport = null;
        }
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color.yellow);
    }

    public Transporter()
        : base("Transporter", "transporter", RoleColor, RoleCategory.Crewmate, Side.Crewmate, Side.Crewmate,
             Crewmate.crewmateSideSet, Crewmate.crewmateSideSet, Crewmate.crewmateEndSet,
             false, VentPermission.CanNotUse, false, false, false)
    {
        target = null;
    }
}
