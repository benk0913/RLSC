using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUSTOM_TEST_ENTITY : MonoBehaviour
{
    private void Start()
    {
        CameraChaseEntity.Instance.FocusOn(new FocusInstance(transform, 5f));
    }
}
