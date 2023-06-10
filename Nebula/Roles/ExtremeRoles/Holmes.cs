namespace Nebula.Roles.CrewmateRoles;

public class Holmes : Role
{

    public static Color RoleColor = new Color(230f / 255f, 190f / 255f, 70f / 255f);

    public static Module.CustomOption isGuessable;
    public static Module.CustomOption surveyCooldownOption;
    public static Module.CustomOption maxSurveyNumberOption;
    public static Module.CustomOption canKnowJobsOption;

    private static CustomButton surveyButton;
    private SpriteLoader surveyButtonSprite = new SpriteLoader("Nebula.Resources.SurveyButton.png", 100f);

    public int surveyDataId { get; private set; }
    public override RelatedRoleData[] RelatedRoleDataInfo
    {
        get => new RelatedRoleData[] {
            new RelatedRoleData(surveyDataId, "Holmes survey", 0, 15), };
    }
    public override void GlobalInitialize(PlayerControl __instance)
    {
        __instance.GetModData().SetRoleData(surveyDataId, 0);
    }

    public override void ButtonInitialize(HudManager __instance)
    {
        if(surveyButton != null)
        {
            surveyButton.Destroy();
        }
        surveyButton = new CustomButton(
            () =>
            {
                byte targetId = Game.GameData.data.myData.currentTarget.PlayerId;
                RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId, surveyDataId, 1);
                Survey(targetId);
                surveyButton.Timer = surveyButton.MaxTimer;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.GetModData().GetRoleData(surveyDataId) < maxSurveyNumberOption.getFloat(); },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { surveyButton.Timer = surveyButton.MaxTimer;},
            surveyButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.survey"
            ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
            surveyButton.MaxTimer = surveyCooldownOption.getFloat();
    }

    public override void CleanUp()
    {
        if(surveyButton != null)
        {
            surveyButton.Destroy();
            surveyButton = null;
        }
    }

    public override void LoadOptionData()
    {
        isGuessable = CreateOption(Color.white, "isGuessable", true);
        surveyCooldownOption = CreateOption(Color.white, "surveyCooldown", 30f, 15f, 45f, 2.5f);
        maxSurveyNumberOption = CreateOption(Color.white, "maxSurveyNumber", 2f, 1f, 15f, 1f);
        canKnowJobsOption = CreateOption(Color.white, "canKnowJobs", true);
    }

    private void Survey(byte playerId)
    {
        PlayerControl p = Helpers.playerById(playerId);
        var data = p.GetModData();
        data.RoleInfo = Helpers.cs(checkColor(data.role.GetActualRole(data)),canKnowJobsOption.getBool() ? Language.Language.GetString("role." + data.role.GetActualRole(data).LocalizeName + ".name") : "???");
    }

    private Color checkColor(Role operRole)
    {
        if (canKnowJobsOption.getBool()) return operRole.Color;
        if (operRole.side == Side.Impostor) return Palette.ImpostorRed;
        else if (operRole.side == Side.Jackal) return Roles.Jackal.Color;
        else if (operRole.side == Side.Crewmate) return Palette.CrewmateBlue;
        else return Roles.ChainShifter.Color;
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(
            (player) =>
            {
                if (player.Object.inVent) return false;
                return true;
            });
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color.yellow);
    }

    public Holmes()
        : base("Holmes", "holmes", RoleColor, RoleCategory.Crewmate, Side.Crewmate, Side.Crewmate,
             Crewmate.crewmateSideSet, Crewmate.crewmateSideSet, Crewmate.crewmateEndSet,
             false, VentPermission.CanNotUse, false, false, false)
    {
        surveyButton = null;
        surveyDataId = Game.GameData.RegisterRoleDataId("holmes.survey");
    }
}
