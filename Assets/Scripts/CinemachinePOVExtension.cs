using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


/*
 
SCRIPT CREDIT TO SAMYAM ON YOUTUBE  
https://youtu.be/5n_hmqHdijM?si=5MLQ8_9rgvYOuHSL
 
*/

public class CinemachinePOVExtension : CinemachineExtension
{
    [SerializeField] float horizonalSpeed = 10f;
    [SerializeField] float verticalSpeed = 10f;
    [SerializeField] float clampAngle = 80f;


    private Vector3 startingRotation;
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (vcam.Follow)
        {
            if(stage == CinemachineCore.Stage.Aim)
            {
                if (startingRotation == null)
                {
                    startingRotation = transform.localRotation.eulerAngles;
                }

                Vector2 deltaInput = InputManager.Instance.lookInput.ReadValue<Vector2>();

                startingRotation.x += deltaInput.x * verticalSpeed * Time.deltaTime;
                startingRotation.y += deltaInput.y * horizonalSpeed * Time.deltaTime;

                startingRotation.y = Mathf.Clamp(startingRotation.y, -clampAngle, clampAngle);

                state.RawOrientation = Quaternion.Euler(-startingRotation.y,startingRotation.x,0f);

            }
        }
    }
}
