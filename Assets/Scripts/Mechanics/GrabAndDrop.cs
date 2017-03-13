using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAndDrop : MonoBehaviour {

	GameObject toLight;
	GameObject grabbedObject;
	float grabbedObjectSize;
	// public GameObject BlueTorchSpawn;
	// public GameObject RedTorchSpawn;
	public PunTeams.Team ourTeam;
	public Vector3 offset = new Vector3(0,0,0);
	// Use this for initialization
	void Start () {
		Debug.Log ("STARTING GRAB AND DROP");
		// BlueTorchSpawn = GameObject.Find ("BlueTorchSpawn");;
		if (Util.blueTorchSpawn == null) {
			Debug.LogError ("BlueTorchSpawn is null");
		}

		// RedTorchSpawn = GameObject.Find ("RedTorchSpawn");
		// RedTorchSpawn = Util.redTorchSpawn;
		if (Util.redTorchSpawn == null) {
			Debug.LogError ("RedTorchSpawn is null");
		}
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

//			grabbedObject = grabObject;
//			grabbedObjectSize = grabObject.GetComponent<Renderer>().bounds.size.magnitude;
			int objViewID = grabObject.GetComponent<PhotonView>().viewID;
			Debug.Log ("TryGrabObject");
			GetComponent<PhotonView>().RPC("GrabbingObject", PhotonTargets.AllBuffered, objViewID);
		}

	[PunRPC]
	public void GrabbingObject(int viewID) {
		//Vector3 offset = Quaternion.AngleAxis(-45, gameObject.transform.right) * gameObject.transform.forward * 2;
		//grabbedObject.transform.position = gameObject.transform.position + offset;

		grabbedObject = PhotonView.Find (viewID).gameObject;
		grabbedObjectSize = grabbedObject.GetComponent<Renderer>().bounds.size.magnitude;

		grabbedObject.GetComponent<CapsuleCollider> ().enabled = false;

		Vector3 offset = Quaternion.AngleAxis(-45, gameObject.transform.right) * gameObject.transform.forward * 2;
		grabbedObject.transform.position = gameObject.transform.position + offset;
		grabbedObject.transform.SetParent (gameObject.transform, true);
		Debug.Log ("Grabbing Object");
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
		//grabbedObject.transform.SetParent (null, false);
		grabbedObject.transform.parent = null;
		grabbedObject.GetComponent<CapsuleCollider> ().enabled = true;
		grabbedObject.transform.position = gameObject.transform.position;
		grabbedObject = null;
	}

	[PunRPC]
	public void ResetFlag(PunTeams.Team team) {
		Debug.Log ("reset flag");
		if (team == PunTeams.Team.red) {
			GameObject.Find ("Torch_Red").transform.position = Util.redTorchSpawn.transform.position;
		} else {
			GameObject.Find ("Torch_Blue").transform.position = Util.blueTorchSpawn.transform.position;
		}
	}

	void OnCollisionEnter (Collision col)
	{
		//if blue team colliding with red torch
		if (col.gameObject.name == "Torch_Red" && ourTeam == PunTeams.Team.blue) {
			//if red torch is lit
			if (Util.redTorchLit == true) {
				//grab it
				TryGrabObject (col.gameObject);
				Debug.Log ("trying to grb flag");
			} 
			//red torch not lit
			else {
				//if holding lighter
				if (grabbedObject.name == "Lighter") {
					toLight = col.gameObject;
					//light the torch
					GetComponent<PhotonView> ().RPC ("LightTorch", PhotonTargets.AllBuffered, "red");
					//LightTorch ("red");
					//col.gameObject.GetComponent<Torchelight> ().IntensityLight = 1;
					//col.gameObject.GetComponent<Torchelight> ().MaxLightIntensity = 3;
				}
			}
		} else if (col.gameObject.name == "Torch_Blue" && ourTeam == PunTeams.Team.red) {
			if (Util.blueTorchLit == true) {
				TryGrabObject (col.gameObject);
				Debug.Log ("trying to grb flag");
			} else {
				
				if (grabbedObject.name == "Lighter") {
					toLight = col.gameObject;
					GetComponent<PhotonView> ().RPC ("LightTorch", PhotonTargets.AllBuffered, "blue");
					//LightTorch ("blue");
					//col.gameObject.GetComponent<Torchelight> ().IntensityLight = 1;
					//col.gameObject.GetComponent<Torchelight> ().MaxLightIntensity = 3;
				}
			}
		} else if ((col.gameObject.name == "Torch_Red" && ourTeam == PunTeams.Team.red) ||
		         (col.gameObject.name == "Torch_Blue" && ourTeam == PunTeams.Team.blue)) {
			// col.gameObject.transform.position = RedTorchSpawn.transform.position;
			GetComponent<PhotonView> ().RPC ("ResetFlag", PhotonTargets.AllBuffered, ourTeam);
			// Debug.Log ("Red reclaiming red torch");
		}

//		else if(col.gameObject.name == "Torch_Blue" && ourTeam == PunTeams.Team.blue)
//		{
//			col.gameObject.transform.position = BlueTorchSpawn.transform.position;
//			Debug.Log ("Blue reclaiming blue torch");
//		}


		else if (col.gameObject.name == "RedTorchSpawn" && ourTeam == PunTeams.Team.red) {
			if (grabbedObject != null) {
				DropObject ();
				// score() 
				GetComponent<PhotonView> ().RPC ("ResetFlag", PhotonTargets.AllBuffered, PunTeams.Team.blue);
			}

		} else if (col.gameObject.name == "BlueTorchSpawn" && ourTeam == PunTeams.Team.blue) {
//			grabbedObject.transform.position = RedTorchSpawn.transform.position;
//			DropObject ();
			if (grabbedObject != null) {
				DropObject ();
				// score() 
				GetComponent<PhotonView> ().RPC ("ResetFlag", PhotonTargets.AllBuffered, PunTeams.Team.red);
			}
		} 

		else if (col.gameObject.name == "Lighter") 
		{
			TryGrabObject (col.gameObject);
		}

	}




	// Update is called once per frame
	void Update () {
		//Debug.Log (GetMouseHoverObject (5)); 

		/*
		if (Input.GetMouseButtonDown(1))
			{
				if(grabbedObject == null)
				{
					TryGrabObject(GetMouseHoverObject(10));
				}
				
				else
				{
					DropObject(); 
				}	
			}

		*/

		if (grabbedObject != null)
			{
			
			 //Vector3 newPosition = gameObject.transform.position+Camera.main.transform.forward*grabbedObjectSize;
			//Vector3 newPosition = gameObject.transform.position;

			//Vector3 offset = Quaternion.AngleAxis(-45, gameObject.transform.right) * gameObject.transform.forward * 2;
			//grabbedObject.transform.position = gameObject.transform.position + offset;

				
			//grabbedObject.GetComponent<CapsuleCollider> ().enabled = false;
			//grabbedObject.transform.SetParent (gameObject.transform, false);
			//Debug.Log ("Grabbed Object");
			//grabbedObject.transform.position = newPosition;
			}
	}

	public string getGrabbedObjectName()
	{
		return grabbedObject.name;
	}

	public void captureFlag()
	{
		if (grabbedObject != null) {
			DropObject ();
			// score() 
			if (ourTeam == PunTeams.Team.red) {
				GetComponent<PhotonView> ().RPC ("ResetFlag", PhotonTargets.AllBuffered, PunTeams.Team.blue);
			} else {
				GetComponent<PhotonView> ().RPC ("ResetFlag", PhotonTargets.AllBuffered, PunTeams.Team.red);
			}

		}
	}

	public void flameOff()
	{
		GetComponent<PhotonView> ().RPC ("RPCFlameOff", PhotonTargets.AllBuffered);
	}

	[PunRPC]
	public void RPCFlameOff()
	{
		if (getGrabbedObjectName () == "Torch_Red") {
			Util.redTorchLit = false; 
			grabbedObject.GetComponent<Torchelight> ().IntensityLight = 0;
			grabbedObject.GetComponent<Torchelight> ().MaxLightIntensity = 0;

		} 
		else if (getGrabbedObjectName() == "Torch_Blue"){
			Util.blueTorchLit = false;
			grabbedObject.GetComponent<Torchelight> ().IntensityLight = 0;
			grabbedObject.GetComponent<Torchelight> ().MaxLightIntensity = 0;
		}
	}

	[PunRPC]
	public void SetToLight()
	{
		
	}
	[PunRPC]
	public void RPCFlameOn()
	{
		Debug.Log ("setting torch light intensity");
		toLight.GetComponent<Torchelight> ().IntensityLight = 1;
		toLight.GetComponent<Torchelight> ().MaxLightIntensity = 3;
		Debug.Log ("Torch light intensity set");
	}

	[PunRPC]
	public void LightTorch(string colour)
	{
		//GetComponent<PhotonView> ().RPC ("RPCFlameOn", PhotonTargets.AllBuffered);
		if (colour == "red") 
		{
			Util.redTorchLit = true;
			GameObject.Find ("Torch_Red").GetComponent<Torchelight> ().IntensityLight = 1;
			GameObject.Find ("Torch_Red").GetComponent<Torchelight> ().MaxLightIntensity = 1;
		}

		else if (colour == "blue") 
		{
			Util.blueTorchLit = true;
			GameObject.Find ("Torch_Blue").GetComponent<Torchelight> ().IntensityLight = 1;
			GameObject.Find ("Torch_Blue").GetComponent<Torchelight> ().MaxLightIntensity = 1;

		}

		toLight = null;

	}



}
