namespace Nebula.Roles.ComplexRoles;

static public class DecideSystem
{
    public static int decideDataId { get; private set; } = (int)Roles.F_Decider.decideCountOption.getFloat();
    public static bool decideAble { get; private set; } = true;

    public static void GlobalInitialize(PlayerControl __instance)
    {
        decideAble = true;
        __instance.GetModData().SetRoleData(decideDataId, (int)Roles.F_Decider.decideCountOption.getFloat());
    }

    static void guesserOnClick(int buttonTarget, MeetingHud __instance)
    {
        if(!decideAble) return;
        if (__instance.CurrentState == MeetingHud.VoteStates.Discussion) return;

        PlayerControl target = Helpers.playerById((byte)__instance.playerStates[buttonTarget].TargetPlayerId);
        if (target == null || target.Data.IsDead) return;

        decideAble = false;
        int data = Game.GameData.data.myData.getGlobalData().GetRoleData(decideDataId);
        data--;
        RPCEventInvoker.UpdateRoleData(PlayerControl.LocalPlayer.PlayerId, decideDataId, data);
        if(PlayerControl.LocalPlayer.GetModData().role == Roles.NiceDecider && (
            (
            FDecider.niceDeciderCannotKillCrewmateOption.getBool() && 
            target.GetModData().role.side == Side.Crewmate && 
            target.GetModData().role != Roles.Madmate && !target.GetModData().extraRole.Contains(Roles.SecondaryMadmate)
            ) && !(
            PlayerControl.LocalPlayer.GetModData().HasExtraRole(Roles.SecondaryMadmate) &&
            FDecider.madmateCanKillEveryoneOption.getBool()
            ) && !(
            PlayerControl.LocalPlayer.GetModData().HasExtraRole(Roles.SecondaryJackal) &&
            FDecider.jackalCanKillEveryoneOption.getBool()
            )
        )
        ){
            RPCEventInvoker.Guess(PlayerControl.LocalPlayer.PlayerId);
        }
        else{
            RPCEventInvoker.Guess(target.PlayerId);
        }
        __instance.playerStates.ToList().ForEach(x => { if (x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
    }

    public static void SetupMeetingButton(MeetingHud __instance)
    {
        if (!PlayerControl.LocalPlayer.Data.IsDead && Game.GameData.data.myData.getGlobalData().GetRoleData(decideDataId) > 0)
        {
            for (int i = 0; i < __instance.playerStates.Length; i++)
            {
                PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                if (playerVoteArea.AmDead || playerVoteArea.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

                GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                GameObject targetBox = UnityEngine.Object.Instantiate(template, playerVoteArea.transform);
                targetBox.name = "ShootButton";
                targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1f);
                SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
                renderer.sprite = FGuesser.targetSprite.GetSprite();
                PassiveButton button = targetBox.GetComponent<PassiveButton>();
                button.OnClick.RemoveAllListeners();
                int copiedIndex = i;
                button.OnClick.AddListener((UnityEngine.Events.UnityAction)(() => guesserOnClick(copiedIndex, __instance)));
            }
        }
    }

    public static void OnMeetingStart(){
        decideAble = true;
    }

    public static void MeetingUpdate(MeetingHud __instance, TMPro.TextMeshPro meetingInfo)
    {
        int left = Game.GameData.data.myData.getGlobalData().GetRoleData(decideDataId);
        if (left <= 0) return;
        meetingInfo.text = Language.Language.GetString("role.decider.decideLeft") + ": " + left;
        meetingInfo.gameObject.SetActive(true);
    }
}

public class FDecider : Template.HasBilateralness
{
    public static Color RoleColor = new Color(241f / 255f, 199f / 255f, 31f / 255f);

    public Module.CustomOption decideCountOption;
    public static Module.CustomOption niceDeciderCannotKillCrewmateOption;
    public static Module.CustomOption madmateCanKillEveryoneOption;
    public static Module.CustomOption jackalCanKillEveryoneOption;

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;

        base.LoadOptionData();
        decideCountOption = CreateOption(Color.white, "decideCount", 1f, 1f, 3f, 1f);
        niceDeciderCannotKillCrewmateOption = CreateOption(Color.white, "niceDeciderCannotKillCrewmate",true);
        madmateCanKillEveryoneOption = CreateOption(Color.white,"madmateCanKillEveryone",false);
        jackalCanKillEveryoneOption = CreateOption(Color.white,"jackalCanKillEveryone",true);

        FirstRole = Roles.NiceDecider;
        SecondaryRole = Roles.EvilDecider;
    }

    public FDecider()
            : base("Decider","decider",RoleColor)
    {
    }

    public override List<Role> GetImplicateRoles() { return new List<Role>() { Roles.EvilDecider, Roles.NiceDecider }; }
}

public class Decider : Template.BilateralnessRole
{
    //インポスターはModで操作するFakeTaskは所持していない
    public Decider(string name, string localizeName, bool isImpostor)
            : base(name, localizeName,
                 isImpostor ? Palette.ImpostorRed : FDecider.RoleColor,
                 isImpostor ? RoleCategory.Impostor : RoleCategory.Crewmate,
                 isImpostor ? Side.Impostor : Side.Crewmate, isImpostor ? Side.Impostor : Side.Crewmate,
                 isImpostor ? ImpostorRoles.Impostor.impostorSideSet : CrewmateRoles.Crewmate.crewmateSideSet,
                 isImpostor ? ImpostorRoles.Impostor.impostorSideSet : CrewmateRoles.Crewmate.crewmateSideSet,
                 isImpostor ? ImpostorRoles.Impostor.impostorEndSet : CrewmateRoles.Crewmate.crewmateEndSet,
                 false, isImpostor ? VentPermission.CanUseUnlimittedVent : VentPermission.CanNotUse,
                 isImpostor, isImpostor, isImpostor, () => { return Roles.F_Decider; }, isImpostor)
    {
        IsHideRole = true;
        CanCallEmergencyMeeting = false;
    }

    public override Assignable AssignableOnHelp => Roles.F_Decider;
    public override HelpSprite[] helpSprite => Roles.F_Decider.helpSprite;

    public override void GlobalInitialize(PlayerControl __instance)
    {
        DecideSystem.GlobalInitialize(__instance);
    }

    public override void SetupMeetingButton(MeetingHud __instance)
    {
        DecideSystem.SetupMeetingButton(__instance);
    }

    public override void MeetingUpdate(MeetingHud __instance, TMPro.TextMeshPro meetingInfo)
    {
        DecideSystem.MeetingUpdate(__instance, meetingInfo);
    }

    public override void OnMeetingStart()
    {
        base.OnMeetingStart();
        DecideSystem.OnMeetingStart();
    }
}