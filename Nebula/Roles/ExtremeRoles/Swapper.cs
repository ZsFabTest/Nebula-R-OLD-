namespace Nebula.Roles.ComplexRoles;

public static class SwapSystem{
    public static int swapDataId { get; private set; } = (int)Roles.F_Swapper.swapCountOption.getFloat();
    public static int swapTargetf = Byte.MaxValue;
    public static int swapTargets = Byte.MaxValue;
    public static bool isSwapped = false;

    public static void GlobalInitialize(PlayerControl __instance)
    {
        swapTargetf = Byte.MaxValue;swapTargets = Byte.MaxValue;isSwapped = false;
        __instance.GetModData().SetRoleData(swapDataId, (int)Roles.F_Swapper.swapCountOption.getFloat());
    }

    public static void OnMeetingStart(){ swapTargetf = Byte.MaxValue; swapTargets = Byte.MaxValue; isSwapped = false; }

    static void guesserOnClick(int buttonTarget, MeetingHud __instance)
    {
        if (__instance.CurrentState == MeetingHud.VoteStates.Discussion) return;
        PlayerControl target = Helpers.playerById((byte)__instance.playerStates[buttonTarget].TargetPlayerId);
        if (target == null || target.Data.IsDead) return;

        if(swapTargetf == target.PlayerId){
            swapTargetf = Byte.MaxValue;
            if(swapTargets != Byte.MaxValue){
                swapTargetf = swapTargets;
                swapTargets = Byte.MaxValue;
            }
        }else if(swapTargets == target.PlayerId){
            swapTargets = Byte.MaxValue;
        }else{
            if(swapTargetf != Byte.MaxValue) swapTargets = target.PlayerId;
            else swapTargetf = target.PlayerId;
        }

        if(swapTargetf is Byte.MaxValue || swapTargets is Byte.MaxValue) isSwapped = false;
        else isSwapped = true;
    }

    public static void OnMeetingEnd(){
        if(swapTargetf is Byte.MaxValue || swapTargets is Byte.MaxValue) return;

        int data = Game.GameData.data.myData.getGlobalData().GetRoleData(swapDataId);
        data--;
        RPCEventInvoker.UpdateRoleData(PlayerControl.LocalPlayer.PlayerId, swapDataId, data);
    }

    public static void SetupMeetingButton(MeetingHud __instance)
    {
        if (!PlayerControl.LocalPlayer.Data.IsDead && Game.GameData.data.myData.getGlobalData().GetRoleData(swapDataId) > 0)
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
                renderer.sprite = FSwapper.targetSprite.GetSprite();
                PassiveButton button = targetBox.GetComponent<PassiveButton>();
                button.OnClick.RemoveAllListeners();
                int copiedIndex = i;
                button.OnClick.AddListener((UnityEngine.Events.UnityAction)(() => guesserOnClick(copiedIndex, __instance)));
            }
        }
    }

    public static void MeetingUpdate(MeetingHud __instance, TMPro.TextMeshPro meetingInfo)
    {
        int left = Game.GameData.data.myData.getGlobalData().GetRoleData(swapDataId);
        if (left <= 0) return;
        meetingInfo.text = Language.Language.GetString("role.swapper.swapLeft") + ": " + left;
        meetingInfo.text += "\n" + Language.Language.GetString("role.swapper.target1") + ": " + (swapTargetf == Byte.MaxValue ? Language.Language.GetString("role.swapper.nobody") : Helpers.playerById((byte)swapTargetf).name);
        meetingInfo.text += "\n" + Language.Language.GetString("role.swapper.target2") + ": " + (swapTargets == Byte.MaxValue ? Language.Language.GetString("role.swapper.nobody") : Helpers.playerById((byte)swapTargets).name);
        meetingInfo.gameObject.SetActive(true);
    }
}

public class FSwapper : Template.HasBilateralness
{
    public static Color RoleColor = new Color(128f / 255f, 36f / 255f, 52f / 255f);

    public Module.CustomOption swapCountOption;
    public static Module.CustomOption canCallEmergencyMeetingOption;

    public static SpriteLoader targetSprite = new SpriteLoader("Nebula.Resources.SwapIcon.png", 150f);

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;

        base.LoadOptionData();
        swapCountOption = CreateOption(Color.white, "swapCount", 3f, 1f, 15f, 1f);
        canCallEmergencyMeetingOption = CreateOption(Color.white, "swapperCanCallEmergencyMeeting", false);

        FirstRole = Roles.NiceSwapper;
        SecondaryRole = Roles.EvilSwapper;
    }

    public FSwapper()
            : base("Swapper","swapper",RoleColor)
    {
    }

    public override List<Role> GetImplicateRoles() { return new List<Role>() { Roles.EvilSwapper, Roles.NiceSwapper }; }
}

public class Swapper : Template.BilateralnessRole
{
    public Swapper(string name, string localizeName, bool isImpostor)
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
        CanCallEmergencyMeeting = FSwapper.canCallEmergencyMeetingOption.getBool();
        SwapSystem.GlobalInitialize(__instance);
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

    public override void OnMeetingEnd(){
        SwapSystem.OnMeetingEnd();
    }
}