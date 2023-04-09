using static Nebula.Roles.NeutralRoles.SchrodingersCat;

namespace Nebula.Roles.CrewmateRoles;

public class WhiteCat : Role
{

    public override bool IsGuessableRole { get => Roles.SchrodingersCat.isGuessable.getBool(); protected set => base.IsGuessableRole = value; }

    public override void OnMurdered(byte murderId)
    {
        if (!Roles.SchrodingersCat.canChangeTeam.getBool()) return;
        Role checkrole = Helpers.playerById(murderId).GetModData().role;
        if (checkrole.side == Side.Impostor && Roles.SchrodingersCat.canBeImpostor.getBool())
        {
            Events.LocalEvent.Activate(new CatEvent(murderId, Roles.RedCat));
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

    public WhiteCat()
        : base("WhiteCat", "whiteCat", Palette.CrewmateBlue, RoleCategory.Crewmate, Side.Crewmate, Side.Crewmate,
             Crewmate.crewmateSideSet, Crewmate.crewmateSideSet, Crewmate.crewmateEndSet,
             true, VentPermission.CanNotUse, false, false, true)
    {
        IsHideRole = true;
    }
}
