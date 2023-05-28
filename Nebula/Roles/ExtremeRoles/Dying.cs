namespace Nebula.Roles.ImpostorRoles{
	public class Dying : Role{
		public class dyingEvent : Events.LocalEvent{
			byte murderId = Byte.MaxValue;

			public dyingEvent(byte murderId) : base(madDuringTimeOption.getFloat()) { this.murderId = murderId; }

			public override void OnTerminal(){
				RPCEventInvoker.UncheckedMurderPlayer(murderId,PlayerControl.LocalPlayer.PlayerId,Game.PlayerData.PlayerStatus.Dead.Id,false);
			}
		}

		public static Module.CustomOption madDuringTimeOption;
		public static Module.CustomOption specialKillCooldownOption;

		bool isMurdered = false;

	    public override void GlobalInitialize(PlayerControl __instance) { isMurdered = false; }

	    public override void OnMurdered(byte murderId) {
	    	if(isMurdered){
	    		isMurdered = false;
	    		killButton.MaxTimer = GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown);
	    		return;
	    	}
	    	Events.LocalEvent.Activate(new dyingEvent(murderId));
	    	isMurdered = true;
	    	RPCEventInvoker.RevivePlayer(PlayerControl.LocalPlayer);
	    	killButton.MaxTimer = specialKillCooldownOption.getFloat();
	    	killButton.Timer = 0f;
	    }

		public override void LoadOptionData(){
			TopOption.tab = Module.CustomOptionTab.GhostRoles;
			madDuringTimeOption = CreateOption(Color.white,"madDuringTime",15f,2.5f,30f,2.5f);
			madDuringTimeOption.suffix = "second";

			specialKillCooldownOption = CreateOption(Color.white,"specialKillCooldown",2.5f,0f,25f,2.5f);
			specialKillCooldownOption.suffix = "second";
		}

		private CustomButton killButton;
	    public override void ButtonInitialize(HudManager __instance)
	    {
	        if(killButton != null)
	        {
	            killButton.Destroy();
	        }
	        killButton = new CustomButton(
	            () =>
	            {
	                Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, Game.GameData.data.myData.currentTarget, Game.PlayerData.PlayerStatus.Dead, true);
	                killButton.Timer = killButton.MaxTimer;
	            },
	            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
	            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
	            () => { killButton.Timer = killButton.MaxTimer; },
	            __instance.KillButton.graphic.sprite,
	            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
	            __instance,
	            Module.NebulaInputManager.modKillInput.keyCode,
	            "button.label.kill"
	        ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
	        killButton.MaxTimer = GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown);
	        killButton.SetButtonCoolDownOption(true);
	    }

	    public override void EditCoolDown(CoolDownType type, float count)
	    {
	    	if((type & CoolDownType.ImpostorsKill) == 0) return;
	        killButton.Timer -= count;
	        killButton.actionButton.ShowButtonText("+" + count + "s");
	    }

	    public override void MyPlayerControlUpdate()
	    {
	        Game.MyPlayerData data = Game.GameData.data.myData;
	        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(true);
	        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget,Palette.ImpostorRed);
	    }

	    public override void CleanUp(){
	    	if(killButton != null){
	    		killButton.Destroy();
	    		killButton = null;
	    	}
	    }

		public Dying()
			: base("Dying","dying",Palette.ImpostorRed,RoleCategory.Impostor,Side.Impostor,Side.Impostor,
		 		Impostor.impostorSideSet,Impostor.impostorSideSet,Impostor.impostorEndSet,
		 		true,VentPermission.CanUseUnlimittedVent,true,true,true){
			isMurdered = false;
			HideKillButtonEvenImpostor = true;
			killButton = null;
		}
	}
}