using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GERandomPlayDialog ", menuName = "Data/GERandomPlayDialog ", order = 2)]
public class GERandomPlayDialog : GameEvent
{
    public string DialogEntityGameObjectName;

    public List<DialogEntity.Dialog> Dialogs = new List<DialogEntity.Dialog>();

    public override void Execute(System.Object obj = null)
    {
        base.Execute(obj);

        GameObject[] rootObjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach(GameObject rootObj in rootObjs)
        {
            DialogEntity[] dEntities = rootObj.GetComponentsInChildren<DialogEntity>();
            foreach(DialogEntity entity in dEntities)
            {
                if(entity.gameObject.name == DialogEntityGameObjectName)
                {
                    CORE.Instance.DelayedInvokation(0.1f, () => { entity.StartDialog(Dialogs[Random.Range(0, Dialogs.Count)]); });
                    
                    return;
                }
            }
        }

    }
}
