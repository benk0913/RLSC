using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopNotificationUI : MonoBehaviour
{
    public static TopNotificationUI Instance;

    public List<TopNotificationInstance> NotificationQueue = new List<TopNotificationInstance>();

    [SerializeField]
    CanvasGroup CG;

    [SerializeField]
    TextMeshProUGUI Field;

    [SerializeField]
    Image Frame; 

    private void Awake()
    {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        ShowRoutineInstance = null;
    }

    public void Show(TopNotificationInstance instance)
    {
        this.gameObject.SetActive(true);

        if(instance.Force)
        {
            if (ShowRoutineInstance != null)
            {
                StopCoroutine(ShowRoutineInstance);
                ShowRoutineInstance = null;
            }
        }

        if(ShowRoutineInstance != null)
        {
            NotificationQueue.Add(instance);
            return;
        }

        ShowRoutineInstance = StartCoroutine(ShowRoutine(instance));
    }

    public void Hide()
    {
        if (ShowRoutineInstance != null)
        {
            StopCoroutine(ShowRoutineInstance);
            ShowRoutineInstance = null;
        }

        CG.alpha = 0f;
        this.gameObject.SetActive(false);
    }

    Coroutine ShowRoutineInstance;
    IEnumerator ShowRoutine(TopNotificationInstance instance)
    {
        yield return 0;

        Field.text = instance.Content;

        if(instance.Color == Color.clear)
        {
            Frame.color = Color.clear;
            Field.color = Color.white;
        }
        else
        {
            Frame.color = instance.Color;
            Field.color = instance.Color;
        }

        while(CG.alpha < 1f)
        {
            CG.alpha += Time.deltaTime;
            yield return 0;
        }

        yield return new WaitForSeconds(instance.Length);

        while (CG.alpha > 0f)
        {
            CG.alpha -= Time.deltaTime;
            yield return 0;
        }


        ShowRoutineInstance = null;

        if(NotificationQueue.Count > 0)
        {
            Show(NotificationQueue[0]);
            NotificationQueue.RemoveAt(0); // don't worry this will not result in an infinite loop.
        }
        else
        {
            Hide();
        }
    }

    public class TopNotificationInstance
    {
        public string Content;
        public Color Color;
        public float Length = 3f;
        public bool Force = false;

        public TopNotificationInstance(string message, Color clr = default, float length = 3f, bool force = false)
        {
            this.Content = message;

            this.Color = clr;

            if (Color == default)
            {
                Color = Color.clear;
            }

            this.Length = length;

            this.Force = force;
        }
    }
}
