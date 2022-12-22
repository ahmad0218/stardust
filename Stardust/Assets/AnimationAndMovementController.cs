// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    Animator animator;
    PlayerInput playerInput;
    CharacterController characterController;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    Vector3 cameraRelativeMovement;
    bool isMovementPressed;
    bool isRunPressed;
    int isWalkingHash;
    int isRunningHash;
    int isJumpingHash;
    float rotationFactorPerFrame = 5.0f;
    float runMultiplier = 4.0f;
    float speed = 2.0f;
    // float velocity = 0.0f;
    // public float acceleration = 0.5f;
    // public float deceleration = 0.5f;
    // int velocityHash;

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");  
        isRunningHash = Animator.StringToHash("isRunning");  
        isJumpingHash = Animator.StringToHash("isJumping"); 

        // playerInput.CharacterControls.Move.started += context => {Debug.Log(context.ReadValue<Vector2>()); };
        // playerInput.CharacterControls.Move.started += onMovementInput;
        // playerInput.CharacterControls.Move.canceled += onMovementInput;
        // playerInput.CharacterControls.Move.performed += onMovementInput; 
        // playerInput.CharacterControls.Run.started += onRun; 
        // playerInput.CharacterControls.Run.canceled += onRun;
    }
 
    // void onRun(InputAction.CallbackContext context)
    // {
    //     isRunPressed =  context.ReadValueAsButton();
    // }

    void handleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;
        Quaternion currentRotation = transform.rotation;
        if(isMovementPressed){
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    // void onMovementInput (InputAction.CallbackContext context)
    // {
    //     Debug.Log(context.ReadValue<Vector2>());
    //     currentMovementInput = context.ReadValue<Vector2>();
    //     currentMovement.x = currentMovementInput.x;
    //     currentMovement.z = currentMovementInput.y;
    //     currentRunMovement.x = currentMovementInput.x * runMultiplier;
    //     currentRunMovement.z = currentMovementInput.y * runMultiplier;
    //     isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0; 
    // }

    // Start is called before the first frame update
    // void Start()
    // {
    //     animator = GetComponent<Animator>();
    //     playerInput = new PlayerInput();
    //     characterController = GetComponent<CharacterController>();
    //     animator = GetComponent<Animator>();
    //     isWalkingHash = Animator.StringToHash("isWalking");  
    //     isRunningHash = Animator.StringToHash("isRunning"); 
    // }

    // Update is called once per frame

    void handleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        bool isJumping = animator.GetBool(isJumpingHash);
        currentMovement.x = 0.0f;
        currentMovement.z = 0.0f;

        // if(isJumping){
        //     animator.SetBool(isJumpingHash, false);
        // } else if((Input.GetKey("j"))){
        //     animator.SetBool(isJumpingHash, true);
        //     return;
        // }
        if((Input.GetKey("w"))){
            currentMovement.z = 1.0f;
        }
        else if((Input.GetKey("s"))){
            currentMovement.z = -1.0f;
        }
        else if((Input.GetKey("a"))){
            currentMovement.x = -1.0f;
        }
        else if((Input.GetKey("d"))){
            currentMovement.x = 1.0f;
        }
        isRunPressed = Input.GetKey("left shift");
        isMovementPressed = currentMovement.x != 0.0f || currentMovement.z != 0.0f; 

        if (!isMovementPressed && (isWalking || isRunning)) {
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isRunningHash, false);
        }
        else if ((isMovementPressed && isRunPressed) && !isRunning) {
            animator.SetBool(isRunningHash, true);
            animator.SetBool(isWalkingHash, false);
        }
        else if ((isMovementPressed && !isRunPressed) && !isWalking) {
            animator.SetBool(isRunningHash, false);
            animator.SetBool(isWalkingHash, true);
        }

        if(!animator.GetBool(isRunningHash) && !animator.GetBool(isWalkingHash)){
            currentMovement.x = 0.0f;
            currentMovement.z = 0.0f;
        } else if (animator.GetBool(isRunningHash)) {
            currentMovement.x = currentMovement.x * runMultiplier;
            currentMovement.z = currentMovement.z * runMultiplier;
        }
    }

    void handleGravity()
    {
        // Debug.Log("hello bitch");
        if(characterController.isGrounded){
            float groundedGravity = -.05f;
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        } else {
            float gravity = -9.8f;
            currentMovement.y += gravity;
            currentRunMovement.y += gravity;
        }
    }

    void Update()
    {
        handleGravity();
        handleRotation();
        handleAnimation();
        // cameraRelativeMovement = ConvertToCameraSpace(currentMovement);
        // characterController.Move(cameraRelativeMovement*Time.deltaTime); 
        
        characterController.Move(currentMovement*speed * Time.deltaTime);
    }

    // Vector3 ConvertToCameraSpace(Vector3 vectorToRotate){
    //     float currentYValue = vectorToRotate.y;
    //     Vector3 cameraForward = Camera.main.transform.forward;
    //     Vector3 cameraRight = Camera.main.transform.right;
    //     cameraForward.y = 0.0f;
    //     cameraRight.y = 0.0f;
    //     Vector3 cameraForwardZProduct = vectorToRotate.z *cameraForward;
    //     Vector3 cameraForwardXProduct = vectorToRotate.x *cameraRight;
    //     Vector3 vectorRotatedToCameraSpace = cameraForwardZProduct + cameraForward;
    //     vectorRotatedToCameraSpace.y = currentYValue;
    //     return vectorRotatedToCameraSpace;
    // }

    void onEnable()
    {
        playerInput.CharacterControls.Enable();
    }
    void onDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
