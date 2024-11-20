using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [Header("Reference"), SerializeField] private Transform cam;
    
    private PlayerController playerController;
    private InputManager inputManager;
    private Quaternion? startRotationHead;
    private Quaternion? startRotationCamera;

    public void SetInputManager(PlayerController player,InputManager inputManager)
    {
        playerController = player;
        this.inputManager = inputManager;
    }

    private IEnumerator RotateToLookAt(Transform target)
    {
        inputManager.DisablePlayerInput();
        startRotationCamera = cam.localRotation;
        startRotationHead = transform.localRotation;
        // Calculate the direction to the target and the target rotation for the head (X axis only)
        Vector3 directionToTarget = target.position - cam.transform.position;
        // directionToTarget.y = 0; // Remove Y component to keep the head level

        Quaternion targetRotationHead = Quaternion.LookRotation(directionToTarget);
        Quaternion lookAt = cam.transform.rotation;
        for (int i = 0; i < 100; i++)
        {
            cam.transform.rotation = Quaternion.Slerp(lookAt, targetRotationHead, i / 99f);
            yield return null;
        }
        
        
  
// Update the player's xRotation and yRotation to match the final rotation
  
       
        //inputManager.LookAround = Vector2.zero;
        inputManager.EnablePlayerInput();
        yield return null;
    }

    private IEnumerator ReturnToStart()
    {
        inputManager.DisablePlayerInput();
        if (startRotationHead.HasValue && startRotationCamera.HasValue)
        {
            Quaternion initialHeadRotation = transform.localRotation;
            Quaternion initialCameraRotation = cam.localRotation;

            Quaternion targetHeadRotation = startRotationHead.Value;
            Quaternion targetCameraRotation = startRotationCamera.Value;

            for (int i = 0; i < 100; i++)
            {
                transform.localRotation = Quaternion.Slerp(initialHeadRotation, targetHeadRotation, i / 99f);
                cam.localRotation = Quaternion.Slerp(initialCameraRotation, targetCameraRotation, i / 99f);
                
                yield return null;
            }

            // Ensure final rotation matches exactly
            transform.localRotation = targetHeadRotation;
            cam.localRotation = targetCameraRotation;

            // Clear start rotations after returning to start
            startRotationHead = null;
            startRotationCamera = null;

          inputManager.EnablePlayerInput();
        }
    }

    public void LookAtTarget(Transform target)
    {
        StartCoroutine(RotateToLookAt(target));
    }

    public void RotateBack()
    {
      
        if (startRotationHead.HasValue && startRotationCamera.HasValue)
        {
            StartCoroutine(ReturnToStart());
        }
    }
}