using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISML
{
    public enum PlayerState { Normal }

    public class PlayerController : NetworkBehaviour
    {
        public static PlayerController Instance { get; private set; }

        [SerializeField]
        float runSpeed = 5;

        [SerializeField]
        float walkSpeed = 2;

        [SerializeField]
        float crouchSpeed = 1.5f;

        [SerializeField]
        float jumpSpeed = 3f;

        [SerializeField]
        float rotationSpeed = 360f;

        [SerializeField]
        float acceleration = 10;

        [SerializeField]
        float deceleration = 10;

        [SerializeField]
        float maxPitch = 50;

        [SerializeField]
        float minPitch = -80;

        [SerializeField]
        float mouseSensitivity = 1;

        [SerializeField]
        float crouchHeight = 1;

        [SerializeField]
        Transform cameraRoot;

        CharacterController characterController;
        Camera playerCamera;
        Vector3 velocity = Vector3.zero;
        Vector2 moveInput = Vector2.zero;
        Vector2 aimInput = Vector2.zero;
        bool crouchInput = false;
        bool walkInput = false;
        bool jumpInput = false;
        bool jumping = false;

        float pitch = 0;
        float yaw = 0;

        float characterDefaultHeight;
        float cameraDefaultHeight;
        float cameraCrouchHeight;

        PlayerState state;
        public PlayerState State
        {
            get { return state; }
        }

        private void Awake()
        {
            if(!Instance)
            {
                Instance = this;
                Instance = this;
                characterController = GetComponent<CharacterController>();
             
                characterDefaultHeight = characterController.height;
                cameraDefaultHeight = cameraRoot.localPosition.y;
                float crouchMul = crouchHeight / characterDefaultHeight;
                cameraCrouchHeight = cameraDefaultHeight * crouchMul;
            }
            else
            {
                Destroy(Instance);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            
            
        }

        // Update is called once per frame
        void Update()
        {
            UpdateState();

        }

        private void LateUpdate()
        {
            LateUpdateState();
        }

        void UpdateState()
        {
            switch (state)
            {
                case PlayerState.Normal:
                    UpdateNormalState();
                    break;
            }
        }

        void UpdateNormalState()
        {
            CheckInput();

            SetPitchAndJaw();

            //UpdateStamina();
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            switch (state)
            {
                case PlayerState.Normal:
                    FixedUpdateNetworkNormalState();
                    break;
            }

        }

        void LateUpdateState()
        {
            switch (state)
            {
                case PlayerState.Normal:
                    LateUpdateNormalState();
                    break;
            }
        }

        void LateUpdateNormalState()
        {
            Pitch();
        }

        void FixedUpdateNetworkNormalState()
        {
            Yaw();

            Crouch();

            Move();
        }

        public override void Spawned()
        {
            base.Spawned();

            // Move the scene camera under the player
            playerCamera = Camera.main;
            playerCamera.transform.parent = cameraRoot;
            playerCamera.transform.localPosition = Vector3.zero;
            playerCamera.transform.localRotation = Quaternion.identity;
            
        }

        private void CheckInput()
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            aimInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            crouchInput = Input.GetAxis("Crouch") > 0;
            walkInput = !crouchInput && Input.GetAxis("Walk") > 0;
            jumpInput = !jumping && !crouchInput && Input.GetAxis("Jump") > 0;
        }

        void SetPitchAndJaw()
        {
            yaw += aimInput.x * mouseSensitivity * Time.deltaTime;
            pitch += -aimInput.y * mouseSensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        void Yaw()
        {
            //characterController.transform.rotation = Quaternion.Lerp(characterController.transform.rotation, Quaternion.Euler(0f, yaw, 0f), Time.fixedDeltaTime);
            //playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, Quaternion.Euler(pitch, 0f, 0f), Time.fixedDeltaTime);
            characterController.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            //playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        void Pitch()
        {
            playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        void Crouch()
        {
            if (crouchInput)
            {
                float cSpeed = 2;
                if (characterController.height > crouchHeight)
                {
                    characterController.height = Mathf.MoveTowards(characterController.height, crouchHeight, Time.fixedDeltaTime * cSpeed);
                    Vector3 collCenter = characterController.center;
                    collCenter.y = characterController.height * .5f;
                    characterController.center = collCenter;
                }

                Vector3 pos = cameraRoot.localPosition;
                if (pos.y > cameraCrouchHeight)
                {
                    pos.y = Mathf.MoveTowards(pos.y, cameraCrouchHeight, Time.fixedDeltaTime * cSpeed);
                    cameraRoot.localPosition = pos;
                }



            }
            else
            {
                float cSpeed = 2;
                if (characterController.height < characterDefaultHeight)
                {
                    characterController.height = Mathf.MoveTowards(characterController.height, characterDefaultHeight, Time.fixedDeltaTime * cSpeed);
                    Vector3 collCenter = characterController.center;
                    collCenter.y = characterController.height * .5f;
                    characterController.center = collCenter;
                }

                Vector3 pos = cameraRoot.localPosition;
                if (pos.y < cameraDefaultHeight)
                {
                    pos.y = Mathf.MoveTowards(pos.y, cameraDefaultHeight, Time.fixedDeltaTime * cSpeed);
                    cameraRoot.localPosition = pos;
                }



            }
        }

        
        void Move()
        {

            Vector3 hVel = velocity;
            hVel.y = 0;
            float vVel = velocity.y;
            float speed = hVel.magnitude;

            Vector3 targetDirection = Vector3.zero;
            if (moveInput.magnitude > 0)
            {
                float hMaxSpeed = GetMoveSpeed();

                speed += Time.deltaTime * acceleration;
                if (speed > hMaxSpeed)
                    speed = hMaxSpeed;

                //if (!sprintInput)
                targetDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
                //else
                //targetDirection = transform.forward;
            }
            else
            {
                // Keep the last direction while decelerating
                if (speed > 0)
                {
                    targetDirection = velocity;
                    targetDirection.y = 0;
                    targetDirection.Normalize();

                    speed -= Time.deltaTime * deceleration;
                    if (speed < 0)
                        speed = 0;
                }

            }

            hVel = targetDirection * speed;


            // 
            // Apply gravity
            //
            if(!characterController.isGrounded)
            {
                
                vVel += Physics.gravity.y * Time.fixedDeltaTime;
                
                jumping = false;

            }
            else
            {
                // Check for jump
                if (jumpInput)
                {
                    if (!jumping)
                    {
                        jumping = true;
                        vVel = jumpSpeed;
                    }
                }
                else
                {
                    vVel = 0;
                }
                
            }


            // 
            // Move character
            //
            velocity = hVel + Vector3.up * vVel;

            characterController.Move(velocity * Time.fixedDeltaTime);
        }

        float GetMoveSpeed()
        {
            if (moveInput.magnitude == 0)
                return 0;

            if (walkInput)
                return walkSpeed;

            if (crouchInput)
                return crouchSpeed;

            //if (sprintInput)
            //    return sprintSpeed;

            return runSpeed;
        }

    }

}
