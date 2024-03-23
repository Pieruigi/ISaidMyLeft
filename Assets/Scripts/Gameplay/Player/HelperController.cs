using Palmmedia.ReportGenerator.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace ISML
{
    public class HelperController : Singleton<HelperController>
    {
        [SerializeField]
        float zoomMax = 100;

        [SerializeField]
        float zoomMin = 40;

        [SerializeField]
        float zoomSpeed = 60;

       
        [SerializeField]
        float maxSpeed = 5;

        [SerializeField]
        float rotationMaxSpeed = 90;

        [SerializeField]
        float acceleration = 10;

        [SerializeField]
        float rotationAcceleration = 100f;



        [SerializeField]
        float cameraHeight = 20;

        [SerializeField]
        Transform cameraRoot;

        float zoom;

        

        Camera helperCamera;

        Vector3 moveInput;
        float yawInput;
        float zoomInput = 0;
        Vector3 velocity;
        float rotationSpeed;
        float yaw = 0;

        private void Update()
        {
            if (PlayerManager.Instance.LocalPlayer.IsCharacter)
                return;

            CheckInput();

            Zoom();

            Yaw();

            Move();
        }

        private void OnEnable()
        {
            PlayerController.OnSpawned += HandleOnSpawned;
        }

        private void OnDisable()
        {
            PlayerController.OnSpawned -= HandleOnSpawned;
        }

        void CheckInput()
        {
            yawInput = 0;
            yawInput += Input.GetKey(KeyCode.Q) ? 1 : 0;
            yawInput += Input.GetKey(KeyCode.E) ? -1 : 0;

            zoomInput = 0;
            zoomInput += Input.GetKey(KeyCode.Z) ? -1 : 0;
            zoomInput += Input.GetKey(KeyCode.X) ? 1 : 0;

            moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        }

        void Zoom()
        {
            if ((zoom == zoomMin && zoomInput > 0) || (zoom == zoomMax && zoomInput < 0))
                return;

            zoom += zoomInput * zoomSpeed * Time.deltaTime;
            zoom = Mathf.Clamp(zoom, zoomMax, zoomMin);
            if(zoom != helperCamera.fieldOfView)
                helperCamera.fieldOfView = zoom;
        }

        void Yaw()
        {
            float targetSpeed = yawInput * rotationMaxSpeed;
            rotationSpeed = Mathf.MoveTowards(rotationSpeed, targetSpeed, rotationAcceleration * Time.deltaTime);
            yaw += rotationSpeed * Time.deltaTime;
            cameraRoot.transform.rotation = Quaternion.Euler(0, yaw, 0);
        }

        void Move()
        {
            float currSpeed = velocity.magnitude;
            float targetSpeed = 0;
            Vector3 targetDir = cameraRoot.forward * moveInput.z + cameraRoot.right * moveInput.x;
            if (moveInput.magnitude > 0)
            {
                if(currSpeed > 0 && Vector3.Dot(targetDir, velocity) < 0)
                    targetSpeed = 0;
                else
                    targetSpeed = maxSpeed;

               
            }
            else
            {
                targetSpeed = 0;
            }

            //if (targetSpeed < 0)
            //    targetSpeed = 0;
            //if (targetSpeed > maxSpeed)
            //    targetSpeed = maxSpeed;
           
            Vector3 targetVelocity = targetDir.normalized * targetSpeed;
            velocity = Vector3.MoveTowards(velocity, targetVelocity, acceleration * Time.deltaTime);
            cameraRoot.position +=  velocity * Time.deltaTime;
        }

        private void HandleOnSpawned()
        {
            Init();
        }

        void Init()
        {
            if (PlayerManager.Instance.LocalPlayer.IsCharacter)
                return;

            cameraRoot.position = new Vector3(cameraRoot.position.x, cameraHeight, cameraRoot.position.z);
            helperCamera = Camera.main;

            
            helperCamera.transform.parent = cameraRoot;
            helperCamera.transform.localPosition = PlayerController.Instance.transform.position;
            helperCamera.transform.localRotation = Quaternion.identity;
            helperCamera.transform.eulerAngles = new Vector3(90f, 0, 0);
            zoom = (zoomMin - zoomMax) / 2f;
            helperCamera.fieldOfView = zoom;
        }
    }

}
