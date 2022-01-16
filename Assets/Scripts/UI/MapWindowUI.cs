using EdgeworldBase;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapWindowUI : MonoBehaviour, WindowInterface
{
    public static MapWindowUI Instance;


    public bool IsOpen;

    public string OpenSound;
    public string HideSound;

    public string DefaultMap;
    public string DefaultPoint;

    public List<MapInstance> Maps = new List<MapInstance>();

    public List<GameObject> VisitedLocations = new List<GameObject>();

    public Transform MapPinsContainer;

    public GameObject MapMarker;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    void Start()
    {
        if(Instance == null)
        {
            return;
        }

        CORE.Instance.SubscribeToEvent("MapUpdated", ()=> { DisplayLocation(); });

        // SetupTooltips();
    }

    // void SetupTooltips()
    // {
    //     foreach(MapInstance map in Maps)
    //     {
    //         foreach(GameObject point in map.Points)
    //         {
    //             point.GetComponent<TooltipTargetUI>()?.SetTooltip(System.Text.RegularExpressions.Regex.Replace(point.name, "([a-z])([A-Z])", "$1 $2"));
                
    //         }
    //     }
    // }

    [Obsolete("Do not call Show directly. Call `CORE.Instance.ShowWindow()` instead.")]
    public void Show(ActorData actorData, object data = null)
    {
        IsOpen = true;

        this.gameObject.SetActive(true);

#if UNITY_ANDROID || UNITY_IOS
        transform.localScale = Vector3.one * 1.075f;
#endif   
        
        AudioControl.Instance.Play(OpenSound);

        RefreshVisitedShades();

        DisplayLocation();
    }

    void RefreshVisitedShades()
    {
        foreach(GameObject shadeObj in VisitedLocations)
        {
            shadeObj.SetActive(!bool.Parse(PlayerPrefs.GetString(shadeObj.name+"_vl","false")));
        }
    }

    public void DisplayLocation(string Map = "", string MapPoint = "")
    {
        if (!IsOpen)
        {
            return;
        }

        if (string.IsNullOrEmpty(Map))
            Map = CORE.Instance.ActiveSceneInfo.Map;

        if (string.IsNullOrEmpty(MapPoint))
            MapPoint = CORE.Instance.ActiveSceneInfo.MapPoint;

        if (string.IsNullOrEmpty(Map))
            Map = DefaultMap;

        if (string.IsNullOrEmpty(MapPoint))
            MapPoint = DefaultPoint;

        MapInstance mapInstance =  Maps.Find(x => x.ObjRef.name == Map);

        if(mapInstance == null)
        {
            CORE.Instance.LogMessageError("NO MAP INSTANCE " + Map);
            return;
        }

        Maps.ForEach(x => x.ObjRef.SetActive(x == mapInstance));
        GameObject mapPointInstance = mapInstance.Points.Find(X => X.name == MapPoint);

        if (mapPointInstance == null)
        {
            CORE.Instance.LogMessageError("NO MAP POINT " + Map + " | " + MapPoint);
            MapMarker.SetActive(false);
            return;
        }

        MapMarker.SetActive(true);
        MapMarker.transform.position = mapPointInstance.transform.position;

        CORE.ClearContainer(MapPinsContainer);
        
        List<GameObject> Points = new List<GameObject>();
        if(CORE.Instance.CurrentParty != null)
        {
            foreach(string mapKey in CORE.Instance.CurrentParty.scenesToMembers.Keys)
            {
                foreach(string player in CORE.Instance.CurrentParty.scenesToMembers[mapKey])
                {
                    string mapPointKey = CORE.Instance.Data.content.Scenes.Find(X=>X.sceneName == mapKey).MapPoint;

                    GameObject point = Points.Find(X=>X.name == mapPointKey);
                    
                    if(point == null)
                    {
                        GameObject mapPoint = mapInstance.Points.Find(X=>X.name == mapPointKey);

                        if(mapPoint == null)
                        {
                            continue;
                        }

                        point = ResourcesLoader.Instance.GetRecycledObject("MapFriendLocation");
                        point.transform.SetParent(MapPinsContainer,false);
                        point.transform.position = mapPoint.transform.position;
                        point.transform.localScale = Vector3.one;
                        point.GetComponent<TooltipTargetUI>().SetTooltip("<u>"+CORE.Instance.Data.content.Scenes.Find(X=>X.sceneName == mapKey).displyName+"</u>"+ System.Environment.NewLine+player);
                    }
                    else
                    {
                        point.GetComponent<TooltipTargetUI>().Text += System.Environment.NewLine+player;   
                    }
                }


            }
        }
    }

    [Obsolete("Do not call Hide directly. Call `CORE.Instance.CloseCurrentWindow()` instead.")]
    public void Hide()
    {
        IsOpen = false;
        this.gameObject.SetActive(false);

        AudioControl.Instance.Play(HideSound);
    }

    [System.Serializable]
    public class MapInstance
    {
        public GameObject ObjRef;

        public List<GameObject> Points = new List<GameObject>();
    }


}
