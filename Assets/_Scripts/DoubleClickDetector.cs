using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleClickDetector : MonoBehaviour
{
    public KeyCode targetKey;
    public float doubleClickTimeThreshold = 0.3f;

    private float lastClickTime = 0f;

    private void Update()
    {
        if (Input.GetKeyDown(targetKey))
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= doubleClickTimeThreshold)
            {
                // Double-click detected
                Debug.Log("Double-clicked " + targetKey.ToString());
            }

            lastClickTime = Time.time;
        }
    }
}
