using Nebula.Patches;

namespace Nebula.Roles.NeutralRoles;

public class Moriarty : Role, Template.HasWinTrigger{
    public class MoriartyEvent : Events.LocalEvent
    {
        PlayerControl target;
        public MoriartyEvent(PlayerControl target) : base(0.1f) { this.target = target; }

        public override void OnActivate()
        {
            RPCEventInvoker.ImmediatelyChangeRole(target, Roles.Moran);
        }
    }

    public bool WinTrigger { get; set; } = false;
    public byte Winner { get; set; } = Byte.MaxValue;

    static public Color RoleColor = new Color(106f / 255f, 252f / 255f, 45f / 255f);

    private Module.CustomOption coolDownOption;

    private SpriteLoader sidekickButtonSprite = new SpriteLoader("Nebula.Resources.SidekickButton.png", 115f, "ui.button.Moriarty.sidekick");
    private SpriteLoader suicideButtonSprite = new SpriteLoader("Nebula.Resources.SuicideButton.png", 115f, "ui.button.Moriarty.suicide");

    int isCreated { get; set; }
    private CustomButton sidekickButton;
    private CustomButton suicideButton;

    public override void GlobalInitialize(PlayerControl __instance)
    {
        __instance.GetModData().SetRoleData(isCreated, 1);
        WinTrigger = false;
    }

    public override void LoadOptionData()
    {
        coolDownOption = CreateOption(Color.white, "coolDown", 15f, 5f, 25f, 2.5f);
        coolDownOption.suffix = "second";
    }

    public override void ButtonInitialize(HudManager __instance)
    {
        if (sidekickButton != null)
        {
            sidekickButton.Destroy();
        }
        sidekickButton = new CustomButton(
            () =>
            {
	            Events.LocalEvent.Activate(new MoriartyEvent(Game.GameData.data.myData.currentTarget));
                RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId, isCreated, -1);
                Game.GameData.data.myData.currentTarget = null;
                sidekickButton.UsesText.text = Game.GameData.data.myData.getGlobalData().GetRoleData(isCreated).ToString();
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead && Game.GameData.data.myData.getGlobalData().GetRoleData(isCreated) > 0; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => { sidekickButton.Timer = sidekickButton.MaxTimer; },
            sidekickButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.LeftSideContent,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.sidekick"
        ).SetTimer(CustomOptionHolder.InitialForcefulAbilityCoolDownOption.getFloat());
        sidekickButton.MaxTimer = coolDownOption.getFloat();
        sidekickButton.UsesText.text = Game.GameData.data.myData.getGlobalData().GetRoleData(isCreated).ToString();

        if (suicideButton != null){
            suicideButton.Destroy();
        }
        suicideButton = new CustomButton(
            () =>
            {
	            PlayerControl target = Game.GameData.data.myData.currentTarget;
                RPCEventInvoker.UncheckedMurderPlayer(target.PlayerId, target.PlayerId, Game.PlayerData.PlayerStatus.Dead.Id, true);
                if(target.GetModData().role == Roles.Holmes){
                    RPCEventInvoker.WinTrigger(this);
                }else{
                    RPCEventInvoker.UncheckedMurderPlayer(PlayerControl.LocalPlayer.PlayerId, PlayerControl.LocalPlayer.PlayerId, Game.PlayerData.PlayerStatus.Dead.Id, true);
                }
                Game.GameData.data.myData.currentTarget = null;
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return Game.GameData.data.myData.currentTarget && PlayerControl.LocalPlayer.CanMove; },
            () => {},
            suicideButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.secondaryAbilityInput.keyCode,
            "button.label.suicide"
        );
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;
        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget();
        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, RoleColor);
    }

    public override void CleanUp()
    {
        base.CleanUp();

        if (sidekickButton != null)
        {
            sidekickButton.Destroy();
            sidekickButton = null;
        }
        if(suicideButton != null){
            suicideButton.Destroy();
            suicideButton = null;
        }
    }

    public override bool CheckAdditionalWin(PlayerControl player, EndCondition condition)
    {
        if (condition == EndCondition.MoriartyWin || condition == EndCondition.MoriartyWinByKillHolmes) return true;
        return false;
    }

    public override void EditDisplayNameColor(byte playerId, ref Color displayColor)
    {
        if (PlayerControl.LocalPlayer.GetModData().role.side == Side.Moriarty)
        {
            displayColor = RoleColor;
        }
    }

    public Moriarty()
        : base("Moriarty", "moriarty", RoleColor, RoleCategory.Neutral, Side.Moriarty, Side.Moriarty,
             new HashSet<Side>() { Side.Moriarty }, new HashSet<Side>() { Side.Moriarty },
             new HashSet<Patches.EndCondition>() { Patches.EndCondition.MoriartyWin, Patches.EndCondition.MoriartyWinByKillHolmes },
             true, VentPermission.CanNotUse, false, false, false)
    {
        sidekickButton = null;
        suicideButton = null;

        Patches.EndCondition.MoriartyWinByKillHolmes.TriggerRole = this;
    }
}

