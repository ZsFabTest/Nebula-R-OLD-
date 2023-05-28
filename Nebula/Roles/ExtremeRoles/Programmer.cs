namespace Nebula.Roles.CrewmateRoles;

public class Programmer : Role{
    public byte targetId = Byte.MaxValue;
    private bool isSet;
    private Arrow? Arrow = null;

    private Module.CustomOption hasArrowOption;

    private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.RepairButton.png", 115f);

    public override void GlobalInitialize(PlayerControl __instance)
    {
        targetId = Byte.MaxValue;
        isSet = false;
    }

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.GhostRoles;
        hasArrowOption = CreateOption(Color.white,"hasArrow",false);
    }

    private CustomButton changeButton;
    public override void ButtonInitialize(HudManager __instance)
    {
        if(changeButton != null){
            changeButton.Destroy();
        }
        changeButton = new CustomButton(
            () => {
                targetId = Game.GameData.data.myData.currentTarget.PlayerId;
                Game.GameData.data.myData.currentTarget = null;
                isSet = true;
                if(hasArrowOption.getBool()){
                    Arrow = new Arrow(Color);
                    Arrow.arrow.SetActive(true);
                }
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && !isSet; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { changeButton.Timer = 0; },
            buttonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.change"
        );
        changeButton.Timer = changeButton.MaxTimer = 0;
    }

    public override void CleanUp(){
        if(changeButton != null){
            changeButton.Destroy();
            changeButton = null;
        }
        if(Arrow != null){
            UnityEngine.Object.Destroy(Arrow.arrow);
            Arrow = null;
        }
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Color);
        if(Arrow != null){
            Arrow.Update(Helpers.playerById(targetId).transform.position);
        }
    }

    public override void OnAnyoneGuarded(byte murderId, byte targetId)
    {
        if(targetId == this.targetId){
            Helpers.PlayQuickFlash(Color);
            this.targetId = murderId;
        }
    }

    public override void EditOthersDisplayNameColor(byte playerId, ref Color displayColor)
    {
        if(playerId == targetId) displayColor = Palette.ImpostorRed;
    }

    public Programmer()
         : base("Programmer","programmer",Palette.CrewmateBlue,RoleCategory.Crewmate,Side.Crewmate,Side.Crewmate,
                Crewmate.crewmateSideSet,Crewmate.crewmateSideSet,Crewmate.crewmateEndSet,
                false,VentPermission.CanNotUse,false,false,false){
        changeButton = null;
        Arrow = null;
    }
}