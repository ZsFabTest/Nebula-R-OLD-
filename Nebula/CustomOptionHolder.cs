﻿using AmongUs.GameOptions;
using Cpp2IL.Core;
using Cpp2IL.Core.Extensions;
using Nebula.Module;
using TMPro;

namespace Nebula;

public class GameRule
{
    public bool dynamicMap { get; }
    public bool canUseEmergencyWithoutDeath { get; }
    public bool canUseEmergencyWithoutSabotage { get; }
    public bool canUseEmergencyWithoutReport { get; }
    public bool severeEmergencyLock { get; }
    public float deathPenaltyForDiscussionTime { get; }
    public int maxMeetingsCount { get; }
    public int vanillaVotingTime { get; }

    public GameRule()
    {
        if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal)
        {
            vanillaVotingTime = GameOptionsManager.Instance.CurrentGameOptions.GetInt(Int32OptionNames.VotingTime);
        }
        else
        {
            vanillaVotingTime = 0;
        }

        if (CustomOptionHolder.meetingOptions.getBool())
        {
            deathPenaltyForDiscussionTime = CustomOptionHolder.deathPenaltyForDiscussionTime.getFloat();
            canUseEmergencyWithoutDeath = CustomOptionHolder.canUseEmergencyWithoutDeath.getBool();
            canUseEmergencyWithoutSabotage = CustomOptionHolder.canUseEmergencyWithoutSabotage.getBool();
            canUseEmergencyWithoutReport = CustomOptionHolder.canUseEmergencyWithoutReport.getBool();
            severeEmergencyLock = CustomOptionHolder.severeEmergencyLock.getBool();
            maxMeetingsCount = (int)CustomOptionHolder.maxNumberOfMeetings.getFloat();
        }
        else
        {
            deathPenaltyForDiscussionTime = 0f;
            canUseEmergencyWithoutDeath = false;
            canUseEmergencyWithoutSabotage = false;
            canUseEmergencyWithoutReport = false;
            severeEmergencyLock = false;
            maxMeetingsCount = 15;
        }

        if (CustomOptionHolder.mapOptions.getBool())
        {
            dynamicMap = CustomOptionHolder.dynamicMap.getBool();
        }
        else
        {
            dynamicMap = false;
        }
    }
}

public class CustomOptionHolder
{
    public static string[] rates = new string[] {
            "option.display.percentage.0" , "option.display.percentage.10", "option.display.percentage.20", "option.display.percentage.30", "option.display.percentage.40",
            "option.display.percentage.50", "option.display.percentage.60", "option.display.percentage.70", "option.display.percentage.80", "option.display.percentage.90", "option.display.percentage.100" };
    public static string[] ratesWithoutZero = rates.SubArray(1,10);
    public static string[] ratesWithoutTerminal = rates.SubArray(1, 9);
    public static string[] ratesSecondary = new string[] {
            "option.display.percentage.andSoForth", "option.display.percentage.10", "option.display.percentage.20", "option.display.percentage.30", "option.display.percentage.40",
            "option.display.percentage.50", "option.display.percentage.60", "option.display.percentage.70", "option.display.percentage.80", "option.display.percentage.90" };
    public static string[] presets = new string[] { "option.display.preset.1", "option.display.preset.2", "option.display.preset.3", "option.display.preset.4", "option.display.preset.5" };
    public static string[] gamemodesNormal = new string[] { "gamemode.standard", "gamemode.freePlay" };
    public static string[] gamemodesHnS = new string[] { "gamemode.standard", "gamemode.freePlay" };

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

    public static string cs(Color c, string s)
    {
        return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
    }

    public static int optionsPage = 0;

    public static CustomOption gameModeNormal;
    public static CustomOption gameModeHnS;
    public static CustomOption GetCurrentGameModeOption()
    {
        switch (GameOptionsManager.Instance.currentGameMode)
        {
            case GameModes.Normal:
                return gameModeNormal;
            case GameModes.HideNSeek:
                return gameModeHnS;
        }
        return gameModeNormal;
    }

    public static CustomOption roleCountOption;
    public static CustomOption crewmateRolesCountMin;
    public static CustomOption crewmateRolesCountMax;
    public static CustomOption neutralRolesCountMin;
    public static CustomOption neutralRolesCountMax;
    public static CustomOption impostorRolesCountMin;
    public static CustomOption impostorRolesCountMax;

    public static CustomOption mapOptions;
    public static CustomOption dynamicMap;
    public static CustomOption exceptSkeld;
    public static CustomOption exceptMIRA;
    public static CustomOption exceptPolus;
    public static CustomOption exceptAirship;
    public static CustomOption additionalVents;
    public static CustomOption additionalWirings;
    public static CustomOption spawnMethod;
    public static CustomOption respawnNearbyFinalPosition;
    public static CustomOption synchronizedSpawning;
    public static CustomOption optimizedMaps;
    public static CustomOption invalidatePrimaryAdmin;
    public static CustomOption invalidateSecondaryAdmin;
    public static CustomOption useClassicAdmin;
    public static CustomOption allowParallelMedBayScans;
    public static CustomOption quietVentsInTheShadow;
    public static CustomOption oneWayMeetingRoomOption;
    public static CustomOption shuffledElectricalOption;


    public static CustomOption RitualOption;
    public static CustomOption NumOfMissionsOption;
    public static CustomOption LengthOfMissionOption;
    public static CustomOption RitualKillCoolDownOption;
    public static CustomOption RitualKillFailedPenaltyOption;
    public static CustomOption RitualSearchCoolDownOption;
    public static CustomOption RitualSearchableDistanceOption;


    public static CustomOption meetingOptions;
    public static CustomOption maxNumberOfMeetings;
    public static CustomOption deathPenaltyForDiscussionTime;
    public static CustomOption canUseEmergencyWithoutDeath;
    public static CustomOption canUseEmergencyWithoutSabotage;
    public static CustomOption canUseEmergencyWithoutReport;
    public static CustomOption severeEmergencyLock;
    public static CustomOption dealAbstentionAsSelfVote;
    public static CustomOption hideVotedIcon;
    public static CustomOption additionalEmergencyCoolDown;
    public static CustomOption additionalEmergencyCoolDownCondition;
    public static CustomOption showRoleOfExiled;
    public static CustomOption useSpecialRoleExiledText;
    public static CustomOption showExtraRoles;
    public static CustomOption showNumberOfEvilNeutralRoles;
    public static CustomOption dontShowImpostorCountIfDidntExile;

    public static CustomOption limiterOptions;
    public static CustomOption timeLimitOption;
    public static CustomOption timeLimitSecondOption;

