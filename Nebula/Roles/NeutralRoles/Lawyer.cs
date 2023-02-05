using Nebula.Events;
using Nebula.Patches;

namespace Nebula.Roles.NeutralRoles;

public class Lawyer : Role
{
    public class LawyerEvent : LocalEvent
    {
        PlayerControl lawyer;
        public LawyerEvent(PlayerControl target) : base(0.2f) { lawyer = target; }
        public override void OnTerminal()
        {
            RPCEventInvoker.ImmediatelyChangeRole(lawyer,Roles.Plaintiff);
            //绿尸寒警告
        }
    }

    public static Color RoleColor = new Color(133f / 255f, 152f / 255f, 25f / 255f);

    public static Module.CustomOption lawyerCanChangeToPlaintiff;
    public static Module.CustomOption lawyerDieWithClient;
    public static Module.CustomOption clientCanKnowLawyer;

    public override void LoadOptionData()
    {
        lawyerCanChangeToPlaintiff = CreateOption(Color.white, "lawyerCanChangeToPlaintiff", true);
        lawyerDieWithClient = CreateOption(Color.white, "lawyerDieWithClient", false).AddInvPrerequisite(lawyerCanChangeToPlaintiff);
        clientCanKnowLawyer = CreateOption(RoleColor, "clientCanKnowLawyer", false);
    }

    public override bool CheckAdditionalWin(PlayerControl player, EndCondition condition)
    {
        if (condition == EndCondition.NoGame) return false;
        if (condition == EndCondition.NobodyWin) return false;
        if (condition == EndCondition.NobodySkeldWin) return false;
        if (condition == EndCondition.NobodyMiraWin) return false;
        if (condition == EndCondition.NobodyPolusWin) return false;
        if (condition == EndCondition.NobodyAirshipWin) return false;

        if (player.Data.IsDead && player.GetModData().FinalData?.status != Game.PlayerData.PlayerStatus.Burned) return false;

        foreach (PlayerControl playerC in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if(!playerC.GetModData().IsAlive) continue;
            foreach(ExtraRole extraRole in player.GetModData().extraRole)
            {
                if(extraRole == Roles.Client)
                {
                    if (playerC.GetModData().role.CheckWin(playerC, condition)) return true;
                }
            }
        }
        return false;
    }

    /*
    foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
    */

    public override void OnDied(byte playerId)
    {
        foreach(ExtraRole extraRole in PlayerControl.LocalPlayer.GetModData().extraRole)
        {
            if (extraRole == Roles.Client)
            {
                RPCEventInvoker.UnsetExtraRole(Helpers.playerById(playerId), Roles.Client, false);
                break;
            }
        }
    }

    public Lawyer()
        : base("Lawyer", "Lawyer", RoleColor, RoleCategory.Neutral, Side.Lawyer, Side.Lawyer,
             new HashSet<Side>() { Side.Lawyer }, new HashSet<Side>() { Side.Lawyer },
             new HashSet<EndCondition>() { },
             true, VentPermission.CanNotUse, false, false, false)
    {
    }
}

public class Plaintiff : Role
{
    public override bool CheckAdditionalWin(PlayerControl player, EndCondition condition)
    {
        if (condition == EndCondition.NoGame) return false;
        if (condition == EndCondition.NobodySkeldWin) return false;
        if (condition == EndCondition.NobodyMiraWin) return false;
        if (condition == EndCondition.NobodyPolusWin) return false;
        if (condition == EndCondition.NobodyAirshipWin) return false;

        if (player.Data.IsDead && player.GetModData().FinalData?.status != Game.PlayerData.PlayerStatus.Burned) return false;

        return true;
    }

    public override bool IsSpawnable()
    {
        return false;
    }

    public Plaintiff()
        : base("Plaintiff", "plaintiff", Lawyer.RoleColor, RoleCategory.Neutral, Side.Opportunist, Side.Opportunist,
             new HashSet<Side>() { Side.Opportunist }, new HashSet<Side>() { Side.Opportunist },
             new HashSet<EndCondition>(),
             true, VentPermission.CanNotUse, false, false, false)
    {
        IsHideRole = true;
    }
}
