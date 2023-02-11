namespace Nebula.Roles.ComplexRoles;

static public class DecideSystem
{
    public static int decideDataId { get; private set; } = (int)Roles.F_Decider.decideCountOption.getFloat();
    public static bool decideAble { get; private set; } = true;

    public static void GlobalInitialize(){
        decideAble = true;
        decideDataId = (int)Roles.F_Decider.decideCountOption.getFloat();
    }

    static void guesserOnClick(int buttonTarget, MeetingHud __instance)
    {
        if(!decideAble) return;
        if (__instance.CurrentState == MeetingHud.VoteStates.Discussion) return;

        PlayerControl target = Helpers.playerById((byte)__instance.playerStates[buttonTarget].TargetPlayerId);
        if (target == null || target.Data.IsDead) return;

        decideAble = false;
        decideDataId--;
        RPCEventInvoker.UncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId, target.PlayerId, Game.PlayerData.PlayerStatus.Guessed.Id,true);
        RPCEventInvoker.CleanDeadBody(target.PlayerId);
    }

    public static void SetupMeetingButton(MeetingHud __instance)
    {
        if (!PlayerControl.LocalPlayer.Data.IsDead && decideDataId > 0)
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
                renderer.sprite = ComplexRoles.FGuesser.targetSprite.GetSprite();
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
        ulong left = (ulong)decideDataId;
        if (left <= 0) return;
        meetingInfo.text = Language.Language.GetString("role.decider.decideLeft") + ": " + left;
        meetingInfo.gameObject.SetActive(true);
    }
}

public class FDecider : Template.HasBilateralness
{
    public static Color RoleColor = new Color(241f / 255f, 199f / 255f, 31f / 255f);

    public Module.CustomOption decideCountOption;

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.CrewmateRoles | Module.CustomOptionTab.ImpostorRoles;

        base.LoadOptionData();
        decideCountOption = CreateOption(Color.white, "decideCount", 1f, 1f, 3f, 1f);
    }

    public FDecider()
            : base("Decider","decider",RoleColor)
    {
    }
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
        IsGuessableRole = false;
        IsHideRole = true;
        CanCallEmergencyMeeting = false;
    }

    public override Assignable AssignableOnHelp => Roles.F_Decider;
    public override HelpSprite[] helpSprite => Roles.F_Decider.helpSprite;

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

    public override void OnRoleRelationSetting()
    {
        RelatedRoles.Add(Roles.Agent);
        RelatedRoles.Add(Roles.EvilAce);
    }
}