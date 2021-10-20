using System.Collections;
using System.Collections.Generic;
using MalbersAnimations.HAP;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    InputHandler inputHandler;
    Animator animator;
    CameraHandler cameraHandler;
    PlayerStatsManager playerStatsManager;
    PlayerLocomotionManager playerLocomotion;
    PlayerAnimatorManager playerAnimatorManager;
    MRider mRider; //this

    InteractableUI interactableUI;
    public GameObject interactableUIGameObject;
    public GameObject itemInteractableGameObject;

    private void Awake()
    {
        cameraHandler = FindObjectOfType<CameraHandler>();
        backStabCollider = GetComponentInChildren<BackStabCollider>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        inputHandler = GetComponent<InputHandler>();
        animator = GetComponent<Animator>();
        mRider = GetComponent<MRider>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        playerLocomotion = GetComponent<PlayerLocomotionManager>();
        interactableUI = FindObjectOfType<InteractableUI>();
    }


    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;

        isInteracting = animator.GetBool("isInteracting");
        canDoCombo = animator.GetBool("canDoCombo");
        isUsingRightHand = animator.GetBool("isUsingRightHand");
        isUsingLeftHand = animator.GetBool("isUsingLeftHand");
        animator.SetBool("IsInAir", isInAir);
        isInvulnerable = animator.GetBool("isInvulnerable");
        isMounted = animator.GetBool("Mount");
        animator.SetBool("isBlocking", isBlocking);
        animator.SetBool("isDead", playerStatsManager.isDead);

        inputHandler.TickInput(delta);
        playerAnimatorManager.canRotate = animator.GetBool("canRotate");
        playerLocomotion.HandleRollingAndSprinting(delta);
        playerLocomotion.HandleJumping();
        playerStatsManager.RegenerateHealth();
        playerStatsManager.RegenerateStamina();
        playerStatsManager.RegenerateFocus();

        CheckForInteractableObject();
    }


    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
        playerLocomotion.HandleMovement(delta);
        playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
        playerLocomotion.HandleRotation(delta);
    }

    private void LateUpdate()
    {
        inputHandler.rollFlag = false;
        inputHandler.rb_Input = false;
        inputHandler.rt_Input = false;
        inputHandler.d_Pad_Up = false;
        inputHandler.d_Pad_Down = false;
        inputHandler.d_Pad_Left = false;
        inputHandler.d_Pad_Right = false;
        inputHandler.f_Input = false;
        inputHandler.jump_Input = false;
        inputHandler.escape_Input = false;
        inputHandler.i_Input = false;
        inputHandler.p_Input = false;
        inputHandler.b_Input = false;

        float delta = Time.deltaTime;
        if (cameraHandler != null)
        {
             cameraHandler.FollowTarget(delta);
             cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
        }

        if (isInAir)
        {
            playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
        }
    }

    #region Player Interactions
    public void CheckForInteractableObject()
    {
        RaycastHit hit;

        if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f, cameraHandler.ignoreLayers))
        {
            if(hit.collider.tag == "Interactable")
            {
                Interactable interactableObject = hit.collider.GetComponent<Interactable>();

                if(interactableObject !=null)
                {
                    string interactableText = interactableObject.interactableText;
                    interactableUI.interactableText.text = interactableText;
                    interactableUIGameObject.SetActive(true);

                    if(inputHandler.f_Input)
                    {
                        hit.collider.GetComponent<Interactable>().Interact(this);
                    }
                }
            }
        }
        else
        {
            if(interactableUIGameObject != null)
            {
                interactableUIGameObject.SetActive(false);
            }

            if(itemInteractableGameObject != null && inputHandler.f_Input)
            {
                itemInteractableGameObject.SetActive(false);
            }
        }
    }

    public void OpenChestInteraction(Transform playerStandsHereWhenOpeningChest)
    {
        playerLocomotion.rigidbody.velocity = Vector3.zero;
        transform.position = playerStandsHereWhenOpeningChest.transform.position;
        playerAnimatorManager.PlayTargetAnimation("Open Chest", true);

    }

    public void PassThroughFogWallInteraction(Transform fogWallEntrance)
    {
        playerLocomotion.rigidbody.velocity = Vector3.zero; //stops the player from ice skating

        Vector3 rotationDirection = fogWallEntrance.transform.right;
        Quaternion turnRotation = Quaternion.LookRotation(rotationDirection);
        transform.rotation = turnRotation;
        //turn over time

        playerAnimatorManager.PlayTargetAnimation("Pass Through Fog", true);
    }

    #endregion
}
