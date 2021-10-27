using UnityEngine;
using System;

namespace PixelCrushers.QuestMachine
{

    /// This is a starter template for custom quest actions. To use it,
    /// make a copy, rename it, and remove the line marked above.
    /// Then fill in your code where indicated below.
    public class GiveItemQuestAction : QuestAction
    {
        UIManager uiManager;
        public WeaponItem weaponItemToGive;


        public override void Execute()
        {
            base.Execute();
            PlayerInventoryManager playerInventoryManager = FindObjectOfType<PlayerInventoryManager>();
            uiManager = FindObjectOfType<UIManager>();
            playerInventoryManager.weaponsInventory.Add(weaponItemToGive);
            uiManager.UpdateUI();
        }

        // Uncomment and edit if you want to override the name shown in the editor.
        //public override string GetEditorName()
        //{
        //    return base.GetEditorName();
        //}

        // Uncomment and edit if you need to save some data to a serializable field
        // (for example, a dictionary to two lists).
        //public override void OnBeforeProxySerialization()
        //{
        //    base.OnBeforeProxySerialization();
        //}

        // Uncomment and edit if you need to copy serialized data back to its original form.
        //public override void OnAfterProxyDeserialization()
        //{
        //    base.OnAfterProxyDeserialization();
        //}
    }

}

/**/