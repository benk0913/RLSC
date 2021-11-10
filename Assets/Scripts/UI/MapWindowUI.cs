using EdgeworldBase;
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
       
    }


    public void Show(ActorData actorData, object data = null)
    {
        IsOpen = true;

        this.gameObject.SetActive(true);
        
        
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
    }

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
