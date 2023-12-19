using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    Animator animator;
    InputActions input;

    int isWalkingHash;
    int isRunningHash;

    Vector2 currentMovement;
    bool movementPressed;
    bool runPressed;

    void Awake(){
        input = new InputActions();
        input.Player.Movement.performed += ctx => {
            currentMovement = ctx.ReadValue<Vector2>();
            movementPressed = currentMovement.x != 0 || currentMovement.y != 0;
        };

        input.Player.Run.performed += ctx => runPressed = ctx.ReadValueAsButton();
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    void HandleMovement(){
        bool isWalking = animator.GetBool("isWalking");
        bool isRunning = animator.GetBool("isRunning");

        if(movementPressed && !isWalking){
            animator.SetBool(isWalkingHash,true);
        }
        if(!movementPressed && isWalking){
            animator.SetBool(isWalkingHash,false);
        }

        if(movementPressed && runPressed && !isRunning){
            animator.SetBool(isRunningHash,true);
        }

        if(!movementPressed || !runPressed && isRunning){
            animator.SetBool(isRunningHash,false);
        }
    }

    void HandleRotation(){
        Vector3 currentPosition = transform.position;

        Vector3 newPosition = new Vector3(currentMovement.x,0,currentMovement.y);
        Vector3 positionToLookAt = currentPosition + newPosition;

        transform.LookAt(positionToLookAt);
    }

    void Update(){
        HandleMovement();
        HandleRotation();
    }

    void OnEnable(){
        input.Player.Enable();
    }
    void OnDisable(){
        input.Player.Disable();
    }
}
