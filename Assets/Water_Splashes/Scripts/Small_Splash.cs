using UnityEngine;
using System.Collections;

public class Small_Splash : MonoBehaviour {


public GameObject SmallSplash;

private float splashFlag = 0;


void Start (){

    SmallSplash.SetActive(false);

}

void Update (){

    if (Input.GetButtonDown("Fire2"))
    {

        if (splashFlag == 0)
        {
				StartCoroutine("TriggerSplash");
        }
       
    }


    
}

   
IEnumerator TriggerSplash (){
    
    splashFlag = 1;
    
    SmallSplash.SetActive(true);

	yield return new  WaitForSeconds (2.1f);

    SmallSplash.SetActive(false);

    splashFlag = 0;

}




}