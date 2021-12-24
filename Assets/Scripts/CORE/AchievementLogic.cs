using System.Collections;
using System.Collections.Generic;
#if !UNITY_ANDROID && !UNITY_IOS
using Steamworks;
#endif
using UnityEngine;
using UnityEngine.Events;

public class AchievementLogic : MonoBehaviour
{
    public static AchievementLogic Instance;
    
#if !UNITY_ANDROID && !UNITY_IOS
    public List<AchievementInstance> Instances = new List<AchievementInstance>();

    public List<ScriptableObject> QuizzWhizzQuestionsAsked = new List<ScriptableObject>();

    void Awake()
    {
        Instance = this;
    }


    protected Callback<UserAchievementStored_t> GetUserAchievmentStored;
    void OnUserAchievmentStored(UserAchievementStored_t pCallBack)
    {
        CORE.Instance.LogMessage("STEAM - ACH STORE COMPLETE!");
    }


    public void StartAchievementLogic()
    {
        try
        {

            if (GetUserAchievmentStored == null)
                GetUserAchievmentStored = Callback<UserAchievementStored_t>.Create(OnUserAchievmentStored);

            foreach (AchievementInstance ach in Instances)
            {
                SteamUserStats.GetAchievement(ach.Key, out ach.State);

                if (!ach.State)
                {
                    switch (ach.Key)
                    {
                        case "NEW_ACHIEVEMENT_1_1"://HUMBLE
                            {
                                AchievementInstance uniqueAch = ach;
                                UnityAction achAction = null;
                                achAction = () =>
                                {
                                    SetAchievment(uniqueAch.Key);
                                    CORE.Instance.UnsubscribeFromEvent("Declined Loot Roll", achAction);
                                };
                                CORE.Instance.SubscribeToEvent("Declined Loot Roll", achAction);

                                break;
                            }
                        case "NEW_ACHIEVEMENT_1_2"://WINGMAN
                            {
                                AchievementInstance uniqueAch = ach;
                                CORE.Instance.ConditionalInvokation(
                                    X =>
                                    {
                                        return CORE.PlayerActor != null &&
                                        CORE.PlayerActor.equips != null &&
                                         CORE.PlayerActor.equips.ContainsKey("Back") &&
                                         CORE.PlayerActor.equips["Back"] != null &&
                                        CORE.PlayerActor.equips["Back"].itemName == "Fairy Wings";
                                    },
                                    () => 
                                    {
                                         SetAchievment(uniqueAch.Key);
                                    }
                                    , 1f);

                                break;
                            }
                        case "NEW_ACHIEVEMENT_1_3"://Edgeworld Experience Connoisseur
                            {
                                //SERVER
                                break;
                            }
                        case "NEW_ACHIEVEMENT_1_4"://JACK OF ALL TRADES
                            {
                                AchievementInstance uniqueAch = ach;
                                CORE.Instance.ConditionalInvokation(
                                    X =>
                                    {
                                        return
                               SocketHandler.Instance.CurrentUser != null &&
                               SocketHandler.Instance.CurrentUser.chars.Find(fsc => fsc.level >= 10) != null &&
                               SocketHandler.Instance.CurrentUser.chars.FindAll(fc => fc.level >= 10).Count >= 4;
                                    },
                                    () => { 
                                        SetAchievment(uniqueAch.Key);
                                         }
                                    , 1f);

                                break;
                            }
                        case "NEW_ACHIEVEMENT_1_5"://Quiz Whizz
                            {
                                AchievementInstance uniqueAch = ach;
                                CORE.Instance.ConditionalInvokation(
                                    X =>
                                    {
                                        foreach (ScriptableObject questionKey in QuizzWhizzQuestionsAsked)
                                        {
                                            if (PlayerPrefs.GetString(questionKey.name, "false") == "false")
                                            {
                                                return false;
                                            }
                                        }

                                        return true;
                                    },
                                    () => { 
                                        SetAchievment(uniqueAch.Key);
                                         }
                                    , 1f);

                                break;
                            }
                        case "NEW_ACHIEVEMENT_1_6"://Element Appperentice
                            {
                                AchievementInstance uniqueAch = ach;
                                CORE.Instance.ConditionalInvokation(
                                    X =>
                                    {
                                        return
                               SocketHandler.Instance.CurrentUser != null &&
                               SocketHandler.Instance.CurrentUser.chars.Find(fsc => fsc.level >= 10) != null;
                                    },
                                    () => { 
                                        SetAchievment(uniqueAch.Key);
                                         }
                                    , 1f);

                                break;
                            }
                        case "NEW_ACHIEVEMENT_1_7"://Someplace Quiet
                            {
                                AchievementInstance uniqueAch = ach;
                                CORE.Instance.ConditionalInvokation(
                                    X => { return CORE.Instance.ActiveSceneInfo != null && CORE.Instance.ActiveSceneInfo.sceneName == "TavernPrivateRoom" && CORE.Instance.CurrentParty != null && CORE.Instance.CurrentParty.members.Length > 1; },
                                    () => { 
                                        SetAchievment(uniqueAch.Key);
                                         }
                                    , 1f);

                                break;
                            }
                        case "NEW_ACHIEVEMENT_1_8"://Helper of Noobz
                            {
                                AchievementInstance uniqueAch = ach;
                                CORE.Instance.ConditionalInvokation(
                                    X =>
                                    {
                                        if (CORE.PlayerActor == null)
                                        {
                                            return false;
                                        }

                                        if (CORE.Instance.CurrentParty == null || CORE.Instance.CurrentParty.members.Length <= 1)
                                        {
                                            return false;
                                        }

                                        foreach (string member in CORE.Instance.CurrentParty.members)
                                        {
                                            ActorData actorDat = CORE.Instance.Room.Actors.Find(x => x.name == member);
                                            if (actorDat == null)
                                            {
                                                return false;
                                            }

                                            if (CORE.PlayerActor.level - actorDat.level < 3)
                                            {
                                                return false;
                                            }
                                        }

                                        return true;
                                    },
                                    () => { 
                                        SetAchievment(uniqueAch.Key);
                                         }
                                    , 1f);

                                break;
                            }
                        case "NEW_ACHIEVEMENT_1_9"://Supportive Manoeuvres
                            {
                                //SERVER
                                break;
                            }
                        case "NEW_ACHIEVEMENT_1_10":// /spit Manoeuvres 
                            {
                                AchievementInstance uniqueAch = ach;
                                UnityAction achAction = null;
                                achAction = () =>
                                {
                                    SetAchievment(uniqueAch.Key);
                                    CORE.Instance.UnsubscribeFromEvent("Declined Party Invite", achAction);
                                    
                                };
                                CORE.Instance.SubscribeToEvent("Declined Party Invite", achAction);

                                break;
                            }
                        case "NEW_ACHIEVEMENT_1_11"://Tour Guide
                            {
                                //SERVER
                                break;
                            }
                        case "NEW_ACHIEVEMENT_1_12"://Alpha Pro
                            {
                                AchievementInstance uniqueAch = ach;
                                CORE.Instance.ConditionalInvokation(
                                    X =>
                                    {
                                        return CORE.PlayerActor != null &&
                                        CORE.PlayerActor.equips != null &&
                                         CORE.PlayerActor.equips.ContainsKey("SkinHat") &&
                                         CORE.PlayerActor.equips["SkinHat"] != null &&
                                        CORE.PlayerActor.equips["SkinHat"].itemName == "EQ Alpha Pro Hat";
                                    },
                                    () => { 
                                        SetAchievment(uniqueAch.Key);
                                         }
                                    , 1f);

                                break;
                            }
                        case "NEW_ACHIEVEMENT_1_13"://Whatsupdoc
                            {
                                //Client different
                                break;
                            }
                    }

                }
            }


            
        }
        catch { }

    }

    public void SetAchievment(string achievementString)
    {
        SteamUserStats.SetAchievement(achievementString);
        SteamUserStats.StoreStats();
    }

#endif
    [System.Serializable]
    public class AchievementInstance
    {
        public string Key;
        public bool State;
    }
    
}