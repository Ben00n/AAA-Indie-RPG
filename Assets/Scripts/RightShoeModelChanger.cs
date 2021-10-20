using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightShoeModelChanger : MonoBehaviour
{
    public List<GameObject> shoeModels;

    private void Awake()
    {
        GetAllShoeModels();
    }

    private void GetAllShoeModels()
    {
        int childrenGameObjects = transform.childCount;

        for (int i = 0; i < childrenGameObjects; i++)
        {
            shoeModels.Add(transform.GetChild(i).gameObject);
        }
    }

    public void UnEquipAllShoeModels()
    {
        foreach (GameObject shoeModel in shoeModels)
        {
            shoeModel.SetActive(false);
        }
    }

    public void EquipShoeModelByName(string shoeName)
    {
        for (int i = 0; i < shoeModels.Count; i++)
        {
            if (shoeModels[i].name == shoeName)
            {
                shoeModels[i].SetActive(true);
            }
        }
    }
}
