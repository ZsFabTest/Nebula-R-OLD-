using Nebula.Events;
using Nebula.Patches;
using Nebula.Roles.ComplexRoles;

namespace Nebula.Roles.ExtraRoles;

public class LastImpostor : ExtraRole
{
    public static Color RoleColor = Palette.ImpostorRed;

    //Assignable.RelatedExtraRoleData[] RelatedExtraRoleDataInfo { get => new Assignable.RelatedExtraRoleData[] { new Assignable.RelatedExtraRoleData("Guesser Shot", Roles.SecondaryGuesser, 0, 20) }; }
    public override RelatedExtraRoleData[] RelatedExtraRoleDataInfo => GuesserSystem.RelatedExtraRoleDataInfo;

    public override void Assignment(AssignMap assignMap)
    {
        List<byte> impostors = new List<byte>();
        foreach(PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if(player.GetModData().role.side == Side.Impostor)
            {
                impostors.Add(player.PlayerId);
            }
        }
        operation(assignMap,impostors);
    }

    private void operation(AssignMap assignMap,List<byte> players)
    {
        foreach(byte playerId in players) assignMap.AssignExtraRole(playerId, id, 0);
    }

    public bool cmp()
    {
        List<PlayerControl> impostors = new List<PlayerControl>();
        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player.GetModData().role.side == Side.Impostor)
            {
                impostors.Add(player);
            }
        }
        foreach (PlayerControl impostor in impostors)
        {
            if (!(impostor.PlayerId == PlayerControl.LocalPlayer.PlayerId) && impostor.GetModData().IsAlive)
            {
                return false;
            }
        }
        return true;
    }

    public override void GlobalInitialize(PlayerControl __instance)
    {
        if(cmp()) GuesserSystem.GlobalInitialize(__instance);
    }

    public override void SetupMeetingButton(MeetingHud __instance)
    {
        if (cmp()) GuesserSystem.SetupMeetingButton(__instance);
    }

    public override void MeetingUpdate(MeetingHud __instance, TMPro.TextMeshPro meetingInfo)
    {
        if (cmp()) GuesserSystem.MeetingUpdate(__instance, meetingInfo);
    }

    public override void EditDescriptionString(ref string description)
    {
        if (cmp()) description += "\n" + Language.Language.GetString("role.LastImpostor.description");
    }

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag)
    {
        if (playerId == PlayerControl.LocalPlayer.PlayerId || Game.GameData.data.myData.CanSeeEveryoneInfo)
            if (cmp()) EditDisplayNameForcely(playerId, ref displayName);
    }

    public override void EditDisplayNameForcely(byte playerId, ref string displayName)
    {
        if (cmp()) displayName += Helpers.cs(Color, "§");
    }

    public LastImpostor()
        : base("LastImpostor","lastImpostor",RoleColor,0)
    {
    }
}
