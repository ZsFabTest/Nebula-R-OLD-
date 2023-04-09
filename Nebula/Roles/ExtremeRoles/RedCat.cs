using static Nebula.Roles.NeutralRoles.SchrodingersCat;

namespace Nebula.Roles.ImpostorRoles;

public class RedCat : Role
{
    public override bool IsGuessableRole { get => Roles.SchrodingersCat.isGuessable.getBool(); protected set => base.IsGuessableRole = value; }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        HideKillButtonEvenImpostor = !Roles.SchrodingersCat.canUseKillButtonI.getBool();
    }

    public override void OnMurdered(byte murderId)
    {
        if (!Roles.SchrodingersCat.canChangeTeam.getBool()) return;
        Role checkrole = Helpers.playerById(murderId).GetModData().role;
        if (checkrole.side == Side.Crewmate && Roles.SchrodingersCat.canBeCrewmate.getBool())
        {
            Events.LocalEvent.Activate(new CatEvent(murderId, Roles.WhiteCat));
        }
        else if (checkrole.side == Side.Jackal && Roles.SchrodingersCat.canBeJackal.getBool())
        {
            Events.LocalEvent.Activate(new CatEvent(murderId, Roles.BlueCat));
        }
        else if (checkrole.side == Side.Pavlov && Roles.SchrodingersCat.canBePavlovsCat.getBool())
        {
            Events.LocalEvent.Activate(new CatEvent(murderId, Roles.PavlovsCat));
        }
    }

    public RedCat()
        : base("RedCat", "redCat", Palette.ImpostorRed, RoleCategory.Impostor, Side.Impostor, Side.Impostor,
             Impostor.impostorSideSet, Impostor.impostorSideSet,
             Impostor.impostorEndSet,
             true, VentPermission.CanUseUnlimittedVent, true, true, true)
    {
        IsHideRole = true;
    }
}
