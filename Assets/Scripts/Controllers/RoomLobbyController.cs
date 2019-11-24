using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomLobbyController : MonoBehaviour {

	public float pollingRate;
	public Text roomNameText;
	public SpriteRenderer sprender;
	public GameObject icon;
	public float radius;
	// Use this for initialization
	void Start () {
		RequestManager.instance.GetNewRoom(this);
		InvokeRepeating("CheckForUpdates", 0f, pollingRate);
	}

	private void CheckForUpdates(){
		// RequestManager.instance.CheckForUpdatesRoomLobby(this);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)){
			// RequestManager.instance.CheckForUpdatesRoomLobby(this);
        }
	}

	public void SetRoomName(){
		roomNameText.text = "Your Room is Named:\n<color=yellow>" + RequestManager.instance.currentRoom.name + "</color>";
	}

	public void SetSprite(Sprite newSprite){
		sprender.sprite = newSprite;
	}

	public void MakeRadialIcons(List<RailsUser> users){
		ClearIcons();
		int count = users.Count;
		float theta = ((2.0f * Mathf.PI) / count) + (Mathf.PI / 2.0f);
		for(int k = 0; k < count; k++){
			float x = radius * Mathf.Cos(theta*(k));
			float y = radius * Mathf.Sin(theta*(k));
			Vector3 newPos = new Vector3(x, y, 0f);
			GameObject go = Instantiate(icon, newPos, Quaternion.identity); 
			Icon newIcon = go.GetComponent<Icon>();
			newIcon.SetAvatar(users[k].avatar);
			newIcon.SetName(users[k].name);
		}
	}

	public void ClearIcons(){
		GameObject[] pastIcons = GameObject.FindGameObjectsWithTag("Icon");
		foreach(GameObject icon in pastIcons){
			GameObject.Destroy(icon);
		}
	}
}
