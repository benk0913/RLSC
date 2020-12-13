using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControlEntity : MonoBehaviour
{
    public void shortShake(float power = 3f)
    {
        CameraChaseEntity.Instance.Shake(1f, power);
    }
}
