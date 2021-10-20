using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public PlayerInventoryManager playerInventory;
    public EquipmentWindowUI equipmentWindowUI;
    public InputHandler inputHandler;

    [Header("UI Windows")]
    public GameObject hudWindow;
    public GameObject escapeWindow;
    public GameObject iconsWindow;
    public GameObject equipmentScreenWindow;
    public GameObject weaponInventoryWindow;
    public GameObject settingsWindow;

    [Header("Equipment Window Slot Selected")]
    public bool rightHandSlot01Selected;
    public bool rightHandSlot02Selected;
    public bool leftHandSlot01Selected;
    public bool leftHandSlot02Selected;

    private bool isEquipmentActive = false;
    private bool isBagActive = false;
    private bool isSettingsActive = false;

    [Header("Weapon Inventory")]
    public GameObject weaponInventorySlotPrefab;
    public Transform weaponInventorySlotsParent;
    WeaponInventorySlot[] weaponInventorySlots;

    private void Awake()
    {
    }

    private void Start()
    {
        weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();
        equipmentWindowUI.LoadWeaponsOnEquipmentScreen(playerInventory);
    }
    public void UpdateUI()
    {
        #region Weapon Inventory Slots
        for (int i =0; i<weaponInventorySlots.Length; i++)
        {
            if(i < playerInventory.weaponsInventory.Count)
            {
                if(weaponInventorySlots.Length < playerInventory.weaponsInventory.Count)
                {
                    Instantiate(weaponInventorySlotPrefab, weaponInventorySlotsParent);
                    weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();
                }
                weaponInventorySlots[i].AddItem(playerInventory.weaponsInventory[i]);
            }
            else
            {
                weaponInventorySlots[i].ClearInventorySlot();
            }
        }

        #endregion
    }

    public void OpenEscapeWindow()
    {
        escapeWindow.SetActive(true);
    }

    public void CloseEscapeWindow()
    {
        escapeWindow.SetActive(false);
    }

    public void CloseAllInventoryWindows()
    {
        ResetAllSelectedSlots();

        if (weaponInventoryWindow.activeInHierarchy)
        {
            isBagActive = !isBagActive;
        }
        if (equipmentScreenWindow.activeInHierarchy)
        {
            isEquipmentActive = !isEquipmentActive;
        }
        if (settingsWindow.activeInHierarchy)
        {
            isSettingsActive = !isSettingsActive;
        }

        weaponInventoryWindow.SetActive(false);
        equipmentScreenWindow.SetActive(false);
        settingsWindow.SetActive(false);
    }

    public void ResetAllSelectedSlots()
    {
        rightHandSlot01Selected = false;
        rightHandSlot02Selected = false;
        leftHandSlot01Selected = false;
        leftHandSlot02Selected = false;
    }

    public void toggleBagUI()
    {
        isBagActive = !isBagActive;
        weaponInventoryWindow.SetActive(isBagActive);
    }

    public void toggleInventoryUI()
    {
        isEquipmentActive = !isEquipmentActive;
        equipmentScreenWindow.SetActive(isEquipmentActive);
    }

    public void toggleSettingsUI()
    {
        isSettingsActive = !isSettingsActive;
        settingsWindow.SetActive(isSettingsActive);
    }

    public void Continue()
    {
        inputHandler.escapeFlag = !inputHandler.escapeFlag;
        CloseEscapeWindow();
        CloseAllInventoryWindows();
        hudWindow.SetActive(true);
        iconsWindow.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
