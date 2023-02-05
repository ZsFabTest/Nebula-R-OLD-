using static UnityEngine.GraphicsBuffer;

namespace Nebula.Roles.NeutralRoles;

public class SchrodingersCat : Role
{
    public static Color RoleColor = ChainShifter.RoleColor;

    public class CatEvent : Events.LocalEvent
    {
        byte murderId;
        Role targetRole;
        public CatEvent(byte murderId,Role targetRole) : base(0.2f)
        {
            this.murderId = murderId;
            this.targetRole = targetRole;
        }

        public override void OnTerminal()
        {
            RPCEventInvoker.RevivePlayer(PlayerControl.LocalPlayer);
            RPCEventInvoker.ImmediatelyChangeRole(PlayerControl.LocalPlayer, this.targetRole);
        }
    }

    public Module.CustomOption isGuessable;
    public Module.CustomOption canBeCrewmate;
    public Module.CustomOption canBeImpostor;
    public Module.CustomOption canBeJackal;
    public Module.CustomOption canUseKillButton;
    public Module.CustomOption killCooldown;
    public Module.CustomOption canChangeTeam;

    public override void LoadOptionData()
    {
        isGuessable = CreateOption(Color.white, "isGuessable", true);
        canBeCrewmate = CreateOption(Palette.CrewmateBlue, "canBeCrewmate", true);
        canBeImpostor = CreateOption(Palette.ImpostorRed, "canBeImpostor", true);
        canBeJackal = CreateOption(Roles.Jackal.Color, "canBeJackal", true);
        canUseKillButton = CreateOption(Roles.Jackal.Color, "canUseKillButton", false).AddPrerequisite(canBeJackal);
        killCooldown = CreateOption(Roles.Jackal.Color, "killCooldown", 25f, 10f, 60f, 2.5f).AddPrerequisite(canUseKillButton);
        canChangeTeam = CreateOption(Color.white, "canAlwaysChangeTeam", true);
    }

    public override bool IsGuessableRole { get => isGuessable.getBool(); protected set => base.IsGuessableRole = value; }

    public override void OnMurdered(byte murderId)
    {
        Role checkrole = Helpers.playerById(murderId).GetModData().role;
        if (checkrole.side == Side.Crewmate && canBeCrewmate.getBool())
        {
            Events.LocalEvent.Activate(new CatEvent(murderId,Roles.WhiteCat));
        }
        else if (checkrole.side == Side.Impostor && canBeImpostor.getBool())
        {
            Events.LocalEvent.Activate(new CatEvent(murderId, Roles.RedCat));
        }
        else if (checkrole.side == Side.Jackal && canBeJackal.getBool())
        {
            Events.LocalEvent.Activate(new CatEvent(murderId, Roles.BlueCat));
        }
    }

    public SchrodingersCat()
     : base("SchorodingersCat", "schrodingersCat", RoleColor, RoleCategory.Neutral, Side.SchrodingersCat, Side.SchrodingersCat,
          new HashSet<Side>() { Side.SchrodingersCat }, new HashSet<Side>() { Side.SchrodingersCat },
          new HashSet<Patches.EndCondition>() { },
          true, VentPermission.CanNotUse, false, false, false)
    {
    }
}
