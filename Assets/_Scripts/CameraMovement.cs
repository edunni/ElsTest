using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform camPosition;

    private void Update()
    {
        SetPosition();
    }

    private void SetPosition()
    {
        transform.position = camPosition.position;
    }
}