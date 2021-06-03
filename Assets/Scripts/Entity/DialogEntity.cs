using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogEntity : MonoBehaviour
{
    public List<DialogPiece> DialogPieces = new List<DialogPiece>();

    public int CurrentIndex = 0;

    [SerializeField]
    TextBubbleUI CurrentBubble;

    public bool isActiveDialog;

    public void StartDialog()
    {
        if(isActiveDialog)
        {
            Continue();
            return;
        }

        CurrentBubble.gameObject.SetActive(true);

        CurrentIndex = 0;

        ShowIndex(CurrentIndex);

        isActiveDialog = true;
    }

    public void Continue()
    {
        CurrentIndex++;

        if (CurrentIndex >= DialogPieces.Count)
        {
            EndDialog();
            return;
        }

        ShowIndex(CurrentIndex);
    }

    public void ShowIndex(int index)
    {
        string content = DialogPieces[index].Content;
        CurrentBubble.Show(transform, content, Continue);
        DialogPieces[index].OnDialogPiece?.Invoke();
    }

    public void EndDialog()
    {
        this.CurrentBubble.gameObject.SetActive(false);
        isActiveDialog = false;
    }

    [System.Serializable]
    public class DialogPiece
    {
        [TextArea(3, 6)]
        public string Content;

        [SerializeField]
        public UnityEvent OnDialogPiece;
        
    }
}
