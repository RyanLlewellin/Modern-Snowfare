using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public float hitPoints = 100f;
	float currentPoints;

	// Use this for initialization
	void Start () {
		currentPoints = hitPoints;
	}
	

	[PunRPC] // can be called indirectly
	// all players recieve notification of something taking damage
	public void TakeDamage (float amt)  {
		currentPoints -= amt;

		if (currentPoints <= 0) {
			Die ();
		}
	}

	void Die(){

		// game objects created locally (crate)
		if (GetComponent<PhotonView> ().instantiationId == 0) {
			Destroy (gameObject);
		} 

		//game objects instantiated over the network (players)
		else {
			// Only the owner of the object destroys the game object
			if (GetComponent<PhotonView>().isMine) {

				// Check to see if this is MY player object. If it's mine, respawn my character
				// Note: make sure character prefab has the tag set to player
				if (gameObject.tag == "Player") {
					// show the standby camera. Optional for now

					NetworkManager nm = GameObject.FindObjectOfType<NetworkManager> ();
					nm.standbyCamera.SetActive(true);
					nm.respawnTimer = 2f;
				}
				PhotonNetwork.Destroy (gameObject);
			}
		}
	}

	/*
	 * DEBUGGING PURPOSES 
	*/
//	void OnGUI(){
//		// If this is my player, kill myself to test respawning
//		if (GetComponent<PhotonView> ().isMine && gameObject.tag == "Player") {
//			if (GUI.Button (new Rect (Screen.width - 225, 0, 225, 30), "I don't wanna be here anymore!")) {
//				Die ();
//			}
//		}
//	}
}