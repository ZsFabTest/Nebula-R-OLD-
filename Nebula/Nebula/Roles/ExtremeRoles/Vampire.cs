namespace Nebula.Roles.ImpostorRoles;

public class Vampire : Role
{
    public static Module.CustomOption dieAfterSuckBlondOption;
    public static Module.CustomOption suckBlondCooldownOption;

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
        dieAfterSuckBlondOption = CreateOption(Color.white, "dieAfterSuckBlond", 5f, 5f, 15f, 2.5f);
        dieAfterSuckBlondOption.suffix = "second";
        suckBlondCooldownOption = CreateOption(Color.white, "suckBlondCooldown", 25f, 15f, 35f, 5f);
        suckBlondCooldownOption.suffix = "second";
    }

    public class SuckBlondEvent : Events.LocalEvent
    {
        PlayerControl target;
        public SuckBlondEvent(PlayerControl target) : base(dieAfterSuckBlondOption.getFloat()) { this.target = target; }
        public override void OnTerminal()
        {
            if (PlayerControl.LocalPlayer.GetModData().IsAlive && target.GetModData().IsAlive)
            {
                RPCEventInvoker.UncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId, target.PlayerId, Game.PlayerData.PlayerStatus.Hemorrhage.Id, false);
            }
        }
    }

    private static CustomButton killButton;

    public override void ButtonInitialize(HudManager __instance)
    {
        if(killButton != null)
        {
            killButton.Destroy();
        }
        killButton = new CustomButton(
            () =>
            {
                Events.LocalEvent.Activate(new SuckBlondEvent(Game.GameData.data.myData.currentTarget));
                Game.GameData.data.myData.currentTarget.ShowFailedMurder();
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && Game.GameData.data.myData.currentTarget != null; },
            () => { killButton.Timer = suckBlondCooldownOption.getFloat(); },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            true,
            dieAfterSuckBlondOption.getFloat(),
            () => { killButton.Timer = suckBlondCooldownOption.getFloat(); },
            "button.label.suckBlond"
        ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
    }

    public override void CleanUp()
    {
        if(killButton != null)
        {
            killButton.Destroy();
            killButton = null;
        }
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(true);
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
    }

    public Vampire()
        : base("Vampire", "vampire", Palette.ImpostorRed, RoleCategory.Impostor, Side.Impostor, Side.Impostor,
             Impostor.impostorSideSet, Impostor.impostorSideSet,
             Impostor.impostorEndSet,
             true, VentPermission.CanUseUnlimittedVent, true, true, true)
    {
        HideKillButtonEvenImpostor = true;
        killButton = null;
    }
}
