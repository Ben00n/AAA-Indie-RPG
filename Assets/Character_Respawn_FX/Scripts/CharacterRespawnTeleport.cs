using UnityEngine;
using System.Collections;

public class CharacterRespawnTeleport : MonoBehaviour {

public GameObject respawnTeleportFX;
public Light teleportLight;
public GameObject characterGeometry; 

private float t = 0.0f;
private float fadeStart = 2;
private float fadeEnd = 0;
private float fadeTime = 1;
private float pauseTime = 0;


void Start (){
    characterGeometry.SetActive(false);
    respawnTeleportFX.SetActive(false);
}

 
void Update (){

    if (Input.GetButtonDown("Fire1"))
    {

		StartCoroutine("SpawnCharacter");

    }

}
     

IEnumerator SpawnCharacter (){

    respawnTeleportFX.SetActive(true);

	yield return new  WaitForSeconds (1.75f);
   
    characterGeometry.SetActive(true);
	StartCoroutine("FadeLight"); 

}


IEnumerator FadeLight (){
   
     while (t < fadeTime) 
     {

         if (pauseTime == 0)
         {
             t += Time.deltaTime;
         }
               
         teleportLight.intensity = Mathf.Lerp(fadeStart, fadeEnd, t / fadeTime);
         yield return 0;

     }              
            
t = 0;
    
}
}