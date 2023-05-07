namespace Nebula.Roles.CrewmateRoles{
    public class Minekeeper : Role{
        public static Color RoleColor = new Color(100f / 255f,50f / 255f,0f / 255f);

        private Module.CustomOption setMineCooldownOption;
        private Module.CustomOption maxMineCountOption;

        public static int mineDataId { get; private set; }
        Vent targetVent = null;
        private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.CloseVentButton.png", 115f, "ui.button.minekeeper.set");

        public override void GlobalInitialize(PlayerControl __instance)
        {
            targetVent = null;
            __instance.GetModData().SetRoleData(mineDataId,0);
        }

        public override void LoadOptionData()
        {
            TopOption.tab = Module.CustomOptionTab.GhostRoles;
            setMineCooldownOption = CreateOption(Color.white,"setMineCooldown",15f,2.5f,45f,2.5f);
            setMineCooldownOption.suffix = "second";

            maxMineCountOption = CreateOption(Color.white,"maxMineCount",5f,1f,30f,1f);
        }

        private CustomButton set;
        public override void ButtonInitialize(HudManager __instance)
        {
            if(set != null){
                set.Destroy();
            }
            set = new CustomButton(
                () => {
                    Module.VentManager.setBomb(targetVent);
                    foreach(Vent v in Module.VentManager.bombVents) Debug.LogWarning(v.name);
                    targetVent = null;
                    RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId,mineDataId,1);
                    set.Timer = set.MaxTimer;
                    set.UsesText.text = ((int)maxMineCountOption.getFloat() - (int)PlayerControl.LocalPlayer.GetModData().GetRoleData(mineDataId)).ToString();
                },
                () => { return !PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.GetModData().GetRoleData(mineDataId) < maxMineCountOption.getFloat(); },
                () => { return targetVent && PlayerControl.LocalPlayer.CanMove; },
                () => { set.Timer = set.MaxTimer; },
                buttonSprite.GetSprite(),
                Expansion.GridArrangeExpansion.GridArrangeParameter.None,
                __instance,
                Module.NebulaInputManager.abilityInput.keyCode,
                "button.label.set"
            ).SetTimer(CustomOptionHolder.InitialModestAbilityCoolDownOption.getFloat());
            set.MaxTimer = setMineCooldownOption.getFloat();

            set.UsesText.text = ((int)maxMineCountOption.getFloat()).ToString();
        }

        public override void CleanUp(){
            if(set != null){
                set.Destroy();
                set = null;
            }
            targetVent = null;
        }

    public override void MyPlayerControlUpdate()
    {
        if (!ShipStatus.Instance) return;

        Vent target = null;
        Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
        float closestDistance = float.MaxValue;
        //Debug.LogWarning("14 " + ShipStatus.Instance.AllVents[14].name);
        for (int i = 0; i < ShipStatus.Instance.AllVents.Length; i++)
        {
            Vent vent = ShipStatus.Instance.AllVents[i];
            //Debug.Log(vent.name);
            if (vent.GetVentData().Sealed) continue;
            float distance = Vector2.Distance(vent.transform.position, truePosition);
            //Debug.Log(vent.name + " " + distance.ToString() + " " + vent.UsableDistance.ToString());
            if (distance <= vent.UsableDistance && distance < closestDistance)
            {
                closestDistance = distance;
                target = vent;
            }
        }
        //try{ Debug.Log(target.name); }catch{ Debug.LogWarning("No Target"); };
        targetVent = target;
    }

        public Minekeeper()
             : base("Minekeeper","minekeeper",RoleColor,RoleCategory.Crewmate,Side.Crewmate,Side.Crewmate,
                    Crewmate.crewmateSideSet,Crewmate.crewmateSideSet,Crewmate.crewmateEndSet,
                    false,VentPermission.CanNotUse,false,false,false){
            set = null;
            targetVent = null;
        }
    }
}