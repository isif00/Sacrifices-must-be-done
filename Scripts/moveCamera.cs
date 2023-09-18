using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCamera : MonoBehaviour
{
    public Transform cameraTransform;

    private void Update()
    {
        transform.position = cameraTransform.position;
    }
}
