using UnityEngine;
using System.Collections;

public class CharacterRespawnSmoke : MonoBehaviour {



	public GameObject respawnSmokeFX;
	public GameObject characterGeometry; 


void Start (){

    respawnSmokeFX.SetActive(false);
    characterGeometry.SetActive(false);

}

 
void Update (){

    if (Input.GetButtonDown("Fire1"))
    {

			StartCoroutine("SpawnCharacter");    

    }
     
}



IEnumerator SpawnCharacter (){

    respawnSmokeFX.SetActive(true);

	yield return new WaitForSeconds (0.4f);

    characterGeometry.SetActive(true);


}
}