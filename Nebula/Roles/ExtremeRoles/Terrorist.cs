namespace Nebula.Roles.ImpostorRoles{
    public class Terrorist : Role{

        private Module.CustomOption explodeCooldownOption;
        private Module.CustomOption explodeRangeOption;

        private SpriteLoader buttonSprite = new SpriteLoader("Nebula.Resources.BuskReviveButton.png", 115f);

        public override void LoadOptionData()
        {
            TopOption.tab = Module.CustomOptionTab.GhostRoles;
            explodeCooldownOption = CreateOption(Color.white,"explodeCooldown",15f,0f,60f,2.5f);
            explodeCooldownOption.suffix = "second";
            explodeRangeOption = CreateOption(Color.white,"explodeRange",1f,0.25f,5f,0.25f);
            explodeRangeOption.suffix = "cross";
        }

        private CustomButton explode;
        public override void ButtonInitialize(HudManager __instance)
        {
            if(explode != null){
                explode.Destroy();
            }
            explode = new CustomButton(
                () => {
                    Vector3 position = PlayerControl.LocalPlayer.GetTruePosition();
                    foreach(PlayerControl p in PlayerControl.AllPlayerControls){
                        if(p.Data.IsDead || p == PlayerControl.LocalPlayer) continue;
                        float dis = Vector2.Distance(position,p.GetTruePosition());
                        if(dis <= explodeRangeOption.getFloat() * 1.8f){
                            Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer,p,Game.PlayerData.PlayerStatus.Killed,false,false);
                        }
                    }
                    RPCEventInvoker.UncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId,PlayerControl.LocalPlayer.PlayerId,Game.PlayerData.PlayerStatus.Killed.Id,false);
                },
                () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
                () => { return PlayerControl.LocalPlayer.CanMove; },
                () => { explode.Timer = explode.MaxTimer; },
                buttonSprite.GetSprite(),
                Expansion.GridArrangeExpansion.GridArrangeParameter.None,
                __instance,
                Module.NebulaInputManager.abilityInput.keyCode,
                "button.label.explode"
            ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
            explode.MaxTimer = explodeCooldownOption.getFloat();
        }

        public override void CleanUp()
        {
            if(explode != null){
                explode.Destroy();
                explode = null;
            }
        }

        public override void AfterTeleport(float time)
        {
            if(explode.Timer < time) explode.Timer = time;
        }

        public Terrorist()
             : base("Terrorist","terrorist",Palette.ImpostorRed,RoleCategory.Impostor,Side.Impostor,Side.Impostor,
                    Impostor.impostorSideSet,Impostor.impostorSideSet,Impostor.impostorEndSet,
                    true,VentPermission.CanUseUnlimittedVent,true,true,true){
            explode = null;
        }
    }
}