using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceCountBar : MonoBehaviour
{
    public Text experienceCountText;

    public void SetExperienceCountText(int experienceCount)
    {
        experienceCountText.text = experienceCount.ToString();
    }
}
