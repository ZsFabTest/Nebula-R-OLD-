namespace Nebula.Roles.ImpostorRoles{
    public class Marker : Template.TImpostor{

        private static Module.CustomOption onlyImpCanSee;

        public override void LoadOptionData(){
            TopOption.tab = Module.CustomOptionTab.GhostRoles;
            onlyImpCanSee = CreateOption(Color.white,"onlyImpCanSee",true);
        }

        private static GameObject guesserUI;

        public static void guesserOnClick(int buttonTarget, MeetingHud __instance)
        {
        if (__instance.CurrentState == MeetingHud.VoteStates.Discussion) return;

        PlayerControl target = Helpers.playerById((byte)__instance.playerStates[buttonTarget].TargetPlayerId);
        if (target == null || target.Data.IsDead) return;

        if (guesserUI != null || !(__instance.state == MeetingHud.VoteStates.Voted || __instance.state == MeetingHud.VoteStates.NotVoted)) return;
        __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(false));

        Transform container = UnityEngine.Object.Instantiate(__instance.transform.FindChild("MeetingContents/PhoneUI"), __instance.transform);
        container.transform.localPosition = new Vector3(0, 0, -50f);
        guesserUI = container.gameObject;

        int i = 0;
        var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
        var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
        var smallButtonTemplate = __instance.playerStates[0].Buttons.transform.Find("CancelButton");
        var textTemplate = __instance.playerStates[0].NameText;

        Transform exitButtonParent = (new GameObject()).transform;
        exitButtonParent.SetParent(container);
        Transform exitButton = UnityEngine.Object.Instantiate(buttonTemplate.transform, exitButtonParent);
        Transform exitButtonMask = UnityEngine.Object.Instantiate(maskTemplate, exitButtonParent);
        exitButton.gameObject.GetComponent<SpriteRenderer>().sprite = smallButtonTemplate.GetComponent<SpriteRenderer>().sprite;
        exitButtonParent.transform.localPosition = new Vector3(2.725f, 2.1f, -5);
        exitButtonParent.transform.localScale = new Vector3(0.25f, 0.9f, 1);
        exitButton.GetComponent<PassiveButton>().OnClick.RemoveAllListeners();
        exitButton.GetComponent<PassiveButton>().OnClick.AddListener((UnityEngine.Events.UnityAction)(() =>
        {
            __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
            UnityEngine.Object.Destroy(container.gameObject);
        }));

        List<Transform> buttons = new List<Transform>();
        Transform selectedButton = null;

        foreach (Role role in Roles.AllRoles)
        {
            //撃てないロールを除外する
            if (role.category == RoleCategory.Complex) continue;
            if (role == Roles.Player) break;
            if (role.side != Side.Crewmate) continue;

            Transform buttonParent = (new GameObject()).transform;
            buttonParent.SetParent(container);
            Transform button = UnityEngine.Object.Instantiate(buttonTemplate, buttonParent);
            Transform buttonMask = UnityEngine.Object.Instantiate(maskTemplate, buttonParent);
            TMPro.TextMeshPro label = UnityEngine.Object.Instantiate(textTemplate, button);
            buttons.Add(button);
            int row = i / 6, col = i % 6;
            buttonParent.localPosition = new Vector3(-3.5f + 1.4f * col, 1.5f - 0.37f * row, -5);
            buttonParent.localScale = new Vector3(0.5f, 0.5f, 1f);
            label.text = Helpers.cs(role.Color, Language.Language.GetString("role." + role.LocalizeName + ".name"));
            label.alignment = TMPro.TextAlignmentOptions.Center;
            label.transform.localPosition = new Vector3(0, 0, label.transform.localPosition.z);
            label.transform.localScale *= 1.7f;
            int copiedIndex = i;

            button.GetComponent<PassiveButton>().OnClick.RemoveAllListeners();
            if (!PlayerControl.LocalPlayer.Data.IsDead) button.GetComponent<PassiveButton>().OnClick.AddListener((System.Action)(() =>
            {
                if (selectedButton != button)
                {
                    selectedButton = button;
                    buttons.ForEach(x => x.GetComponent<SpriteRenderer>().color = x == selectedButton ? Color.red : Color.white);
                }
                else
                {
                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                        UnityEngine.Object.Destroy(container.gameObject);
                        return;
                    }

                    PlayerControl focusedTarget = Helpers.playerById((byte)__instance.playerStates[buttonTarget].TargetPlayerId);
                    RPCEventInvoker.SetRoleInfo(focusedTarget,Helpers.cs(role.Color,Language.Language.GetString("role." + role.LocalizeName + ".name")),onlyImpCanSee.getBool());

                    // Reset the GUI
                    __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                    UnityEngine.Object.Destroy(container.gameObject);
                }
            }));

            i++;
        }

        FastDestroyableSingleton<HatManager>.Instance.GetNamePlateById("nameplate_NoPlate").CoLoadViewData((Il2CppSystem.Action<NamePlateViewData>)((n) =>
        {
            foreach (var b in buttons)
                b.GetComponent<SpriteRenderer>().sprite = n.Image;
        }));

        container.transform.localScale *= 0.85f * 0.85f;

        for (int index = 0; index < 3; index++)
        {
            Transform TeambuttonParent = new GameObject().transform;
            TeambuttonParent.SetParent(container);
            Transform Teambutton = UnityEngine.Object.Instantiate(buttonTemplate, TeambuttonParent);
            Teambutton.FindChild("ControllerHighlight").gameObject.SetActive(false);
            Transform TeambuttonMask = UnityEngine.Object.Instantiate(maskTemplate, TeambuttonParent);
            TMPro.TextMeshPro Teamlabel = UnityEngine.Object.Instantiate(textTemplate, Teambutton);
            Teambutton.GetComponent<SpriteRenderer>().sprite = FastDestroyableSingleton<HatManager>.Instance.GetNamePlateById("nameplate_NoPlate")?.viewData?.viewData?.Image;
            TeambuttonParent.localPosition = new(-2.75f + (index * 1.75f), 2.225f, -200);
            TeambuttonParent.localScale = new(0.55f, 0.55f, 1f);
            Teamlabel.color = index is 0 ? Palette.CrewmateBlue : index is 1 ? Palette.ImpostorRed : Roles.ChainShifter.Color;
            string text = index is 0 ? "crewmate" : (index is 1 ? "impostor" : "neutral");
            Teamlabel.text = Language.Language.GetString("guess.team." + text);
            Teamlabel.alignment = TMPro.TextAlignmentOptions.Center;
            Teamlabel.transform.localPosition = new Vector3(0, 0, Teamlabel.transform.localPosition.z);
            Teamlabel.transform.localScale *= 1.6f;
            Teamlabel.autoSizeTextContainer = true;
            int copiedIndex = index;
            Teambutton.GetComponent<PassiveButton>().OnClick.RemoveAllListeners();
            if (!PlayerControl.LocalPlayer.Data.IsDead) Teambutton.GetComponent<PassiveButton>().OnClick.AddListener((System.Action)(() => {
                i = 0;
                selectedButton = null;
                foreach(Transform button in buttons) UnityEngine.Object.Destroy(button.gameObject);
                buttons = new List<Transform>();
                foreach (Role role in Roles.AllRoles)
                {
                    //撃てないロールを除外する
                    if (role.category == RoleCategory.Complex) continue;
                    if (role == Roles.Player) break;
                    if (copiedIndex is 0 && role.side != Side.Crewmate) continue;
                    else if (copiedIndex is 1 && role.side != Side.Impostor) continue;
                    else if (copiedIndex is 2 && (role.side == Side.Impostor || role.side == Side.Crewmate)) continue;

                    //Debug.LogWarningFormat(copiedIndex.ToString() + " + " + role.Name);

                    Transform buttonParent = (new GameObject()).transform;
                    buttonParent.SetParent(container);
                    Transform button = UnityEngine.Object.Instantiate(buttonTemplate, buttonParent);
                    Transform buttonMask = UnityEngine.Object.Instantiate(maskTemplate, buttonParent);
                    TMPro.TextMeshPro label = UnityEngine.Object.Instantiate(textTemplate, button);
                    buttons.Add(button);
                    int row = i / 6, col = i % 6;
                    buttonParent.localPosition = new Vector3(-3.5f + 1.4f * col, 1.5f - 0.37f * row, -5);
                    buttonParent.localScale = new Vector3(0.5f, 0.5f, 1f);
                    label.text = Helpers.cs(role.Color, Language.Language.GetString("role." + role.LocalizeName + ".name"));
                    label.alignment = TMPro.TextAlignmentOptions.Center;
                    label.transform.localPosition = new Vector3(0, 0, label.transform.localPosition.z);
                    label.transform.localScale *= 1.7f;

                    button.GetComponent<PassiveButton>().OnClick.RemoveAllListeners();
                    if (!PlayerControl.LocalPlayer.Data.IsDead) button.GetComponent<PassiveButton>().OnClick.AddListener((System.Action)(() =>
                    {
                        if (selectedButton != button)
                        {
                            selectedButton = button;
                            buttons.ForEach(x => x.GetComponent<SpriteRenderer>().color = x == selectedButton ? Color.red : Color.white);
                        }
                        else
                        {
                            if (PlayerControl.LocalPlayer.Data.IsDead)
                            {
                                __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                                UnityEngine.Object.Destroy(container.gameObject);
                                return;
                            }

                            PlayerControl focusedTarget = Helpers.playerById((byte)__instance.playerStates[buttonTarget].TargetPlayerId);
                            RPCEventInvoker.SetRoleInfo(focusedTarget,Helpers.cs(role.Color,Language.Language.GetString("role." + role.LocalizeName + ".name")),onlyImpCanSee.getBool());

                            // Reset the GUI
                            __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                            UnityEngine.Object.Destroy(container.gameObject);
                        }
                    }));

                    i++;
                }

                FastDestroyableSingleton<HatManager>.Instance.GetNamePlateById("nameplate_NoPlate").CoLoadViewData((Il2CppSystem.Action<NamePlateViewData>)((n) =>
                {
                    foreach (var b in buttons)
                        b.GetComponent<SpriteRenderer>().sprite = n.Image;
                }));
            }));
        }
        }

        public override void SetupMeetingButton(MeetingHud __instance)
        {
            for (int i = 0; i < __instance.playerStates.Length; i++)
            {
                PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                if(playerVoteArea.AmDead) continue;
                GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                GameObject targetBox = UnityEngine.Object.Instantiate(template, playerVoteArea.transform);
                targetBox.name = "ShootButton";
                targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1f);
                SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
                renderer.sprite = ComplexRoles.FGuesser.targetSprite.GetSprite();
                PassiveButton button = targetBox.GetComponent<PassiveButton>();
                button.OnClick.RemoveAllListeners();
                int copiedIndex = i;
                button.OnClick.AddListener((UnityEngine.Events.UnityAction)(() => guesserOnClick(copiedIndex, __instance)));
            }
        }

        public Marker() : base("Marker","marker",true){
            CanCallEmergencyMeeting = false;
        }
    }
}