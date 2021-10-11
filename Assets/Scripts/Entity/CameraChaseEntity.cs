using Ferr;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraChaseEntity : MonoBehaviour
{
    public static CameraChaseEntity Instance;

    [SerializeField]
    public Transform ReferenceObject;

    [SerializeField]
    Camera FocusCam;

    public float Speed = 1f;


    public float YOffset = 3f;

    public float Extrapolation = 1f;

    public bool IsFocusing = false;

    float DefaultSize;

    public Camera CurrentCam;

    public Vector3 deltaPosition;
    Vector3 lastPosition;

    FocusInstance CurrentFocus;

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

                //if(Speed > 0)
                //{
                //    transform.position = new Vector3(ReferenceObject.transform.position.x,ReferenceObject.transform.position.y, transform.position.z);
                //}
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
        IsFocusing = true;

        if (FocusRoutineInstance != null)
        {
            StopCoroutine(FocusRoutineInstance);
            FocusRoutineInstance = null;
        }

        FocusCam.gameObject.SetActive(true);

        UniversalAdditionalCameraData cameraData = CurrentCam.GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Add(FocusCam);

        CurrentFocus = focus;

        FocusRoutineInstance = StartCoroutine(FocusRoutine());
    }

    IEnumerator FocusRoutine()
    {
        Vector3 endPoint = new Vector3(CurrentFocus.FocusObj.position.x, CurrentFocus.FocusObj.position.y, transform.position.z);

        float t = 0f;
        while(t<1f)
        {
            t += 1f * Time.deltaTime;

            transform.position  = Vector3.Lerp(transform.position, endPoint, t);
            FocusCam.transform.position = transform.position;

            CurrentCam.orthographicSize = Mathf.Lerp(CurrentCam.orthographicSize, CurrentFocus.CustomSize, t);
            FocusCam.orthographicSize = CurrentCam.orthographicSize;

            yield return 0;
        }

        while(true)
        {
            if(Input.GetKeyDown(InputMap.Map["Exit"]) || Input.GetButtonDown("Joystick 8"))
            {
                break;
            }
            yield return 0;
        }

        Unfocus();
    }

    public void Unfocus()
    {
        if(!IsFocusing)
        {
            return;
        }

        CurrentFocus = null;

        
        FocusCam.gameObject.SetActive(false);
        
        CurrentCam.orthographicSize = DefaultSize;

        if (FocusRoutineInstance != null)
        {
            StopCoroutine(FocusRoutineInstance);
            FocusRoutineInstance = null;
        }

        IsFocusing = false;
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
