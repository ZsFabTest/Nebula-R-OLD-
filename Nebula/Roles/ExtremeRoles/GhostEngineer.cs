namespace Nebula.Roles.GhostRoles;

public class GhostEngineer : GhostRole
{
    static public Color RoleColor = new Color(63f / 255f, 72f / 255f, 204f / 255f);

    private Module.CustomOption maxRepairOption;
    public override void LoadOptionData()
    {
        maxRepairOption = CreateOption(Color.white, "maxRepair", 2f, 1f, 10f, 1f);
    }

    public override bool IsAssignableTo(Game.PlayerData player)
    {
        return player.role.side == Side.Crewmate;
    }

    public int repairId { get; private set; }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        __instance.GetModData().SetRoleData(repairId,(int)maxRepairOption.getFloat());
    }

    CustomButton repairButton;
    private SpriteLoader repairButtonSprite = new SpriteLoader("Nebula.Resources.RepairButton.png", 115f, "ui.button.ghostEngineer.repair");

    public override void ButtonInitialize(HudManager __instance)
    {
        if (repairButton != null)
        {
            repairButton.Destroy();
        }
        repairButton = new CustomButton(
            () =>
            {
                Helpers.RepairSabotage();
                RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId,repairId,-1);
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.GetModData().GetRoleData(repairId) > 0; },
            () => { return Helpers.SabotageIsActive() && PlayerControl.LocalPlayer.CanMove; },
            () => { repairButton.Timer = 0; },
            repairButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.repair"
        );
        repairButton.MaxTimer = repairButton.Timer = 0;
    }

    public override void CleanUp()
    {
        if (repairButton != null)
        {
            repairButton.Destroy();
            repairButton = null;
        }
    }

    public GhostEngineer() : base("GhostEngineer", "ghostEngineer", RoleColor)
    {
    }
}
