namespace Nebula.Roles.ImpostorRoles;

public class ChainKiller : Role{

	public class ChainKillerEvent : Events.LocalEvent{
		PlayerControl target;
		public ChainKillerEvent(PlayerControl target) : base(0.1f) { this.target = target; }
		public override void OnActivate(){
			var r = Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, target, Game.PlayerData.PlayerStatus.Dead, true);
			if(r != Helpers.MurderAttemptResult.PerformKill) return;
			killEvent(1);
		}
		private void killEvent(int deepth){
			if(deepth >= maxKillCountAtOnceOption.getFloat()) return;
			PlayerControl next = Patches.PlayerControlPatch.SetMyTarget(1.8f * chainKillRangeOption.getFloat());
			if(next == null) return;
			var r = Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, next, Game.PlayerData.PlayerStatus.Dead, true);
			if(r != Helpers.MurderAttemptResult.PerformKill) return;
			killEvent(deepth + 1);
		}
	}

	private Module.CustomOption killCooldownOption;
	public static Module.CustomOption maxKillCountAtOnceOption;
	public static Module.CustomOption chainKillRangeOption;

	public override void LoadOptionData(){
		TopOption.tab = Module.CustomOptionTab.GhostRoles;
		killCooldownOption = CreateOption(Color.white,"killCooldown",32.5f,2.5f,60f,2.5f);
		killCooldownOption.suffix = "second";
		maxKillCountAtOnceOption = CreateOption(Color.white,"maxKillCountAtOnce",3f,2f,15f,1f);
		chainKillRangeOption = CreateOption(Color.white,"chainKillRange",1f,0.25f,3f,0.25f);
		chainKillRangeOption.suffix = "cross";
	}

	private CustomButton killButton;
	public override void ButtonInitialize(HudManager __instance){
		if(killButton != null){
			killButton.Destroy();
		}
		killButton = new CustomButton(
			() => {
				Events.LocalEvent.Activate(new ChainKillerEvent(Game.GameData.data.myData.currentTarget));
				Game.GameData.data.myData.currentTarget = null;
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
        killButton.MaxTimer = killCooldownOption.getFloat();
        killButton.SetButtonCoolDownOption(true);
	}

	public override void CleanUp(){
		if(killButton != null){
			killButton.Destroy();
			killButton = null;
		}
	}

    public override void EditCoolDown(CoolDownType type, float count)
    {
        killButton.Timer -= count;
        killButton.actionButton.ShowButtonText("+" + count + "s");
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(true);
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, Palette.ImpostorRed);
    }

	public override void AfterTeleport(float time){
		if(killButton.Timer < time) killButton.Timer = time; 
	}

	public ChainKiller()
		 : base("ChainKiller","chainKiller",Palette.ImpostorRed,RoleCategory.Impostor,Side.Impostor,Side.Impostor,
		 		Impostor.impostorSideSet,Impostor.impostorSideSet,Impostor.impostorEndSet,
		 		true,VentPermission.CanUseUnlimittedVent,true,true,true){
		HideKillButtonEvenImpostor = true;
		killButton = null;
	}
}