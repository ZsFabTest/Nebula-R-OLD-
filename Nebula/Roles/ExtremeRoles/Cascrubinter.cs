namespace Nebula.Roles.NeutralRoles;

public class Cascrubinter : Role,Template.HasWinTrigger{
    public static Color RoleColor = new Color(99f / 255f,31f / 255f,12f / 255f);
    public bool WinTrigger { get; set; } = false;
    public byte Winner { get; set; } = Byte.MaxValue;

    private Module.CustomOption changeRoleAfterTargetDiedOption;
    public static PlayerControl target;

    public override void LoadOptionData(){
        changeRoleAfterTargetDiedOption = CreateOption(Color.white,"changeRoleAfterTargetDied",new string[] { "role.cascrubinter.keep","role.cascrubinter.toOpportunist","role.cascrubinter.toAmnesiac" });
    }
    public override void OnAnyoneDied(byte playerId){
        if(playerId == target.PlayerId){
            switch(changeRoleAfterTargetDiedOption.getSelection()){
                case 1:
                    RPCEventInvoker.ImmediatelyChangeRole(PlayerControl.LocalPlayer,Roles.Opportunist);
                    break;
                case 2:
                    RPCEventInvoker.ImmediatelyChangeRole(PlayerControl.LocalPlayer,Roles.Amnesiac);
                    break;
            }
        }
    }

    public override void GlobalInitialize(PlayerControl __instance){
        WinTrigger = false;
    }

    public override void Initialize(PlayerControl __instance)
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
        RPCEventInvoker.MultipleVote(PlayerControl.LocalPlayer, 255); //ͶƱ����
    }
    */

    public override void OnExiledPre(byte[] voters, byte playerId)
    {
        if(playerId == target.PlayerId && !PlayerControl.LocalPlayer.Data.IsDead) RPCEventInvoker.WinTrigger(this);
    }

    public Cascrubinter()
        : base("Cascrubinter","cascrubinter",RoleColor,RoleCategory.Neutral,Side.Cascrubinter,Side.Cascrubinter,
        new HashSet<Side>() { Side.Cascrubinter },new HashSet<Side>() { Side.Cascrubinter },
        new HashSet<Patches.EndCondition>() { Patches.EndCondition.CascrubinterWin },
        true, VentPermission.CanNotUse, true, false, false){
        Patches.EndCondition.CascrubinterWin.TriggerRole = this;
    }
}