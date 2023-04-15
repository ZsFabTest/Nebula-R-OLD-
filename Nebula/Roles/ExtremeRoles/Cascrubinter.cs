namespace Nebula.Roles.NeutralRoles;

public class Cascrubinter : Role,Template.HasWinTrigger{
    public static Color RoleColor = new Color(99f / 255f,31f / 255f,12f / 255f);
    public bool WinTrigger { get; set; } = false;
    public byte Winner { get; set; } = Byte.MaxValue;

    public static PlayerControl target;

    public override void GlobalInitialize(PlayerControl __instance)
    {
        List<PlayerControl> players = PlayerControl.AllPlayerControls.ToArray().ToList();
        for (int i = players.Count - 1; i >= 0; i--)
        {
            PlayerControl p = players[i];
            if(p.Data.IsDead || p.PlayerId == PlayerControl.LocalPlayer.PlayerId || p.GetModData().role.side != Side.Crewmate) players.Remove(p);
        }
        if(players.Count == 0)
        {
            RPCEventInvoker.ImmediatelyChangeRole(PlayerControl.LocalPlayer,Roles.Opportunist);
            return;
        }
        target = players[NebulaPlugin.rnd.Next(0,players.Count)];
        WinTrigger = false;
    }

    /*
    public override void GlobalIntroInitialize(PlayerControl __instance)
    {
        GlobalInitialize(__instance);
    }
    */

    public override void EditOthersDisplayNameColor(byte playerId, ref Color displayColor)
    {
        if(playerId == target.PlayerId) displayColor = RoleColor;
    }

    /*
    public override void OnMeetingStart(){
        RPCEventInvoker.MultipleVote(PlayerControl.LocalPlayer, 255); //Õ∂∆±≤‚ ‘
    }
    */

    public override void OnExiledPre(byte[] voters, byte playerId)
    {
        if(playerId == target.PlayerId && !PlayerControl.LocalPlayer.Data.IsDead) RPCEventInvoker.WinTrigger(this);
    }

    public override void MyPlayerControlUpdate()
    {
        if(target.Data.IsDead && target.GetModData().Status != Game.PlayerData.PlayerStatus.Exiled) RPCEventInvoker.ImmediatelyChangeRole(PlayerControl.LocalPlayer,Roles.Opportunist);
    }

    public Cascrubinter()
        : base("Cascrubinter","cascrubinter",RoleColor,RoleCategory.Neutral,Side.Cascrubinter,Side.Cascrubinter,
        new HashSet<Side>() { Side.Cascrubinter },new HashSet<Side>() { Side.Cascrubinter },
        new HashSet<Patches.EndCondition>() { Patches.EndCondition.CascrubinterWin },
        true, VentPermission.CanNotUse, true, false, false){
        Patches.EndCondition.CascrubinterWin.TriggerRole = this;
    }
}