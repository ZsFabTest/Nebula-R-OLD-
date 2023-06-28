namespace Nebula.Roles.NeutralRoles;

public class Amnesiac : Role{
    public class AmnesiacEvent : Events.LocalEvent{
        PlayerControl target;
        public AmnesiacEvent(byte targetId) : base(0.1f) { target = Helpers.playerById(targetId); }
        public override void OnActivate()
        {
            switch (Amnesiac.targetsRoleModeOption.getSelection()){
                case 0:
                    RPCEventInvoker.ImmediatelyChangeRole(PlayerControl.LocalPlayer,target.GetModData().role);
                    break;
                case 1:
                    RPCEventInvoker.ImmediatelyChangeRole(PlayerControl.LocalPlayer,target.GetModData().role);
                    if(target.GetModData().role.side == Side.Crewmate) RPCEventInvoker.ImmediatelyChangeRole(target,Roles.Crewmate);
                    else if(target.GetModData().role.side == Side.Impostor) RPCEventInvoker.ImmediatelyChangeRole(target,Roles.Impostor);
                    else RPCEventInvoker.ImmediatelyChangeRole(target,Roles.Opportunist);
                    break;
                case 2:
                    RPCEventInvoker.SwapRole(PlayerControl.LocalPlayer,target);
                    break;
                case 3:
                    RPCEventInvoker.ImmediatelyChangeRole(PlayerControl.LocalPlayer,target.GetModData().role);
                    RPCEventInvoker.ImmediatelyChangeRole(target,Roles.Opportunist);
                    break;
            }
        }
    }

    static public Color RoleColor = new Color(210f / 255f, 220f / 255f, 234f / 255f);

    public byte deadBodyId;
    private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.PoltergeistButton.png", 115f);
    public static Module.CustomOption targetsRoleModeOption;
    private Module.CustomOption rememberCoolDownOption;

    public override void LoadOptionData()
    {
        targetsRoleModeOption = CreateOption(Color.white,"targetsRoleMode",new string[] { "role.amnesiac.targetsRoleMode.dontShift","role.amnesiac.targetsRoleMode.erase","role.amnesiac.targetsRoleMode.toAmnesiac","role.amnesiac.targetsRoleMode.toOpportunist" });
        rememberCoolDownOption = CreateOption(Color.white,"rememberCoolDown",27.5f,15f,40f,2.5f);
        rememberCoolDownOption.suffix = "second";
    }

    public override void MyPlayerControlUpdate()
    {
        if (PlayerControl.LocalPlayer.Data.IsDead) return;

        /* 捕食対象の探索 */

        {
            DeadBody body = Patches.PlayerControlPatch.SetMyDeadTarget();
            if (body)
            {
                deadBodyId = body.ParentId;
            }
            else
            {
                deadBodyId = byte.MaxValue;
            }
            Patches.PlayerControlPatch.SetDeadBodyOutline(body, Color.yellow);
        }
    }

    private CustomButton remember;
    public override void ButtonInitialize(HudManager __instance){
        if(remember != null){
            remember.Destroy();
        }
        remember = new CustomButton(
            () =>
            {
                byte targetId = deadBodyId;
                Events.LocalEvent.Activate(new AmnesiacEvent(targetId));
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return deadBodyId != Byte.MaxValue && PlayerControl.LocalPlayer.CanMove; },
            () => { remember.Timer = remember.MaxTimer; },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.remember"
        ).SetTimer(CustomOptionHolder.InitialAbilityCoolDownOption.getFloat());
        remember.MaxTimer = rememberCoolDownOption.getFloat();    
    }

    public override void CleanUp(){
        if(remember != null){
            remember.Destroy();
            remember = null;
        }
    }

    public Amnesiac()
        : base("Amnesiac","amnesiac",RoleColor,RoleCategory.Neutral,Side.Amnesiac,Side.Amnesiac,
        new HashSet<Side>() { Side.Amnesiac },new HashSet<Side>() { Side.Amnesiac },
        new HashSet<Patches.EndCondition>() {},
        true,VentPermission.CanNotUse,false,false,false){
        remember = null;
    }
}