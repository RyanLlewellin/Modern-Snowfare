using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAndDrop : MonoBehaviour {

	GameObject grabbedObject;
	public PunTeams.Team ourTeam;
	public Vector3 offset = new Vector3(0,0,0);

	// Use this for initialization
	void Start () {
		ourTeam = PhotonNetwork.player.GetTeam();
	}

	/*
	GameObject GetMouseHoverObject(float range)
	{
		Vector3 position = this.gameObject.transform.position;
		RaycastHit raycastHit;
		Vector3 target = position + Camera.main.transform.forward * range; 
		if (Physics.Linecast(position, target, out raycastHit))
		{
			return raycastHit.collider.gameObject;
			
		}
		return null; 
	}*/

	void TryGrabObject(GameObject grabObject)
	{
		if(grabObject == null)
		{
			return;
		}
		
		int objViewID = grabObject.GetComponent<PhotonView>().viewID;
		GetComponent<PhotonView>().RPC("GrabbingObject", PhotonTargets.AllBuffered, objViewID);
	}

	[PunRPC]
	public void GrabbingObject(int viewID) {

		grabbedObject = PhotonView.Find(viewID).gameObject;

		grabbedObject.GetComponent<CapsuleCollider> ().enabled = false;
		grabbedObject.transform.SetParent (gameObject.transform, false);
		Vector3 offset = Quaternion.AngleAxis(-45, gameObject.transform.right) * gameObject.transform.forward * 2;
		grabbedObject.transform.position = gameObject.transform.position + offset;
	}

	public void DropObject()
	{
		GetComponent<PhotonView>().RPC("DroppingObject", PhotonTargets.AllBuffered);
	}

	[PunRPC]
	public void DroppingObject(){
		if (grabbedObject == null)
		{
			return;
		}

		grabbedObject.transform.parent = null;
		grabbedObject.GetComponent<CapsuleCollider> ().enabled = true;
		grabbedObject.transform.position = gameObject.transform.position;
		grabbedObject = null;
	}

	[PunRPC]
	public void ResetFlag(PunTeams.Team team) {

		if (team == PunTeams.Team.red) {
			GameObject.Find ("Torch_Red").transform.position = Util.defaultRedFlag;
		} else {
			GameObject.Find ("Torch_Blue").transform.position = Util.defaultBlueFlag;
		}
	}

	void OnCollisionEnter (Collision col)
	{
		// If we collide with the opponents flag
		if ((col.gameObject.name == "Torch_Red" && ourTeam == PunTeams.Team.blue) ||
		    (col.gameObject.name == "Torch_Blue" && ourTeam == PunTeams.Team.red)) 
		{
			TryGrabObject (col.gameObject);
		}
		// If we collide with our own flag (red team)
		else if (col.gameObject.name == "Torch_Red" && ourTeam == PunTeams.Team.red) 
		{
			// Reset the flag only if its not at the base
			if (col.gameObject.transform.position != Util.defaultRedFlag) {
				GetComponent<PhotonView> ().RPC ("ResetFlag", PhotonTargets.AllBuffered, ourTeam);
			}
		} 
		// If we collide with our own flag (blue team)
		else if(col.gameObject.name == "Torch_Blue" && ourTeam == PunTeams.Team.blue) 
		{
			// Reset the flag only if its not at the base
			if (col.gameObject.transform.position != Util.defaultBlueFlag) {
				GetComponent<PhotonView> ().RPC ("ResetFlag", PhotonTargets.AllBuffered, ourTeam);
			}
		}
	}

	public string GetGrabbedObjectName()
	{
		if (grabbedObject == null) {
			return "";
		}
		return grabbedObject.name;
	}

	public void CaptureFlag()
	{
		if (grabbedObject != null) {
			DropObject ();

			IncreaseTeamScore (ourTeam);

			//If we're red team, reset the blue flag
			if (ourTeam == PunTeams.Team.red) {
				GetComponent<PhotonView> ().RPC ("ResetFlag", PhotonTargets.AllBuffered, PunTeams.Team.blue);
			} 
			else {
				GetComponent<PhotonView> ().RPC ("ResetFlag", PhotonTargets.AllBuffered, PunTeams.Team.red);
			}

		}
	}

	public void IncreaseTeamScore( PunTeams.Team team )
	{
		//We need to know which property we have to change, blue or red
		string property = RoomProperty.BlueScore;

		if( team == PunTeams.Team.red )
		{
			property = RoomProperty.RedScore;
		}

		ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();
		//In case the property doesn't yet exist, create it with a score of 1
		newProperties.Add( property, 1 );

		if( PhotonNetwork.room.customProperties.ContainsKey( property ) == true )
		{
			//if the property does exist, we just add one to the old value
			newProperties[ property ] = (int)PhotonNetwork.room.customProperties[ property ] + 1;
		}

		PhotonNetwork.room.SetCustomProperties( newProperties );
	}

}
