﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour {

	struct Message
	{
		public Message(string strValue, Color colValue)
		{
			stringData = strValue;
			colorData = colValue;
		}
			
		public string stringData { get; private set; }
		public Color colorData { get; private set; }
	}

	/*
	 * @NOTE(Llewellin):
	 * Using an enumerator for colors because I can't send the Color object
	 * as a parameter in an RPC call. So I send an enum to AddTextMessage_RPC, then
	 * call GetColor() to retrieve the color.
	 */
	public enum MColor
	{
		spawn, 
		redTeam,
		blueTeam,
        neutralTeam
	}

	List<Message> textMessages;
	int maxTextMessages = 5;
	GUIStyle textStyle;

	// Use this for initialization
	void Start () {
		textMessages = new List<Message>();
		textStyle = new GUIStyle ();
		textStyle.fontStyle = FontStyle.Bold;
	}

	void OnGUI() {

		// Status of the connection found in the top left corner.
		// GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

		GUILayout.BeginArea (new Rect (0, 0, Screen.width, Screen.height));
		GUILayout.BeginVertical ();
		GUILayout.FlexibleSpace ();

		// Iterate over the messages, setting the color and displaying the text
		foreach (Message msg in textMessages) {
			textStyle.normal.textColor = msg.colorData;
			GUILayout.Label (msg.stringData, textStyle);
		}

		GUILayout.EndVertical ();
		GUILayout.EndArea ();
	}

    /**
     * Create a message indicating which team has picked up the flag.
     */
    public void AddFlagPickupMessage(PunTeams.Team pickupTeam) {
        string teamStr = "";
        MColor color = MColor.neutralTeam;

        switch (pickupTeam) {
            case PunTeams.Team.red:
                teamStr = "Red";
                color = MColor.redTeam;
                break;

            case PunTeams.Team.blue:
                teamStr = "Blue";
                color = MColor.blueTeam;
                break;

            case PunTeams.Team.none:
                teamStr = "Neutral";
                color = MColor.neutralTeam;
                break;

            default:
                Debug.Assert(false, "Panic: Invalid team value");
                break;
        }

        string message = string.Format(">>> {0} team has the flag", teamStr);
        GetComponent<PhotonView>().RPC("AddTextMessage_RPC", 
            PhotonTargets.AllBuffered, message, color);
    }

    public void AddFlagCaptureMessage(PunTeams.Team captureTeam) {
        string teamStr = "";
        MColor color = MColor.neutralTeam;

        switch (captureTeam) {
            case PunTeams.Team.red:
                teamStr = "Red";
                color = MColor.redTeam;
                break;

            case PunTeams.Team.blue:
                teamStr = "Blue";
                color = MColor.blueTeam;
                break;

            case PunTeams.Team.none:
                teamStr = "Neutral";
                color = MColor.neutralTeam;
                break;

            default:
                Debug.Assert(false, "Panic: Invalid team value");
                break;
        }

        string message = string.Format(">>> {0} team has captured the flag", teamStr);
        GetComponent<PhotonView>().RPC("AddTextMessage_RPC",
            PhotonTargets.AllBuffered, message, color);
    }

	public void AddSpawnMessage(string playerName) {
		string message = "Spawning player: " + playerName;
		GetComponent<PhotonView>().RPC("AddTextMessage_RPC", 
			PhotonTargets.AllBuffered, message, MColor.spawn);
	}

	public void AddRedKillMessage(string murderer, string victim) {
		string message = string.Format("{0} killed {1}",
			murderer, victim);
		GetComponent<PhotonView>().RPC("AddTextMessage_RPC", 
			PhotonTargets.AllBuffered, message, MColor.redTeam);
	}

	public void AddBlueKillMessage(string murderer, string victim) {
		string message = string.Format("{0} killed {1}",
			murderer, victim);
		GetComponent<PhotonView>().RPC("AddTextMessage_RPC", 
			PhotonTargets.AllBuffered, message, MColor.blueTeam);
	}

	[PunRPC]
	void AddTextMessage_RPC(string m, MColor mC) {
		//When the max text messages have been recieved, remove the oldest one to make room
		while (textMessages.Count >= maxTextMessages) {
			textMessages.RemoveAt(0);
		}
		Color color = GetColor (mC);
		Message msg = new Message (m, color);
		textMessages.Add(msg);
	}

	Color GetColor( MColor color )
	{
		switch( color )
		{
		case MColor.spawn:
			return Color.black;
		case MColor.redTeam:
			return Color.red;
		case MColor.blueTeam:
			return Color.blue;
        case MColor.neutralTeam:
            return Color.gray;
		}

		return Color.white;
	}

	/*
	 * @TODO(Llewellin):
	 * Get these two functions working
	 
	/// <summary>Needed to update the team lists when joining a room.</summary>
	/// <remarks>Called by PUN. See enum PhotonNetworkingMessage for an explanation.</remarks>
	public void OnJoinedRoom()
	{
		string playerName = Util.localPlayer.GetPhotonView ().owner.NickName;
		string message = playerName + " has joined";
		GetComponent<PhotonView>().RPC("AddTextMessage_RPC", 
			PhotonTargets.AllBuffered, message, MColor.spawn);
	}

	public void OnLeftRoom()
	{
		string playerName = Util.localPlayer.GetPhotonView ().owner.NickName;
		string message = playerName + " has left";
		GetComponent<PhotonView>().RPC("AddTextMessage_RPC", 
			PhotonTargets.AllBuffered, message, MColor.spawn);
	}
	*/
}
