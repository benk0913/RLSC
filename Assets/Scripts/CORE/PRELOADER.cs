using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PRELOADER : MonoBehaviour
{
    void Awake()
    {
        StartCoroutine(AsyncPreload());
    }

    IEnumerator AsyncPreload()
    {
        AsyncOperation asyncOP = SceneManager.LoadSceneAsync("CORELoader");
        
        asyncOP.allowSceneActivation = false;

        while(!asyncOP.isDone)
        {
            yield return 0;
        }

        yield return 0;

        asyncOP.allowSceneActivation = true;
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().name != "PRELOADER")
        {
            Destroy(this.gameObject);
        }
    }
}
