using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
    InputHandler inputHandler;
    PlayerInventoryManager playerInventoryManager;
    PlayerStatsManager playerStatsManager;

    [Header("Equipment Model Changers")]
    //Head Equipment
    HelmetModelChanger helmetModelChanger;

    //Shoulder Equipment
    LeftShoulderModelChanger leftShoulderModelChanger;
    RightShoulderModelChanger rightShoulderModelChanger;

    //Torso Equipment
    TorsoModelChanger torsoModelChanger;
    UpperLeftArmModelChanger upperLeftArmModelChanger;
    UpperRightArmModelChanger upperRightArmModelChanger;

    //Leg Equipment
    HipModelChanger hipModelChanger;
    LeftKneeModelChanger leftKneeModelChanger;
    RightKneeModelChanger rightKneeModelChanger;
    LeftShoeModelChanger leftShoeModelChanger;
    RightShoeModelChanger rightShoeModelChanger;

    //Hand Equipment
    LowerLeftArmModelChanger lowerLeftArmModelChanger;
    LowerRightArmModelChanger lowerRightArmModelChanger;
    LeftHandModelChanger leftHandModelChanger;
    RightHandModelChanger rightHandModelChanger;
    LeftElbowModelChanger leftElbowModelChanger;
    RightElbowModelChanger rightElbowModelChanger;

    //Back Equipment
    BackModelChanger backModelChanger;

    [Header("Default Naked Models")]
    public GameObject nakedHeadModel;
    public string nakedTorsoModel;
    public string nakedLeftShoulder;
    public string nakedRightShoulder;
    public string nakedBack;
    public string nakedUpperLeftArm;
    public string nakedUpperRightArm;
    public string nakedLowerLeftArm;
    public string nakedLowerRightArm;
    public string nakedLeftHand;
    public string nakedRightHand;
    public string nakedLeftElbow;
    public string nakedRightElbow;
    public string nakedHipModel;
    public string nakedLeftKnee;
    public string nakedRightKnee;
    public string nakedRightShoe;
    public string nakedLeftShoe;

    public BlockingCollider blockingCollider;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();

        helmetModelChanger = GetComponentInChildren<HelmetModelChanger>();
        leftShoulderModelChanger = GetComponentInChildren<LeftShoulderModelChanger>();
        rightShoulderModelChanger = GetComponentInChildren<RightShoulderModelChanger>();
        torsoModelChanger = GetComponentInChildren<TorsoModelChanger>();
        hipModelChanger = GetComponentInChildren<HipModelChanger>();
        leftKneeModelChanger = GetComponentInChildren<LeftKneeModelChanger>();
        rightKneeModelChanger = GetComponentInChildren<RightKneeModelChanger>();
        rightShoeModelChanger = GetComponentInChildren<RightShoeModelChanger>();
        leftShoeModelChanger = GetComponentInChildren<LeftShoeModelChanger>();
        upperLeftArmModelChanger = GetComponentInChildren<UpperLeftArmModelChanger>();
        upperRightArmModelChanger = GetComponentInChildren<UpperRightArmModelChanger>();
        lowerLeftArmModelChanger = GetComponentInChildren<LowerLeftArmModelChanger>();
        lowerRightArmModelChanger = GetComponentInChildren<LowerRightArmModelChanger>();
        leftHandModelChanger = GetComponentInChildren<LeftHandModelChanger>();
        rightHandModelChanger = GetComponentInChildren<RightHandModelChanger>();
        leftElbowModelChanger = GetComponentInChildren<LeftElbowModelChanger>();
        rightElbowModelChanger = GetComponentInChildren<RightElbowModelChanger>();
        backModelChanger = GetComponentInChildren<BackModelChanger>();
    }

    private void Start()
    {
        EquipAllEquipmentModelsOnStart();
    }

    private void EquipAllEquipmentModelsOnStart()
    {
        helmetModelChanger.UnEquipAllHelmetModels();
        if(playerInventoryManager.currentHelmetEquipment != null)
        {
            nakedHeadModel.SetActive(false);
            helmetModelChanger.EquipHelmetModelByName(playerInventoryManager.currentHelmetEquipment.helmetModelName);
            playerStatsManager.physicalDamageAbsorptionHead = playerInventoryManager.currentHelmetEquipment.physicalDefense;
            Debug.Log("Helmet Absorption is " + playerStatsManager.physicalDamageAbsorptionHead + "%");
        }
        else
        {
            nakedHeadModel.SetActive(true);
            playerStatsManager.physicalDamageAbsorptionHead = 0;
        }

        //TORSO EQUIPMENT
        torsoModelChanger.UnEquipAllTorsoModels();
        upperLeftArmModelChanger.UnEquipAllModels();
        upperRightArmModelChanger.UnEquipAllModels();

        if (playerInventoryManager.currentTorsoEquipment != null)
        {
            torsoModelChanger.EquipTorsoModelByName(playerInventoryManager.currentTorsoEquipment.torsoModelName);
            upperLeftArmModelChanger.EquipModelByName(playerInventoryManager.currentTorsoEquipment.upperLeftArmModelName);
            upperRightArmModelChanger.EquipModelByName(playerInventoryManager.currentTorsoEquipment.upperRightArmModelName);
            playerStatsManager.physicalDamageAbsorptionTorso = playerInventoryManager.currentTorsoEquipment.physicalDefense;
            Debug.Log("Torso Absorption is " + playerStatsManager.physicalDamageAbsorptionTorso + "%");
        }
        else
        {
            torsoModelChanger.EquipTorsoModelByName(nakedTorsoModel);
            upperLeftArmModelChanger.EquipModelByName(nakedUpperLeftArm);
            upperRightArmModelChanger.EquipModelByName(nakedUpperRightArm);
            playerStatsManager.physicalDamageAbsorptionTorso = 0;
        }

        //SHOULDER EQUIPMENT
        leftShoulderModelChanger.UnEquipAllModels();
        rightShoulderModelChanger.UnEquipAllModels();

        if (playerInventoryManager.currentShoulderEquipment != null)
        {
            leftShoulderModelChanger.EquipModelByName(playerInventoryManager.currentShoulderEquipment.leftShoulderModelName);
            rightShoulderModelChanger.EquipModelByName(playerInventoryManager.currentShoulderEquipment.rightShoulderModelName);
            playerStatsManager.physicalDamageAbsorptionShoulders = playerInventoryManager.currentShoulderEquipment.physicalDefense;
            Debug.Log("Shoulder Absorption is " + playerStatsManager.physicalDamageAbsorptionShoulders + "%");
        }
        else
        {
            leftShoulderModelChanger.EquipModelByName(nakedLeftShoulder);
            rightShoulderModelChanger.EquipModelByName(nakedRightShoulder);
            playerStatsManager.physicalDamageAbsorptionShoulders = 0;
        }

        //HIP EQUIPMENT
        hipModelChanger.UnEquipAllHipModels();
        leftKneeModelChanger.UnEquipAllModels();
        rightKneeModelChanger.UnEquipAllModels();

        if (playerInventoryManager.currentHipEquipment != null)
        {
            hipModelChanger.EquipHipModelByName(playerInventoryManager.currentHipEquipment.hipModelName);
            leftKneeModelChanger.EquipModelByName(playerInventoryManager.currentHipEquipment.leftKneeModelName);
            rightKneeModelChanger.EquipModelByName(playerInventoryManager.currentHipEquipment.rightKneeModelName);
            playerStatsManager.physicalDamageAbsorptionHips = playerInventoryManager.currentHipEquipment.physicalDefense;
            Debug.Log("Hips Absorption is " + playerStatsManager.physicalDamageAbsorptionHips + "%");
        }
        else
        {
            hipModelChanger.EquipHipModelByName(nakedHipModel);
            leftKneeModelChanger.EquipModelByName(nakedLeftKnee);
            rightKneeModelChanger.EquipModelByName(nakedRightKnee);
            playerStatsManager.physicalDamageAbsorptionHips = 0;
        }

        //SHOE EQUIPMENT
        leftShoeModelChanger.UnEquipAllShoeModels();
        rightShoeModelChanger.UnEquipAllShoeModels();
        
        if(playerInventoryManager.currentShoesEquipment != null)
        {
            leftShoeModelChanger.EquipShoeModelByName(playerInventoryManager.currentShoesEquipment.leftShoeModelName);
            rightShoeModelChanger.EquipShoeModelByName(playerInventoryManager.currentShoesEquipment.rightShoeModelName);
            playerStatsManager.physicalDamageAbsorptionShoes = playerInventoryManager.currentShoesEquipment.physicalDefense;
            Debug.Log("Shoes Absorption is " + playerStatsManager.physicalDamageAbsorptionShoes + "%");
        }
        else
        {
            leftShoeModelChanger.EquipShoeModelByName(nakedLeftShoe);
            rightShoeModelChanger.EquipShoeModelByName(nakedRightShoe);
            playerStatsManager.physicalDamageAbsorptionShoes = 0;
        }

        //HAND EQUIPMENT
        lowerLeftArmModelChanger.UnEquipAllModels();
        lowerRightArmModelChanger.UnEquipAllModels();
        leftHandModelChanger.UnEquipAllModels();
        rightHandModelChanger.UnEquipAllModels();
        leftElbowModelChanger.UnEquipAllModels();
        rightElbowModelChanger.UnEquipAllModels();


        if(playerInventoryManager.currentHandEquipment != null)
        {
            lowerLeftArmModelChanger.EquipModelByName(playerInventoryManager.currentHandEquipment.lowerLeftArmModelName);
            lowerRightArmModelChanger.EquipModelByName(playerInventoryManager.currentHandEquipment.lowerRightArmModelName);
            leftHandModelChanger.EquipModelByName(playerInventoryManager.currentHandEquipment.leftHandModelName);
            rightHandModelChanger.EquipModelByName(playerInventoryManager.currentHandEquipment.rightHandModelName);
            leftElbowModelChanger.EquipModelByName(playerInventoryManager.currentHandEquipment.leftElbowModelName);
            rightElbowModelChanger.EquipModelByName(playerInventoryManager.currentHandEquipment.rightElbowModelName);
            playerStatsManager.physicalDamageAbsorptionHands = playerInventoryManager.currentHandEquipment.physicalDefense;
            Debug.Log("Hands Absorption is " + playerStatsManager.physicalDamageAbsorptionHands + "%");
        }
        else
        {
            lowerLeftArmModelChanger.EquipModelByName(nakedLowerLeftArm);
            lowerRightArmModelChanger.EquipModelByName(nakedLowerRightArm);
            leftHandModelChanger.EquipModelByName(nakedLeftHand);
            rightHandModelChanger.EquipModelByName(nakedRightHand);
            leftElbowModelChanger.EquipModelByName(nakedLeftElbow);
            rightElbowModelChanger.EquipModelByName(nakedRightElbow);
            playerStatsManager.physicalDamageAbsorptionHands = 0;
        }

        //BACK EQUIPMENT
        backModelChanger.UnEquipAllModels();
        if(playerInventoryManager.currentBackEquipment != null)
        {
            backModelChanger.EquipModelByName(playerInventoryManager.currentBackEquipment.backModelName);
            playerStatsManager.physicalDamageAbsorptionBack = playerInventoryManager.currentBackEquipment.physicalDefense;
            Debug.Log("Back Absorption is " + playerStatsManager.physicalDamageAbsorptionBack + "%");
        }
        else
        {
            backModelChanger.EquipModelByName(nakedBack);
            playerStatsManager.physicalDamageAbsorptionBack = 0;
        }

    }

    public void OpenBlockingCollider()
    {
        blockingCollider.SetColliderDamageAbsorption(playerInventoryManager.rightWeapon);
        blockingCollider.EnableBlockingCollider();
    }

    public void CloseBlockingCollider()
    {
        blockingCollider.DisableBlockingCollider();
    }
}
