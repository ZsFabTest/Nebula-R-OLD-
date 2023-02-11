namespace Nebula.Roles.ComplexRoles;

static public class SwapSystem
{
    public static int swapDataId;
    public static bool isSwapped;
    public static int playerId1;
    public static int playerId2;

    public static void GlobalInitialize(){
        swapDataId = (int)Roles.F_Swapper.swapCount.getFloat();
        isSwapped = false;
        playerId1 = -1;
        playerId2 = -1;
    }

    static void guesserOnClick(int buttonTarget, MeetingHud __instance)
    {
        if (__instance.CurrentState == MeetingHud.VoteStates.Discussion) return;

        PlayerControl target = Helpers.playerById((byte)__instance.playerStates[buttonTarget].TargetPlayerId);
        if (target == null || target.Data.IsDead) return;

        if(playerId1 == target.PlayerId){
            playerId1 = -1;
            return;
        }else if(playerId2 == target.PlayerId){
            playerId2 = -1;
            return;
        }else if (playerId1 == -1){
            playerId1 = target.PlayerId;
            return;
        }else if(playerId2 == -1){
            playerId2 = target.PlayerId;
        }

        if(playerId1 != -1 && playerId2 != -1) isSwapped = true;
        else isSwapped = false;
    }

    public static void OnMeetingStart(){
        playerId1 = -1;
        playerId2 = -1;
        isSwapped = false;
    }

    public static void SetupMeetingButton(MeetingHud __instance)
    {
        if (!PlayerControl.LocalPlayer.Data.IsDead && swapDataId > 0)
        {
            for (int i = 0; i < __instance.playerStates.Length; i++)
            {
                PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                if (playerVoteArea.AmDead) continue;

                GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                GameObject targetBox = UnityEngine.Object.Instantiate(template, playerVoteArea.transform);
                targetBox.name = "ShootButton";
                targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1f);
                SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
                renderer.sprite = ComplexRoles.FSwapper.targetSprite.GetSprite();
                PassiveButton button = targetBox.GetComponent<PassiveButton>();
                button.OnClick.RemoveAllListeners();
                int copiedIndex = i;
                button.OnClick.AddListener((UnityEngine.Events.UnityAction)(() => guesserOnClick(copiedIndex, __instance)));
            }
        }
    }

    public static void MeetingUpdate(MeetingHud __instance, TMPro.TextMeshPro meetingInfo)
    {
        if (swapDataId <= 0) return;
        meetingInfo.text = Language.Language.GetString("role.swapper.swapLeft") + ": " + swapDataId;
        meetingInfo.text += "\n" + Language.Language.GetString("role.swapper.target1") + ": " + (playerId1 != -1 ? Helpers.playerById((byte)playerId1).name : Language.Language.GetString("role.swapper.nobody"));
        meetingInfo.text += "\n" + Language.Language.GetString("role.swapper.target2") + ": " + (playerId2 != -1 ? Helpers.playerById((byte)playerId2).name : Language.Language.GetString("role.swapper.nobody"));
        meetingInfo.gameObject.SetActive(true);
    }
}

public class FSwapper : Template.HasBilateralness
{
    public static Color RoleColor = new Color(140f / 255f,40f / 255f,78f / 255f);

    public Module.CustomOption swapCount;
    public Module.CustomOption swapperCanStartEmergencyMeetingOption;
    public Module.CustomOption swapperCanFixSabotageOption;

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.CrewmateRoles | Module.CustomOptionTab.ImpostorRoles;

        base.LoadOptionData();
        swapCount = CreateOption(Color.white, "swapCount", 3f, 1f, 15f, 1f);
        swapperCanStartEmergencyMeetingOption = CreateOption(Color.white, "swapperCanStartEmergencyMeeting", false);
        swapperCanFixSabotageOption = CreateOption(Color.white, "swapperCanFixSabotage", false);
    }

    public static SpriteLoader targetSprite = new SpriteLoader("Nebula.Resources.SwapperCheck.png", 150f);

    public FSwapper() : base("Swapper", "swapper", RoleColor)
    {
    }
}

public class Swapper : Template.BilateralnessRole
{
    public override RelatedExtraRoleData[] RelatedExtraRoleDataInfo => GuesserSystem.RelatedExtraRoleDataInfo;
    //インポスターはModで操作するFakeTaskは所持していない
    public Swapper(string name, string localizeName, bool isImpostor)
            : base(name, localizeName,
                 isImpostor ? Palette.ImpostorRed : FSwapper.RoleColor,
                 isImpostor ? RoleCategory.Impostor : RoleCategory.Crewmate,
                 isImpostor ? Side.Impostor : Side.Crewmate, isImpostor ? Side.Impostor : Side.Crewmate,
                 isImpostor ? ImpostorRoles.Impostor.impostorSideSet : CrewmateRoles.Crewmate.crewmateSideSet,
                 isImpostor ? ImpostorRoles.Impostor.impostorSideSet : CrewmateRoles.Crewmate.crewmateSideSet,
                 isImpostor ? ImpostorRoles.Impostor.impostorEndSet : CrewmateRoles.Crewmate.crewmateEndSet,
                 false, isImpostor ? VentPermission.CanUseUnlimittedVent : VentPermission.CanNotUse,
                 isImpostor, isImpostor, isImpostor, () => { return Roles.F_Swapper; }, isImpostor)
    {
        IsGuessableRole = false;
        IsHideRole = true;
    }

    public override Assignable AssignableOnHelp => Roles.F_Swapper;
    public override HelpSprite[] helpSprite => Roles.F_Swapper.helpSprite;

    public override void GlobalInitialize(PlayerControl __instance)
    {
        SwapSystem.GlobalInitialize();
        canFixSabotage = Roles.F_Swapper.swapperCanFixSabotageOption.getBool();
        CanCallEmergencyMeeting = Roles.F_Swapper.swapperCanStartEmergencyMeetingOption.getBool();
    }

    public override void SetupMeetingButton(MeetingHud __instance)
    {
        SwapSystem.SetupMeetingButton(__instance);
    }

    public override void MeetingUpdate(MeetingHud __instance, TMPro.TextMeshPro meetingInfo)
    {
        SwapSystem.MeetingUpdate(__instance, meetingInfo);
    }

    public override void OnMeetingStart()
    {
        SwapSystem.OnMeetingStart();
    }
}
