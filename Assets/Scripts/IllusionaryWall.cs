using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IllusionaryWall : MonoBehaviour
{
    public bool wallHasBeenHit;
    public Material illusionaryWallMaterial;
    public float alpha;
    public float fadeTimer = 2.5f;
    public MeshCollider meshCollider;

    public AudioClip illusionaryWallSound;


    private void Start()
    {
        Color fullWallColor = new Color(1, 1, 1, 1);
        illusionaryWallMaterial.color = fullWallColor;
        alpha = illusionaryWallMaterial.color.a;
    }

    private void Update()
    {
        if(wallHasBeenHit)
        {
            FadeIllusionaryWall();
        }
    }


    public void FadeIllusionaryWall()
    {
        alpha = illusionaryWallMaterial.color.a;
        alpha -= Time.deltaTime / fadeTimer;
        Color fadedWallColor = new Color(1, 1, 1,alpha);
        illusionaryWallMaterial.color = fadedWallColor;

        if (meshCollider.enabled)
        {
            meshCollider.enabled = false;
            AudioManager.Instance.PlaySound(illusionaryWallSound, gameObject);
        }

        if(alpha <= 0)
        {
            Destroy(this);
        }
    }
}
