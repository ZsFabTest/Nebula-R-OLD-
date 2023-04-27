using TMPro;

namespace Nebula.Roles.ExtraRoles;

public class LastImpostor : ExtraRole
{
    public Module.CustomOption canSpawnOption;
    public Module.CustomOption GuessCountOption;

    bool hasGuesserUI = false;

    public override void LoadOptionData()
    {
        TopOption.AddCustomPrerequisite(() => { return true; });
        canSpawnOption = CreateOption(Color.white, "canSpawn", false);
        GuessCountOption = CreateOption(Color.white, "guessCount",5f,1f,15f,1f);
    }

    public override void SetupMeetingButton(MeetingHud __instance){
        ComplexRoles.GuesserSystem.SetupMeetingButton(__instance);
    }

    public override void GlobalInitialize(PlayerControl __instance){
        hasGuesserUI = false;
        __instance.GetModData().SetExtraRoleData(Roles.SecondaryGuesser.id, (ulong)GuessCountOption.getFloat());
        if(PlayerControl.LocalPlayer.GetModData().extraRole.Contains(Roles.SecondaryGuesser)){
            hasGuesserUI = true;
        }
    }

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag)
    {
        if (playerId == PlayerControl.LocalPlayer.PlayerId || Game.GameData.data.myData.CanSeeEveryoneInfo)
            EditDisplayNameForcely(playerId, ref displayName);
    }

    public override void EditDisplayNameForcely(byte playerId, ref string displayName)
    {
        displayName += Helpers.cs(Color, "LI");
    }

    public override void MeetingUpdate(MeetingHud __instance, TextMeshPro meetingInfo)
    {
        if(!hasGuesserUI) ComplexRoles.GuesserSystem.MeetingUpdate(__instance, meetingInfo);
    }

    public override bool IsSpawnable()
    {
        return false;
    }

    public override void MyPlayerControlUpdate()
    {
        if(PlayerControl.LocalPlayer.GetModData().role.side != Side.Impostor) RPCEventInvoker.UnsetExtraRole(PlayerControl.LocalPlayer,this,false);
    }

    public LastImpostor() : base("LastImpostor", "lastImpostor", Palette.ImpostorRed, 0)
    {
    }
}
