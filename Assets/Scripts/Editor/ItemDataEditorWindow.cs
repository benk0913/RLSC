using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class ItemDataEditorWindow : EditorWindow
{
    private List<ItemData> itemDataList;
    private Dictionary<ItemType, List<ItemData>> categorizedData;
    private Vector2 _scrollView;
    private bool _isShowingCashShopItems;
    ItemType _currentItemType;
    public List<string> _rarities = new List<string>() { "Common","Uncommon","Rare","Epic","Legendary","One of a kind"};
    private List<ItemData> _rewardPools = new List<ItemData>();
    private List<QuestData> _quests = new List<QuestData>();
    private List<ClassJob> classJobDataList;
    private Dictionary<ItemData, List<ClassJob>> categorizedClassJobData;
    
    [MenuItem("Window/Item Data Editor")]
    static void Init()
    {
        ItemDataEditorWindow window = (ItemDataEditorWindow)EditorWindow.GetWindow(typeof(ItemDataEditorWindow));
        window.Show();
    }

    void OnEnable()
    {
        LoadItemData();
    }

    void LoadItemData()
    {;
        itemDataList = new List<ItemData>(Resources.FindObjectsOfTypeAll<ItemData>());
        categorizedData = new Dictionary<ItemType, List<ItemData>>();
        _rewardPools.Clear();
        
        foreach (ItemData itemData in itemDataList)
        {
            if (itemData.Type != null)
            {
                if (!categorizedData.ContainsKey(itemData.Type))
                {
                    categorizedData[itemData.Type] = new List<ItemData>();
                }
                categorizedData[itemData.Type].Add(itemData);

                if (itemData.Type.name == "Reward Pool")
                {
                    _rewardPools.Add(itemData);
                }
            }
        }
        
        classJobDataList = new List<ClassJob>(Resources.FindObjectsOfTypeAll<ClassJob>());
        categorizedClassJobData = new Dictionary<ItemData, List<ClassJob>>();
        
        foreach (ClassJob classJob in classJobDataList)
        {
            if (classJob.DropsOnDeath.Count > 0)
            {
                foreach (ItemData droppedItem in classJob.DropsOnDeath)
                {
                    if (!categorizedClassJobData.ContainsKey(droppedItem))
                    {
                        categorizedClassJobData[droppedItem] = new List<ClassJob>();
                    }
                    categorizedClassJobData[droppedItem].Add(classJob);
                }
            }
        }
        
        _quests = new List<QuestData>(Resources.FindObjectsOfTypeAll<QuestData>());
    }

    void OnGUI()
    {
        _scrollView = GUILayout.BeginScrollView(_scrollView);
        
        foreach (ItemType key in categorizedData.Keys)
        {
            EditorGUILayout.ObjectField(key, typeof(ItemType), false);
        }

        _isShowingCashShopItems = GUILayout.Toggle(_isShowingCashShopItems, "Show Cash Shop Items");

        GUILayout.Label("Item Data by Type", EditorStyles.boldLabel);

        if (Selection.activeObject != null && Selection.activeObject is ItemType selectedType)
        {
            _currentItemType = selectedType;
        }

        if (_currentItemType != null)
        {
            
            if (categorizedData == null)
            {
                LoadItemData();
            }

            List<ItemData> items = categorizedData[_currentItemType];

            items = items.OrderBy(x => _rarities.IndexOf(x.Rarity.name)).ToList();

            foreach (ItemData item in items)
            {
                if (!_isShowingCashShopItems && item.CashShopItem)
                {
                    return;
                }
                
                GUILayout.BeginHorizontal();


                DrawOnGUISprite(item.Icon);

                GUILayout.Space(64);
                
                GUILayout.BeginVertical();
                EditorGUILayout.ObjectField(item, typeof(ItemData), false);
                EditorGUILayout.LabelField(item.Rarity.name);
                GUILayout.EndVertical();

                EditorGUILayout.LabelField("Sources:");
                
                foreach (ItemData rewardPool in _rewardPools)
                {
                    if (!rewardPool.Pool.Contains(item))
                    {
                        continue;
                    }
                    
                    EditorGUILayout.LabelField(rewardPool.name);
                }

                if (categorizedClassJobData.ContainsKey(item))
                {
                    foreach (ClassJob monster in categorizedClassJobData[item])
                    {
                        EditorGUILayout.LabelField(monster.name);
                    }
                }

                foreach (QuestData quest in _quests)
                {
                    if (quest.Rewards.FindAll(x => x.Type.name == "Add Item").FindAll(y => y.Value == item.name || (y.ObjectValue != null && y.ObjectValue.name == item.name)).Count == 0)
                        continue;
                    
                    if (quest.IsHidden)
                    {
                        EditorGUILayout.LabelField("(Hidden)"+quest.name);
                    }
                    else
                    {
                        EditorGUILayout.LabelField(quest.name);
                    }
                }


                GUILayout.FlexibleSpace();



                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();
        
    }
    
    void DrawOnGUISprite(Sprite sprite)
    {
        Rect c = sprite.rect;
        float slotSize = 64;
        
        float aspectRatio = c.width / c.height;
        float newWidth, newHeight;
    
        if (aspectRatio > 1)
        {
            newWidth = slotSize;
            newHeight = slotSize / aspectRatio;
        }
        else
        {
            newWidth = slotSize * aspectRatio;
            newHeight = slotSize;
        }
    
        Rect rect = GUILayoutUtility.GetRect(newWidth, newHeight);
    
        if (Event.current.type == EventType.Repaint)
        {
            var tex = sprite.texture;
            c.xMin /= tex.width;
            c.xMax /= tex.width;
            c.yMin /= tex.height;
            c.yMax /= tex.height;
    
            // Center the sprite in the 64x64 slot
            float xOffset = (slotSize - newWidth) / 2;
            float yOffset = (slotSize - newHeight) / 2;
            rect.x += xOffset;
            rect.y += yOffset;
    
            rect.width = newWidth;
            rect.height = newHeight;
    
            GUI.DrawTextureWithTexCoords(rect, tex, c);
        }
    }
}