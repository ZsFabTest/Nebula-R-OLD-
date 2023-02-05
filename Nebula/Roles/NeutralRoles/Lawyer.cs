using Nebula.Events;
using Nebula.Patches;
using Nebula.Roles.CrewmateRoles;

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
    public class PlaintiffEvent : LocalEvent
    {
        PlayerControl target;
        public PlaintiffEvent(PlayerControl target) : base(0.2f)
        {
            this.target = target;
        }
        public override void OnTerminal()
        {
            RPCEventInvoker.UnsetExtraRole(target, Roles.Client, false);
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

    public virtual bool PrivateCheckWin(PlayerControl player, Patches.EndCondition winReason)
    {
        //Madmateの場合は元陣営の勝利を無効化する
        if (player.IsMadmate()) return false;

        //単独勝利ロール
        if (winReason.TriggerRole != null)
            return winReason.TriggerRole.Winner == player.PlayerId;


        return winReasons.Contains(winReason);
    }

    public override bool CheckAdditionalWin(PlayerControl player, EndCondition condition)
    {
        if (condition == EndCondition.NoGame) return false;
        if (condition == EndCondition.NobodyWin) return false;
        if (condition == EndCondition.NobodySkeldWin) return false;
        if (condition == EndCondition.NobodyMiraWin) return false;
        if (condition == EndCondition.NobodyPolusWin) return false;
        if (condition == EndCondition.NobodyAirshipWin) return false;

        foreach (PlayerControl playerC in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if(!playerC.GetModData().IsAlive) continue;
            foreach(ExtraRole extraRole in playerC.GetModData().extraRole)
            {
                if(extraRole.id == Roles.Client.id)
                {
                    if (PrivateCheckWin(playerC, condition)) return true;
                    return false;
                }
            }
        }
        return false;
    }

    public override void EditDisplayNameColor(byte playerId, ref Color displayColor)
    {
        foreach(ExtraRole extraRole in Helpers.playerById(playerId).GetModData().extraRole)
        {
            if (extraRole.id == Roles.Client.id)
            {
                displayColor = RoleColor;
                return;
            }
        }
    }

    /*
    foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
    */

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

    public Plaintiff()
        : base("Plaintiff", "plaintiff", Lawyer.RoleColor, RoleCategory.Neutral, Side.Opportunist, Side.Opportunist,
             new HashSet<Side>() { Side.Opportunist }, new HashSet<Side>() { Side.Opportunist },
             new HashSet<EndCondition>(),
             true, VentPermission.CanNotUse, false, false, false)
    {
        IsHideRole = true;
    }
}
