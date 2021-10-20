using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionManager : MonoBehaviour
{

    private float fadeTime;
    private bool fadingIn;

    public Text regionName;

    private void Start()
    {
        regionName.CrossFadeAlpha(0, 0.0f, false);
        fadeTime = 0;
        fadingIn = false;
    }

    private void Update()
    {
        if(fadingIn)
        {
            FadeIn();
        }
        else if(regionName.color.a != 0)
        {
            regionName.CrossFadeAlpha(0, 0.5f, false);
        }
    }

    void FadeIn()
    {
        regionName.CrossFadeAlpha(1, 0.5f, false);
        fadeTime += Time.deltaTime;
        if (regionName.color.a == 1 && fadeTime > 1.5f)
        {
            fadingIn = false;
            fadeTime = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Region")
        {
            fadingIn = true;
            regionName.text = other.name;
        }
    }
}