public class Moran : Role{

    public override bool IsSpawnable() => Roles.Moriarty.IsSpawnable();
    public override bool IsUnsuitable { get { return false; } }

    private bool equipRifleFlag;

    /* オプション */
    private Module.CustomOption snipeCoolDownOption;
    private Module.CustomOption shotSizeOption;
    private Module.CustomOption shotEffectiveRangeOption;
    public Module.CustomOption storeRifleOnFireOption;
    public Module.CustomOption showAimAssistOption;
    public Module.CustomOption aimAssistDelayOption;
    public Module.CustomOption aimAssistDurationOption;

    private CustomMessage? message;

    private Dictionary<byte, SpriteRenderer> Guides = new Dictionary<byte, SpriteRenderer>();
    private float rifleCounter = 0f;


    public override void LoadOptionData()
    {
        TopOption.AddCustomPrerequisite(() => Roles.Moriarty.IsSpawnable());
        snipeCoolDownOption = CreateOption(Color.white, "snipeCoolDown", 20f, 5f, 60f, 2.5f);
        snipeCoolDownOption.suffix = "second";
        shotSizeOption = CreateOption(Color.white, "shotSize", 1f, 0.5f, 4f, 0.25f);
        shotSizeOption.suffix = "cross";
        shotEffectiveRangeOption = CreateOption(Color.white, "shotEffectiveRange", 20f, 2f, 40f, 2f);
        shotEffectiveRangeOption.suffix = "cross";

        storeRifleOnFireOption = CreateOption(Color.white, "storeRifleOnFire", false);

        showAimAssistOption = CreateOption(Color.white, "showAimAssist", false);
        aimAssistDelayOption = CreateOption(Color.white, "aimAssistDelay", 2f, 1f, 10f, 1f);
        aimAssistDelayOption.suffix = "second";
        aimAssistDelayOption.AddPrerequisite(showAimAssistOption);
        aimAssistDurationOption = CreateOption(Color.white, "aimAssistDuration", 10f, 2.5f, 60f, 2.5f);
        aimAssistDurationOption.suffix = "second";
        aimAssistDurationOption.AddPrerequisite(showAimAssistOption);
    }

