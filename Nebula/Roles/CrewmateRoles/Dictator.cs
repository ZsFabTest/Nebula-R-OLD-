namespace Nebula.Roles.CrewmateRoles;

public class Dictator : Role{
    public static Color RoleColor = new Color(236f / 255f,107f / 255f,38f / 255f);

    private bool isVoted = false;
    private byte target;
    private byte voteId = 255;

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
    }

    public override void GlobalIntroInitialize(PlayerControl __instance)
    {
        isVoted = false;
    }

    public override void OnVote(byte targetId)
    {
        if(targetId <= 14)
        {
            RPCEventInvoker.MultipleVote(PlayerControl.LocalPlayer, voteId);
            isVoted = true;
            target = targetId;
        }
    }

    public override void OnVoteCanceled(int weight)
    {
        isVoted = false;
    }

    public override void OnMeetingEnd()
    {
        if (isVoted)
        {
            byte playerId = PlayerControl.LocalPlayer.PlayerId;
            RPCEventInvoker.UncheckedMurderPlayer(playerId,playerId, Game.PlayerData.PlayerStatus.Suicide.Id,false);
            RPCEventInvoker.CleanDeadBody(playerId);
            RPCEventInvoker.UncheckedExilePlayer(target, Game.PlayerData.PlayerStatus.Exiled.Id);
        }
    }

    public Dictator() : base("Dictator", "dictator", RoleColor, RoleCategory.Crewmate, Side.Crewmate, Side.Crewmate,
             Crewmate.crewmateSideSet, Crewmate.crewmateSideSet, Crewmate.crewmateEndSet,
             false, VentPermission.CanNotUse, false, false, false)
    {
        CanCallEmergencyMeeting = false;
    }
}