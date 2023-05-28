namespace Nebula.Roles.ComplexRoles;

public class FMover : Template.HasBilateralness
{
    public static Color RoleColor = new Color(10f / 255f, 4f / 255f, 106f / 255f);

    public Module.CustomOption cooldownOption;
    public Module.CustomOption numOfMovingOption;
    public Module.CustomOption movingDuringOption;

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;

        base.LoadOptionData();
        cooldownOption = CreateOption(Color.white,"cooldown",15f,0f,45f,2.5f);
        cooldownOption.suffix = "second";
        numOfMovingOption = CreateOption(Color.white,"numofMoving",5f,1f,15f,1f);
        movingDuringOption = CreateOption(Color.white,"movingDuring",5f,1f,30f,1f);
        movingDuringOption.suffix = "second";

        FirstRole = Roles.NiceMover;
        SecondaryRole = Roles.EvilMover;
    }

    public FMover()
            : base("Mover","mover",RoleColor)
    {
    }

    public override List<Role> GetImplicateRoles() { return new List<Role>() { Roles.EvilMover, Roles.NiceMover }; }
}

public class Mover : Template.BilateralnessRole
{
    //インポスターはModで操作するFakeTaskは所持していない
    public Mover(string name, string localizeName, bool isImpostor)
            : base(name, localizeName,
                 isImpostor ? Palette.ImpostorRed : FMover.RoleColor,
                 isImpostor ? RoleCategory.Impostor : RoleCategory.Crewmate,
                 isImpostor ? Side.Impostor : Side.Crewmate, isImpostor ? Side.Impostor : Side.Crewmate,
                 isImpostor ? ImpostorRoles.Impostor.impostorSideSet : CrewmateRoles.Crewmate.crewmateSideSet,
                 isImpostor ? ImpostorRoles.Impostor.impostorSideSet : CrewmateRoles.Crewmate.crewmateSideSet,
                 isImpostor ? ImpostorRoles.Impostor.impostorEndSet : CrewmateRoles.Crewmate.crewmateEndSet,
                 false, isImpostor ? VentPermission.CanUseUnlimittedVent : VentPermission.CanNotUse,
                 isImpostor, isImpostor, isImpostor, () => { return Roles.F_Mover; }, isImpostor)
    {
        IsHideRole = true;
        moveDataId = 5;
        target = null;
        move = null;
    }

    public int moveDataId;
    public Console target;
    public static Console pickUpTarget;
    private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.RepairButton.png",115f);

    public override void GlobalInitialize(PlayerControl __instance)
    {
        moveDataId = (int)Roles.F_Mover.numOfMovingOption.getFloat();
        target = null;
    }

    public override Assignable AssignableOnHelp => Roles.F_Mover;
    public override HelpSprite[] helpSprite => Roles.F_Mover.helpSprite;

    private CustomButton move;
    public static bool isMoving;
    public override void ButtonInitialize(HudManager __instance)
    {
        if(move != null){
            move.Destroy();
        }
        move = new CustomButton(
            () => {
                pickUpTarget = target;
                Debug.LogWarning(pickUpTarget.ConsoleId.ToString());
                RPCEventInvoker.SetConsoleStatus(pickUpTarget,false,PlayerControl.LocalPlayer.transform.position);
                target = null;
                move.SetLabel("button.label.mover.put");
                isMoving = true;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && (target || isMoving) && moveDataId > 0; },
            () => { move.Timer = move.MaxTimer; },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            true,
            Roles.F_Mover.movingDuringOption.getFloat(),
            () => { OnMeetingStart(); },
            "button.label.mover.move"
        ).SetTimer(CustomOptionHolder.InitialAbilityCoolDownOption.getFloat());
        move.MaxTimer = Roles.F_Mover.cooldownOption.getFloat();
        move.UsesText.text = moveDataId.ToString();

        move.SetSuspendAction(() =>
        {
            OnMeetingStart();
        });
    }

    public override void MyPlayerControlUpdate()
    {
        target = GetConsole();
    }

    private static Console GetConsole()
    {
        return ShipStatus.Instance.AllConsoles.FirstOrDefault(x =>
        {
            if (x == null) return false;
            float num = Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), x.transform.position);
            return num <= x.usableDistance;
        });
    }

    public override void CleanUp(){
        if(move != null){
            move.Destroy();
            move = null;
        }
    }

    public override void OnMeetingStart()
    {
        RPCEventInvoker.SetConsoleStatus(pickUpTarget,true,PlayerControl.LocalPlayer.transform.position);
        isMoving = false;
        move.SetLabel("button.label.mover.move");
        move.Timer = move.MaxTimer;
        move.isEffectActive = false;
        move.actionButton.cooldownTimerText.color = Palette.EnabledColor;
        moveDataId--;
        move.UsesText.text = moveDataId.ToString();
    }
}