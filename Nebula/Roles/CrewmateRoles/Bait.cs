using static Nebula.Roles.CrewmateRoles.Bait;

namespace Nebula.Roles.CrewmateRoles;

public class Bait : Role
{
    static public Color RoleColor = new Color(0f / 255f, 247f / 255f, 255f / 255f);

    public Module.CustomOption killerCanKnowBaitKillByFlash;
    public Module.CustomOption canBeExtraRole;

    public class BaitEvent : Events.LocalEvent
    {
        byte murderId;
        public BaitEvent(byte murderId) : base(0.2f + (float)NebulaPlugin.rnd.NextDouble() * 0.2f)
        {
            this.murderId = murderId;
        }

        public override void OnTerminal()
        {
            RPCEventInvoker.UncheckedCmdReportDeadBody(murderId, PlayerControl.LocalPlayer.PlayerId);
        }
    }

    public override void OnMurdered(byte murderId)
    {
        if (MeetingHud.Instance) return;

        //Baitが発動しない場合
        if (PlayerControl.LocalPlayer.IsMadmate() && PlayerControl.AllPlayerControls[murderId].Data.Role.Role == RoleTypes.Impostor) return;

        //少しの時差の後レポート
        Events.LocalEvent.Activate(new BaitEvent(murderId));
    }

    //キルしたプレイヤーにフラッシュ
    public override void OnDied(byte playerId)
    {
        if (!killerCanKnowBaitKillByFlash.getBool()) return;
        if (!Game.GameData.data.deadPlayers.ContainsKey(playerId)) return;
        if (Game.GameData.data.deadPlayers[playerId].MurderId != PlayerControl.LocalPlayer.PlayerId) return;

        //Baitが発動しない場合
        if (Helpers.playerById(playerId).IsMadmate() && PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.Impostor) return;

        Helpers.PlayQuickFlash(Color);
    }

    public override void OnRoleRelationSetting()
    {
        RelatedRoles.Add(Roles.Spy);
        RelatedRoles.Add(Roles.Madmate);
    }

    public override void PreloadOptionData()
    {
        defaultUnassignable.Add(Roles.Lover);
    }

    public override void LoadOptionData()
    {
        TopOption.tab = Module.CustomOptionTab.CrewmateRoles | Module.CustomOptionTab.Modifiers;
        killerCanKnowBaitKillByFlash = CreateOption(Color.white, "killerCanKnowBaitKillByFlash", true);
        canBeExtraRole = CreateOption(Color.white, "canBeExtraRole", false);
    }

    public override bool IsUnsuitable { get { return canBeExtraRole.getBool(); } }

    public override void SpawnableTest(ref Dictionary<Role, int> DefinitiveRoles, ref HashSet<Role> SpawnableRoles)
    {
        if (canBeExtraRole.getBool()) return;
        base.SpawnableTest(ref DefinitiveRoles, ref SpawnableRoles);
    }

    public Bait()
        : base("Bait", "bait", RoleColor, RoleCategory.Crewmate, Side.Crewmate, Side.Crewmate,
             Crewmate.crewmateSideSet, Crewmate.crewmateSideSet, Crewmate.crewmateEndSet,
             false, VentPermission.CanNotUse, false, false, false)
    {
    }
}

public class SecondaryBait : ExtraRole
{
    public SecondaryBait() : base("Bait", "bait", RoleColor, 0)
    {
        IsHideRole = true;
    }

    private void _sub_Assignment(Patches.AssignMap assignMap, List<byte> players, int count)
    {
        int chance = Roles.Bait.RoleChanceOption.getSelection() + 1;

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
        if (!Roles.Bait.canBeExtraRole.getBool()) return;

        List<byte> crewmates = new List<byte>();

        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (!player.GetModData()?.role.CanHaveExtraAssignable(this) ?? true) continue;

            switch (player.GetModData()?.role.category)
            {
                case RoleCategory.Crewmate:
                    crewmates.Add(player.PlayerId);
                    break;
            }
        }

        _sub_Assignment(assignMap, crewmates, (int)Roles.Bait.RoleCountOption.getFloat());
    }

    public override void EditDescriptionString(ref string description)
    {
        description += "\n" + Language.Language.GetString("role.bait.description");
    }

    public override void EditDisplayName(byte playerId, ref string displayName, bool hideFlag)
    {
        bool showFlag = false;
        if (playerId == PlayerControl.LocalPlayer.PlayerId || Game.GameData.data.myData.CanSeeEveryoneInfo) showFlag = true;

        if (showFlag) EditDisplayNameForcely(playerId, ref displayName);
    }

    public override void EditDisplayNameForcely(byte playerId, ref string displayName)
    {
        displayName += Helpers.cs(
                Bait.RoleColor, "®");
    }

    public override void EditSpawnableRoleShower(ref string suffix, Role role)
    {
        if (IsSpawnable() && role.CanHaveExtraAssignable(this)) suffix += Helpers.cs(Color, "®");
    }

    public override void OnMurdered(byte murderId)
    {
        if (MeetingHud.Instance) return;

        //Baitが発動しない場合
        if (PlayerControl.LocalPlayer.IsMadmate() && PlayerControl.AllPlayerControls[murderId].Data.Role.Role == RoleTypes.Impostor) return;

        //少しの時差の後レポート
        Events.LocalEvent.Activate(new BaitEvent(murderId));
    }

    //キルしたプレイヤーにフラッシュ
    public override void OnDied(byte playerId)
    {
        if (!Roles.Bait.killerCanKnowBaitKillByFlash.getBool()) return;
        if (!Game.GameData.data.deadPlayers.ContainsKey(playerId)) return;
        if (Game.GameData.data.deadPlayers[playerId].MurderId != PlayerControl.LocalPlayer.PlayerId) return;

        //Baitが発動しない場合
        if (Helpers.playerById(playerId).IsMadmate() && PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.Impostor) return;

        Helpers.PlayQuickFlash(Color);
    }

    public override bool IsSpawnable()
    {
        return Roles.Bait.canBeExtraRole.getBool();
    }

    public override Module.CustomOption? RegisterAssignableOption(Role role)
    {
        if (role.category == RoleCategory.Impostor || role.category == RoleCategory.Neutral) return null;
        Module.CustomOption option = role.CreateOption(new Color(0.8f, 0.95f, 1f), "option.canBeBait", role.DefaultExtraAssignableFlag(this), true).HiddenOnDisplay(true).SetIdentifier("role." + role.LocalizeName + ".canBeBait");
        option.AddPrerequisite(CustomOptionHolder.advanceRoleOptions);
        option.AddCustomPrerequisite(() => { return Roles.SecondaryBait.IsSpawnable(); });
        return option;
    }
}