    public static CustomOption DevicesOption;
    public static CustomOption RestrictModeOption;
    public static CustomOption AdminLimitOption;
    public static CustomOption VitalsLimitOption;
    public static CustomOption CameraAndDoorLogLimitOption;
    public static CustomOption UnlimitedCameraSkeldOption;
    public static CustomOption UnlimitedCameraPolusOption;
    public static CustomOption UnlimitedCameraAirshipOption;
    public static CustomOption ShowTimeLeftOnConsolesOption;
    public static CustomOption ShowTimeLeftOnMeetingOption;
    public static CustomOption LimitedAdmin;

    public static CustomOption? GetUnlimitedCameraOption()
    {
        switch (GameOptionsManager.Instance.CurrentGameOptions.MapId)
        {
            case 0:
                return UnlimitedCameraSkeldOption;
            case 2:
                return UnlimitedCameraPolusOption;
            case 4:
                return UnlimitedCameraAirshipOption;
            default:
                return null;
        }
    }

    public static CustomOption TasksOption;
    public static CustomOption RandomizedWiringOption;
    public static CustomOption StepsOfWiringOption;
    public static CustomOption MeistersManifoldsOption;
    public static CustomOption MeistersFilterOption;
    public static CustomOption MeistersFuelEnginesOption;
    public static CustomOption DangerousDownloadSpotOption;
    public static CustomOption UseVanillaSafeTaskOption;

    public static CustomOption SabotageOption;
    public static CustomOption SabotageCoolDownOption;
    public static CustomOption SkeldReactorTimeLimitOption;
    public static CustomOption SkeldO2TimeLimitOption;
    public static CustomOption MIRAReactorTimeLimitOption;
    public static CustomOption MIRAO2TimeLimitOption;
    public static CustomOption SeismicStabilizersTimeLimitOption;
    public static CustomOption AvertCrashTimeLimitOption;
    public static CustomOption BlackOutStrengthOption;
    public static CustomOption CanUseDoorDespiteSabotageOption;

    public static CustomOption SoloFreePlayOption;
    public static CustomOption CountOfDummiesOption;

    public static CustomOption advanceRoleOptions;

    public static CustomOption exclusiveAssignmentParent;
    public static CustomOption exclusiveAssignmentMorphingAndPainter;
    public static CustomOption exclusiveAssignmentRaiderAndSniper;
    public static CustomOption exclusiveAssignmentArsonistAndEmpiric;
    public static CustomOption exclusiveAssignmentAlienAndNavvy;
    public static CustomOption exclusiveAssignmentBaitAndProvocateur;
    public static CustomOption exclusiveAssignmentPsychicAndSeer;
    public static List<Tuple<CustomOption, List<CustomOption>>> exclusiveAssignmentList;
    public static List<Roles.Role> exclusiveAssignmentRoles;

    public static CustomOption CoolDownOption;
    public static CustomOption InitialKillCoolDownOption;
    public static CustomOption InitialAbilityCoolDownOption;
    public static CustomOption InitialForcefulAbilityCoolDownOption;
    public static CustomOption InitialModestAbilityCoolDownOption;
    public static CustomOption KillCoolDownProceedIgnoringParent;
    public static CustomOption KillCoolDownProceedIgnoringLadder;
    public static CustomOption KillCoolDownProceedIgnoringMovingPlatform;
    public static CustomOption KillCoolDownProceedIgnoringDoorGame;
    public static CustomOption KillCoolDownProceedIgnoringSecurityCamera;
    public static CustomOption KillCoolDownProceedIgnoringBlackOutGame;
    public static CustomOption KillCoolDownProceedIgnoringCommReceiver;
    public static CustomOption KillCoolDownProceedIgnoringEmergencySabotage;

    public static CustomOption SecretRoleOption;
    public static CustomOption NumOfSecretCrewmateOption;
    public static CustomOption NumOfSecretImpostorOption;
    public static CustomOption ChanceOfSecretCrewmateOption;
    public static CustomOption ChanceOfSecretImpostorOption;
    public static CustomOption RequiredTasksForArousal;
    public static CustomOption RequiredNumOfKillingForArousal;

    public static CustomOption streamersOption;
    public static CustomOption enforcePreventingSpoilerOption;

    public static CustomOption HnSOption;
    public static CustomOption ValidPerksOption;
    public static CustomOption MustDoTasksToWinOption;

    public static void AddExclusiveAssignment(ref List<ExclusiveAssignment> exclusiveAssignments)
    {
        if (!exclusiveAssignmentParent.getBool()) return;

        if (exclusiveAssignmentMorphingAndPainter.getBool())
            exclusiveAssignments.Add(new ExclusiveAssignment(Roles.Roles.Morphing, Roles.Roles.Painter));
        if (exclusiveAssignmentRaiderAndSniper.getBool())
            exclusiveAssignments.Add(new ExclusiveAssignment(Roles.Roles.Raider, Roles.Roles.Sniper));
        if (exclusiveAssignmentArsonistAndEmpiric.getBool())
            exclusiveAssignments.Add(new ExclusiveAssignment(Roles.Roles.Arsonist, Roles.Roles.Empiric));
        if (exclusiveAssignmentAlienAndNavvy.getBool())
            exclusiveAssignments.Add(new ExclusiveAssignment(Roles.Roles.Alien, Roles.Roles.Navvy));
        if (exclusiveAssignmentBaitAndProvocateur.getBool())
            exclusiveAssignments.Add(new ExclusiveAssignment(Roles.Roles.Bait, Roles.Roles.Provocateur));
        if (exclusiveAssignmentPsychicAndSeer.getBool())
            exclusiveAssignments.Add(new ExclusiveAssignment(Roles.Roles.Psychic, Roles.Roles.Seer));

        foreach (var tuple in exclusiveAssignmentList)
        {
            if (!tuple.Item1.getBool()) continue;

            exclusiveAssignments.Add(new ExclusiveAssignment(
                tuple.Item2[0].getSelection() > 0 ? exclusiveAssignmentRoles[tuple.Item2[0].getSelection() - 1] : null,
                tuple.Item2[1].getSelection() > 0 ? exclusiveAssignmentRoles[tuple.Item2[1].getSelection() - 1] : null,
                tuple.Item2[2].getSelection() > 0 ? exclusiveAssignmentRoles[tuple.Item2[2].getSelection() - 1] : null
                ));
        }
    }

    public static CustomGameMode GetCustomGameMode()
    {
        if(GameOptionsManager.Instance.currentGameMode==GameModes.Normal)
            return CustomGameModes.GetGameMode(gameModeNormal.getSelection());
        if (GameOptionsManager.Instance.currentGameMode == GameModes.HideNSeek)
            return CustomGameModes.GetGameMode(gameModeHnS.getSelection()+8);

        return CustomGameMode.Standard;
    }

