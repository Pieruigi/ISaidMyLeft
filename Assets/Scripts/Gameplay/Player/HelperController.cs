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

       
        [SerializeField]
        float maxSpeed = 5;

        [SerializeField]
        float yawMaxSpeed = 60f;

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
        float yawInput;
        float zoomInput = 0;
        Vector3 velocity;
        float yaw = 0;
        Vector3 buttonDownPosition;
        float startingYaw;
        float yawSpeed = 0;

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

       float targetYawInput = 0;
       //float targetYawDiff = 0;
        void CheckInput()
        {
            //yawInput = 0;
            //yawInput += Input.GetKey(KeyCode.Q) ? 1 : 0;
            //yawInput += Input.GetKey(KeyCode.E) ? -1 : 0;
            
            if (Input.GetMouseButtonDown(1))
            {
                buttonDownPosition = Input.mousePosition;
                startingYaw = yaw;

                //targetYawDiff = targetYawInput - yawInput;
                targetYawInput = 0;
                yawInput = 0;
            }
            else if (Input.GetMouseButton(1))
            {
                targetYawInput = (Input.mousePosition.x - buttonDownPosition.x) / Screen.width;
                //yawInput = Mathf.MoveTowards(yawInput, (Input.mousePosition.x - buttonDownPosition.x) / Screen.width, Time.deltaTime * .2f);
            }
            //else if (Input.GetMouseButtonUp(1))
            //{
            //    targetYawInput = 0;
            //}

            yawInput = Mathf.MoveTowards(yawInput, targetYawInput/* + targetYawDiff*/, Time.deltaTime);
           

            // Keyboard
            zoomInput = 0;
            zoomInput += Input.GetKey(KeyCode.Z) ? -1 : 0;
            zoomInput += Input.GetKey(KeyCode.X) ? 1 : 0;

            // Mouse wheel
            if(zoomInput == 0)
                zoomInput = Input.mouseScrollDelta.y * 10;

            moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            if(moveInput == Vector3.zero && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) )
            {
                if(MouseOnTheLeft() || MouseOnTheRight())
                    moveInput.x = MouseOnTheLeft() ? -1 : 1;
                if (MouseOnTheTop() || MouseOnTheBottom())
                    moveInput.z = MouseOnTheBottom() ? -1 : 1;
            }
        }

        bool MouseOnTheLeft()
        {
            return Input.mousePosition.x < borderSize;
        }

        bool MouseOnTheRight()
        {
            return Input.mousePosition.x > Screen.width - borderSize;
        }

        bool MouseOnTheBottom()
        {
            return Input.mousePosition.y < borderSize;
        }

        bool MouseOnTheTop()
        {
            return Input.mousePosition.y > Screen.height - borderSize;
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
            float targetYaw = startingYaw + yawInput * 180f;

            float tmp = 0;
            //if (targetYaw < yaw)
            //{
            //    tmp = Mathf.SmoothDamp(-yaw, -targetYaw, ref yawSpeed, .5f, yawMaxSpeed);
            //    yaw = -tmp;
            //}
            //else
            //{
                tmp = Mathf.SmoothDamp(yaw, targetYaw, ref yawSpeed, .25f, yawMaxSpeed);
                yaw = tmp;
            //}
                

            Debug.Log($"targetYaw:{targetYaw}, yaw:{yaw}");
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
