using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;

    public bool x_Input;
    public bool b_Input;
    public bool roll_Input;
    public bool f_Input;
    public bool rb_Input;
    public bool rt_Input;
    public bool q_Input;
    public bool v_Input;
    public bool jump_Input;
    public bool escape_Input;
    public bool i_Input;
    public bool p_Input;
    public bool lockOnInput;
    public bool right_Stick_Right_Input;
    public bool right_Stick_Left_Input;

    public bool d_Pad_Up;
    public bool d_Pad_Left;
    public bool d_Pad_Right;
    public bool d_Pad_Down;

    public bool consumingFlag;
    public bool rollFlag;
    public bool sheatheFlag;
    public bool sprintFlag;
    public bool comboFlag;
    public bool lockOnFlag;
    public bool escapeFlag;
    public float rollInputTimer;

    public Transform backStabRayCastStartPoint;


    PlayerControls inputActions;
    PlayerCombatManager playerCombatManager;
    PlayerInventoryManager playerInventoryManager;
    PlayerManager playerManager;
    PlayerEffectsManager playerEffectsManager;
    PlayerStatsManager playerStatsManager;
    BlockingCollider blockingCollider;
    CameraHandler cameraHandler;
    PlayerAnimatorManager playerAnimatorManager;
    UIManager uiManager;
    PlayerWeaponSlotManager weaponSlotManager;

    Vector2 movementInput;
    Vector2 cameraInput;


    private void Awake()
    {
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerManager = GetComponent<PlayerManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        playerEffectsManager = GetComponent<PlayerEffectsManager>();
        blockingCollider = GetComponentInChildren<BlockingCollider>();
        uiManager = FindObjectOfType<UIManager>();
        cameraHandler = FindObjectOfType<CameraHandler>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        weaponSlotManager = GetComponent<PlayerWeaponSlotManager>();

    }


    public void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerControls();
            inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            inputActions.PlayerActions.RB.performed += i => rb_Input = true;
            inputActions.PlayerActions.RT.performed += i => rt_Input = true;
            inputActions.PlayerActions.Q.performed += i => q_Input = true;
            inputActions.PlayerActions.Q.canceled += i => q_Input = false;
            inputActions.PlayerQuickSlots.DPadRight.performed += i => d_Pad_Right = true;
            inputActions.PlayerQuickSlots.DPadLeft.performed += i => d_Pad_Left = true;
            inputActions.PlayerActions.F.performed += i => f_Input = true;
            inputActions.PlayerActions.Roll.performed += i => roll_Input = true;
            inputActions.PlayerActions.Roll.canceled += i => roll_Input = false;
            inputActions.PlayerActions.Jump.performed += i => jump_Input = true;
            inputActions.PlayerActions.Inventory.performed += i => escape_Input = true;
            inputActions.PlayerActions.LockOn.performed += i => lockOnInput = true;
            inputActions.PlayerMovement.LockOnTargetRight.performed += i => right_Stick_Right_Input = true;
            inputActions.PlayerMovement.LockOnTargetLeft.performed += i => right_Stick_Left_Input = true;
            inputActions.PlayerActions.V.performed += i => v_Input = true;
            inputActions.PlayerActions.OpenBag.performed += i => i_Input = true;
            inputActions.PlayerActions.OpenEquipment.performed += i => p_Input = true;
            inputActions.PlayerActions.X.performed += i => x_Input = true;
            inputActions.PlayerActions.B.performed += i => b_Input = true;
        }

        inputActions.Enable();
    }


    private void OnDisable()
    {
        inputActions.Disable();
    }


    public void TickInput(float delta)
    {
        HandleMoveInput(delta);
        HandleRollInput(delta);
        HandleCombatInput(delta);
        HandleQuickSlotsInput();
        HandleEscapeInput();
        HandleInventoryInput();
        HandleEquipmentInput();
        HandleLockOnInput();
        HandleVInput();
        HandleUseConsumableInput();
    }

    private void HandleMoveInput(float delta)
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }

    private void HandleRollInput(float delta)
    {

        if (roll_Input)
        {
            rollInputTimer += delta;

            if (playerStatsManager.currentStamina <= 0)
            {
                roll_Input = false;
                sprintFlag = false;
            }

            if (moveAmount > 0.5f && playerStatsManager.currentStamina > 0)
            {
                sprintFlag = true;
            }
        }
        else
        {
            sprintFlag = false;
            if (rollInputTimer > 0 && rollInputTimer < 0.5f)
            {
                rollFlag = true;
            }

            rollInputTimer = 0;
        }
    }

    private void HandleCombatInput(float delta)
    {

        if (rb_Input && !consumingFlag && !playerManager.isBlocking)
        {
            playerCombatManager.HandleRBAction();
            playerCombatManager.HandleDualAction();
        }

        if (rt_Input && !consumingFlag && !playerManager.isBlocking)
        {
            playerCombatManager.HandleRTAction();
            playerCombatManager.HandleDualHeavyAction();
        }

        if (q_Input && !consumingFlag)
        {
            playerCombatManager.HandleQAction();
        }
        else
        {
            playerManager.isBlocking = false;

            if (blockingCollider.blockingCollider.enabled)
            {
                blockingCollider.DisableBlockingCollider();
            }
        }
    }

    private void HandleQuickSlotsInput()
    {
        if (d_Pad_Right)
        {
            playerInventoryManager.ChangeRightWeapon();
        }
        else if (d_Pad_Left)
        {
            playerInventoryManager.ChangeLeftWeapon();
        }
    }

    private void HandleEscapeInput()
    {
        if (escape_Input)
        {
            escapeFlag = !escapeFlag;
            uiManager.CloseAllInventoryWindows();


            if (escapeFlag)
            {
                uiManager.OpenEscapeWindow();
                uiManager.hudWindow.SetActive(false);
                uiManager.iconsWindow.SetActive(false);
            }
            else
            {
                uiManager.CloseEscapeWindow();
                uiManager.CloseAllInventoryWindows();
                uiManager.hudWindow.SetActive(true);
                uiManager.iconsWindow.SetActive(true);
            }
        }
    }

    private void HandleInventoryInput()
    {
        if (i_Input)
        {
            uiManager.UpdateUI();
            uiManager.toggleBagUI();
        }
    }
    private void HandleEquipmentInput()
    {
        if (p_Input)
        {
            uiManager.UpdateUI();
            uiManager.toggleInventoryUI();
        }
    }

    private void HandleLockOnInput()
    {
        if (lockOnInput && lockOnFlag == false)
        {

            lockOnInput = false;
            cameraHandler.HandleLockOn();
            if (cameraHandler.nearestLockOnTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                lockOnFlag = true;
            }
        }
        else if (lockOnInput && lockOnFlag)
        {
            lockOnInput = false;
            lockOnFlag = false;
            cameraHandler.ClearLockOnTargets();
        }

        if (lockOnFlag && right_Stick_Left_Input)
        {
            right_Stick_Left_Input = false;
            cameraHandler.HandleLockOn();
            if (cameraHandler.leftLockTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.leftLockTarget;
            }
        }

        if (lockOnFlag && right_Stick_Right_Input)
        {
            right_Stick_Right_Input = false;
            cameraHandler.HandleLockOn();
            if (cameraHandler.rightLockTarget != null)
            {
                cameraHandler.currentLockOnTarget = cameraHandler.rightLockTarget;
            }
        }

        cameraHandler.SetCameraHeight();
    }

    private void HandleVInput()
    {
        if (v_Input && !consumingFlag)
        {
            v_Input = false;
            playerCombatManager.AttemptBackStab();
        }
    }

    private void HandleUseConsumableInput()
    {
        if(b_Input && !playerManager.isInteracting && !consumingFlag)
        {
            consumingFlag = true;
            b_Input = false;
            playerInventoryManager.currentConsumable.AttemptToConsumeItem(playerAnimatorManager, weaponSlotManager, playerEffectsManager);
        }
    }
}
