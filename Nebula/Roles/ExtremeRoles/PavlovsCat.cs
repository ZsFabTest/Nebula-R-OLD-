using static Nebula.Roles.NeutralRoles.SchrodingersCat;

namespace Nebula.Roles.NeutralRoles;

public class PavlovsCat : Role
{
    private static CustomButton killButton;

    public override bool IsGuessableRole { get => Roles.SchrodingersCat.isGuessable.getBool(); protected set => base.IsGuessableRole = value; }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;

        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(
            (player) =>
            {
                if (player.Object.inVent) return false;
                if (player.GetModData().role.side == Side.Pavlov)
                {
                    return false;
                }
                return true;
            });

        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color);
    }

    public override void ButtonInitialize(HudManager __instance)
    {
        if(killButton != null)
        {
            killButton.Destroy();
        }
        killButton = new CustomButton(
            () =>
            {
                Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, Game.GameData.data.myData.currentTarget, Game.PlayerData.PlayerStatus.Dead, true);
                killButton.Timer = killButton.MaxTimer;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && Roles.SchrodingersCat.canUseKillButtonP.getBool(); },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { killButton.Timer = killButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            "button.label.kill"
        ).SetTimer(Roles.SchrodingersCat.killCooldownP.getFloat());
        killButton.MaxTimer = Roles.SchrodingersCat.killCooldownP.getFloat();
        killButton.SetButtonCoolDownOption(true);
    }

    public override void OnMurdered(byte murderId)
    {
        if (!Roles.SchrodingersCat.canChangeTeam.getBool()) return;
        Role checkrole = Helpers.playerById(murderId).GetModData().role;
        if (checkrole.side == Side.Crewmate && Roles.SchrodingersCat.canBeCrewmate.getBool())
        {
            Events.LocalEvent.Activate(new CatEvent(murderId, Roles.WhiteCat));
        }
        else if (checkrole.side == Side.Impostor && Roles.SchrodingersCat.canBeImpostor.getBool())
        {
            Events.LocalEvent.Activate(new CatEvent(murderId, Roles.RedCat));
        }
        else if (checkrole.side == Side.Jackal && Roles.SchrodingersCat.canBeJackal.getBool())
        {
            Events.LocalEvent.Activate(new CatEvent(murderId, Roles.BlueCat));
        }
        /*
        else if (checkrole.side == Side.Pavlov && Roles.SchrodingersCat.canBePavlovsCat.getBool())
        {
            Events.LocalEvent.Activate(new CatEvent(murderId, Roles.PavlovsCat));
        }
        */
    }

    public override void CleanUp()
    {
        if (killButton != null)
        {
            killButton.Destroy();
            killButton = null;
        }
    }

    public override void EditDisplayNameColor(byte playerId, ref Color displayColor)
    {
        if (PlayerControl.LocalPlayer.GetModData().role.side == Side.Pavlov)
        {
            displayColor = Color;
        }
    }

    public PavlovsCat()
        : base("PavlovsCat", "pavlovsCat", Roles.Pavlov.Color, RoleCategory.Neutral, Side.Pavlov, Side.Pavlov,
             new HashSet<Side>() { Side.Pavlov }, new HashSet<Side>() { Side.Pavlov },
             new HashSet<Patches.EndCondition>() { Patches.EndCondition.PavlovWin },
             true, VentPermission.CanUseUnlimittedVent, true, true, true)
    {
        killButton = null;
        IsHideRole = true;
    }
}
