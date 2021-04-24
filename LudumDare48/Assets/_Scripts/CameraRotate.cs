using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField]
    private float maxRotation = 5;
    [SerializeField]
    private float rotationIncrement = 0.1f;
    [SerializeField]
    private float haltAngle = 0.1f;
    private float currentRotation;
    private void Update()
    {
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        if (horizontal > 0)
        {
            if (currentRotation > -maxRotation) Rotate(-1);
        } else if (horizontal < 0) {
            if (currentRotation < maxRotation) Rotate(1);
        } else {
            if (currentRotation < -haltAngle)
            {
                Rotate(1);
            }
            else if (currentRotation > haltAngle)
            {
                Rotate(-1);
            } else {
                transform.localRotation = Quaternion.identity;
                currentRotation = 0;
            }
        }
    }
    private void Rotate(int direction)
    {
        transform.Rotate(0, 0, direction * rotationIncrement * Time.deltaTime);
        currentRotation += direction * rotationIncrement * Time.deltaTime; 
    }
}
