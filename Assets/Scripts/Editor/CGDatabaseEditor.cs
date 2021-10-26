using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using I2.Loc;
using System.Linq;

[CustomEditor(typeof(CGDatabase))]
public class CGDatabaseEditor : Editor
{

    public static void ForceSyncCG(ServerEnvironment ServerEnvironment)
    {
        string[] guids = AssetDatabase.FindAssets("t:CGDatabase", new[] { "Assets" });

        foreach (string guid in guids)
        {
            CGDatabase db = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(CGDatabase)) as CGDatabase;
            Debug.Log("Syncing " + db.name);

            
            AutofillDatabase(db);

            ServerEnvironment OriginalServerEnvironment = db.ServerEnvironment;
            db.ServerEnvironment = ServerEnvironment;

            WebRequest.SendWebRequest(db.ServerEnvironment.CGUrl, JsonConvert.SerializeObject(db, Formatting.None));

            db.ServerEnvironment = OriginalServerEnvironment;
        }
    }

    public override void OnInspectorGUI()
    {
        CGDatabase db = (CGDatabase)target;

        if (GUILayout.Button("Autofill Current Scene Data"))
        {
            AutofillCurrentSceneInfo(db);
        }

        if (GUILayout.Button("Sync With Server"))
        {
            AutofillDatabase(db);
            WebRequest.SendWebRequest(db.ServerEnvironment.CGUrl, JsonConvert.SerializeObject(db, Formatting.None));
        }

        DrawDefaultInspector();

        if (GUILayout.Button("CUSTOM SCRIPT 2"))
        {
            List<string> terms = db.Localizator.mSource.GetTermsList();
            List<string> newTerms = new List<string>();

            GameObject[] rootObjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

            foreach(GameObject rootObj in rootObjs)
            {
                TooltipTargetUI[] tt = rootObj.GetComponentsInChildren<TooltipTargetUI>();

                foreach(TooltipTargetUI t in tt)
                {
                    if (string.IsNullOrEmpty(t.Text)) continue;

                    if (!terms.Contains(t.Text) && !newTerms.Contains(t.Text))
                    {
                        newTerms.Add(t.Text);
                    }
                }

                DialogEntity[] dialogEntities = rootObj.GetComponentsInChildren<DialogEntity>();

                foreach (DialogEntity de in dialogEntities)
                {
                    DialogEntity.Dialog dialog = de.DefaultDialog;

                    AddDialogtoLocalization(dialog,terms,newTerms);
                }
            }
            
            foreach (string newTerm in newTerms)
            {
                db.Localizator.mSource.AddTerm(newTerm, eTermType.Text, true);
            }




            EditorUtility.SetDirty(db.Localizator);
        }

        if (GUILayout.Button("CUSTOM SCRIPT"))
        {
            List<string> terms = db.Localizator.mSource.GetTermsList();
            List<string> newTerms = new List<string>();
            for(int i=0;i<InputMap.Map.Keys.Count;i++)
            {
                string key = InputMap.Map.Keys.ElementAt(i);
                if (!terms.Contains(key) && !newTerms.Contains(key))
                {
                    newTerms.Add(key);
                }

                if (!terms.Contains(InputMap.Map[key].ToString()) && !newTerms.Contains(InputMap.Map[key].ToString()))
                {
                    newTerms.Add(InputMap.Map[key].ToString());
                }
            }

            for (int i = 0; i < db.content.Abilities.Count; i++)
            {
                string key = db.content.Abilities[i].name;
                if (!terms.Contains(key) && !newTerms.Contains(key))
                {
                    newTerms.Add(key);
                }
            }

            for (int i = 0; i < db.content.Abilities.Count; i++)
            {
                string key = db.content.Abilities[i].Description;
                if (!terms.Contains(key) && !newTerms.Contains(key))
                {
                    newTerms.Add(key);
                }
            }

            for (int i = 0; i < db.content.Buffs.Count; i++)
            {
                string key = db.content.Buffs[i].name;
                if (!terms.Contains(key) && !newTerms.Contains(key))
                {
                    newTerms.Add(key);
                }
            }

            for (int i = 0; i < db.content.Buffs.Count; i++)
            {
                string key = db.content.Buffs[i].Description;
                if (!terms.Contains(key) && !newTerms.Contains(key))
                {
                    newTerms.Add(key);
                }
            }

            List<ItemData> megaListOfItems = new List<ItemData>();
            megaListOfItems.AddRange(db.content.Items);
            db.content.CashShop.CashShopStores.ForEach(X => megaListOfItems.AddRange(X.StoreItems));
            for (int i = 0; i < megaListOfItems.Count; i++)
            {
                string key = megaListOfItems[i].DisplayName;
                if (!terms.Contains(key) && !newTerms.Contains(key))
                {
                    newTerms.Add(key);
                }
            }

            for (int i = 0; i < megaListOfItems.Count; i++)
            {
                string key = megaListOfItems[i].Description;
                if (!terms.Contains(key) && !newTerms.Contains(key))
                {
                    newTerms.Add(key);
                }
            }

            for (int i = 0; i < db.content.Classes.Count; i++)
            {
                string key = db.content.Classes[i].name;
                if (!terms.Contains(key) && !newTerms.Contains(key))
                {
                    newTerms.Add(key);
                }
            }

            for (int i = 0; i < db.content.Equips.Count; i++)
            {
                string key = db.content.Equips[i].name;
                if (!terms.Contains(key) && !newTerms.Contains(key))
                {
                    newTerms.Add(key);
                }
            }

            for (int i = 0; i < db.content.Expeditions.Count; i++)
            {
                string key = db.content.Expeditions[i].name;
                if (!terms.Contains(key) && !newTerms.Contains(key))
                {
                    newTerms.Add(key);
                }
            }

            for (int i = 0; i < db.content.Realms.Count; i++)
            {
                string key = db.content.Realms[i].Name;
                if (!terms.Contains(key) && !newTerms.Contains(key))
                {
                    newTerms.Add(key);
                }
            }

            for (int i = 0; i < db.content.Scenes.Count; i++)
            {
                string key = db.content.Scenes[i].displyName;
                if (!terms.Contains(key) && !newTerms.Contains(key))
                {
                    newTerms.Add(key);
                }
            }

            for (int i = 0; i < db.content.States.Count; i++)
            {
                string key = db.content.States[i].name;
                if (!terms.Contains(key) && !newTerms.Contains(key))
                {
                    newTerms.Add(key);
                }
            }

            for (int i = 0; i < db.content.Visuals.BodyParts.Count; i++)
            {
                string key = db.content.Visuals.BodyParts[i].name;
                if (!terms.Contains(key) && !newTerms.Contains(key))
                {
                    newTerms.Add(key);
                }
            }

            foreach (string newTerm in newTerms)
            {
                db.Localizator.mSource.AddTerm(newTerm, eTermType.Text, true);
            }

            
            EditorUtility.SetDirty(db.Localizator);
            //ADD CUSTOM DROPS
            // ClassJob[] jobs = db.content.Classes.FindAll(x=>x.DropsOnDeath.Count > 0).ToArray();
            // foreach(ClassJob job in jobs)
            // {
            //     Debug.Log("CUSTOM SCRIPT "+job.name);
            //     foreach(ItemData item in db.content.temporaryData)
            //     {
            //         job.DropsOnDeath.Add(item);
            //         EditorUtility.SetDirty(job);
            //     }
            // }

            //SKIN OVERRIDE REPLACE
            // foreach(ItemData item in db.content.Items)
            // {

            //     foreach(ItemData.TypeBasedOverride ooverride in item.TypeBasedOverrides)
            //     {
            //         NSkinSet newSet = new NSkinSet();
            //         newSet.TargetSprite = ooverride.Skinset.TargetSprite;
            //         newSet.TargetSpriteFemale = ooverride.Skinset.TargetSpriteFemale;
            //         newSet.BareSkin = ooverride.Skinset.BareSkin;
            //         newSet.Hair = ooverride.Skinset.Hair;
            //         newSet.Part = ooverride.Skinset.Part;

            //         ooverride.nSkinset = newSet;
            //         ooverride.Skinset =null;

            //     }
            //     EditorUtility.SetDirty(item);
            // }

            // foreach(ItemData item in db.content.Items)
            // {
            //     item.SkinOverride.Clear();
            //     EditorUtility.SetDirty(item);
            // }

            //string bigString = "";
            //foreach(Ability ability in db.content.Abilities)
            //{
            //    bigString += System.Environment.NewLine + ability.name + " | " + ability.Description; 
            //}
            //Debug.Log(bigString);


        }


        if (GUILayout.Button("CUSTOM SCRIPT 3 "))
        {
            List<string> terms = db.Localizator.mSource.GetTermsList();
            List<string> newTerms = new List<string>();
            

            for (int i = 0; i < db.content.Scenes.Count; i++)
            {
                string key = db.content.Scenes[i].displyName;
                if (!terms.Contains(key) && !newTerms.Contains(key))
                {
                    newTerms.Add(key);
                }

                key = db.content.Scenes[i].sceneName;
                if (!terms.Contains(key) && !newTerms.Contains(key))
                {
                    newTerms.Add(key);
                }
            }



            foreach (string newTerm in newTerms)
            {
                db.Localizator.mSource.AddTerm(newTerm, eTermType.Text, true);
            }


            EditorUtility.SetDirty(db.Localizator);


        }

    }

    void AddDialogtoLocalization(DialogEntity.Dialog dialog, List<string> terms, List<string> newTerms)
    {
        foreach (DialogEntity.DialogPiece piece in dialog.DialogPieces)
        {
            if (string.IsNullOrEmpty(piece.Content)) continue;

            if (!terms.Contains(piece.Content) && !newTerms.Contains(piece.Content))
            {
                newTerms.Add(piece.Content);
            }


        }

        foreach (DialogEntity.DialogDecision decision in dialog.Decisions)
        {
            if (string.IsNullOrEmpty(decision.Content)) continue;

            if (!terms.Contains(decision.Content) && !newTerms.Contains(decision.Content))
            {
                newTerms.Add(decision.Content);
            }

            AddDialogtoLocalization(decision.DefaultDialog, terms, newTerms);
        }
    }
    
    public void SubFunctionA(ItemData item, SkinSet set)
    {
        NSkinSet newSet = new NSkinSet();
                    newSet.TargetSprite = set.TargetSprite;
                    newSet.TargetSpriteFemale = set.TargetSpriteFemale;
                    newSet.BareSkin = set.BareSkin;
                    newSet.Hair = set.Hair;
                    newSet.Part = set.Part;

                    foreach(SkinSet subset in set.BundledSkins)
                    {
                        SubFunctionA(item, subset);
                    }

                    item.NewSkinOverride.Add(newSet);
    }

    public void AutofillCurrentSceneInfo(CGDatabase db)
    {

        SceneInfo currentInfo = db.content.Scenes.Find(x => x.sceneName == EditorSceneManager.GetActiveScene().name);

        if (currentInfo == null)
        {
            currentInfo = new SceneInfo();
            currentInfo.sceneName = EditorSceneManager.GetActiveScene().name;

            db.content.Scenes.Add(currentInfo);
        }

        SpawnerEntity[] spawners = FindObjectsOfType<SpawnerEntity>();

        currentInfo.Mobs.Clear();

        foreach (SpawnerEntity spawner in spawners)
        {
            if(spawner.PlayersSpawner)
            {
                currentInfo.playerSpawnX = spawner.transform.position.x;
                currentInfo.playerSpawnY = spawner.transform.position.y;
                continue;
            }

            MobSpawn spawn = new MobSpawn();
            spawn.monsterName = spawner.MobKey;
            spawn.positionX = spawner.transform.position.x;
            spawn.positionY = spawner.transform.position.y;
            spawn.respawnSeconds = spawner.RespawnSeconds;
            spawn.IrellevantMob = spawner.IrellevantMob;

            currentInfo.Mobs.Add(spawn);
        }



        PortalEntity[] portals = FindObjectsOfType<PortalEntity>();

        currentInfo.Portals.Clear();

        foreach (PortalEntity portal in portals)
        {
            
            currentInfo.Portals.Add(portal.PortalReference);
            currentInfo.Portals[currentInfo.Portals.Count-1].portalPositionX = portal.transform.position.x;
            currentInfo.Portals[currentInfo.Portals.Count-1].portalPositionY = portal.transform.position.y;
        }

        InteractableEntity[] interactables = FindObjectsOfType<InteractableEntity>(true);

        currentInfo.Interactables.Clear();

        foreach (InteractableEntity interactable in interactables)
        {
            currentInfo.Interactables.Add(new SceneInteractable(interactable.Data.interactableName, interactable.Data.interactableId,Mathf.RoundToInt(interactable.transform.position.x), Mathf.RoundToInt(interactable.transform.position.y)));
        }



        VendorEntity[] vendors = FindObjectsOfType<VendorEntity>();

        currentInfo.Vendors.Clear();

        foreach (VendorEntity vendor in vendors)
        {
            currentInfo.Vendors.Add(vendor.VendorReference);
        }
        EditorUtility.SetDirty(db);
    }
    
    public static void AutofillDatabase(CGDatabase db)
    {
        string[] guids;

        guids = AssetDatabase.FindAssets("t:ClassJob", new[] { "Assets/" + db.DataPath });
        db.content.Classes.Clear();
        foreach (string guid in guids)
        {
            db.content.Classes.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(ClassJob)) as ClassJob);
        }

        guids = AssetDatabase.FindAssets("t:Ability", new[] { "Assets/" + db.DataPath });
        db.content.Abilities.Clear();
        foreach (string guid in guids)
        {
            db.content.Abilities.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Ability)) as Ability);
        }

        guids = AssetDatabase.FindAssets("t:Buff", new[] { "Assets/" + db.DataPath });
        db.content.Buffs.Clear();
        foreach (string guid in guids)
        {
            db.content.Buffs.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Buff)) as Buff);
        }

        guids = AssetDatabase.FindAssets("t:State", new[] { "Assets/" + db.DataPath });
        db.content.States.Clear();
        foreach (string guid in guids)
        {
            db.content.States.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(State)) as State);
        }

        guids = AssetDatabase.FindAssets("t:InteractableData", new[] { "Assets/" + db.DataPath });
        db.content.Interactables.Clear();
        foreach (string guid in guids)
        {
            db.content.Interactables.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(InteractableData)) as InteractableData);
        }

        guids = AssetDatabase.FindAssets("t:Expedition", new[] { "Assets/" + db.DataPath });
        db.content.Expeditions.Clear();
        foreach (string guid in guids)
        {
            db.content.Expeditions.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Expedition)) as Expedition);
        }

        guids = AssetDatabase.FindAssets("t:SkinSet", new[] { "Assets/" + db.DataPath });
        db.content.Visuals.SkinSets.Clear();
        foreach (string guid in guids)
        {
            db.content.Visuals.SkinSets.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(SkinSet)) as SkinSet);
        }

        guids = AssetDatabase.FindAssets("t:ItemData", new[] { "Assets/" + db.DataPath });
        db.content.Items.Clear();
        foreach (string guid in guids)
        {
            ItemData item = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(ItemData)) as ItemData;
            if(string.IsNullOrEmpty(item.DisplayName))
            {
                item.DisplayName = item.name;
                EditorUtility.SetDirty(item);
            }
            db.content.Items.Add(item);
        }

        guids = AssetDatabase.FindAssets("t:BodyPart", new[] { "Assets/" + db.DataPath });
        db.content.Visuals.BodyParts.Clear();
        foreach (string guid in guids)
        {
            db.content.Visuals.BodyParts.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(BodyPart)) as BodyPart);
        }

        guids = AssetDatabase.FindAssets("t:Emote", new[] { "Assets/" + db.DataPath });
        db.content.Emotes.Clear();
        foreach (string guid in guids)
        {
            db.content.Emotes.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Emote)) as Emote);
        }

        db.content.ExpChart.Clear();
        for (int lvl = 1; lvl <= db.content.MaxLevel; lvl++) {
            db.content.ExpChart.Add(calculateExpToTargetLevel(lvl));
        }

        db.content.LatestVersion = Application.version;

        EditorUtility.SetDirty(db);
    }

    private static int calculateExpToTargetLevel(int lvl)
    {
        return (int)(50 * (Mathf.Pow(lvl, 3) - 6 * Mathf.Pow(lvl, 2) + 17 * lvl - 12) / 3);
    }
}