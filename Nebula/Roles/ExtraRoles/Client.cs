using Nebula.Roles.NeutralRoles;

namespace Nebula.Roles.ExtraRoles;

public class Client : ExtraRole
{
    public override void Assignment(Patches.AssignMap assignMap)
    {
        if (!Roles.Lawyer.IsSpawnable()) return;

        List<byte> players = new List<byte>();
        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (!player.GetModData()?.role.CanHaveExtraAssignable(this) ?? true) continue;
            players.Add(player.PlayerId);
        }
        byte playerId = players[NebulaPlugin.rnd.Next(players.Count)];
        assignMap.AssignExtraRole(playerId, id, 0);
    }

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag)
    {
        if (playerId == PlayerControl.LocalPlayer.PlayerId || Game.GameData.data.myData.CanSeeEveryoneInfo)
        {
            EditDisplayNameForcely(playerId, ref displayName);
            return;
        }
        else if (Lawyer.clientCanKnowLawyer.getBool())
        {
            if(Helpers.playerById(playerId).GetModData().role == Roles.Lawyer)
            {
                EditDisplayNameForcely(playerId, ref displayName);
            }
        }
    }

    public override void EditDisplayNameForcely(byte playerId, ref string displayName)
    {
        displayName += Helpers.cs(Color, "@");
    }

    public override void OnDied(byte playerId)
    {
        if(PlayerControl.LocalPlayer.GetModData().role == Roles.Lawyer)
        {
            Events.LocalEvent.Activate(new Lawyer.LawyerEvent(PlayerControl.LocalPlayer));
        }
    }

    public Client() : base("Client", "client", Roles.Lawyer.Color, 0)
    {
        IsHideRole = true;
    }
}
