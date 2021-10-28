using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GEPlayDialog ", menuName = "Data/GEPlayDialog ", order = 2)]
public class GEPlayDialog : GameEvent
{
    public string DialogEntityGameObjectName;

    public DialogEntity.Dialog Dialog;

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
                    entity.StartDialog(Dialog);
                    return;
                }
            }
        }

    }
}
