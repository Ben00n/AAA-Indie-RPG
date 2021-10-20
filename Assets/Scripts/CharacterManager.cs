using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [Header("Lock On Transform")]
    public Transform lockOnTransform;

    [Header("Combat Colliders")]
    public BackStabCollider backStabCollider;

    [Header("Interaction")]
    public bool isInteracting;

    [Header("Combat Flags")]
    public bool isBlocking;
    public bool isInvulnerable;
    public bool canDoCombo;
    public bool isUsingRightHand;
    public bool isUsingLeftHand;

    [Header("Movement Flags")]
    public bool isRotatingWithRootMotion;
    public bool canRotate;
    public bool isSprinting;
    public bool isInAir;
    public bool isGrounded;
    public bool isMounted; // this


    public int pendingDamage;

}
