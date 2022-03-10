using UnityEngine;
using System.Collections;
using System.Linq;
using EdgeworldBase;
using UnityEngine.UI;

public class KeyBindingWindowUI : MonoBehaviour {

    public static KeyBindingWindowUI Instance;

    [SerializeField]
    Transform Container;

    KeyBindingPieceUI LastClickedPiece;

    [SerializeField]
    ScrollRect Scroll;

    [SerializeField]
    SelectionGroupUI SGroup;

    public bool IsWaitingForKeyActive;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CORE.Instance.ConditionalInvokation(X=>!ResourcesLoader.Instance.m_bLoading,()=>{Init();});
    }

    void OnDisable()
    {
        CloseBinding();
    }

    Coroutine RefreshingRoutine;
	public void Init()
    {
        Clear();

        if(RefreshingRoutine != null)
        {
            StopCoroutine(RefreshingRoutine);
        }

        RefreshingRoutine = CORE.Instance.DelayedInvokation(0.1f,()=>
        {    
            GameObject tempPiece;
            string key;
            for(int i=0;i<InputMap.Map.Count;i++)
            {
                tempPiece = ResourcesLoader.Instance.GetRecycledObject("KeyBindingPiece");
                tempPiece.transform.SetParent(Container, false);
                key = InputMap.Map.Keys.ElementAt(i);
                KeyBindingPieceUI KeyBindingPiece = tempPiece.GetComponent<KeyBindingPieceUI>();
                KeyBindingPiece.SetInfo(key, InputMap.Map[key]);
            }


            CORE.Instance.DelayedInvokation(0.1f,()=>SGroup.RefreshGroup());
        });
    }

    public void OnKeyBindingPieceClicked(KeyBindingPieceUI KeyBindingPiece)
    {
        if (LastClickedPiece == KeyBindingPiece)
        {
            CloseBinding();
            return;
        }

        KeyBindingPiece.SetBinding();
        IsWaitingForKeyActive = true;

        if (LastClickedPiece != null)
        {
            LastClickedPiece.CloseBinding();
        }

        LastClickedPiece = KeyBindingPiece;
    }

    public void CloseBinding()
    {
        IsWaitingForKeyActive = false;

        if (LastClickedPiece != null)
        {
            LastClickedPiece.CloseBinding();
            LastClickedPiece = null;
        }
    }

    private void Clear()
    {
        for(int i=0;i<Container.childCount;i++)
        {
            Container.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ScrollDown()
    {
        Scroll.verticalNormalizedPosition -= 1f *Time.deltaTime;
    }

    public void ScrollUp()
    {
        Scroll.verticalNormalizedPosition += 1f*Time.deltaTime;
    }
}