    public static IEnumerator<object> GetStringMixedSelections(string topSelection, float min, float mid, float step1, float max, float step2)
    {
        yield return topSelection;
        if (min > max)
        {
            float temp = max;
            max = min;
            min = max;
        }
        if (mid > max)
        {
            float temp = mid;
            mid = max;
            max = mid;
        }
        if (min > mid)
        {
            float temp = mid;
            mid = min;
            min = mid;
        }

        if (step1 < 0) step1 *= -1;
        if (step2 < 0) step2 *= -1;

        float t = min;
        while (t < mid)
        {
            yield return t;
            t += step1;
        }

        t = mid;
        while (t < max)
        {
            yield return t;
            t += step2;
        }
        yield return max;
    }

    public static void Load()
    {
        CustomOption.optionSaver = new DataSaver("options.dat");

        gameModeNormal = CustomOption.Create(Color.white, "option.gameMode", gamemodesNormal, gamemodesNormal[0], null, true, false, "", CustomOptionTab.Settings).SetGameMode(CustomGameMode.All).SetIdentifier("option.gameModeNormal");
        gameModeHnS = CustomOption.Create(Color.white, "option.gameMode", gamemodesHnS, gamemodesHnS[0], null, true, false, "", CustomOptionTab.Settings).SetGameMode(CustomGameMode.All).SetIdentifier("option.gameModeHnS");


        roleCountOption = CustomOption.Create(Color.white, "option.roleCount", new string[] { "option.empty" }, "option.empty", null, true, false, "", CustomOptionTab.Settings).SetGameMode(CustomGameMode.All).HiddenOnDisplay(true);
        CustomOption.RegisterTopOption(roleCountOption);
        crewmateRolesCountMin = CustomOption.Create(new Color(204f / 255f, 204f / 255f, 0, 1f), "option.minimumCrewmateRoles", 0f, 0f, 15f, 1f, roleCountOption, true, false, "", CustomOptionTab.Settings).HiddenOnDisplay(true);
        crewmateRolesCountMax = CustomOption.Create(new Color(204f / 255f, 204f / 255f, 0, 1f), "option.maximumCrewmateRoles", 0f, 0f, 15f, 1f, roleCountOption, false, false, "", CustomOptionTab.Settings).HiddenOnDisplay(true);
        neutralRolesCountMin = CustomOption.Create(new Color(204f / 255f, 204f / 255f, 0, 1f), "option.minimumNeutralRoles", 0f, 0f, 15f, 1f, roleCountOption, false, false, "", CustomOptionTab.Settings).HiddenOnDisplay(true);
        neutralRolesCountMax = CustomOption.Create(new Color(204f / 255f, 204f / 255f, 0, 1f), "option.maximumNeutralRoles", 0f, 0f, 15f, 1f, roleCountOption, false, false, "", CustomOptionTab.Settings).HiddenOnDisplay(true);
        impostorRolesCountMin = CustomOption.Create(new Color(204f / 255f, 204f / 255f, 0, 1f), "option.minimumImpostorRoles", 0f, 0f, 5f, 1f, roleCountOption, false, false, "", CustomOptionTab.Settings).HiddenOnDisplay(true);
        impostorRolesCountMax = CustomOption.Create(new Color(204f / 255f, 204f / 255f, 0, 1f), "option.maximumImpostorRoles", 0f, 0f, 5f, 1f, roleCountOption, false, false, "", CustomOptionTab.Settings).HiddenOnDisplay(true);

        SoloFreePlayOption = CustomOption.Create(Color.white, "option.soloFreePlayOption", new string[] { "option.empty" }, "option.empty", null, true, false, "", CustomOptionTab.Settings).SetGameMode(CustomGameMode.FreePlay).AddCustomPrerequisite(() => { return PlayerControl.AllPlayerControls.Count == 1; });
        CustomOption.RegisterTopOption(SoloFreePlayOption);
        CountOfDummiesOption = CustomOption.Create(Color.white, "option.countOfDummies", 0, 0, 14, 1, SoloFreePlayOption).SetGameMode(CustomGameMode.All);

        meetingOptions = CustomOption.Create(Color.white, "option.meetingOptions", false, null, true, false, "", CustomOptionTab.Settings).SetGameMode(~(CustomGameMode.AllHnS));
        CustomOption.RegisterTopOption(meetingOptions);
        maxNumberOfMeetings = CustomOption.Create(Color.white, "option.maxNumberOfMeetings", 10, 0, 15, 1, meetingOptions);
        deathPenaltyForDiscussionTime = CustomOption.Create(Color.white, "option.deathPenaltyForDiscussionTime", 5f, 0f, 30f, 1f, meetingOptions);
        deathPenaltyForDiscussionTime.suffix = "second";
        additionalEmergencyCoolDown = CustomOption.Create(Color.white, "option.additionalEmergencyCoolDown", 10f, 0f, 60f, 5f, meetingOptions);
        additionalEmergencyCoolDown.suffix = "second";
        additionalEmergencyCoolDownCondition = CustomOption.Create(Color.white, "option.additionalEmergencyCoolDownCondition", 2f, 0f, 15f, 1f, meetingOptions);
        additionalEmergencyCoolDownCondition.isHidden = true;
        canUseEmergencyWithoutDeath = CustomOption.Create(Color.white, "option.canUseEmergencyWithoutDeath", false, meetingOptions);
        canUseEmergencyWithoutSabotage = CustomOption.Create(Color.white, "option.canUseEmergencyWithoutSabotage", false, meetingOptions);
        canUseEmergencyWithoutReport = CustomOption.Create(Color.white, "option.canUseEmergencyWithoutReport", false, meetingOptions);
        severeEmergencyLock = CustomOption.Create(Color.white, "option.severeEmergencyLock", false, meetingOptions);
        dealAbstentionAsSelfVote = CustomOption.Create(Color.white, "option.dealAbstentionAsSelfVote", false, meetingOptions);
        hideVotedIcon = CustomOption.Create(Color.white, "option.hideVotedIcon", false, meetingOptions);
        showRoleOfExiled = CustomOption.Create(Color.white, "option.showRoleOfExiled", false, meetingOptions).AddCustomPrerequisite(() =>
        {
            try
            {
                return GameOptionsManager.Instance.currentNormalGameOptions.ConfirmImpostor;
            }
            catch
            {
                return false;
            }
        });
        useSpecialRoleExiledText = CustomOption.Create(Color.white,"option.useSpecialRoleExiledText",true,meetingOptions).AddCustomPrerequisite(() => {
            try{
                return showRoleOfExiled.getBool() && GameOptionsManager.Instance.currentNormalGameOptions.ConfirmImpostor;
            }
            catch{
                return false;
            }
        });
        showExtraRoles = CustomOption.Create(Color.white,"option.showExtraRoles",true,meetingOptions).AddCustomPrerequisite(() => {
            try
            {
                return GameOptionsManager.Instance.currentNormalGameOptions.ConfirmImpostor && useSpecialRoleExiledText.getBool();
            }
            catch
            {
                return false;
            }
        });
        showNumberOfEvilNeutralRoles = CustomOption.Create(Color.white,"option.showNumberOfEvilNeutralRoles",true,meetingOptions).AddCustomPrerequisite(() =>
        {
            try
            {
                return GameOptionsManager.Instance.currentNormalGameOptions.ConfirmImpostor && useSpecialRoleExiledText.getBool();
            }
            catch
            {
                return false;
            }
        });
        dontShowImpostorCountIfDidntExile = CustomOption.Create(Color.white,"option.dontShowImpostorCountIfDidntExile",false,meetingOptions).AddCustomPrerequisite(() =>
        {
            try
            {
                return GameOptionsManager.Instance.currentNormalGameOptions.ConfirmImpostor && useSpecialRoleExiledText.getBool();
            }
            catch
            {
                return false;
            }
        });

        additionalEmergencyCoolDown.alternativeOptionScreenBuilder = (refresher) =>
        {
            if (additionalEmergencyCoolDown.getBool())
            {
                return new MetaScreenContent[][] {
               
                    new MetaScreenContent[]
                    {
                        new MSMargin(1.9f),
                       new CustomOption.MSOptionString(additionalEmergencyCoolDown,3f, additionalEmergencyCoolDown.getName(), 2f, 0.8f, TMPro.TextAlignmentOptions.MidlineRight, TMPro.FontStyles.Bold),
                    new MSString(0.2f, ":", TMPro.TextAlignmentOptions.Center, TMPro.FontStyles.Bold),
                    new MSButton(0.4f, 0.4f, "<<", TMPro.FontStyles.Bold, () =>
                    {
                        additionalEmergencyCoolDown.addSelection(-1);
                        refresher();
                    }),
                    new MSString(0.6f, additionalEmergencyCoolDown.getString(), 2f, 0.6f, TMPro.TextAlignmentOptions.Center, TMPro.FontStyles.Bold),
                    new MSButton(0.4f, 0.4f, ">>", TMPro.FontStyles.Bold, () =>
                    {
                        additionalEmergencyCoolDown.addSelection(1);
                        refresher();
                    }),
                    new MSMargin(0.2f),
                    new CustomOption.MSOptionString(additionalEmergencyCoolDownCondition,1.2f, additionalEmergencyCoolDownCondition.getName(), 2f, 0.8f, TMPro.TextAlignmentOptions.MidlineRight, TMPro.FontStyles.Bold),
                    new MSString(0.2f, ":", TMPro.TextAlignmentOptions.Center, TMPro.FontStyles.Bold),
                    new MSButton(0.4f, 0.4f, "<<", TMPro.FontStyles.Bold, () =>
                    {
                        additionalEmergencyCoolDownCondition.addSelection(-1);
                        refresher();
                    }),
                    new MSString(0.6f, additionalEmergencyCoolDownCondition.getString(), 2f, 0.6f, TMPro.TextAlignmentOptions.Center, TMPro.FontStyles.Bold),
                    new MSButton(0.4f, 0.4f, ">>", TMPro.FontStyles.Bold, () =>
                    {
                        additionalEmergencyCoolDownCondition.addSelection(1);
                        refresher();
                    }),
                    new MSMargin(1f)
                    }
                };
            }
            else
            {
                return new MetaScreenContent[][] {

                    new MetaScreenContent[]
                    {
                        new MSMargin(1.9f),
                       new CustomOption.MSOptionString(additionalEmergencyCoolDown,3f, additionalEmergencyCoolDown.getName(), 2f, 0.8f, TMPro.TextAlignmentOptions.MidlineRight, TMPro.FontStyles.Bold),
                    new MSString(0.2f, ":", TMPro.TextAlignmentOptions.Center, TMPro.FontStyles.Bold),
                    new MSButton(0.4f, 0.4f, "<<", TMPro.FontStyles.Bold, () =>
                    {
                        additionalEmergencyCoolDown.addSelection(-1);
                        refresher();
                    }),
                    new MSString(0.6f, additionalEmergencyCoolDown.getString(), 2f, 0.6f, TMPro.TextAlignmentOptions.Center, TMPro.FontStyles.Bold),
                    new MSButton(0.4f, 0.4f, ">>", TMPro.FontStyles.Bold, () =>
                    {
                        additionalEmergencyCoolDown.addSelection(1);
                        refresher();
                    }),
                    new MSMargin(4.38f)
                    }
                };
            }
        };

        mapOptions = CustomOption.Create(Color.white, "option.mapOptions", false, null, true, false, "", CustomOptionTab.Settings).SetGameMode(CustomGameMode.All);
        CustomOption.RegisterTopOption(mapOptions);
        dynamicMap = CustomOption.Create(Color.white, "option.playRandomMaps", false, mapOptions).SetGameMode(CustomGameMode.All);
        exceptSkeld = CustomOption.Create(Color.white, "option.exceptSkeld", false, dynamicMap).SetGameMode(CustomGameMode.All);
        exceptMIRA = CustomOption.Create(Color.white, "option.exceptMIRA", false, dynamicMap).SetGameMode(CustomGameMode.All);
        exceptPolus = CustomOption.Create(Color.white, "option.exceptPolus", false, dynamicMap).SetGameMode(CustomGameMode.All);
        exceptAirship = CustomOption.Create(Color.white, "option.exceptAirship", false, dynamicMap).SetGameMode(CustomGameMode.All);
        additionalVents = CustomOption.Create(Color.white, "option.additionalVents", false, mapOptions).SetGameMode(CustomGameMode.All);
        spawnMethod = CustomOption.Create(Color.white, "option.spawnMethod", new string[] { "option.spawnMethod.default", "option.spawnMethod.selectable", "option.spawnMethod.random" }, "option.spawnMethod.default", mapOptions).SetGameMode(CustomGameMode.All);
        respawnNearbyFinalPosition = CustomOption.Create(Color.white, "option.respawnNearbyFinalPosition", false, mapOptions).SetGameMode(CustomGameMode.All).AddCustomPrerequisite(() => spawnMethod.getSelection() == 2);
        synchronizedSpawning = CustomOption.Create(Color.white, "option.synchronizedSpawning", false, mapOptions).SetGameMode(CustomGameMode.All);
        optimizedMaps = CustomOption.Create(Color.white, "option.optimizedMaps", true, mapOptions).SetGameMode(CustomGameMode.All);
        invalidatePrimaryAdmin = CustomOption.Create(Color.white, "option.invalidatePrimaryAdmin", new string[] { "option.switch.off", "option.invalidatePrimaryAdmin.onlyAirship", "option.switch.on" }, "option.switch.off", mapOptions).SetGameMode(CustomGameMode.All);
        invalidateSecondaryAdmin = CustomOption.Create(Color.white, "option.invalidateSecondaryAdmin", true, mapOptions).SetGameMode(CustomGameMode.All);
        useClassicAdmin = CustomOption.Create(Color.white, "option.useClassicAdmin", false, mapOptions).SetGameMode(CustomGameMode.All);
        allowParallelMedBayScans = CustomOption.Create(Color.white, "option.allowParallelMedBayScans", false, mapOptions).SetGameMode(CustomGameMode.All);
        quietVentsInTheShadow = CustomOption.Create(Color.white, "option.quietVentsInTheShadow", false, mapOptions).SetGameMode(CustomGameMode.All);
        oneWayMeetingRoomOption = CustomOption.Create(Color.white, "option.oneWayMeetingRoom", false, mapOptions).SetGameMode(CustomGameMode.All);
        shuffledElectricalOption = CustomOption.Create(Color.white, "option.shuffledElectrical", false, mapOptions).SetGameMode(CustomGameMode.All);

        spawnMethod.alternativeOptionScreenBuilder = (refresher) =>
        {
            MetaScreenContent getSuitableContent()
            {
                if (spawnMethod.getSelection() is 1 or 2)
                {
                    int selection = spawnMethod.getSelection();
                    return new MSButton(1.6f, 0.4f, "Customize", TMPro.FontStyles.Bold, () =>
                    {
                        Action<byte> refresher = null;
                        if (selection == 1)
                        {
                            refresher = (mapId) => MetaDialog.OpenMapDialog(mapId, true, (obj, id) =>
                            Map.MapData.MapDatabase[id].SetUpSelectiveSpawnPointButton(obj, () =>
                            {
                                MetaDialog.EraseDialog(1);
                                refresher(id);
                            }));
                        }
                        if (selection == 2)
                        {
                            refresher = (mapId) => MetaDialog.OpenMapDialog(mapId, true, (obj, id) =>
                            Map.MapData.MapDatabase[id].SetUpSpawnPointButton(obj, () =>
                                  {
                                      MetaDialog.EraseDialog(1);
                                      refresher(id);
                                  }));
                        }
                        refresher(GameOptionsManager.Instance.CurrentGameOptions.MapId);
                    });
                }
                else
                    return new MSMargin(1.7f);
            }

            return new MetaScreenContent[][] {
                    new MetaScreenContent[]
                    {
                        new MSMargin(1.9f),
                       new CustomOption.MSOptionString(spawnMethod,3f, spawnMethod.getName(), 2f, 0.8f, TMPro.TextAlignmentOptions.MidlineRight, TMPro.FontStyles.Bold),
                    new MSString(0.2f, ":", TMPro.TextAlignmentOptions.Center, TMPro.FontStyles.Bold),
                    new MSButton(0.4f, 0.4f, "<<", TMPro.FontStyles.Bold, () =>
                    {
                        spawnMethod.addSelection(-1);
                        refresher();
                    }),
                    new MSString(1.5f, spawnMethod.getString(), 2f, 0.6f, TMPro.TextAlignmentOptions.Center, TMPro.FontStyles.Bold),
                    new MSButton(0.4f, 0.4f, ">>", TMPro.FontStyles.Bold, () =>
                    {
                        spawnMethod.addSelection(1);
                        refresher();
                    }),
                    new MSMargin(0.2f),
                    getSuitableContent(),
                    new MSMargin(1f)
                    }
                };
        };

        limiterOptions = CustomOption.Create(Color.white, "option.limitOptions", false, null, true, false, "", CustomOptionTab.Settings).SetGameMode(~CustomGameMode.AllHnS);
        CustomOption.RegisterTopOption(limiterOptions);
        timeLimitOption = CustomOption.Create(Color.white, "option.timeLimitOption", 20f, 1f, 80f, 1f, limiterOptions).SetGameMode(CustomGameMode.All);
        timeLimitSecondOption = CustomOption.Create(Color.white, "option.timeLimitSecondOption", 0f, 0f, 55f, 5f, limiterOptions).SetGameMode(CustomGameMode.All);
        timeLimitOption.suffix = "minute";
        timeLimitSecondOption.suffix = "second";

        DevicesOption = CustomOption.Create(Color.white, "option.devicesOption", false, null, true, false, "", CustomOptionTab.Settings).SetGameMode(CustomGameMode.All);
        CustomOption.RegisterTopOption(DevicesOption);
        RestrictModeOption = CustomOption.Create(Color.white, "option.devicesOption.restrictModeOption", new string[] { "option.devicesOption.perDiscussion", "option.devicesOption.perGame" }, "option.devicesOption.perDiscussion", DevicesOption).SetGameMode(CustomGameMode.All);
        AdminLimitOption = CustomOption.Create(Color.white, "option.devicesOption.admin", GetStringMixedSelections("option.display.infinity", 1f, 10f, 1f, 100f, 5f), "option.display.infinity", DevicesOption).SetGameMode(CustomGameMode.All);
        AdminLimitOption.suffix = "second";
        VitalsLimitOption = CustomOption.Create(Color.white, "option.devicesOption.vitals", GetStringMixedSelections("option.display.infinity", 1f, 10f, 1f, 100f, 5f), "option.display.infinity", DevicesOption).SetGameMode(CustomGameMode.All);
        VitalsLimitOption.suffix = "second";
        CameraAndDoorLogLimitOption = CustomOption.Create(Color.white, "option.devicesOption.cameraAndDoorLog", GetStringMixedSelections("option.display.infinity", 1f, 10f, 1f, 100f, 5f), "option.display.infinity", DevicesOption).SetGameMode(CustomGameMode.All);
        CameraAndDoorLogLimitOption.suffix = "second";
        UnlimitedCameraSkeldOption = CustomOption.Create(Color.white, "option.devicesOption.unlimitedCameraSkeld", new string[] { "option.display.none", "option.devicesOption.camera.central", "option.devicesOption.camera.east", "option.devicesOption.camera.north", "option.devicesOption.camera.west" }, "option.display.none", DevicesOption).SetGameMode(CustomGameMode.All).AddPrerequisite(CameraAndDoorLogLimitOption);
        UnlimitedCameraPolusOption = CustomOption.Create(Color.white, "option.devicesOption.unlimitedCameraPolus", new string[] { "option.display.none", "option.devicesOption.camera.east", "option.devicesOption.camera.central", "option.devicesOption.camera.northeast", "option.devicesOption.camera.south", "option.devicesOption.camera.southwest", "option.devicesOption.camera.northwest" }, "option.display.none", DevicesOption).SetGameMode(CustomGameMode.All).AddPrerequisite(CameraAndDoorLogLimitOption);
        UnlimitedCameraAirshipOption = CustomOption.Create(Color.white, "option.devicesOption.unlimitedCameraAirship", new string[] { "option.display.none", "option.devicesOption.camera.engineRoom", "option.devicesOption.camera.vault", "option.devicesOption.camera.records", "option.devicesOption.camera.security", "option.devicesOption.camera.cargoBay", "option.devicesOption.camera.meetingRoom" }, "option.display.none", DevicesOption).SetGameMode(CustomGameMode.All).AddPrerequisite(CameraAndDoorLogLimitOption);
        ShowTimeLeftOnConsolesOption = CustomOption.Create(Color.white, "option.devicesOption.showTimeLeftOnConsoles", false, DevicesOption).SetGameMode(CustomGameMode.All);
        ShowTimeLeftOnMeetingOption = CustomOption.Create(Color.white, "option.devicesOption.showTimeLeftOnMeeting", false, DevicesOption).SetGameMode(CustomGameMode.All);
        LimitedAdmin = CustomOption.Create(Color.white, "option.devicesOption.limitedAdmin", false, DevicesOption).SetGameMode(CustomGameMode.All);

        LimitedAdmin.alternativeOptionScreenBuilder = (refresher) =>
        {
            MetaScreenContent getSuitableContent()
            {
                if (LimitedAdmin.getSelection() == 1)
                    return new MSButton(1.6f, 0.4f, "Customize", TMPro.FontStyles.Bold, () =>
                    {
                        Action<byte> refresher = null;
                        refresher = (mapId) => MetaDialog.OpenMapDialog(mapId, true, (obj, id) => Map.MapData.MapDatabase[id].SetUpAdminRoomButton(obj, () =>
                        {
                            MetaDialog.EraseDialog(1);
                            refresher(id);
                        }));
                        refresher(GameOptionsManager.Instance.CurrentGameOptions.MapId);
                    });
                else
                    return new MSMargin(1.7f);
            }

            return new MetaScreenContent[][] {
                    new MetaScreenContent[]
                    {
                        new MSMargin(1.9f),
                      new CustomOption.MSOptionString(LimitedAdmin,3f, LimitedAdmin.getName(), 2f, 0.8f, TMPro.TextAlignmentOptions.MidlineRight, TMPro.FontStyles.Bold),
                    new MSString(0.2f, ":", TMPro.TextAlignmentOptions.Center, TMPro.FontStyles.Bold),
                    new MSButton(0.4f, 0.4f, "<<", TMPro.FontStyles.Bold, () =>
                    {
                        LimitedAdmin.addSelection(-1);
                        refresher();
                    }),
                    new MSString(1.5f, LimitedAdmin.getString(), 2f, 0.6f, TMPro.TextAlignmentOptions.Center, TMPro.FontStyles.Bold),
                    new MSButton(0.4f, 0.4f, ">>", TMPro.FontStyles.Bold, () =>
                    {
                        LimitedAdmin.addSelection(1);
                        refresher();
                    }),
                    new MSMargin(0.2f),
                    getSuitableContent(),
                    new MSMargin(1f)
                    }
                };
        };


        TasksOption = CustomOption.Create(Color.white, "option.tasksOption", false, null, true, false, "", CustomOptionTab.Settings);
        CustomOption.RegisterTopOption(TasksOption);
        additionalWirings = CustomOption.Create(Color.white, "option.additionalWirings", false, TasksOption).SetGameMode(CustomGameMode.All);
        RandomizedWiringOption = CustomOption.Create(Color.white, "option.randomizedWiring", false, TasksOption).SetGameMode(CustomGameMode.All);
        StepsOfWiringOption = CustomOption.Create(Color.white, "option.stepsOfWiring", 3f, 1f, 10f, 1f, TasksOption).SetGameMode(CustomGameMode.All);
        MeistersManifoldsOption = CustomOption.Create(Color.white, "option.meistersManifolds", false, TasksOption).SetGameMode(CustomGameMode.All);
        MeistersFilterOption = CustomOption.Create(Color.white, "option.meistersO2Filter", false, TasksOption).SetGameMode(CustomGameMode.All);
        MeistersFuelEnginesOption = CustomOption.Create(Color.white, "option.meistersFuelEngines", false, TasksOption).SetGameMode(CustomGameMode.All);
        DangerousDownloadSpotOption = CustomOption.Create(Color.white, "option.dangerousDownloadSpot", false, TasksOption).SetGameMode(CustomGameMode.All);
        UseVanillaSafeTaskOption = CustomOption.Create(Color.white, "option.useVanillaSafeTask", true, TasksOption).SetGameMode(CustomGameMode.All);

        SabotageOption = CustomOption.Create(Color.white, "option.sabotageOption", false, null, true, false, "", CustomOptionTab.Settings).SetGameMode(~(CustomGameMode.AllHnS));
        CustomOption.RegisterTopOption(SabotageOption);
        SabotageCoolDownOption = CustomOption.Create(Color.white, "option.sabotageCoolDown", 30f, 5f, 60f, 5f, SabotageOption).SetGameMode(CustomGameMode.All);
        SabotageCoolDownOption.suffix = "second";
        SkeldReactorTimeLimitOption = CustomOption.Create(Color.white, "option.skeldReactorTimeLimit", 30f, 15f, 60f, 5f, SabotageOption).SetGameMode(CustomGameMode.All);
        SkeldReactorTimeLimitOption.suffix = "second";
        SkeldO2TimeLimitOption = CustomOption.Create(Color.white, "option.skeldO2TimeLimit", 30f, 15f, 60f, 5f, SabotageOption).SetGameMode(CustomGameMode.All);
        SkeldO2TimeLimitOption.suffix = "second";
        MIRAReactorTimeLimitOption = CustomOption.Create(Color.white, "option.MIRAReactorTimeLimit", 45f, 20f, 80f, 5f, SabotageOption).SetGameMode(CustomGameMode.All);
        MIRAReactorTimeLimitOption.suffix = "second";
        MIRAO2TimeLimitOption = CustomOption.Create(Color.white, "option.MIRAO2TimeLimit", 45f, 20f, 80f, 5f, SabotageOption).SetGameMode(CustomGameMode.All);
        MIRAO2TimeLimitOption.suffix = "second";
        SeismicStabilizersTimeLimitOption = CustomOption.Create(Color.white, "option.seismicStabilizersTimeLimit", 60f, 20f, 120f, 5f, SabotageOption).SetGameMode(CustomGameMode.All);
        SeismicStabilizersTimeLimitOption.suffix = "second";
        AvertCrashTimeLimitOption = CustomOption.Create(Color.white, "option.avertCrashTimeLimit", 90f, 20f, 180f, 5f, SabotageOption).SetGameMode(CustomGameMode.All);
        AvertCrashTimeLimitOption.suffix = "second";
        BlackOutStrengthOption = CustomOption.Create(Color.white, "option.blackOutStrength", 1f, 0.125f, 2f, 0.125f, SabotageOption).SetGameMode(CustomGameMode.All);
        BlackOutStrengthOption.suffix = "cross";
        CanUseDoorDespiteSabotageOption = CustomOption.Create(Color.white, "option.canUseDoorDespiteSabotage", false, SabotageOption).SetGameMode(CustomGameMode.All);

        SecretRoleOption = CustomOption.Create(Color.white, "option.secretRole", false, null, true, false, "", CustomOptionTab.Settings).SetGameMode(CustomGameMode.Standard);
        CustomOption.RegisterTopOption(SecretRoleOption);
        NumOfSecretCrewmateOption = CustomOption.Create(Color.white, "option.secretCrewmate", 2f, 0f, 15f, 1f, SecretRoleOption);
        ChanceOfSecretCrewmateOption = Module.CustomOption.Create(Color.white, "option.chanceOfSecretCrewmate", CustomOptionHolder.rates, CustomOptionHolder.rates[0], SecretRoleOption);
        NumOfSecretImpostorOption = CustomOption.Create(Color.white, "option.secretImpostor", 2f, 0f, 5f, 1f, SecretRoleOption);
        ChanceOfSecretImpostorOption = Module.CustomOption.Create(Color.white, "option.chanceOfSecretImpostor", CustomOptionHolder.rates, CustomOptionHolder.rates[0], SecretRoleOption);
        RequiredTasksForArousal = CustomOption.Create(Color.white, "option.requiredTasksForArousal", 3f, 1f, 6f, 1f, SecretRoleOption).AddPrerequisite(NumOfSecretCrewmateOption);
        RequiredNumOfKillingForArousal = CustomOption.Create(Color.white, "option.requiredNumOfKillingForArousal", 2f, 1f, 5f, 1f, SecretRoleOption).AddPrerequisite(NumOfSecretImpostorOption);

        advanceRoleOptions = CustomOption.Create(Color.white, "option.advanceRoleOptions", false, null, true, false, "", CustomOptionTab.Settings).SetGameMode(CustomGameMode.Standard);
        CustomOption.RegisterTopOption(advanceRoleOptions);

        //ロールのオプションを読み込む
        Roles.Role.LoadAllOptionData();
        Roles.GhostRole.LoadAllOptionData();
        Roles.ExtraRole.LoadAllOptionData();

        CoolDownOption = CustomOption.Create(Color.white, "option.coolDownOption", new string[] { "option.empty" }, "option.empty", null, true, false, "", CustomOptionTab.AdvancedSettings).SetGameMode(CustomGameMode.All);
        CustomOption.RegisterTopOption(CoolDownOption);
        InitialKillCoolDownOption = CustomOption.Create(Color.white, "option.initialKillCoolDown", 10f, 5f, 30f, 2.5f, CoolDownOption).SetGameMode(CustomGameMode.All);
        InitialKillCoolDownOption.suffix = "second";
        InitialAbilityCoolDownOption = CustomOption.Create(Color.white, "option.initialAbilityCoolDown", 15f, 5f, 30f, 2.5f, CoolDownOption).SetGameMode(CustomGameMode.All);
        InitialAbilityCoolDownOption.suffix = "second";
        InitialForcefulAbilityCoolDownOption = CustomOption.Create(Color.white, "option.initialForcefulAbilityCoolDown", 20f, 5f, 30f, 2.5f, CoolDownOption).SetGameMode(CustomGameMode.All);
        InitialForcefulAbilityCoolDownOption.suffix = "second";
        InitialModestAbilityCoolDownOption = CustomOption.Create(Color.white, "option.initialModestAbilityCoolDown", 10f, 5f, 30f, 2.5f, CoolDownOption).SetGameMode(CustomGameMode.All);
        InitialModestAbilityCoolDownOption.suffix = "second";

        MetaScreenContent[][] AddKillCoolDownProceedIgnoringOptions(Action refresher,params CustomOption[] options)
        {
            List<MetaScreenContent[]> result = new();
            result.Add(new MetaScreenContent[]{
                new CustomOption.MSOptionString("option.killCoolDownProceedIgnoring",5f, Language.Language.GetString("option.killCoolDownProceedIgnoring"), 2f, 0.6f, TMPro.TextAlignmentOptions.Center, TMPro.FontStyles.Bold),
                 new MSMargin(2f)
            });

            List<MetaScreenContent> contents=new();
            foreach (var option in options)
            {
                var o = option;
                contents.Add(
                     new MSButton(2f, 0.4f, o.getName(), TMPro.FontStyles.Bold, () =>
                     {
                         o.addSelection(1);
                         refresher();
                     }, o.getBool() ? Color.yellow : Color.white).EditFontSize(3f,0.3f)
                    );
                contents.Add(new MSMargin(0.1f));

                if (contents.Count == 6)
                {
                    result.Add(contents.ToArray());
                    contents.Clear();
                }
            }
            if(contents.Count>0) result.Add(contents.ToArray());

            return result.ToArray();
        }

        KillCoolDownProceedIgnoringParent = CustomOption.Create(Color.white, "option.killCoolDownProceedIgnoring", new string[] { "option.empty" }, "option.empty", CoolDownOption).SetGameMode(CustomGameMode.All);
        KillCoolDownProceedIgnoringLadder = CustomOption.Create(Color.white, "option.killCoolDownProceedIgnoringLadder", false, KillCoolDownProceedIgnoringParent).HiddenOnMetaScreen(true).SetGameMode(CustomGameMode.All);
        KillCoolDownProceedIgnoringMovingPlatform = CustomOption.Create(Color.white, "option.killCoolDownProceedIgnoringMovingPlatform", false, KillCoolDownProceedIgnoringParent).HiddenOnMetaScreen(true).SetGameMode(CustomGameMode.All);
        KillCoolDownProceedIgnoringDoorGame = CustomOption.Create(Color.white, "option.killCoolDownProceedIgnoringDoorGame", false, KillCoolDownProceedIgnoringParent).HiddenOnMetaScreen(true).SetGameMode(CustomGameMode.All);
        KillCoolDownProceedIgnoringSecurityCamera = CustomOption.Create(Color.white, "option.killCoolDownProceedIgnoringSecurityCamera", false, KillCoolDownProceedIgnoringParent).HiddenOnMetaScreen(true).SetGameMode(CustomGameMode.All);
        KillCoolDownProceedIgnoringBlackOutGame = CustomOption.Create(Color.white, "option.killCoolDownProceedIgnoringBlackOutGame", false, KillCoolDownProceedIgnoringParent).HiddenOnMetaScreen(true).SetGameMode(CustomGameMode.All);
        KillCoolDownProceedIgnoringCommReceiver = CustomOption.Create(Color.white, "option.killCoolDownProceedIgnoringCommReceiver", false, KillCoolDownProceedIgnoringParent).HiddenOnMetaScreen(true).SetGameMode(CustomGameMode.All);
        KillCoolDownProceedIgnoringEmergencySabotage = CustomOption.Create(Color.white, "option.killCoolDownProceedIgnoringEmergencySabotage", false, KillCoolDownProceedIgnoringParent).HiddenOnMetaScreen(true).SetGameMode(CustomGameMode.All);

        KillCoolDownProceedIgnoringParent.alternativeOptionScreenBuilder = (refresher) =>
            AddKillCoolDownProceedIgnoringOptions(refresher,
            KillCoolDownProceedIgnoringLadder,
            KillCoolDownProceedIgnoringMovingPlatform,
            KillCoolDownProceedIgnoringDoorGame,
            KillCoolDownProceedIgnoringSecurityCamera,
            KillCoolDownProceedIgnoringBlackOutGame,
            KillCoolDownProceedIgnoringCommReceiver,
            KillCoolDownProceedIgnoringEmergencySabotage
            );
        

        streamersOption = CustomOption.Create(Color.white, "option.streamersOption", false, null, true, false, "", CustomOptionTab.Settings).SetGameMode(~CustomGameMode.AllHnS);
        CustomOption.RegisterTopOption(streamersOption);
        enforcePreventingSpoilerOption = CustomOption.Create(Color.white, "option.streamersOption.enforcePreventingSpoiler", false, streamersOption).SetGameMode(CustomGameMode.All);

        HnSOption = CustomOption.Create(Color.white, "option.hideAndSeekOption", new string[] { "option.empty" }, "option.empty", null, true, false, "", CustomOptionTab.Settings).SetGameMode(CustomGameMode.AllHnS);
        CustomOption.RegisterTopOption(HnSOption);
        ValidPerksOption = CustomOption.Create(Color.white, "option.hideAndSeekOption.validPerks", 3,0,5,1, HnSOption, false, false, "", CustomOptionTab.Settings).SetGameMode(CustomGameMode.AllHnS);
        MustDoTasksToWinOption = CustomOption.Create(Color.white, "option.hideAndSeekOption.mustDoTasksToWin", false, HnSOption, false, false, "", CustomOptionTab.Settings).SetGameMode(CustomGameMode.AllHnS);

        exclusiveAssignmentParent = CustomOption.Create(new Color(204f / 255f, 204f / 255f, 0, 1f), "option.exclusiveAssignment", false, null, true, false, "", CustomOptionTab.AdvancedSettings).SetGameMode(CustomGameMode.Standard | CustomGameMode.FreePlay);
        CustomOption.RegisterTopOption(exclusiveAssignmentParent);
        exclusiveAssignmentMorphingAndPainter = CustomOption.Create(Color.white, "option.exclusiveAssignment.MorphingAndPainter", true, exclusiveAssignmentParent);
        exclusiveAssignmentRaiderAndSniper = CustomOption.Create(Color.white, "option.exclusiveAssignment.RaiderAndSniper", true, exclusiveAssignmentParent);
        exclusiveAssignmentArsonistAndEmpiric = CustomOption.Create(Color.white, "option.exclusiveAssignment.ArsonistAndEmpiric", true, exclusiveAssignmentParent);
        exclusiveAssignmentAlienAndNavvy = CustomOption.Create(Color.white, "option.exclusiveAssignment.AlienAndNavvy", true, exclusiveAssignmentParent);
        exclusiveAssignmentBaitAndProvocateur = CustomOption.Create(Color.white, "option.exclusiveAssignment.BaitAndProvocateur", true, exclusiveAssignmentParent);
        exclusiveAssignmentPsychicAndSeer = CustomOption.Create(Color.white, "option.exclusiveAssignment.PsychicAndSeer", false, exclusiveAssignmentParent);
        exclusiveAssignmentRoles = new List<Roles.Role>();
        foreach (Roles.Role role in Roles.Roles.AllRoles)
        {
            if (!role.HideInExclusiveAssignmentOption)
            {
                exclusiveAssignmentRoles.Add(role);
            }
        }
        string[] roleList = new string[exclusiveAssignmentRoles.Count + 1];
        for (int i = 0; i < roleList.Length - 1; i++)
        {
            roleList[1 + i] = "role." + exclusiveAssignmentRoles[i].LocalizeName + ".name";
        }
        roleList[0] = "option.exclusiveAssignmentRole.none";

        exclusiveAssignmentList = new List<Tuple<CustomOption, List<CustomOption>>>();
        for (int i = 0; i < 5; i++)
        {
            exclusiveAssignmentList.Add(new Tuple<CustomOption, List<CustomOption>>(CustomOption.Create(new Color(180f / 255f, 180f / 255f, 0, 1f), "option.exclusiveAssignment" + (i + 1), false, exclusiveAssignmentParent, false), new List<CustomOption>()));

            for (int r = 0; r < 3; r++)
            {
                exclusiveAssignmentList[exclusiveAssignmentList.Count - 1].Item2.Add(
                    CustomOption.Create(Color.white, "option.exclusiveAssignmentRole" + (r + 1), roleList, "option.exclusiveAssignmentRole.none", exclusiveAssignmentList[exclusiveAssignmentList.Count - 1].Item1, false)
                    .SetIdentifier("option.exclusiveAssignment" + (i + 1) + ".Role" + (r + 1))
                    );
            }
        }
        Map.MapData.CreateOptionData();
    }
}
