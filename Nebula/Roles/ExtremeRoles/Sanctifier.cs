namespace Nebula.Roles.CrewmateRoles;
using NeutralRoles;

public class Sanctifier : Role
{
    public static Color RoleColor = new Color(135f / 255f, 225f / 255f, 140f / 255f);

    public static CustomButton cleanButton;

    public static Module.CustomOption canUnsetMadmateOption;
    public static Module.CustomOption canUnsetGuesserOption;
    public static Module.CustomOption canUnsetSidesickOption;
    public static Module.CustomOption maxUnsetNumberOption;
    private Module.CustomOption cooldown;

    private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.EraseButton.png", 115f);

    public int cleanDataId { get; private set; }
    public override RelatedRoleData[] RelatedRoleDataInfo
    {
        get => new RelatedRoleData[] {
            new RelatedRoleData(cleanDataId, "Sanctifier Clean", 0, 15) };
    }
    public override void GlobalInitialize(PlayerControl __instance)
    {
        __instance.GetModData().SetRoleData(cleanDataId, 0);
    }

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
        canUnsetMadmateOption = CreateOption(Palette.ImpostorRed, "canUnsetMadmate", true);
        canUnsetGuesserOption = CreateOption(Roles.NiceGuesser.Color, "canUnsetGuesser", true).AddPrerequisite(Roles.F_Guesser.secondoryRoleOption);
        canUnsetSidesickOption = CreateOption(Roles.Jackal.Color, "canUnsetSidekick", true).AddInvPrerequisite(Sidekick.SidekickTakeOverOriginalRoleOption);
        maxUnsetNumberOption = CreateOption(Color.white, "maxUnsetNumber", 1f, 1f, 5f, 1f);
        cooldown = CreateOption(Color.white,"cooldown",25f,2.5f,50f,2.5f);
        cooldown.suffix = "second";
    }

    public override void ButtonInitialize(HudManager __instance)
    {
        if(cleanButton != null)
        {
            cleanButton.Destroy();
        }
        cleanButton = new CustomButton(
            () =>
            {
                PlayerControl target = Game.GameData.data.myData.currentTarget;
                try{
                    RPCEventInvoker.UnsetExtraRole(target, Roles.Drunk, true);
                    RPCEventInvoker.UnsetExtraRole(target, Roles.Bloody, true);
                    RPCEventInvoker.UnsetExtraRole(target, Roles.Confused, true);
                    RPCEventInvoker.UnsetExtraRole(target, Roles.Flash, true);
                    RPCEventInvoker.UnsetExtraRole(target, Roles.SecondaryJackal, true);
                    RPCEventInvoker.UnsetExtraRole(target, Roles.SecondaryBait, true);
                    RPCEventInvoker.UnsetExtraRole(target,Roles.Cheater,true);
                    if(canUnsetMadmateOption.getBool()) RPCEventInvoker.UnsetExtraRole(target, Roles.SecondaryMadmate, true);
                    if(canUnsetGuesserOption.getBool()) RPCEventInvoker.UnsetExtraRole(target, Roles.SecondaryGuesser, true);
                    if(canUnsetSidesickOption.getBool()) RPCEventInvoker.UnsetExtraRole(target, Roles.SecondarySidekick, true);
                    if(canUnsetMadmateOption.getBool() && target.GetModData().role == Roles.Madmate) RPCEventInvoker.ChangeRole(target, Roles.Crewmate);
                }catch(Exception e){ Debug.LogError(e.StackTrace); }
                RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId, cleanDataId, 1);
                cleanButton.Timer = cleanButton.MaxTimer;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.GetModData().GetRoleData(cleanDataId) < maxUnsetNumberOption.getFloat(); },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { cleanButton.Timer = cleanButton.MaxTimer; },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.sanctify"
        ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
        cleanButton.MaxTimer = cooldown.getFloat();
    }

    public override void CleanUp()
    {
        if(cleanButton != null)
        {
            cleanButton.Destroy();
        }
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(
            (player) =>
            {
                return true;
            });
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color.yellow);
    }

    public Sanctifier()
        : base("Sanctifier", "sanctifier", RoleColor, RoleCategory.Crewmate, Side.Crewmate, Side.Crewmate,
             Crewmate.crewmateSideSet, Crewmate.crewmateSideSet, Crewmate.crewmateEndSet,
             false, VentPermission.CanNotUse, false, false, false)
    {
        cleanButton = null;
        cleanDataId = Game.GameData.RegisterRoleDataId("sanctifier.clean");
    }
}


