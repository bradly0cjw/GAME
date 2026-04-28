using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target = null;  // you need to assign the target
    float smoothTime = 0.3f;
    float smoothSpeed = 0.25f;
    Vector3 offset = new Vector3(0, 5, -10);
    private Vector3 velocity = Vector3.zero;

    float elapsed = 0;
    float duration = 1;

    
    // Start is called before the first frame update
    void Start()
    {
        //goFollow();
    }

    // LateUpdate is called after all the Update called
    void LateUpdate()
    {
        if (target != null)
        {
            offset = -3 * target.forward + target.up * 2.5f;
            Vector3 desiredPosition = target.position + offset;
            Vector2 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = desiredPosition;
            transform.LookAt(target);
        }

        // another version:
        //Vector3 targetPosition = target.position + offset;
        // Smoothly move the camera towards that target position
        //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        //transform.LookAt(target);
    }

    private void goFollow()
    {
        if (target != null)
        {
            offset = -target.forward * 3 + target.up * 2.5f;
            transform.position = target.position + offset;
            transform.LookAt(target);
        }
    }
}
