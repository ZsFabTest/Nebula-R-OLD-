﻿using Nebula.Patches;
using System.Reflection.Metadata.Ecma335;

namespace Nebula.Roles;

public class Side
{
    public enum IntroDisplayOption
    {
        STANDARD,
        SHOW_ALL,
        SHOW_ONLY_ME,
        Yanderes
    }

    public static Side Crewmate = new Side("Crewmate", "crewmate", IntroDisplayOption.SHOW_ALL, Palette.CrewmateBlue, (PlayerStatistics statistics, ShipStatus status) =>
    {
        if (GameOptionsManager.Instance.currentGameMode == GameModes.Normal)
        {
            if (statistics.GetAlivePlayers(Impostor) == 0 &&
            statistics.GetAlivePlayers(Jackal) == 0 &&
            statistics.GetAlivePlayers(Pavlov) == 0 &&
            statistics.GetAlivePlayers(Moriarty) == 0 &&
            !Game.GameData.data.AllPlayers.Values.Any((p) => p.IsAlive && (p.extraRole.Contains(Roles.SecondarySidekick) && p.extraRole.Contains(Roles.SecondaryJackal))))
            {
                return EndCondition.CrewmateWinByVote;
            }
            if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks && GameData.Instance.CompletedTasks > 0)
            {
                return EndCondition.CrewmateWinByTask;
            }
        }else if (GameOptionsManager.Instance.currentGameMode == GameModes.HideNSeek)
        {
            if (GameManager.Instance.Cast<HideAndSeekManager>().LogicFlowHnS.AllTimersExpired())
                return EndCondition.CrewmateWinHnS;
            if (statistics.AliveCrewmates > 0 && statistics.AliveImpostors == 0)
                return EndCondition.CrewmateWinHnS;
        }
        return null;
    });

    public static Side Impostor = new Side("Impostor", "impostor", IntroDisplayOption.STANDARD, Palette.ImpostorRed, (PlayerStatistics statistics, ShipStatus status) =>
    {
        if(statistics.AliveImpostors == 1 && Roles.LastImpostor.canSpawnOption.getBool()){
            foreach(PlayerControl p in PlayerControl.AllPlayerControls){
                if(!p.Data.IsDead && p.GetModData().role.side == Side.Impostor && !p.GetModData().extraRole.Contains(Roles.LastImpostor)){
                    RPCEventInvoker.AddExtraRole(p,Roles.LastImpostor,0);
                    break;
                }
            }
        }
        if (GameOptionsManager.Instance.currentGameMode == GameModes.Normal)
        {
            if (Game.GameData.data != null)
            {
                int aliveUnawakened = 0;
                foreach (var p in Game.GameData.data.AllPlayers.Values)
                {
                    if (p.IsAlive && p.role == Roles.Covert && p.GetRoleData(Roles.Covert.canKillId) == 0) aliveUnawakened++;
                }
                if (aliveUnawakened == statistics.AliveImpostors && aliveUnawakened>0)
                {
                    List<Game.PlayerData> candidate = new List<Game.PlayerData>(Game.GameData.data.AllPlayers.Values.Where((p) => p.role == Roles.Covert));
                    RPCEventInvoker.UpdateRoleData(candidate[NebulaPlugin.rnd.Next(candidate.Count)].id, Roles.Covert.canKillId, 1);
                }
            }

            //Sabotage
            if (status.Systems != null)
            {
                ISystemType systemType = status.Systems.ContainsKey(SystemTypes.LifeSupp) ? status.Systems[SystemTypes.LifeSupp] : null;
                if (systemType != null)
                {
                    LifeSuppSystemType lifeSuppSystemType = systemType.TryCast<LifeSuppSystemType>();
                    if (lifeSuppSystemType != null && lifeSuppSystemType.Countdown < 0f)
                    {
                        lifeSuppSystemType.Countdown = 10000f;
                        return EndCondition.ImpostorWinBySabotage;
                    }
                }
                ISystemType systemType2 = status.Systems.ContainsKey(SystemTypes.Reactor) ? status.Systems[SystemTypes.Reactor] : null;
                if (systemType2 == null)
                {
                    systemType2 = status.Systems.ContainsKey(SystemTypes.Laboratory) ? status.Systems[SystemTypes.Laboratory] : null;
                }
                if (systemType2 != null)
                {
                    ICriticalSabotage criticalSystem = systemType2.TryCast<ICriticalSabotage>();
                    if (criticalSystem != null && criticalSystem.Countdown < 0f)
                    {
                        criticalSystem.ClearSabotage();
                        return EndCondition.ImpostorWinBySabotage;
                    }
                }
            }

            if (statistics.AliveImpostors > 0 &&
            statistics.AliveJackals == 0 &&
            statistics.AlivePavlov == 0 &&
            statistics.AliveMoriarty == 0 &&
            !Game.GameData.data.AllPlayers.Values.Any((p) => p.IsAlive && (p.extraRole.Contains(Roles.SecondarySidekick) || p.extraRole.Contains(Roles.SecondaryJackal))) &&
            (statistics.TotalAlive - statistics.AliveSpectre - statistics.AliveMadmate) <= (statistics.AliveImpostors - statistics.AliveInLoveImpostors) * 2 &&
            (statistics.AliveImpostorCouple + statistics.AliveImpostorTrilemma == 0 ||
            statistics.AliveImpostorCouple * 2 + statistics.AliveImpostorTrilemma * 3 >= statistics.AliveCouple * 2 + statistics.AliveTrilemma * 3))
            {
                if (TempData.LastDeathReason == DeathReason.Kill)
                {
                    return EndCondition.ImpostorWinByKill;
                }
                else
                {
                    return EndCondition.ImpostorWinByVote;
                }
            }
        }
        else if (GameOptionsManager.Instance.currentGameMode == GameModes.HideNSeek)
        {
            if (statistics.AliveCrewmates == 0 && statistics.AliveImpostors>0) return EndCondition.ImpostorWinHnS;
        }

        return null;
    });

    public static Side Jackal = new Side("Jackal", "jackal", IntroDisplayOption.STANDARD, NeutralRoles.Jackal.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        if ((statistics.AliveJackals - statistics.AliveInLoveJackals) * 2 >= (statistics.TotalAlive-statistics.AliveSpectre) && statistics.GetAlivePlayers(Impostor) - statistics.AliveImpostorsWithSidekick <= 0 && statistics.GetAlivePlayers(Pavlov) == 0 &&
        (statistics.AliveJackalCouple + statistics.AliveJackalTrilemma == 0 ||
        statistics.AliveJackalCouple * 2 + statistics.AliveJackalTrilemma * 3 >= statistics.AliveCouple * 2 + statistics.AliveTrilemma * 3))
        {
            return EndCondition.JackalWin;
        }
        return null;
    });

    public static Side Jester = new Side("Jester", "jester", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.Jester.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        if (Roles.Jester.WinTrigger)
        {
            return EndCondition.JesterWin;
        }
        return null;
    });

    public static Side Vulture = new Side("Vulture", "vulture", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.Vulture.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        if (Roles.Vulture.WinTrigger)
        {
            return EndCondition.VultureWin;
        }
        return null;
    });

    public static Side Arsonist = new Side("Arsonist", "arsonist", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.Arsonist.RoleColor, (PlayerStatistics statistics, ShipStatus side) =>
    {
        if (Roles.Arsonist.WinTrigger)
        {
            return EndCondition.ArsonistWin;
        }
        return null;
    });

    public static Side Empiric = new Side("Empiric", "empiric", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.Empiric.RoleColor, (PlayerStatistics statistics, ShipStatus side) =>
    {
        if (Roles.Empiric.WinTrigger)
        {
            return EndCondition.EmpiricWin;
        }
        return null;
    });

    public static Side Paparazzo = new Side("Paparazzo", "paparazzo", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.Paparazzo.RoleColor, (PlayerStatistics statistics, ShipStatus side) =>
    {
        if (Roles.Paparazzo.WinTrigger)
        {
            return EndCondition.PaparazzoWin;
        }
        return null;
    });

    public static Side Opportunist = new Side("Opportunist", "opportunist", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.Opportunist.RoleColor, (PlayerStatistics statistics, ShipStatus side) =>
    {
        return null;
    });

    public static Side Avenger = new Side("Avenger", "avenger", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.Avenger.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        return null;
    }, (EndCondition endCondition, PlayerStatistics statistics, ShipStatus status) =>
    {
        if (endCondition.IsNoBodyWinEnd) return null;
        if (Roles.Avenger.canTakeOverSabotageWinOption.getBool() && endCondition == EndCondition.ImpostorWinBySabotage) return null;

        foreach (var player in Game.GameData.data.AllPlayers.Values)
        {
            if (!player.IsAlive) continue;
            if (player.role != Roles.Avenger) continue;

            if (player.GetRoleData(Roles.Avenger.avengerCheckerId) == 1)
                return EndCondition.AvengerWin;
        }
        return null;
    });

    public static Side Spectre = new Side("Spectre", "spectre", IntroDisplayOption.STANDARD, NeutralRoles.Spectre.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        if (Roles.Spectre.canTakeOverTaskWinOption.getBool()) return null;

        bool flag = false;
        foreach (var player in Game.GameData.data.AllPlayers.Values)
        {
            if (!player.IsAlive) continue;
            if (player.role != Roles.Spectre) continue;
            if(player.Tasks==null || player.Tasks?.Completed==player.Tasks?.AllTasks){ flag = true;  break; }
        }
        if (!flag) return null;

        if(TasksHandler.TotalAlivesTasks > 0 && TasksHandler.CompletedAlivesTasks == TasksHandler.TotalAlivesTasks)return EndCondition.SpectreWin;
        return null;
    }, (EndCondition endCondition, PlayerStatistics statistics, ShipStatus status) =>
    {
        if (endCondition.IsNoBodyWinEnd) return null;

        if (endCondition == EndCondition.CrewmateWinByTask && (Roles.Spectre.canTakeOverTaskWinOption.getBool()||TasksHandler.TotalAlivesTasks==0)) return null;
        if (endCondition == EndCondition.ImpostorWinBySabotage && Roles.Spectre.canTakeOverSabotageWinOption.getBool()) return null;
        if (endCondition == EndCondition.ArsonistWin) return null;
        if (endCondition == EndCondition.EmpiricWin) return null;
        if (endCondition == EndCondition.JesterWin) return null;
        if (endCondition == EndCondition.LoversWin) return null;
        if (endCondition == EndCondition.VultureWin) return null;
        if (endCondition == EndCondition.AvengerWin) return null;
        if (endCondition == EndCondition.SpectreWin) return null;
        if (endCondition == EndCondition.YandereWin) return null;
        if (endCondition == EndCondition.MoriartyWin || endCondition == EndCondition.MoriartyWinByKillHolmes) return null;

        foreach (var player in Game.GameData.data.AllPlayers.Values)
        {
            if (!player.IsAlive) continue;
            if (player.role != Roles.Spectre) continue;
            if (player.Tasks != null && player.Tasks?.Completed == player.Tasks?.AllTasks) return EndCondition.SpectreWin;
        }
        return null;
    });

    public static Side ChainShifter = new Side("ChainShifter", "chainShifter", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.ChainShifter.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        return null;
    });

    public static Side Madman = new Side("Madman", "madman", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.Madman.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        return null;
    });

    public static Side SchrodingersCat = new Side("SchrodingersCat", "schrodingersCat", IntroDisplayOption.SHOW_ONLY_ME, NeutralRoles.SchrodingersCat.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        return null;
    });

    public static Side Pavlov = new Side("Pavlov", "pavlov", IntroDisplayOption.STANDARD, NeutralRoles.Pavlov.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        if ((statistics.AlivePavlov - statistics.AliveInLovePavlov) * 2 >= (statistics.TotalAlive - statistics.AliveSpectre) && statistics.AliveImpostors == 0 &&
        (statistics.AlivePavlovCouple + statistics.AlivePavlovTrilemma == 0 ||
        statistics.AlivePavlovCouple * 2 + statistics.AlivePavlovTrilemma * 3 >= statistics.AliveCouple * 2 + statistics.AliveTrilemma * 3) &&
        statistics.AliveJackals == 0 && statistics.AliveMoriarty == 0)
        {
            return EndCondition.PavlovWin;
        }
        return null;
    });

    public static Side Moriarty = new Side("Moriarty","moriarty",IntroDisplayOption.STANDARD, NeutralRoles.Moriarty.RoleColor,(PlayerStatistics statistics,ShipStatus status) => {
        if ((statistics.AliveMoriarty - statistics.AliveInLoveMoriarty) * 2 >= statistics.TotalAlive && statistics.AliveImpostors == 0 &&
        (statistics.AliveMoriartyCouple + statistics.AliveMoriartyTrilemma == 0 ||
        statistics.AliveMoriartyCouple * 2 + statistics.AliveMoriartyTrilemma * 3 >= statistics.AliveCouple * 2 + statistics.AliveTrilemma * 3) &&
        statistics.AliveJackals == 0 && statistics.AlivePavlov == 0)
        {
            return EndCondition.MoriartyWin;
        }
        else if (Roles.Moriarty.WinTrigger) return EndCondition.MoriartyWinByKillHolmes;
        return null;
    });

    public static Side Cascrubinter = new Side("Cascrubinter","cascrubinter",IntroDisplayOption.SHOW_ONLY_ME,NeutralRoles.Cascrubinter.RoleColor,(PlayerStatistics statistics,ShipStatus status) => {
        if(Roles.Cascrubinter.WinTrigger){
            return EndCondition.CascrubinterWin;
        }
        return null;
    });

    public static Side Amnesiac = new Side("Amnesiac","amnesiac",IntroDisplayOption.SHOW_ONLY_ME,NeutralRoles.Amnesiac.RoleColor,(PlayerStatistics statistics,ShipStatus status) => {
        return null;
    });

    public static Side Yandere = new Side("Yandere","yandere",IntroDisplayOption.Yanderes,NeutralRoles.Yandere.RoleColor,(PlayerStatistics statistics,ShipStatus status) => {
        return null;
    },(EndCondition endCondition, PlayerStatistics statistics, ShipStatus status) => {
        if (endCondition.IsNoBodyWinEnd) return null;
        if(statistics.AliveYandere == 0) return null;
        if (endCondition == EndCondition.ArsonistWin) return null;
        if (endCondition == EndCondition.EmpiricWin) return null;
        if (endCondition == EndCondition.JesterWin) return null;
        if (endCondition == EndCondition.LoversWin) return null;
        if (endCondition == EndCondition.VultureWin) return null;
        if (endCondition == EndCondition.AvengerWin) return null;
        if (endCondition == EndCondition.SpectreWin) return null;
        if (endCondition == EndCondition.YandereWin) return null;
        if(!Roles.Yandere.GetLover().GetModData().HasExtraRole(Roles.SecretCrush)) RPCEventInvoker.SetExtraRole(Roles.Yandere.GetLover(),Roles.SecretCrush,0);
        return EndCondition.YandereWin;
    });

    public static Side Guesser = new Side("Guesser","guesser",IntroDisplayOption.STANDARD,ComplexRoles.FGuesser.RoleColor,(PlayerStatistics statistics,ShipStatus status) => {
        if(Roles.F_Guesser.WinTrigger){
            return EndCondition.GuesserWin;
        }
        return null;
    });

    /*
    public static Side SantaClaus = new Side("SantaClaus", "santaClaus", IntroDisplayOption.STANDARD, NeutralRoles.SantaClaus.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        if (statistics.TotalAlive == 0) return null;

        foreach (var player in Game.GameData.data.AllPlayers.Values)
        {
            if (!player.IsAlive) continue;

            if (player.role.side != Side.SantaClaus && !player.HasExtraRole(Roles.TeamSanta)) return null;

        }

        return EndCondition.SantaWin;
    });
    */

    public static Side GamePlayer = new Side("GamePlayer", "gamePlayer", IntroDisplayOption.SHOW_ONLY_ME, Palette.CrewmateBlue, (PlayerStatistics statistics, ShipStatus status) =>
    {
        return null;
    });

    public static Side Extra = new Side("Extra", "extra", IntroDisplayOption.STANDARD, new Color(150, 150, 150), (PlayerStatistics statistics, ShipStatus side) =>
    {
        if (Game.GameData.data.IsCanceled)
        {
            return EndCondition.NoGame;
        }


        if (CustomOptionHolder.limiterOptions.getBool())
        {
            if (Game.GameData.data.Timer < 1f)
            {
                switch (GameOptionsManager.Instance.CurrentGameOptions.MapId)
                {
                    case 0:
                    case 3:
                        return EndCondition.NobodySkeldWin;
                    case 1:
                        return EndCondition.NobodyMiraWin;
                    case 2:
                        return EndCondition.NobodyPolusWin;
                    case 4:
                        return EndCondition.NobodyAirshipWin;
                }
            }
        }

            //Lovers単独勝利
            if (statistics.TotalAlive <= 3)
        {
            if (statistics.AliveCouple == 1 && statistics.AliveTrilemma == 0) return EndCondition.LoversWin;
        }


            //Trilemma
            if (statistics.TotalAlive <= 5)
        {
            if (statistics.AliveTrilemma == 1 && statistics.AliveCouple == 0) return EndCondition.TrilemmaWin;
        }

        if (statistics.TotalAlive == 0)
        {
            return EndCondition.NobodyWin;
        }
        return null;
    });

    public static Side RitualCrewmate = new Side("Crewmate", "crewmate", IntroDisplayOption.STANDARD, Palette.CrewmateBlue, (PlayerStatistics statistics, ShipStatus status) =>
    {
        return null;
    });

    public static Side VOID = new Side("VOID", "void", IntroDisplayOption.SHOW_ONLY_ME, MetaRoles.VOID.RoleColor, (PlayerStatistics statistics, ShipStatus status) =>
    {
        return null;
    });

    public static List<Side> AllSides = new List<Side>()
        {
            Crewmate, Impostor,
            Jackal, Jester, Vulture, Empiric, Arsonist, Paparazzo, Avenger,ChainShifter,Spectre,/*SantaClaus,*/
            GamePlayer,
            Extra,VOID,
            RitualCrewmate,
            Madman,SchrodingersCat,Pavlov,Moriarty,Cascrubinter,Amnesiac,Guesser,Yandere
        };

    public IntroDisplayOption ShowOption { get; }
    public Color color { get; }
    public string side { get; }
    public string localizeSide { get; }

    public EndCriteriaChecker endCriteriaChecker { get; }
    public EndTakeoverChecker endTakeoverChecker { get; }

    private Side(string side, string localizeSide, IntroDisplayOption displayOption, Color color, EndCriteriaChecker endCriteriaChecker, EndTakeoverChecker endTakeoverChecker)
    {
        this.side = side;
        this.localizeSide = localizeSide;
        this.ShowOption = displayOption;
        this.color = color;
        this.endCriteriaChecker = endCriteriaChecker;
        this.endTakeoverChecker = endTakeoverChecker;
    }

    private Side(string side, string localizeSide, IntroDisplayOption displayOption, Color color, EndCriteriaChecker endCriteriaChecker) :
        this(side, localizeSide, displayOption, color, endCriteriaChecker, (a1, a2, a3) => null)
    {
    }
}
