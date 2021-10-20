using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralItemPickup : Interactable
{
    public Item item;

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);

        PickUpItem(playerManager);
    }

    private void PickUpItem(PlayerManager playerManager)
    {

        PlayerLocomotionManager playerLocomotion;
        PlayerAnimatorManager animatorHandler;

        playerLocomotion = playerManager.GetComponent<PlayerLocomotionManager>();
        animatorHandler = playerManager.GetComponentInChildren<PlayerAnimatorManager>();

        playerLocomotion.rigidbody.velocity = Vector3.zero;
        animatorHandler.PlayTargetAnimation("Pick Up Item", true);
        playerManager.itemInteractableGameObject.GetComponentInChildren<Text>().text = item.itemName;
        playerManager.itemInteractableGameObject.GetComponentInChildren<RawImage>().texture = item.itemIcon.texture;
        playerManager.itemInteractableGameObject.SetActive(true);
        Destroy(gameObject);
    }
}
