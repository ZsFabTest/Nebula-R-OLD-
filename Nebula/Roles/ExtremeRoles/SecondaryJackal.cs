namespace Nebula.Roles.ExtraRoles;

public class SecondaryJackal : ExtraRole{
    private Module.CustomOption impostorCanBeSecondaryJackalOption;
    //public Module.CustomOption IgnoringNumOfJackalOption;

    public override void LoadOptionData(){
        impostorCanBeSecondaryJackalOption = CreateOption(Color.white,"impostorCanBeSecondaryJackal",false);
        //IgnoringNumOfJackalOption = CreateOption(Color.white,"IgnoringJackal",false);
    }

    public override void EditDisplayRoleName(byte playerId, ref string roleName, bool isIntro)
    => EditDisplayRoleNameForcely(playerId, ref roleName);

    public override void EditDisplayRoleNameForcely(byte playerId, ref string displayName)
    {
        displayName = Helpers.cs(Color, Language.Language.GetString("role.secondaryJackal.prefix")) + displayName;
    }

    public override bool HasCrewmateTask(byte playerId)
    {
        return false;
    }

    public override bool HasExecutableFakeTask(byte playerId)
    {
        return true;
    }

    public override bool CheckAdditionalWin(PlayerControl player, Patches.EndCondition condition)
    {
        return Roles.Jackal.winReasons.Contains(condition);
    }

    public override void EditSpawnableRoleShower(ref string suffix, Role role)
    {
        if (IsSpawnable() && role.CanHaveExtraAssignable(this)){
            if(role.side == Side.Impostor && !impostorCanBeSecondaryJackalOption.getBool()) return;
            suffix += Helpers.cs(Color, "J");
        }
    }

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag)
    {
        bool showFlag = Game.GameData.data.myData.CanSeeEveryoneInfo;

        if (PlayerControl.LocalPlayer.PlayerId == playerId) showFlag = true;
        else
        {
            if (PlayerControl.LocalPlayer.GetModData().role.side == Side.Jackal || PlayerControl.LocalPlayer.GetModData().HasExtraRole(Roles.SecondarySidekick) || PlayerControl.LocalPlayer.GetModData().HasExtraRole(Roles.SecondaryJackal))
            {
                    showFlag = true;
            }
        }

        if (showFlag) EditDisplayNameForcely(playerId, ref displayName);
    }

    public override void EditDisplayNameColor(byte playerId, ref Color displayColor) => Roles.Jackal.EditDisplayNameColor(playerId,ref displayColor);

    private void _sub_Assignment(Patches.AssignMap assignMap, List<byte> players, int count)
    {
        int chance = RoleChanceOption.getSelection() + 1;

        byte playerId;
        for (int i = 0; i < count; i++)
        {
            //割り当てられない場合終了
            if (players.Count == 0) return;

            if (chance <= NebulaPlugin.rnd.Next(10)) continue;

            playerId = players[NebulaPlugin.rnd.Next(players.Count)];
            if (Helpers.playerById(playerId).GetModData().HasExtraRole(Roles.SecondaryMadmate)){
                i--;
                players.Remove(playerId);
                continue;
            }
            assignMap.AssignExtraRole(playerId, id, 0);
            players.Remove(playerId);
        }
    }

    public override void Assignment(Patches.AssignMap assignMap)
    {
        //Debug.LogWarningFormat("SecondaryJackal: " + IsSpawnable().ToString());
        if(!IsSpawnable()) return;
        
        List<byte> players = new List<byte>();

        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (!player.GetModData()?.role.CanHaveExtraAssignable(this) ?? true) continue;
            if (player.GetModData()?.role == Roles.Player){
                players.Add(player.PlayerId);
                continue;
            }

            switch (player.GetModData()?.role.category)
            {
                case RoleCategory.Crewmate:
                    players.Add(player.PlayerId);
                    break;
                case RoleCategory.Impostor:
                    if(!impostorCanBeSecondaryJackalOption.getBool()) break;
                    players.Add(player.PlayerId);
                    break;
            }
        }

        _sub_Assignment(assignMap, players, (int)RoleCountOption.getFloat());
    }


    public override Module.CustomOption? RegisterAssignableOption(Role role)
    {
        if (role.category == RoleCategory.Neutral) return null;

        Module.CustomOption option = role.CreateOption(new Color(0.8f, 0.95f, 1f), "option.canBeSecondaryJackal", role.DefaultExtraAssignableFlag(this), true).HiddenOnDisplay(true).SetIdentifier("role." + role.LocalizeName + ".canBeSecondaryJackal");
        option.AddPrerequisite(CustomOptionHolder.advanceRoleOptions);
        option.AddCustomPrerequisite(() => { return IsSpawnable(); });
        return option;
    }

    public SecondaryJackal() : base("SecondaryJackal","secondaryJackal",NeutralRoles.Jackal.RoleColor,0){
    }
}