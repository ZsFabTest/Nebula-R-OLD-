namespace Nebula.Roles.NeutralRoles;

public class Yandere : Role{
    public static Color RoleColor = new Color(200f / 255f,22f / 255f,115f / 255f);

    public PlayerControl myLover;
    private Dictionary<byte, float> progress;
    private List<byte> activePlayer;
    private List<Objects.Arrow?> arrows;
    private Objects.Arrow? arrow;
    SpriteLoader arrowSprite = new SpriteLoader("role.spectre.arrow");

    public Module.CustomOption killCoolDownOption;
    public Module.CustomOption stayDuringOption;
    public Module.CustomOption stayRangeOption;

    public override void LoadOptionData()
    {
        killCoolDownOption = CreateOption(Color.white,"killCooldown",5f,1f,25f,1f);
        killCoolDownOption.suffix = "second";
        stayDuringOption = CreateOption(Color.white,"stayDuringTime",15f,5f,30f,2.5f);
        stayDuringOption.suffix = "second";
        stayRangeOption = CreateOption(Color.white,"stayRangeTime",1f,0.1f,5f,0.1f);
        stayRangeOption.suffix = "cross";
    }

    public override void Initialize(PlayerControl __instance)
    {
        List<PlayerControl> players = PlayerControl.AllPlayerControls.ToArray().ToList();
        for(int i = 0;i < players.Count;i++){
            if(players[i].PlayerId == PlayerControl.LocalPlayer.PlayerId){
                players.RemoveAt(i);
                break;
            }
        }
        myLover = players[NebulaPlugin.rnd.Next(players.Count)];
    }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        progress = new();
        activePlayer = new();
        arrows = new();
        arrow = null;
    }

    public PlayerControl GetLover(){ return myLover; }

    private CustomButton killButton;
    public override void ButtonInitialize(HudManager __instance)
    {
        if(killButton != null) killButton.Destroy();
        killButton = new CustomButton(
            () => { 
                PlayerControl target = Game.GameData.data.myData.currentTarget;
                var res = Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, target, Game.PlayerData.PlayerStatus.Dead);
                killButton.Timer = killButton.MaxTimer;
                Game.GameData.data.myData.currentTarget = null;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove && activePlayer.Count > 0; },
            () => { killButton.Timer = killButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
            "button.label.kill"
        ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
        killButton.MaxTimer = killCoolDownOption.getFloat();
    }

    public override void CleanUp(){
        if(killButton != null){
            killButton.Destroy();
            killButton = null;
        }
        if(arrow != null){
            UnityEngine.GameObject.Destroy(arrow.arrow);
            arrow = null;
        }
        foreach (var a in arrows) if (a != null) GameObject.Destroy(a.arrow);
        arrows.Clear();
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget((player) => {
            if(activePlayer.Contains(player.PlayerId)) return true;
            return false;
        });
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, RoleColor);

        if (MeetingHud.Instance != null) return;
        float time = Time.deltaTime / stayDuringOption.getFloat();
        foreach (PlayerControl player in PlayerControl.AllPlayerControls){
            if(player.Data.IsDead) continue;
            if(player.PlayerId == PlayerControl.LocalPlayer.PlayerId || player.PlayerId == myLover.PlayerId) continue;
            if(!player.gameObject.active) continue;
            if(activePlayer.Contains(player.PlayerId)) continue;

            if(Vector2.Distance(player.transform.position,myLover.transform.position) <= 1f){
                if (!progress.ContainsKey(player.PlayerId))
                {
                    progress.Add(player.PlayerId, 0);
                }
                progress[player.PlayerId] += time;

                if(progress[player.PlayerId] > 1){
                    activePlayer.Add(player.PlayerId);
                }
            }
        }

        int i = 0;
        foreach(byte playerId in activePlayer){
            PlayerControl player = Helpers.playerById(playerId);
            if(player.Data.IsDead) continue;
            if (arrows.Count >= i) arrows.Add(null);
            var arrow = arrows[i];
            RoleSystem.TrackSystem.PlayerTrack_MyControlUpdate(ref arrow,player,Color.white,arrowSprite);
            arrows[i] = arrow;
            i++;
        }

        if(myLover.Data.IsDead) RPCEventInvoker.UncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId,PlayerControl.LocalPlayer.PlayerId,Game.PlayerData.PlayerStatus.Suicide.Id,true);

        int removed = arrows.Count - i;
        for (; i < arrows.Count; i++) if (arrows[i] != null) GameObject.Destroy(arrows[i].arrow);
        arrows.RemoveRange(arrows.Count - removed, removed);

        RoleSystem.TrackSystem.PlayerTrack_MyControlUpdate(ref arrow, myLover,RoleColor,arrowSprite);
    }

    public Yandere()
        : base("Yandere", "yandere", RoleColor, RoleCategory.Neutral, Side.Yandere, Side.Yandere,
             new HashSet<Side>() { Side.Yandere }, new HashSet<Side>() { Side.Yandere },
             new HashSet<Patches.EndCondition>() { Patches.EndCondition.YandereWin },
             true, VentPermission.CanUseUnlimittedVent, true, true, true)
    {
        killButton = null;
        myLover = null;
        progress = null;
        activePlayer = null;
        arrows = null;
        arrow = null;
    }
}