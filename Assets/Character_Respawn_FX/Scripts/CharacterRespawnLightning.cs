using UnityEngine;
using System.Collections;

public class CharacterRespawnLightning : MonoBehaviour {

public GameObject respawnLightningFX;
public Light lightningLight;
public GameObject characterGeometry; 

private float t = 0.0f;
private float fadeStart = 2;
private float fadeEnd = 0;
private float fadeTime = 1;
private float pauseTime = 0;


void Start (){

    lightningLight.intensity = 0;
    respawnLightningFX.SetActive(false);
    characterGeometry.SetActive(false);

}

 
void Update (){

    if (Input.GetButtonDown("Fire1"))
    {

		StartCoroutine("SpawnCharacter");  

    }

}


IEnumerator SpawnCharacter (){
  
    respawnLightningFX.SetActive(true);

	yield return new WaitForSeconds (0.25f);

    lightningLight.intensity = 2;

	yield return new WaitForSeconds (1.1f);

    characterGeometry.SetActive(true);

    // Fade out the lightning light
	StartCoroutine("FadeLight"); 

}


IEnumerator FadeLight (){
   
     while (t < fadeTime) 
     {

         if (pauseTime == 0)
         {
             t += Time.deltaTime;
         }
               
         lightningLight.intensity = Mathf.Lerp(fadeStart, fadeEnd, t / fadeTime);
         yield return 0;

     }              
            
t = 0;
    
}
}