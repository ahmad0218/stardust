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
    bool isMovementPressed;
    bool isRunPressed;
    int isWalkingHash;
    int isRunningHash;
    float rotationFactorPerFrame = 15.0f;
    float runMultiplier = 3.0f;
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

        playerInput.CharacterControls.Move.started += context => {Debug.Log(context.ReadValue<Vector2>()); };
        // playerInput.CharacterControls.Move.started += onMovementInput;
        // playerInput.CharacterControls.Move.canceled += onMovementInput;
        // playerInput.CharacterControls.Move.performed += onMovementInput; 
        // playerInput.CharacterControls.Run.started += onRun; 
        // playerInput.CharacterControls.Run.canceled += onRun;
        isWalkingHash = Animator.StringToHash("isWalking");  
        isRunningHash = Animator.StringToHash("isRunning"); 
    }
 
    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed =  context.ReadValueAsButton();
    }

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

    void onMovementInput (InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<Vector2>());
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0; 
    }

    // Start is called before the first frame update
    // void Start()
    // {
    //     animator = GetComponent<Animator>();
    //     isWalkingHash = Animator.StringToHash("isWalking");  
    //     isRunningHash = Animator.StringToHash("isRunning");  
    //     // velocityHash = Animator.StringToHash("Velocity"); 
    // }

    // Update is called once per frame

    void handleAnimation()
    {
        // Debug.Log(animator.GetBool(isWalkingHash));
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        // bool forwardPressed = Input.GetKey("w");
        // bool runPressed = Input.GetKey("left shift");

        if (isMovementPressed && !isWalking) {
            animator.SetBool(isWalkingHash, true);
        }
        else if (!isMovementPressed && isWalking) {
            animator.SetBool(isWalkingHash, false);
        }
        if (!isRunning && (isMovementPressed && isRunPressed)) {
            animator.SetBool(isRunningHash, true);
        }
        else if (isRunning && (!isMovementPressed && !isRunPressed)) {
            animator.SetBool(isRunningHash, false);
        }
        // if (forwardPressed && velocity<1.0f){
        //     velocity += Time.deltaTime * acceleration;
        // }
        // if (!forwardPressed && velocity>0.0f){
        //     velocity -= Time.deltaTime * deceleration;
        // }
        // if (!forwardPressed && velocity <0.0f){
        //     velocity = 0.0f;
        // }
        // animator.SetFloat(velocityHash, velocity);
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

        if(isRunPressed){
            characterController.Move(currentRunMovement * Time.deltaTime);
        } else {
            characterController.Move(currentMovement * Time.deltaTime);
        }
    }

    void onEnable()
    {
        playerInput.CharacterControls.Enable();
    }
    void onDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
