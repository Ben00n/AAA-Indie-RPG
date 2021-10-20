using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusPointBar : MonoBehaviour
{
    public Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void SetMaxFocus(float maxFocus)
    {
        slider.maxValue = maxFocus;
        slider.value = maxFocus;
    }

    public void SetCurrentFocus(float currentFocus)
    {
        slider.value = currentFocus;
    }

}
