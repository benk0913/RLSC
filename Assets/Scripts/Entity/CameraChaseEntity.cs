using Ferr;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChaseEntity : MonoBehaviour
{
    public static CameraChaseEntity Instance;

    [SerializeField]
    public Transform ReferenceObject;


    public float Speed = 1f;


    public float YOffset = 3f;

    public float Extrapolation = 1f;

    float DefaultSize;

    Camera CurrentCam;

    Vector3 deltaPosition;
    Vector3 lastPosition;

    FocusInstance CurrentFocus;
    Camera FocusCam;
    Coroutine FocusRoutineInstance;
    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CurrentCam = GetComponent<Camera>();
        DefaultSize = CurrentCam.orthographicSize;
    }

    void LateUpdate()
    {
        if(CurrentFocus != null)
        {
            return;
        }

        deltaPosition = transform.position - lastPosition;
        lastPosition = transform.position;

        if (ReferenceObject == null)
        {
            ActorData foundActor = CORE.Instance.Room.PlayerActor;

            if (foundActor != null && foundActor.ActorEntity != null)
            {
                ReferenceObject = foundActor.ActorEntity.transform;
            }

            return;
        }
        
        Vector3 targetPosition = new Vector3(ReferenceObject.transform.position.x + ((deltaPosition.x > 0? 1f:-1f)*Extrapolation), ReferenceObject.transform.position.y + YOffset, transform.position.z);

        
        transform.position = Vector3.Lerp(transform.position,
            targetPosition, 
            (Speed * Time.deltaTime * Vector2.Distance(transform.position,targetPosition))*0.5f);
    }

    public void Shake(float power = 3f, float duration = 1f)
    {
        CameraShake.Shake(Vector3.one * power, duration);
    }

    public void FocusOn(FocusInstance focus)
    {
        if(CurrentFocus != null)
        {
            Unfocus();
        }

        CurrentFocus = focus;

        FocusRoutineInstance = StartCoroutine(FocusRoutine());
    }

    IEnumerator FocusRoutine()
    {
        FocusCam = Instantiate(ResourcesLoader.Instance.GetObject("FocusCamera")).GetComponent<Camera>();

        FocusCam.transform.SetParent(transform);
        FocusCam.transform.localScale = Vector3.one;
        FocusCam.transform.position = Vector3.zero;

        Vector2 startPoint = transform.position;
        Vector3 endPoint = new Vector3(CurrentFocus.FocusObj.position.x, CurrentFocus.FocusObj.position.y, transform.position.z);

        float t = 0f;
        while(t<1f)
        {
            t += 1f * Time.deltaTime;

            transform.position = FocusCam.transform.position = Vector3.Lerp(startPoint, endPoint, t);
            CurrentCam.orthographicSize = FocusCam.orthographicSize = Mathf.Lerp(DefaultSize, CurrentFocus.CustomSize, t);

            yield return 0;
        }

        while(true)
        {
            if(Input.GetKeyDown(InputMap.Map["Escape"]))
            {
                break;
            }
            yield return 0;
        }

        Unfocus();
    }

    public void Unfocus()
    {
        CurrentFocus = null;
        Destroy(FocusCam.gameObject);
        FocusCam = null;
        CurrentCam.orthographicSize = DefaultSize;
        StopCoroutine(FocusRoutineInstance);
        FocusRoutineInstance = null;
    }
}

[System.Serializable]
public class FocusInstance
{
    public Transform FocusObj;
    public float CustomSize;

    public FocusInstance(Transform focusObj, float customSize)
    {
        this.FocusObj = focusObj;
        this.CustomSize = customSize;
    }
}
