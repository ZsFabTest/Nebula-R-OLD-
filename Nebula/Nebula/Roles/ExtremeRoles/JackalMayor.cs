namespace Nebula.Roles.NeutralRoles;

public class JackalMayor : Role
{
    static public Color RoleColor = Roles.Jackal.Color;

    public int votesId { get; private set; }
    public override RelatedRoleData[] RelatedRoleDataInfo { get => new RelatedRoleData[] { new RelatedRoleData(votesId, "Vote Stock", 0, 20) }; }

    private Module.CustomOption minVoteOption;
    private Module.CustomOption maxVoteOption;
    private Module.CustomOption maxVoteStockOption;
    private Module.CustomOption killCoolDownOption;

    //投じる票数の表示
    private TMPro.TextMeshPro countText;

    //今投票したときに投じる票数
    private byte numOfVote = 1;

    public override void GlobalInitialize(PlayerControl __instance)
    {
        Game.GameData.data.playersArray[__instance.PlayerId].SetRoleData(votesId, 0);
    }

    public override void OnMeetingStart()
    {
        RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId, votesId, 1);
        if(Game.GameData.data.myData.getGlobalData().GetRoleData(votesId) > maxVoteStockOption.getFloat())
        {
            RPCEventInvoker.UpdateRoleData(PlayerControl.LocalPlayer.PlayerId,votesId,(int)maxVoteStockOption.getFloat());
        }
    }

    public override void OnVote(byte targetId)
    {
        RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId, votesId, -numOfVote);
    }

    public override void OnVoteCanceled(int weight)
    {
        RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId, votesId, weight);
    }

    public override void SetupMeetingButton(MeetingHud __instance)
    {
        numOfVote = 1;

        if ((int)minVoteOption.getFloat() >= (int)maxVoteOption.getFloat())
        {
            //入れうる票が固定になる場合
            numOfVote = (byte)maxVoteOption.getFloat();
            if (numOfVote > Game.GameData.data.myData.getGlobalData().GetRoleData(votesId))
            {
                numOfVote = (byte)Game.GameData.data.myData.getGlobalData().GetRoleData(votesId);
            }
        }
        else
        {

            if (!PlayerControl.LocalPlayer.Data.IsDead)
            {
                GameObject template, button;
                PassiveButton passiveButton;
                SpriteRenderer renderer;

                template = __instance.SkipVoteButton.Buttons.transform.Find("CancelButton").gameObject;
                button = UnityEngine.Object.Instantiate(template, __instance.SkipVoteButton.transform);
                button.SetActive(true);
                button.name = "MayorButton";
                button.transform.position += new Vector3(1.5f, 0f);
                renderer = button.GetComponent<SpriteRenderer>();
                renderer.sprite = Images.GlobalImage.GetMeetingButtonLeft();
                passiveButton = button.GetComponent<PassiveButton>();
                passiveButton.OnClick.RemoveAllListeners();
                passiveButton.OnClick.AddListener((UnityEngine.Events.UnityAction)(() =>
                {
                    if (numOfVote > 0)
                    {
                        numOfVote--;
                        RPCEventInvoker.MultipleVote(PlayerControl.LocalPlayer, numOfVote);
                    }
                }));

                template = __instance.SkipVoteButton.Buttons.transform.Find("CancelButton").gameObject;
                button = UnityEngine.Object.Instantiate(template, __instance.SkipVoteButton.transform);
                button.SetActive(true);
                button.name = "MayorButton";
                button.transform.position += new Vector3(2.7f, 0f);
                renderer = button.GetComponent<SpriteRenderer>();
                renderer.sprite = Images.GlobalImage.GetMeetingButtonRight();
                passiveButton = button.GetComponent<PassiveButton>();
                passiveButton.OnClick.RemoveAllListeners();
                passiveButton.OnClick.AddListener((UnityEngine.Events.UnityAction)(() =>
                {
                    if (numOfVote < maxVoteOption.getFloat() && numOfVote < Game.GameData.data.myData.getGlobalData().GetRoleData(votesId))
                    {
                        numOfVote++;
                        RPCEventInvoker.MultipleVote(PlayerControl.LocalPlayer, numOfVote);
                    }
                }));



                countText = UnityEngine.Object.Instantiate(__instance.TitleText, __instance.SkipVoteButton.transform);
                countText.gameObject.SetActive(true);
                countText.alignment = TMPro.TextAlignmentOptions.Center;
                countText.transform.position = __instance.SkipVoteButton.CancelButton.transform.position;
                countText.transform.position += new Vector3(1.54f, 0f);
                countText.color = Palette.White;
                countText.transform.localScale *= 0.8f;
                countText.text = "";
            }
        }

        RPCEventInvoker.MultipleVote(PlayerControl.LocalPlayer, numOfVote);
    }

    private CustomButton killButton;
    public override void ButtonInitialize(HudManager __instance)
    {
        if (killButton != null)
        {
            killButton.Destroy();
        }
        killButton = new CustomButton(
            () =>
            {
                Helpers.checkMuderAttemptAndKill(PlayerControl.LocalPlayer, Game.GameData.data.myData.currentTarget, Game.PlayerData.PlayerStatus.Dead, true);
                killButton.Timer = killButton.MaxTimer;
                Game.GameData.data.myData.currentTarget = null;
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
        killButton.MaxTimer = killCoolDownOption.getFloat();
        killButton.SetButtonCoolDownOption(true);
    }

    public override void MeetingUpdate(MeetingHud __instance, TMPro.TextMeshPro meetingInfo)
    {

        int count = Game.GameData.data.myData.getGlobalData().GetRoleData(votesId);

        countText.text = numOfVote.ToString() + "/" + count;

    }

    public override void LoadOptionData()
    {
        minVoteOption = CreateOption(Color.white, "minVote", 0f, 0f, 20f, 1f);
        maxVoteOption = CreateOption(Color.white, "maxVote", 5f, 0f, 20f, 1f);
        maxVoteStockOption = CreateOption(Color.white, "maxVoteStock", 5f, 0f, 20f, 1f);
        killCoolDownOption = CreateOption(Color.white, "killCoolDown", 20f, 10f, 60f, 2.5f);
        killCoolDownOption.suffix = "second";
    }

    public override void OnRoleRelationSetting()
    {
        RelatedRoles.Add(Roles.Opportunist);
    }

    public override void MyPlayerControlUpdate()
    {
        Game.MyPlayerData data = Game.GameData.data.myData;

        data.currentTarget = Patches.PlayerControlPatch.SetMyTarget(
            (player) =>
            {
                if (player.Object.inVent) return false;
                if (player.GetModData().role.side == Side.Jackal)
                {
                    return false;
                }
                else if (player.GetModData().HasExtraRole(Roles.SecondarySidekick))
                {
                    return false;
                }
                return true;
            });

        Patches.PlayerControlPatch.SetPlayerOutline(data.currentTarget, RoleColor);
    }

    public override void OnKillPlayer(byte targetId)
    {
        RPCEventInvoker.AddAndUpdateRoleData(PlayerControl.LocalPlayer.PlayerId, votesId, 1);
    }

    public override void EditDisplayNameColor(byte playerId, ref Color displayColor) => Roles.Jackal.EditDisplayNameColor(playerId,ref displayColor);
    /*
    {
        if (PlayerControl.LocalPlayer.GetModData().role.side == Side.Jackal)
        {
            displayColor = RoleColor;
        }
        else if (PlayerControl.LocalPlayer.GetModData().HasExtraRole(Roles.SecondarySidekick))
        {
            displayColor = RoleColor;
        }
    }
    */

    public override void CleanUp()
    {
        if(killButton != null)
        {
            killButton.Destroy();
            killButton = null;
        }
    }


    public JackalMayor()
        : base("JackalMayor", "jackalMayor", RoleColor, RoleCategory.Neutral, Side.Jackal, Side.Jackal,
             new HashSet<Side> { Side.Jackal }, new HashSet<Side> { Side.Jackal }, new HashSet<Patches.EndCondition> { Patches.EndCondition.JackalWin },
             true, VentPermission.CanNotUse, false, false, true)
    {
        votesId = Game.GameData.RegisterRoleDataId("mayor.votes");
    }
}