    public override void EditDisplayNameColor(byte playerId, ref Color displayColor)
    {
        if (PlayerControl.LocalPlayer.GetModData().role.side == Side.Moriarty)
        {
            displayColor = Color;
        }
    }
    private PlayerControl GetShootPlayer(float shotSize, float effectiveRange)
    {
        PlayerControl result = null;
        float num = effectiveRange;
        Vector3 pos;
        float mouseAngle = Game.GameData.data.myData.getGlobalData().MouseAngle;
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            //自分自身は撃ち抜かれない
            if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

            if (player.Data.IsDead) continue;


            pos = player.transform.position - PlayerControl.LocalPlayer.transform.position;
            pos = new Vector3(
                pos.x * MathF.Cos(mouseAngle) + pos.y * MathF.Sin(mouseAngle),
                pos.y * MathF.Cos(mouseAngle) - pos.x * MathF.Sin(mouseAngle));
            if (Math.Abs(pos.y) < shotSize && (!(pos.x < 0)) && pos.x < num)
            {
                num = pos.x;
                result = player;
            }
        }
        return result;
    }

    /* ボタン */
    static private CustomButton sniperButton;
    static private CustomButton killButton;
    public override void ButtonInitialize(HudManager __instance)
    {
        if (sniperButton != null)
        {
            sniperButton.Destroy();
        }
        sniperButton = new CustomButton(
            () =>
            {
                if (equipRifleFlag)
                {
                    RPCEventInvoker.SniperSettleRifle();
                }
                else
                {
                    Objects.SoundPlayer.PlaySound(Module.AudioAsset.SniperEquip);
                    RPCEventInvoker.ObjectInstantiate(Objects.ObjectTypes.SniperRifle.Rifle, PlayerControl.LocalPlayer.transform.position);
                }
                rifleCounter = 0f;
                equipRifleFlag = !equipRifleFlag;

                sniperButton.SetLabel(equipRifleFlag ? "button.label.unequip" : "button.label.equip");
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove; },
            () =>
            {
                sniperButton.Timer = sniperButton.MaxTimer;
                sniperButton.SetLabel("button.label.equip");
            },
            snipeButtonSprite.GetSprite(),
            Expansion.GridArrangeExpansion.GridArrangeParameter.None,
            __instance,
            Module.NebulaInputManager.abilityInput.keyCode,
            "button.label.equip"
        );
        sniperButton.Timer = sniperButton.MaxTimer = 0f;
        sniperButton.actionButton.transform.SetSiblingIndex(HudManager.Instance.ImpostorVentButton.transform.GetSiblingIndex());

        if (killButton != null)
        {
            killButton.Destroy();
        }
        killButton = new CustomButton(
            () =>
            {
                PlayerControl target = GetShootPlayer(shotSizeOption.getFloat() * 0.4f, shotEffectiveRangeOption.getFloat());
                if (target != null)
                {
                    var res = Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, target, Game.PlayerData.PlayerStatus.Sniped, false, false);

                    if(target.GetModData().role == Roles.Holmes){
                        RPCEventInvoker.WinTrigger(Roles.Moriarty);
                    }

                    killButton.Timer = killButton.MaxTimer;
                }

                killButton.Timer = killButton.MaxTimer;
                if (storeRifleOnFireOption.getBool())
                {
                    RPCEventInvoker.SniperSettleRifle();
                    equipRifleFlag = false;
                    sniperButton.SetLabel("button.label.equip");
                }
            },
            () => { return !PlayerControl.LocalPlayer.Data.IsDead; },
            () => { return PlayerControl.LocalPlayer.CanMove && equipRifleFlag; },
            () => { killButton.Timer = killButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            Expansion.GridArrangeExpansion.GridArrangeParameter.AlternativeKillButtonContent,
            __instance,
            Module.NebulaInputManager.modKillInput.keyCode,
             "button.label.snipe"
        ).SetTimer(CustomOptionHolder.InitialKillCoolDownOption.getFloat());
        killButton.MaxTimer = snipeCoolDownOption.getFloat();
        killButton.FireOnClicked = true;
        killButton.SetButtonCoolDownOption(true);
    }

    public byte deadBodyId;

    public override void OnMeetingStart()
    {
        RPCEventInvoker.SniperSettleRifle();
        equipRifleFlag = false;
    }

    /* 画像 */
    private SpriteLoader snipeButtonSprite = new SpriteLoader("Nebula.Resources.SnipeButton.png", 115f, "ui.button.moran.equip");

    public override HelpSprite[] helpSprite => new HelpSprite[]
    {
            new HelpSprite(snipeButtonSprite,"role.moran.help.equip",0.3f)
    };

    private Sprite snipeArrowSprite = null;
    public Sprite getSnipeArrowSprite()
    {
        if (snipeArrowSprite) return snipeArrowSprite;
        snipeArrowSprite = Helpers.loadSpriteFromResources("Nebula.Resources.SniperRifleArrow.png", 200f);
        return snipeArrowSprite;
    }

    private static Sprite guideSprite = null;
    public static Sprite getGuideSprite()
    {
        if (guideSprite) return guideSprite;
        guideSprite = Helpers.loadSpriteFromResources("Nebula.Resources.SniperGuide.png", 100f);
        return guideSprite;
    }

    private void UpdateGuide(SpriteRenderer guide, Vector3 pos, Color color)
    {
        guide.color = color;
        Vector3 dir = pos - PlayerControl.LocalPlayer.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x);

        float oldAng = guide.transform.eulerAngles.z * (float)Math.PI / 180f;
        Vector2 newPos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Vector2 oldPos = new Vector2(Mathf.Cos(oldAng), Mathf.Sin(oldAng));
        newPos = oldPos + (newPos - oldPos) * 0.15f;

        angle = Mathf.Atan2(newPos.y, newPos.x);
        guide.transform.eulerAngles = new Vector3(0, 0, angle * 180f / (float)Math.PI);
        guide.transform.localPosition = new Vector3(Mathf.Cos(angle) * 2f, Mathf.Sin(angle) * 2f, -30f);
    }

    private IEnumerator GetMessageUpdater()
    {
        while (true)
        {
            bool flag = false;
            foreach (var p in PlayerControl.AllPlayerControls)
            {
                if (p == PlayerControl.LocalPlayer) continue;
                if (p.GetModData().isInvisiblePlayer)
                {
                    if (p.transform.position.Distance(PlayerControl.LocalPlayer.transform.position) < 5.0f)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (flag) yield return null;
            else break;
        }
    }

    public override void MyPlayerControlUpdate()
    {
        if (message == null || !message.isActive)
        {
            foreach (var p in PlayerControl.AllPlayerControls)
            {
                if (p == PlayerControl.LocalPlayer) continue;
                if (p.GetModData().isInvisiblePlayer)
                {
                    if (p.transform.position.Distance(PlayerControl.LocalPlayer.transform.position) < 5.0f)
                    {
                        message = new CustomMessage(new Vector3(0, -1.5f, 0), false, Language.Language.GetString("role.sniper.nearMessage"), GetMessageUpdater(), 1.0f, Palette.ImpostorRed);
                        break;
                    }
                }
            }
        }

        if (equipRifleFlag)
        {
            RPCEventInvoker.UpdatePlayerControl();
        }

        if (!showAimAssistOption.getBool()) return;
        if (equipRifleFlag)
        {
            rifleCounter += Time.deltaTime;

            float r = 0f, g = 0f, b = 0f, a = 0f;

            if (rifleCounter > aimAssistDelayOption.getFloat())
            {
                float value = 1f - (rifleCounter - aimAssistDelayOption.getFloat());
                if (value > 0) r = g = b = a = value;
                r = 0.2f + 0.8f * r;
                g = 0.4f + 0.6f * g;
                g = 0.8f + 0.2f * b;
                a = 0.6f + 0.4f * a;

                if (rifleCounter > aimAssistDelayOption.getFloat() + aimAssistDurationOption.getFloat())
                {
                    value = rifleCounter - aimAssistDelayOption.getFloat() - aimAssistDurationOption.getFloat();
                    if (value < 0) value = 0f;
                    a *= 1f - value;
                }
            }

            Color color = new Color(r, g, b, a);

            foreach (var guide in Guides)
            {
                guide.Value.color = Color.clear;
            }

            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.IsDead) continue;
                if (!Guides.ContainsKey(player.PlayerId)) continue;

                UpdateGuide(Guides[player.PlayerId], player.transform.position, color);
            }

            foreach (var deadBody in Helpers.AllDeadBodies())
            {
                UpdateGuide(Guides[deadBody.ParentId], deadBody.transform.position, color);
            }
        }
        else
        {
            foreach (var guide in Guides)
            {
                guide.Value.color *= 0.7f;
            }
        }
    }

    public override void OnDied()
    {
        if (equipRifleFlag)
        {
            RPCEventInvoker.SniperSettleRifle();
            equipRifleFlag = false;
        }
    }

    public override void Initialize(PlayerControl __instance)
    {
        equipRifleFlag = false;

        foreach (var player in Game.GameData.data.AllPlayers.Values)
        {
            if (player.id == PlayerControl.LocalPlayer.PlayerId) continue;

            var obj = new GameObject("SniperGuide");
            var renderer = obj.AddComponent<SpriteRenderer>();

            renderer.sprite = getGuideSprite();
            renderer.transform.parent = FastDestroyableSingleton<HudManager>.Instance.transform;
            renderer.color = new Color(0, 0, 0, 0);
            renderer.transform.position = new Vector3(0, 0, -30f);
            Guides[player.id] = renderer;
        }

        message = null;
    }


    public override void CleanUp()
    {
        if (equipRifleFlag)
        {
            RPCEventInvoker.SniperSettleRifle();
            equipRifleFlag = false;
        }

        if (sniperButton != null)
        {
            sniperButton.Destroy();
            sniperButton = null;
        }

        if (killButton != null)
        {
            killButton.Destroy();
            killButton = null;
        }


        foreach (var sprite in Guides)
        {
            UnityEngine.Object.Destroy(sprite.Value);
        }
        Guides.Clear();
    }

    public override bool CheckAdditionalWin(PlayerControl player, EndCondition condition)
    {
        if (condition == EndCondition.MoriartyWin || condition == EndCondition.MoriartyWinByKillHolmes) return true;
        return false;
    }

    public Moran()
        : base("Moran", "moran", Moriarty.RoleColor, RoleCategory.Neutral, Side.Moriarty, Side.Moriarty,
             new HashSet<Side>() { Side.Moriarty }, new HashSet<Side>() { Side.Moriarty },
             new HashSet<Patches.EndCondition>() { Patches.EndCondition.MoriartyWin, Patches.EndCondition.MoriartyWinByKillHolmes },
             true, VentPermission.CanUseLimittedVent, true, false, false){

        sniperButton = null;
        killButton = null;
    }
}