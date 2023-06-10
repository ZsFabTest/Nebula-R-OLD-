namespace Nebula.Roles.ImpostorRoles;

public class EvilBusker : Role
{
    private CustomButton buskButton;

    private Module.CustomOption buskCoolDownOption;
    private Module.CustomOption buskDurationOption;
    private Module.CustomOption killCoolDownOption;

    private SpriteLoader pseudocideButtonSprite = new SpriteLoader("Nebula.Resources.BuskPseudocideButton.png", 115f);
    private SpriteLoader reviveButtonSprite = new SpriteLoader("Nebula.Resources.BuskReviveButton.png", 115f);

    public override HelpSprite[] helpSprite => new HelpSprite[]
    {
            new HelpSprite(pseudocideButtonSprite,"role.busker.help.pseudocide",0.3f),
            new HelpSprite(reviveButtonSprite,"role.busker.help.revive",0.3f)
    };

    public bool pseudocideFlag = false;

    private void dieBusker()
    {
        FastDestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);
        pseudocideFlag = false;
        RPCEventInvoker.UpdatePlayerVisibility(PlayerControl.LocalPlayer.PlayerId, true);
    }

    public override bool CanHaveGhostRole { get => false; }

    private bool checkPseudocide()
    {
        if (!pseudocideFlag) return false;

        foreach (var deadBody in Helpers.AllDeadBodies())
        {
            if (deadBody.ParentId == PlayerControl.LocalPlayer.PlayerId)
            {
                return true;
            }
        }
        dieBusker();
        return false;
    }

    private bool checkCanReviveOrAlive()
    {
        if (!PlayerControl.LocalPlayer.CanMove) return false;
        if (!PlayerControl.LocalPlayer.Data.IsDead) return true;

        var mapData = Map.MapData.GetCurrentMapData();
        if (mapData == null) return false;

        return mapData.isOnTheShip(PlayerControl.LocalPlayer.GetTruePosition());
    }

    public override void OnMeetingStart()
    {
        if (pseudocideFlag)
        {
            dieBusker();
        }
    }

    private CustomButton killButton;

    public override void ButtonInitialize(HudManager __instance)
    {
        base.ButtonInitialize(__instance);

        pseudocideFlag = false;

        if (buskButton != null)
        {
            buskButton.Destroy();
        }
        buskButton = new CustomButton(
            () =>
            {
                RPCEventInvoker.UpdatePlayerVisibility(PlayerControl.LocalPlayer.PlayerId, false);
                RPCEventInvoker.SuicideWithoutOverlay(Game.PlayerData.PlayerStatus.Pseudocide.Id);
                FastDestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(true);
                pseudocideFlag = true;

                buskButton.Sprite = reviveButtonSprite.GetSprite();
                buskButton.SetLabel("button.label.busker.revive");
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead || checkPseudocide(); },
            () => { return checkCanReviveOrAlive(); },
            () =>
            {
                buskButton.Timer = buskButton.MaxTimer;
            },
            pseudocideButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            true,
           buskDurationOption.getFloat(),
           () => { dieBusker(); },
            "button.label.busker.pseudocide"
        ).SetTimer(CustomOptionHolder.InitialModestAbilityCoolDownOption.getFloat());
        buskButton.MaxTimer = buskCoolDownOption.getFloat();

        buskButton.SetSuspendAction(() =>
        {
            if (buskButton.EffectDuration - buskButton.Timer < 1f) return;
            if (!checkPseudocide()) return;
            if (!checkCanReviveOrAlive()) return;
            RPCEventInvoker.UpdatePlayerVisibility(PlayerControl.LocalPlayer.PlayerId, true);
            RPCEventInvoker.RevivePlayer(PlayerControl.LocalPlayer, true, false, true);
            RPCEventInvoker.SetPlayerStatus(PlayerControl.LocalPlayer.PlayerId, Game.PlayerData.PlayerStatus.Alive);
            buskButton.Timer = buskButton.MaxTimer;
            buskButton.isEffectActive = false;
            buskButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;

            buskButton.Sprite = pseudocideButtonSprite.GetSprite();
            buskButton.SetLabel("button.label.busker.pseudocide");
            pseudocideFlag = false;
        });

        if(killButton != null) killButton.Destroy();
        killButton = new CustomButton(
            () => { 
                PlayerControl target = Game.GameData.data.myData.currentTarget;

                var res = Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, target, Game.PlayerData.PlayerStatus.Dead, false, true);
                if (res != Helpers.MurderAttemptResult.SuppressKill)
                    killButton.Timer = killButton.MaxTimer;
                Game.GameData.data.myData.currentTarget = null;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead || pseudocideFlag; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { killButton.Timer = killButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            "button.label.kill"
        ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
        killButton.MaxTimer = killCoolDownOption.getFloat();
        killButton.SetButtonCoolDownOption(true);
    }

    public override void EditCoolDown(CoolDownType type, float count)
    {
        killButton.Timer -= count;
        killButton.actionButton.ShowButtonText("+" + count + "s");
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
    }

    public override void CleanUp()
    {
        if (buskButton != null)
        {
            buskButton.Destroy();
            buskButton = null;
        }
        if (killButton != null)
        {
            killButton.Destroy();
            killButton = null;
        }
    }

    public override void PreloadOptionData()
    {
        extraAssignableOptions.Add(Roles.Lover, null);
    }

    public override void LoadOptionData()
    {
        buskCoolDownOption = CreateOption(Color.white, "buskCoolDown", 30f, 5f, 60f, 2.5f);
        buskCoolDownOption.suffix = "second";

        buskDurationOption = CreateOption(Color.white, "buskDuration", 7.5f, 5f, 10f, 2.5f);
        buskDurationOption.suffix = "second";

        killCoolDownOption = CreateOption(Color.white, "killCoolDown", 25f, 5f, 60f, 2.5f);
        killCoolDownOption.suffix = "second";
    }

    public override void OnRoleRelationSetting()
    {
        RelatedRoles.Add(Roles.Madmate);
        RelatedRoles.Add(Roles.Sheriff);
        RelatedRoles.Add(Roles.Avenger);
    }

    public EvilBusker()
        : base("EvilBusker", "evilBusker", Palette.ImpostorRed, RoleCategory.Impostor, Side.Impostor, Side.Impostor,
             Impostor.impostorSideSet, Impostor.impostorSideSet, Impostor.impostorEndSet,
             true, VentPermission.CanUseUnlimittedVent, true, true, true)
    {
        HideKillButtonEvenImpostor = true;
        buskButton = null;
        killButton = null;
    }
}