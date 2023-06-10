namespace Nebula.Roles.ImpostorRoles;

public class Assassin : Role
{
    private Module.CustomOption assassinateCooldownOption;
    private Module.CustomOption hasLeavesAfterAssassinOption;
    private Module.CustomOption showMurderColorDuringTimeOption;
    private Module.CustomOption maxExistenceTimeOption;

    private SpriteLoader AssassinMarkButtonSprite = new SpriteLoader("Nebula.Resources.AssassinMarkButton.png", 115f);
    private SpriteLoader AssassinateButtonSprite = new SpriteLoader("Nebula.Resources.AssassinateButton.png", 115f);

    public override void LoadOptionData()
    {
        assassinateCooldownOption = CreateOption(Color.white, "assassinateCooldown", 25f, 15f, 35f, 5f);
        assassinateCooldownOption.suffix = "second";

        hasLeavesAfterAssassinOption = CreateOption(Color.white,"hasLeavesAfterAssassin",true);
        showMurderColorDuringTimeOption = CreateOption(Color.white,"colorDuringTime",3f,0f,15f,1f).AddPrerequisite(hasLeavesAfterAssassinOption);
        showMurderColorDuringTimeOption.suffix = "second";
        maxExistenceTimeOption = CreateOption(Color.white,"maxExistenceTime",10f,1f,30f,1f).AddPrerequisite(hasLeavesAfterAssassinOption);
        maxExistenceTimeOption.suffix = "second";
    }

    public static CustomButton chooseTarget;
    public static CustomButton assassinate;
    public PlayerControl assassinateTarget;
    private Arrow? Arrow = null;
    private long LastTime;
    CustomObject Objeck;

    public override void GlobalInitialize(PlayerControl __instance)
    {
        Objeck = null;
        LastTime = long.MaxValue;
    }

    public override void ButtonInitialize(HudManager __instance)
    {
        if(chooseTarget != null)
        {
            chooseTarget.Destroy();
        }
        chooseTarget = new CustomButton(
            () =>
            {
                PlayerControl target = Game.GameData.data.myData.currentTarget;
                assassinateTarget = target;
                chooseTarget.Timer = 5f;
                Game.GameData.data.myData.currentTarget.ShowFailedMurder();
                Arrow = new Arrow(Color.black);
                Arrow.arrow.SetActive(true);
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && Game.GameData.data.myData.currentTarget != null; },
            () => { chooseTarget.Timer = 5f; },
            AssassinMarkButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.mark"
        ).SetTimer(CustomOptionHolder.InitialAbilityCoolDownOption.getFloat());

        if (assassinate != null)
        {
            assassinate.Destroy();
        }
        assassinate = new CustomButton(
            () =>
            {
                RPCEventInvoker.ObjectInstantiate(CustomObject.Type.TeleportEvidence,PlayerControl.LocalPlayer.GetTruePosition());
                PlayerControl target = assassinateTarget;
                var res = Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, target, Game.PlayerData.PlayerStatus.Dead, false, true);
                if (res != Helpers.MurderAttemptResult.SuppressKill)
                    assassinate.Timer = assassinate.MaxTimer;
                assassinateTarget = null;
                assassinate.Timer = assassinateCooldownOption.getFloat();
                Objeck = RPCEventInvoker.ObjectInstantiate(CustomObject.Type.Leaves,target.transform.position);
                LastTime = System.DateTime.Now.ToFileTime();
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && assassinateTarget != null; },
            () => { assassinate.Timer = assassinateCooldownOption.getFloat(); },
            AssassinateButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            "button.label.assassinate"
        ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
    }

    public override void CleanUp()
    {
        base.CleanUp();
        if(chooseTarget != null)
        {
            chooseTarget.Destroy();
            chooseTarget = null;
        }
        if(assassinate != null)
        {
            assassinate.Destroy();
            assassinate = null;
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
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
        if(Arrow != null){
            if(assassinateTarget == null){
                UnityEngine.Object.Destroy(Arrow.arrow);
                Arrow = null;
                return;
            }
            Arrow.Update(assassinateTarget.transform.position);
        }
        if(Objeck != null){
            //Debug.Log((System.DateTime.Now.ToFileTime() - LastTime).ToString());
            if((System.DateTime.Now.ToFileTime() - LastTime) / 1e7f >= maxExistenceTimeOption.getFloat()){
                LastTime = long.MaxValue;
                RPCEvents.ObjectDestroy(Objeck.Id);
                Objeck = null;
                return;
            }
            if((System.DateTime.Now.ToFileTime() - LastTime) / 1e7f >= showMurderColorDuringTimeOption.getFloat()){
                RPCEvents.ObjectUpdate(Objeck.Id,1);
            }else RPCEvents.ObjectUpdate(Objeck.Id,0);
        }
    }

    public override void OnMeetingEnd()
    {
        assassinateTarget = null;
    }

    public override void EditCoolDown(CoolDownType type, float count)
    {
        assassinate.Timer -= count;
        assassinate.actionButton.ShowButtonText("+" + count + "s");
    }

    public Assassin()
        : base("Assassin", "assassin", Palette.ImpostorRed, RoleCategory.Impostor, Side.Impostor, Side.Impostor,
             Impostor.impostorSideSet, Impostor.impostorSideSet,
             Impostor.impostorEndSet,
             true, VentPermission.CanUseUnlimittedVent, true, true, true)
    {
        assassinateTarget = null;
        HideKillButtonEvenImpostor = true;
        chooseTarget = null;
        assassinate = null;
        Arrow = null;
        LastTime = long.MaxValue;
        Objeck = null;
    }
}
