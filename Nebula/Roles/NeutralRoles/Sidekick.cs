﻿namespace Nebula.Roles.NeutralRoles;

static public class SidekickSystem
{

}

public class Sidekick : Role
{
    public override RelatedRoleData[] RelatedRoleDataInfo
    {
        get => new RelatedRoleData[] {
            new RelatedRoleData(Roles.Jackal.jackalDataId, "Jackal Identifier", 0, 15)
            };
    }

    static private CustomButton killButton;

    static public Module.CustomOption SidekickCanKillOption;
    static public Module.CustomOption SidekickTakeOverOriginalRoleOption;
    static public Module.CustomOption SidekickKillCoolDownOption;
    static public Module.CustomOption SidekickCanCreateSidekickOption;
    static public Module.CustomOption SidekickCanUseVentsOption;

    public override void LoadOptionData()
    {
        TopOption.AddPrerequisite(Jackal.CanCreateSidekickOption).AddCustomPrerequisite(() => Roles.Jackal.IsSpawnable());

        SidekickCanKillOption = CreateOption(Color.white, "canKill", false);
        SidekickKillCoolDownOption = CreateOption(Color.white, "killCoolDown", 20f, 10f, 60f, 2.5f);
        SidekickKillCoolDownOption.suffix = "second";
        SidekickTakeOverOriginalRoleOption = CreateOption(Color.white, "takeOverOriginalRole", true);
        SidekickCanCreateSidekickOption = CreateOption(Color.white, "canCreateSidekick", false);
        SidekickCanUseVentsOption = CreateOption(Color.white, "canUseVents", false);

        SidekickCanKillOption.AddPrerequisite(SidekickTakeOverOriginalRoleOption);
        SidekickKillCoolDownOption.AddPrerequisite(SidekickCanKillOption);
        SidekickCanUseVentsOption.AddPrerequisite(SidekickTakeOverOriginalRoleOption);
    }

    public override void MyPlayerControlUpdate()
    {
        if (killButton != null)
        {
            int jackalId = Game.GameData.data.AllPlayers[PlayerControl.LocalPlayer.PlayerId].GetRoleData(Roles.Jackal.jackalDataId);

            Game.MyPlayerData data = Game.GameData.data.myData;
            data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(
            (player) =>
            {
                if (player.Object.inVent) return false;
                if (player.GetModData().role.side == Side.Jackal)
                {
                    return false;
                }
                else if (player.GetModData().HasExtraRole(Roles.SecondarySidekick))
                {
                    return false;
                }
                return true;
            });
            Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
        }
    }

    public override void ButtonInitialize(HudManager __instance)
    {
        if (killButton != null)
        {
            killButton.Destroy();
        }
        killButton = null;

        if (SidekickCanKillOption.getBool())
        {
            killButton = new CustomButton(
                () =>
                {
                    Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, Game.GameData.data.myData.currentTarget, Game.PlayerData.PlayerStatus.Dead, true);

                    killButton.Timer = killButton.MaxTimer;
                    Game.GameData.data.myData.currentTarget = null;
                },
                () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
                () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
                () => { killButton.Timer = killButton.MaxTimer; },
                __instance.KillButton.graphic.sprite,
                Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
                __instance,
                Module.NebulaInputManager.modKillInput.keyCode
            ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
            killButton.MaxTimer = SidekickKillCoolDownOption.getFloat();
        }
        GlobalIntroInitialize(PlayerControl.LocalPlayer);
    }
    public override void CleanUp()
    {
        if (killButton != null)
        {
            killButton.Destroy();
            killButton = null;
        }

    }

    public override void GlobalIntroInitialize(PlayerControl __instance)
    {
        canMoveInVents = SidekickCanUseVentsOption.getBool();
        VentPermission = SidekickCanUseVentsOption.getBool() ? VentPermission.CanUseUnlimittedVent : VentPermission.CanNotUse;
    }

    public override void EditDisplayNameColor(byte playerId, ref Color displayColor) => Roles.Jackal.EditDisplayNameColor(playerId,ref displayColor);
    /*
    {
        if (PlayerControl.LocalPlayer.GetModData().role.side == Side.Jackal)
        {
            displayColor = Color;
        }
        else if (PlayerControl.LocalPlayer.GetModData().HasExtraRole(Roles.SecondarySidekick))
        {
            displayColor = Color;
        }
    }
    */

    public override bool IsSpawnable()
    {
        if (!Roles.Jackal.IsSpawnable()) return false;
        if (!Jackal.CanCreateSidekickOption.getBool()) return false;
        if (!SidekickTakeOverOriginalRoleOption.getBool()) return false;

        return true;
    }

    public Sidekick()
        : base("Sidekick", "sidekick", Jackal.RoleColor, RoleCategory.Neutral, Side.Jackal, Side.Jackal,
             new HashSet<Side>() { Side.Jackal }, new HashSet<Side>() { Side.Jackal },
             new HashSet<Patches.EndCondition>() { Patches.EndCondition.JackalWin },
             true, VentPermission.CanNotUse, true, true, true)
    {
        killButton = null;

        Allocation = AllocationType.None;
        CreateOptionFollowingRelatedRole = true;
    }
}

public class SecondarySidekick : ExtraRole
{
    public override RelatedExtraRoleData[] RelatedExtraRoleDataInfo { get => new RelatedExtraRoleData[] { new RelatedExtraRoleData("Jackal Identifer", this, 0, 14) }; }

    public override void EditDisplayNameColor(byte playerId, ref Color displayColor) => Roles.Jackal.EditDisplayNameColor(playerId,ref displayColor);
    /*
    {
        if (PlayerControl.LocalPlayer.GetModData().role == Roles.Jackal)
        {
            if (PlayerControl.LocalPlayer.GetModData().GetRoleData(Roles.Jackal.jackalDataId) == (int)Helpers.playerById(playerId).GetModData().GetExtraRoleData(Roles.SecondarySidekick))
            {
                displayColor = Color;
            }
        }
    }
    */

    public override bool CheckAdditionalWin(PlayerControl player, Patches.EndCondition condition)
    {
        return condition == Patches.EndCondition.JackalWin;
    }

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag)
    {
        bool showFlag = Game.GameData.data.myData.CanSeeEveryoneInfo;

        if (PlayerControl.LocalPlayer.PlayerId == playerId) showFlag = true;
        else
        {
            if (PlayerControl.LocalPlayer.GetModData().role == Roles.Jackal)
            {
                if (PlayerControl.LocalPlayer.GetModData().GetRoleData(Roles.Jackal.jackalDataId) == (int)Helpers.playerById(playerId).GetModData().GetExtraRoleData(Roles.SecondarySidekick))
                {
                    showFlag = true;
                }
            }
        }

        if (showFlag) EditDisplayNameForcely(playerId, ref displayName);
    }

    public override void EditDisplayNameForcely(byte playerId, ref string displayName)
    {
        displayName += Helpers.cs(
                Jackal.RoleColor, "#");
    }

    public SecondarySidekick() : base("Sidekick", "sidekick", Jackal.RoleColor, 0)
    {
        IsHideRole = true;
    }
}
