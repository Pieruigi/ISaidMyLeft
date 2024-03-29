using Palmmedia.ReportGenerator.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

       
               

        //[SerializeField]
        //float yawMaxSpeed = 60f;

        [SerializeField]
        float acceleration = 10;

        float borderSize;

        [SerializeField]
        float cameraHeight = 20;

        [SerializeField]
        Transform cameraRoot;

        float zoom;

        Camera helperCamera;

        Vector3 moveInput;
        Vector3 buttonMoveDownPosition;
        Vector3 moveCurrVel;
        Vector3 targetMove;
        Vector3 startingMove;
        Vector3 move;

        [SerializeField] float moveSpeed = 50;
        [SerializeField] float moveSmoothTime = .25f;
        //float maxSpeed = 50;


        float zoomInput = 0;
        Vector3 velocity;
        
        float yawInput;
        float yaw = 0;
        Vector3 buttonRotateDownPosition;
        float startingYaw;
        float yawCurrSpeed = 0;
        float targetYaw;
        [SerializeField] float yawSpeed = 180;
        [SerializeField] float yawSmoothTime = .25f;

        float pitchInput;
        float pitch = 0;
        float startingPitch;
        float minPitch = 60;
        float maxPitch = 90;
        float targetPitch;
        float pitchCurrSpeed;
        [SerializeField] float pitchSpeed = 30;
        [SerializeField] float pitchSmoothTime = .25f;


        //float targetYawInput = 0;

        private void Start()
        {
            borderSize = Screen.width / 16f;
            
        }

        private void Update()
        {
            if (PlayerManager.Instance.LocalPlayer.IsCharacter || !PlayerController.Instance)
                return;

            CheckInput();

            Zoom();

           
            Rotation();

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
                      
            if (Input.GetMouseButtonDown(1))
            {
                buttonRotateDownPosition = Input.mousePosition;
                // Yaw
                startingYaw = targetYaw;
                yawInput = 0;
                // Pitch
                startingPitch = targetPitch;
                pitchInput = 0;
            }
            else if (Input.GetMouseButton(1))
            {
                yawInput = (Input.mousePosition.x - buttonRotateDownPosition.x) / Screen.width;
                pitchInput = -(Input.mousePosition.y - buttonRotateDownPosition.y) / Screen.height;
            }


            // Keyboard
            zoomInput = 0;
            zoomInput += Input.GetKey(KeyCode.Z) ? -1 : 0;
            zoomInput += Input.GetKey(KeyCode.X) ? 1 : 0;

            // Mouse wheel
            if(zoomInput == 0)
                zoomInput = Input.mouseScrollDelta.y * 10;


            if (Input.GetMouseButtonDown(0))
            {
                buttonMoveDownPosition = Input.mousePosition;
                startingMove = targetMove;

                moveInput = Vector3.zero;
            }
            else if (Input.GetMouseButton(0))
            {
                float moveX = (Input.mousePosition.x - buttonMoveDownPosition.x) / Screen.width;
                float moveZ = (Input.mousePosition.y - buttonMoveDownPosition.y) / Screen.height;
                moveInput = new Vector3(-moveX, 0, -moveZ);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                moveInput = Vector3.zero;
            }
          
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

        
        void Rotation()
        {
            targetPitch = startingPitch + pitchInput * pitchSpeed;
            targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
            pitch = Mathf.SmoothDamp(pitch, targetPitch, ref pitchCurrSpeed, pitchSmoothTime);

            targetYaw = startingYaw + yawInput * yawSpeed;
            yaw = Mathf.SmoothDamp(yaw, targetYaw, ref yawCurrSpeed, yawSmoothTime);

            
            //helperCamera.transform.localRotation = Quaternion.Euler(0, yaw, 0);
            cameraRoot.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        }

       

        void Move()
        {
            targetMove = startingMove + moveInput * moveSpeed;
            move = Vector3.SmoothDamp(move, targetMove, ref moveCurrVel, moveSmoothTime);
            cameraRoot.position += move * Time.deltaTime;
        }

      
        private void HandleOnSpawned()
        {
            Init();
        }

        void Init()
        {
            if (PlayerManager.Instance.LocalPlayer.IsCharacter)
                return;

            cameraRoot.position = new Vector3(PlayerController.Instance.transform.position.x, 0, PlayerController.Instance.transform.position.z);
            cameraRoot.eulerAngles = new Vector3(90f, 0, 0);
            
            helperCamera = Camera.main;
            helperCamera.transform.parent = cameraRoot;
            helperCamera.transform.position = cameraRoot.position + Vector3.up * cameraHeight;
            helperCamera.transform.localRotation = Quaternion.identity;
            //helperCamera.transform.localEulerAngles = new Vector3(0, 0, 0);
            startingPitch = 90;
            startingYaw = 0;

            zoom = (zoomMin - zoomMax) / 2f;
            helperCamera.fieldOfView = zoom;
        }
    }

}
