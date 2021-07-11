using UnityEngine;
using System.Collections;
using System.Linq;
using EdgeworldBase;
using UnityEngine.UI;

public class KeyBindingWindowUI : MonoBehaviour {

    [SerializeField]
    Transform Container;

    KeyBindingPieceUI LastClickedPiece;

    [SerializeField]
    ScrollRect Scroll;

    [SerializeField]
    SelectionGroupUI SGroup;

    void Start()
    {
        CORE.Instance.ConditionalInvokation(X=>!ResourcesLoader.Instance.m_bLoading,()=>{Init();});
    }

	public void Init()
    {
        Clear();

        GameObject tempPiece;
        string key;
        for(int i=0;i<InputMap.Map.Count;i++)
        {
            tempPiece = ResourcesLoader.Instance.GetRecycledObject("KeyBindingPiece");
            tempPiece.transform.SetParent(Container, false);
            key = InputMap.Map.Keys.ElementAt(i);
            KeyBindingPieceUI KeyBindingPiece = tempPiece.GetComponent<KeyBindingPieceUI>();
            KeyBindingPiece.SetInfo(key, InputMap.Map[key]);
            KeyBindingPiece.m_btn.onClick.AddListener(delegate {OnKeyBindingPieceClicked(KeyBindingPiece);});
        }

        SGroup.RefreshGroup();
        //CORE.Instance.DelayedInvokation(0.1f,()=>SGroup.RefreshGroup());
    }

    public void OnKeyBindingPieceClicked(KeyBindingPieceUI KeyBindingPiece)
    {
        bool IsPieceAlreadyActive = KeyBindingPiece.isWaitingForKey;
        if (LastClickedPiece != null)
        {
            LastClickedPiece.CloseBinding();
        }

        if (IsPieceAlreadyActive)
        {
            // keep the piece disabled and clear the last clicked piece
            LastClickedPiece = null;
        }
        else 
        {
            // save the last piece so it can be closed if another one is clicked
            LastClickedPiece = KeyBindingPiece;
            KeyBindingPiece.OnClick();
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
