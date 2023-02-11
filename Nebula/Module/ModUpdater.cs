using BepInEx.Configuration;
using UnityEngine.UI;
using System.Reflection;
using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Nebula.Patches;

namespace Nebula.Module;

[HarmonyPatch(typeof(AnnouncementPopUp), nameof(AnnouncementPopUp.UpdateAnnounceText))]
public static class AnnouncementPatch
{
    private static bool ShownFlag = false;
    private static ConfigEntry<int> AnnounceVersion = null;
    private static string Announcement = "";

    private static string FormatRoleString(Match match, string str, string key, string defaultString)
    {
        foreach (var role in Roles.Roles.AllRoles)
        {
            if (role.Name.ToUpper() == key)
            {
                str = str.Replace(match.Value, Helpers.cs(role.Color, Language.Language.GetString("role." + role.LocalizeName + ".name")));
                return str;
            }
        }
        foreach (var role in Roles.Roles.AllExtraRoles)
        {
            if (role.Name.ToUpper() == key)
            {
                str = str.Replace(match.Value, Helpers.cs(role.Color, Language.Language.GetString("role." + role.LocalizeName + ".name")));
                return str;
            }
        }

        str = str.Replace(match.Value, defaultString);
        return str;
    }

    private static string FormatString(string str)
    {
        Regex regex;

        //旧式の変換
        foreach (var role in Roles.Roles.AllRoles)
        {
            str = str.Replace("%ROLE:" + role.Name.ToUpper() + "%", Helpers.cs(role.Color, Language.Language.GetString("role." + role.LocalizeName + ".name")));
        }
        foreach (var role in Roles.Roles.AllExtraRoles)
        {
            str = str.Replace("%ROLE:" + role.Name.ToUpper() + "%", Helpers.cs(role.Color, Language.Language.GetString("role." + role.LocalizeName + ".name")));
        }

        regex = new Regex("%ROLE:[A-Z]+\\([a-zA-Z0-9 ]+\\)%");
        foreach (Match match in regex.Matches(str))
        {
            var split = match.Value.Split(':', '(', ')');
            str = FormatRoleString(match, str, split[1], split[2]);
        }

        str = str.Replace("%/COLOR%", "</color>");

        regex = new Regex("%OPTION\\([a-zA-Z\\.0-9]+\\)\\,\\([a-zA-Z\\.0-9 ]+\\)%");
        foreach (Match match in regex.Matches(str))
        {
            var split = match.Value.Split('(', ')');

            str = str.Replace(match.Value,
                Language.Language.CheckValidKey(split[1]) ?
                Language.Language.GetString(split[1]) : split[3]);
        }

        return str;
    }

    public static bool LoadAnnouncement()
    {
        if (AnnounceVersion == null)
        {
            AnnounceVersion = NebulaPlugin.Instance.Config.Bind("Announce", "Version", (int)0);
        }

        HttpClient http = new HttpClient();
        http.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        var response = http.GetAsync(new System.Uri($"https://raw.githubusercontent.com/Dolly1016/Nebula/master/announcement.json"), HttpCompletionOption.ResponseContentRead).Result;


        try
        {
            if (response.StatusCode != HttpStatusCode.OK) return false;
            if (response.Content == null) return false;
            string json = response.Content.ReadAsStringAsync().Result;
            JObject jObj = JObject.Parse(json);
            JToken? version = jObj["Version"];
            if (version == null) return false;
            int Version = int.Parse(version.ToString());

            //既にみたことがあれば出さない
            if (AnnounceVersion.Value == Version)
            {
                ShownFlag = true;
            }
            //更新する
            AnnounceVersion.Value = Version;

            string lang = Language.Language.GetLanguage((uint)AmongUs.Data.DataManager.Settings.Language.CurrentLanguage);
            if (jObj[lang] != null)
                Announcement = jObj[lang].ToString();
            else if (jObj["English"] != null)
                Announcement = jObj["English"].ToString();
            else if (jObj["Japanese"] != null)
                Announcement = jObj["Japanese"].ToString();
            else
            {
                Announcement = "-Invalid Announcement-";
                return false;
            }
            Announcement = FormatString(Announcement);
        }
        catch (System.Exception ex)
        {
        }
        return !ShownFlag;
    }

    public static bool Prefix(AnnouncementPopUp __instance)
    {
        if (!ShownFlag)
        {
            if (LoadAnnouncement())
            {
                AnnouncementPopUp.UpdateState = AnnouncementPopUp.AnnounceState.Success;
                ShownFlag = true;
            }
        }
        else { LoadAnnouncement(); }

        __instance.AnnounceTextMeshPro.text = Announcement;

        return false;
    }
}

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