namespace Nebula.Roles.ImpostorRoles{
    public class Plumber : Role{
        private Module.CustomOption createVentCooldownOption;
        private Module.CustomOption maxCreateVentCountOption;

        public override void LoadOptionData()
        {
            TopOption.tab = Module.CustomOptionTab.GhostRoles;
            createVentCooldownOption = CreateOption(Color.white,"createVentCooldown",25f,5f,45f,2.5f);
            createVentCooldownOption.suffix = "second";
            maxCreateVentCountOption = CreateOption(Color.white,"maxCreateVentCount",5f,2f,10f,1f);
        }

        public int ventDataId { get; private set; }
        private CustomButton createVent;

        public override void GlobalInitialize(PlayerControl __instance)
        {
            __instance.GetModData().SetRoleData(ventDataId,0);
        }

        /*
        public override void GlobalFinalizeInGame(PlayerControl __instance)
        {
            var allVentsList = ShipStatus.Instance.AllVents.ToList();
            foreach(Vent newVent in vents){
                newVent.gameObject.SetActive(false);
                newVent.DestroyChildren();
                allVentsList.Remove(newVent);
            }
            ShipStatus.Instance.AllVents = allVentsList.ToArray();
        }
        */

        private bool checkVent()
        {
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (!PlayerControl.LocalPlayer.Data.IsDead) return true;

            var mapData = Map.MapData.GetCurrentMapData();
            if (mapData == null) return false;

            return mapData.isOnTheShip(PlayerControl.LocalPlayer.GetTruePosition());
        }

        private SpriteLoader ButtonSprite = new SpriteLoader("Nebula.Resources.CloseVentButton.png",115f);

        public override void ButtonInitialize(HudManager __instance)
        {
            if(createVent != null){
                createVent.Destroy();
            }
            createVent = new CustomButton(
                () => {
                    Vector3 playerPos = PlayerControl.LocalPlayer.transform.position;
                    RPCEventInvoker.Dig(new Vector3(playerPos.x,playerPos.y,playerPos.z + 1f));
                    createVent.Timer = createVent.MaxTimer;
                    RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId,ventDataId,1);
                    //Debug.Log(Game.GameData.data.myData.getGlobalData().GetRoleData(ventDataId).ToString());
                    createVent.UsesText.text = ((int)maxCreateVentCountOption.getFloat() - Game.GameData.data.myData.getGlobalData().GetRoleData(ventDataId)).ToString();
                },
                () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
                () => { return checkVent() && PlayerControl.LocalPlayer.GetModData().GetRoleData(ventDataId) < maxCreateVentCountOption.getFloat(); },
                () => { createVent.Timer = createVent.MaxTimer; },
                ButtonSprite.GetSprite(),
                Expansion.GridArrangeExpansion.GridArrangeParameter.None,
                __instance,
                Module.NebulaInputManager.abilityInput.keyCode,
                "button.label.dig"
            ).SetTimer(CustomOptionHolder.InitialAbilityCoolDownOption.getFloat());
            createVent.MaxTimer = createVentCooldownOption.getFloat();
            createVent.UsesText.text = ((int)maxCreateVentCountOption.getFloat()).ToString();
        }

        public override void CleanUp(){
            if(createVent != null){
                createVent.Destroy();
                createVent = null;
            }
        }

        public Plumber()
             : base("Plumber","plumber",Palette.ImpostorRed,RoleCategory.Impostor,Side.Impostor,Side.Impostor,
                    Impostor.impostorSideSet,Impostor.impostorSideSet,Impostor.impostorEndSet,
                    true,VentPermission.CanUseUnlimittedVent,true,true,true){
            createVent = null;
        }
    }
}