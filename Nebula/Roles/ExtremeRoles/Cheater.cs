namespace Nebula.Roles.ExtraRoles;

public class Cheater : ExtraRole{
	public static Color RoleColor = new Color(66f / 255f,119f / 255f,230f / 255f);

	private Module.CustomOption maxLoversOption;

	public override void LoadOptionData(){
		maxLoversOption = CreateOption(Color.white,"maxLovers",2f,1f,10f,1f);
	}

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

            players.Add(player.PlayerId);
        }

        _sub_Assignment(assignMap, players, (int)RoleCountOption.getFloat());
    }

	public override void Initialize(PlayerControl __instance){
		List<PlayerControl> players = PlayerControl.AllPlayerControls.ToArray().ToList();
        PlayerControl player;
        for (int i = 0; i < maxLoversOption.getFloat(); i++)
        {
            if (players.Count == 0) return;

            if (5 <= NebulaPlugin.rnd.Next(10)) continue;

            player = players[NebulaPlugin.rnd.Next(players.Count)];
            if(player.PlayerId == PlayerControl.LocalPlayer.PlayerId){
                players.Remove(player);
                continue;
            }
            RPCEventInvoker.AddExtraRole(player,Roles.FakeLover,PlayerControl.LocalPlayer.PlayerId);
            players.Remove(player);
        }
	}

    public override void EditSpawnableRoleShower(ref string suffix, Role role)
    {
        if (IsSpawnable() && role.CanHaveExtraAssignable(this))
        {
            suffix += Helpers.cs(RoleColor, "ξ");
        }
    }

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag)
    {
        bool showFlag = false;
        if (playerId == PlayerControl.LocalPlayer.PlayerId || Game.GameData.data.myData.CanSeeEveryoneInfo) showFlag = true;

        if (showFlag) EditDisplayNameForcely(playerId, ref displayName);
    }

    public override void EditDisplayNameForcely(byte playerId, ref string displayName)
    {
        displayName += Helpers.cs(RoleColor, "ξ");
    }

    public override void EditDescriptionString(ref string description)
    {
        description += "\n" + Language.Language.GetString("role.cheater.description");
    }

    public override Module.CustomOption? RegisterAssignableOption(Role role)
    {
        Module.CustomOption option = role.CreateOption(new Color(0.8f, 0.95f, 1f), "option.canBeCheater", role.DefaultExtraAssignableFlag(this), true).HiddenOnDisplay(true).SetIdentifier("role." + role.LocalizeName + ".canBeCheater");
        option.AddPrerequisite(CustomOptionHolder.advanceRoleOptions);
        option.AddCustomPrerequisite(() => { return Roles.Cheater.IsSpawnable(); });
        return option;
    }

	public Cheater() : base("Cheater","cheater",RoleColor,0){
	}
}