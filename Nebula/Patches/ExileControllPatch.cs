using static MeetingHud;

namespace Nebula.Patches;

[HarmonyPatch]
class ExileControllerPatch
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    class ExileControllerBeginPatch
    {

        public static void Postfix(ExileController __instance, [HarmonyArgument(0)] ref GameData.PlayerInfo exiled, [HarmonyArgument(1)] bool tie)
        {
            /*
            if (CustomOptionHolder.meetingOptions.getBool() && CustomOptionHolder.showRoleOfExiled.getBool() && GameManager.Instance.LogicOptions.GetConfirmImpostor())
            {
                var role = exiled.GetModData()?.role;
                if (role != null) __instance.completeString = Language.Language.GetString("game.exile.roleText").Replace("%PLAYER%", exiled.PlayerName).Replace("%ROLE%", Language.Language.GetString("role." + role.LocalizeName + ".name"));
            }
            */

            OnExiled(exiled);
        }
    }


    [HarmonyPatch(typeof(ExileController), nameof(ExileController.ReEnableGameplay))]
    class ExileControllerReEnableGameplayPatch
    {
        public static void Postfix(ExileController __instance)
        {
            if(CustomOptionHolder.meetingOptions.getBool() && CustomOptionHolder.additionalEmergencyCoolDown.getFloat() > 0f)
            {
                int deadPlayers = 0;
                foreach(var p in PlayerControl.AllPlayerControls)
                {
                    if (p.Data.IsDead) deadPlayers++;
                }
                if (deadPlayers <= (int)CustomOptionHolder.additionalEmergencyCoolDownCondition.getFloat())
                {
                    ShipStatus.Instance.EmergencyCooldown += CustomOptionHolder.additionalEmergencyCoolDown.getFloat();
                }
            }
        }
    }


    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    class BaseExileControllerPatch
    {
        public static bool Prefix(ExileController __instance)
        {
            WrapUpPrefix(__instance);

            WrapUpPostfix();

            List<Il2CppSystem.Collections.IEnumerator> sequence = new List<Il2CppSystem.Collections.IEnumerator>();

            if (DestroyableSingleton<TutorialManager>.InstanceExists || !GameManager.Instance.LogicFlow.IsGameOverDueToDeath())
            {
                sequence.Add(ShipStatus.Instance.PrespawnStep());
                sequence.Add(Effects.Action(new System.Action(() => { __instance.ReEnableGameplay(); })));
            }
            sequence.Add(Effects.Action(new System.Action(() =>
            {
                UnityEngine.Object.Destroy(__instance.gameObject);
            })));

            var refArray = new Il2CppReferenceArray<Il2CppSystem.Collections.IEnumerator>(sequence.ToArray());
            HudManager.Instance.StartCoroutine(Effects.Sequence(refArray));


            return false;
        }
    }

    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    class AirshipExileControllerPatch
    {
        public static void Prefix(AirshipExileController __instance)
        {
            WrapUpPrefix(__instance);
        }
        public static void Postfix(AirshipExileController __instance)
        {
            WrapUpPostfix();
        }
    }

    [HarmonyPatch(typeof(PbExileController), nameof(PbExileController.PlayerSpin))]
    class ExilePolusHatFixPatch
    {
        public static void Prefix(PbExileController __instance)
        {
            __instance.Player.cosmetics.hat.transform.localPosition = new Vector3(-0.2f, 0.6f, 1.1f);
        }
    }


    static void OnExiled(GameData.PlayerInfo? exiled)
    {
        if (exiled != null)
        {
            byte[] voters = MeetingHudPatch.GetVoters(exiled.PlayerId);
            exiled.GetModData().role.OnExiledPre(voters, exiled.PlayerId);

            if (exiled.PlayerId == PlayerControl.LocalPlayer.PlayerId)
            {
                Helpers.RoleAction(exiled.PlayerId, (role) => { role.OnExiledPre(voters); });
            }
        }

        Events.Schedule.OnPostMeeting();

        if (exiled != null)
        {
            byte[] voters = MeetingHudPatch.GetVoters(exiled.PlayerId);

            if (exiled.GetModData().role.OnExiledPost(voters, exiled.PlayerId))
            {
                Game.GameData.data.playersArray[exiled.PlayerId].Die(Game.PlayerData.PlayerStatus.Exiled);


                PlayerControl @object = exiled.Object;
                if (@object)
                {
                    @object.Exiled();
                }
                exiled.IsDead = true;


                Helpers.RoleAction(exiled.PlayerId, (role) => { role.OnDied(exiled.PlayerId); });
                Helpers.RoleAction(PlayerControl.LocalPlayer.PlayerId, (role) => { role.OnAnyoneDied(exiled.PlayerId); });

                if (exiled.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    Helpers.RoleAction(exiled.PlayerId, (role) => { role.OnExiledPost(voters); });
                    Helpers.RoleAction(exiled.PlayerId, (role) => { role.OnDied(); });

                    Game.GameData.data.myData.CanSeeEveryoneInfo = true;
                }
            }
            else
            {
                exiled.IsDead = false;
            }
        }

        Objects.CustomButton.OnMeetingEnd();
        Objects.CustomObject.OnMeetingEnd();
        Expansion.GridArrangeExpansion.OnMeetingEnd();
    }

    static void WrapUpPrefix(ExileController __instance)
    {
        __instance.exiled = null;
    }

    static void WrapUpPostfix()
    {

        Game.GameData.data.ColliderManager.OnMeetingEnd();
        Game.GameData.data.UtilityTimer.OnMeetingEnd();

        Helpers.RoleAction(Game.GameData.data.myData.getGlobalData(), (r) => r.OnMeetingEnd());

        //死体はすべて消去される
        foreach (Game.DeadPlayerData deadPlayerData in Game.GameData.data.deadPlayers.Values)
        {
            deadPlayerData.EraseBody();
        }
    }

    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), new Type[] { typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
    class ExileControllerMessagePatch
    {
        static void Postfix(ref string __result, [HarmonyArgument(0)] StringNames id)
        {
            try
            {
                if (ExileController.Instance != null && ExileController.Instance.exiled != null)
                {
                    PlayerControl player = Helpers.playerById(ExileController.Instance.exiled.Object.PlayerId);
                    if (id is StringNames.ImpostorsRemainP or StringNames.ImpostorsRemainS)
                    {
                        bool flag = false;
                        foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                        {
                            if (p.GetModData().role == Roles.Roles.Bartender && p.GetModData().IsAlive) flag = true;
                            //Debug.LogWarning(string.Format("ExileControllPatch - {0} : {1}", p.name, p.GetModData().role.LocalizeName));
                        }
                        if (flag) __result += "\n" + Language.Language.GetString("text.exile.bartenderAddition");
                    }
                    if (player == null) return;
                    // Exile role text
                    if ((id is StringNames.ExileTextPN or StringNames.ExileTextSN or StringNames.ExileTextPP or StringNames.ExileTextSP) && 
                    CustomOptionHolder.meetingOptions.getBool() && CustomOptionHolder.showRoleOfExiled.getBool())
                    {
                        __result = Language.Language.GetString("text.exile.role").Replace("%PLAYER%",player.Data.PlayerName);
                        string roleText = Language.Language.GetString("role." + player.GetModData().role.GetActualRole(player.GetModData()).LocalizeName + ".name");
                        foreach(Roles.ExtraRole extra in player.GetModData().extraRole){
                            roleText += Language.Language.GetString("text.exile.connection") + Language.Language.GetString("role." + extra.LocalizeName + ".name");
                        }
                        __result = __result.Replace("%ROLE%",roleText);
                    }
                    // Hide number of remaining impostors on Jester win
                    if (id is StringNames.ImpostorsRemainP or StringNames.ImpostorsRemainS)
                    {
                        if (player.GetModData().role == Roles.Roles.Jester){
                            __result = Language.Language.GetString("text.exile.jesterAddition");
                            return;
                        }
                        else if (player == Roles.NeutralRoles.Cascrubinter.target) __result = Language.Language.GetString("text.exile.cascrubinterAddition");
                    } 
                }
            }
            catch
            {
                // pass - Hopefully prevent leaving while exiling to softlock game
            }
        }
    }
}