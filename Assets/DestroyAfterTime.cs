using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float timeUntillDestroyed = 3;

    private void Awake()
    {
        Destroy(gameObject, timeUntillDestroyed);
    }
}
