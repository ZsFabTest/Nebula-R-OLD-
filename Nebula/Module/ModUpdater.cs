using BepInEx.Configuration;
using UnityEngine.UI;
using System.Reflection;
using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Nebula.Patches;

namespace Nebula.Module;

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
public class ModUpdaterButton
{
    private static GameObject GenerateButton(GameObject template, Color color, string text, System.Action? action, bool mirror, ref int buttons)
    {
        buttons++;

        var button = UnityEngine.Object.Instantiate(template, null);
        button.transform.localPosition = new Vector3(mirror ? -button.transform.localPosition.x : button.transform.localPosition.x, button.transform.localPosition.y + ((float)buttons * 0.6f), button.transform.localPosition.z);

        var renderer = button.gameObject.GetComponent<SpriteRenderer>();
        renderer.color = color;

        var child = button.transform.GetChild(0);
        child.GetComponent<TextTranslatorTMP>().enabled = false;
        var tmpText = child.GetComponent<TMPro.TMP_Text>();
        tmpText.SetText(Language.Language.GetString(text));
        tmpText.color = color;

        PassiveButton passiveButton = button.GetComponent<PassiveButton>();
        passiveButton.OnClick = new Button.ButtonClickedEvent();
        if (action != null) passiveButton.OnClick.AddListener(action);
        passiveButton.OnMouseOut.AddListener((UnityEngine.Events.UnityAction)(() => renderer.color = tmpText.color));

        AspectPosition aspectPosition = button.GetComponent<AspectPosition>();
        if (mirror) aspectPosition.Alignment = AspectPosition.EdgeAlignments.RightBottom;
        aspectPosition.DistanceFromEdge = new Vector3(0.6f, 0.35f + 0.6f * buttons, -5f);


        return button;
    }

    private static void Prefix(MainMenuManager __instance)
    {
        var template = GameObject.Find("ExitGameButton");
        int mirrorButtons = -1;
        GenerateButton(template, new Color(86f / 255f, 97f / 255f, 234f / 255f), "title.button.github", () => Application.OpenURL("https://github.com/ZsFabTest/Nebula-R"), true, ref mirrorButtons);
    }
} 