// Script Created By Lewis Farrell - S15118289 //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform lookTarget;
    public Transform camPosition;
    public float smoothSpeed = 10f;

    void FixedUpdate()
    {
        if (camPosition != null)
            // Move camera position overtime to target position (with offset)
            transform.position = Vector3.Lerp(transform.position, camPosition.transform.position, smoothSpeed * Time.deltaTime);

        if (lookTarget != null)
            // Rotate camera to look at target
            transform.LookAt(lookTarget, lookTarget.up);
    }

    public void SetupTarget(Transform lookTarget, Transform cameraPosition)
    {
        this.lookTarget = lookTarget;
        camPosition = cameraPosition;
    }
}
