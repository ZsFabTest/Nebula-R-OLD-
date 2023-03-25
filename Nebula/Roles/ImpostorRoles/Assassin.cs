namespace Nebula.Roles.ImpostorRoles;

public class Assassin : Role
{
    public static Module.CustomOption assassinateCooldownOption;

    private SpriteLoader AssassinMarkButtonSprite = new SpriteLoader("Nebula.Resources.AssassinMarkButton.png", 100f);
    private SpriteLoader AssassinateButtonSprite = new SpriteLoader("Nebula.Resources.AssassinateButton.png", 115f);

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
        assassinateCooldownOption = CreateOption(Color.white, "assassinateCooldown", 25f, 15f, 35f, 5f);
        assassinateCooldownOption.suffix = "second";
    }

    public static CustomButton chooseTarget;
    public static CustomButton assassinate;
    public PlayerControl assassinateTarget;

    public override void ButtonInitialize(HudManager __instance)
    {
        if(chooseTarget != null)
        {
            chooseTarget.Destroy();
        }
        chooseTarget = new CustomButton(
            () =>
            {
                PlayerControl target = Game.GameData.data.myData.currentTarget;
                assassinateTarget = target;
                chooseTarget.Timer = 5f;
                Game.GameData.data.myData.currentTarget.ShowFailedMurder();
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && Game.GameData.data.myData.currentTarget != null; },
            () => { chooseTarget.Timer = 5f; },
            AssassinMarkButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.mark"
        ).SetTimer(CustomOptionHolder.InitialAbilityCoolDownOption.getFloat());

        if (assassinate != null)
        {
            assassinate.Destroy();
        }
        assassinate = new CustomButton(
            () =>
            {
                PlayerControl target = assassinateTarget;
                var res = Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, target, Game.PlayerData.PlayerStatus.Dead, false, true);
                if (res != Helpers.MurderAttemptResult.SuppressKill)
                    assassinate.Timer = assassinate.MaxTimer;
                assassinateTarget = null;
                assassinate.Timer = assassinateCooldownOption.getFloat();
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && assassinateTarget != null; },
            () => { assassinate.Timer = assassinateCooldownOption.getFloat(); },
            AssassinateButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            "button.label.assassinate"
        ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
    }

    public override void CleanUp()
    {
        base.CleanUp();
        if(chooseTarget != null)
        {
            chooseTarget.Destroy();
            chooseTarget = null;
        }
        if(assassinate != null)
        {
            assassinate.Destroy();
            assassinate = null;
        }
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
    }

    public override void OnMeetingEnd()
    {
        assassinateTarget = null;
    }

    public Assassin()
        : base("Assassin", "assassin", Palette.ImpostorRed, RoleCategory.Impostor, Side.Impostor, Side.Impostor,
             Impostor.impostorSideSet, Impostor.impostorSideSet,
             Impostor.impostorEndSet,
             true, VentPermission.CanUseUnlimittedVent, true, true, true)
    {
        assassinateTarget = null;
        HideKillButtonEvenImpostor = true;
        chooseTarget = null;
        assassinate = null;
    }
}
