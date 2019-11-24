using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using SimpleJSON;

[System.Serializable]
public class RailsUser{
	public int id;
	public string name;
	public Sprite avatar;
}

[System.Serializable]
public class Room{
	public int id, chancellor_id, president_id, disorder_count, fascist_policy_count, liberal_policy_count;
	public string name, last_updated;
	public List<RailsUser> users;

	public Room(int id, int chancellor_id, int president_id, int disorder_count, int fascist_policy_count, int liberal_policy_count, string name){
		this.id = id;
		this.chancellor_id = chancellor_id;
		this.president_id = president_id;
		this.disorder_count = disorder_count;
		this.fascist_policy_count = fascist_policy_count;
		this.liberal_policy_count = liberal_policy_count;
		this.name = name;
	}

	public Room(){
		users = new List<RailsUser>();
	}

	public int CommitteeCount(){
		//CHANGE THIS WHEN YOU FIGURE OUT REMOVING PLAYERS
		return users.Count - 2;
	}
}

public class RequestManager : MonoBehaviour {

	public static RequestManager instance;
	public bool isLocalGame;
	private string url;

	public Room currentRoom;

	public int testRoomId;

	void Awake(){
		if(instance != null){
			GameObject.Destroy(instance);
		}
		instance = this;
		#if UNITY_EDITOR || UNITY_STANDALONE
			if(isLocalGame){
				url = "http://localhost:3000";
			}else{
				url = "http://104.236.242.234";
			}
		#elif UNITY_IPHONE || UNITY_ANDROID
			url = "http://104.236.242.234";
		#endif
		DontDestroyOnLoad(gameObject);
	}

	void Start(){
		currentRoom = new Room();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator GetRequest(string uri){
	    bool requestFinished = false;
	    bool requestErrorOccurred = false;

	    UnityWebRequest request = UnityWebRequest.Get(uri);
	    yield return request.Send();

	    requestFinished = true;
	    if (request.isNetworkError){
	        Debug.Log("Something went wrong, and returned error: " + request.error);
	        requestErrorOccurred = true;
	    }
	    else{
	        // Show results as text

	        if (request.responseCode == 200){
	        }
	        else{
	            Debug.Log("Request failed (status:" + request.responseCode + ")");
	            requestErrorOccurred = true;
	        }
	        if (!requestErrorOccurred){
	            yield return null;
	            // process results
	        }
	    }
	}

	IEnumerator GetRoomRequest(string uri, RoomLobbyController rlc){
		bool requestFinished = false;
	    bool requestErrorOccurred = false;
	    UnityWebRequest request = UnityWebRequest.Get(uri);
	    yield return request.Send();

	    if (request.isNetworkError){
	        Debug.Log("Something went wrong, and returned error: " + request.error);
	        requestErrorOccurred = true;
	    }
	    else{
	        if (!requestErrorOccurred){
	            yield return null;
	            // process results
	            var N = JSON.Parse(request.downloadHandler.text);

	            Debug.Log(request.downloadHandler.text);

	            currentRoom.id = N["room"]["id"].AsInt;
				currentRoom.name = N["room"]["name"];
				currentRoom.chancellor_id = N["room"]["chancellor_id"].AsInt;
				currentRoom.president_id = N["room"]["president_id"].AsInt;
				currentRoom.disorder_count = N["room"]["disorder_count"].AsInt;
				currentRoom.fascist_policy_count = N["room"]["fascist_policy_count"].AsInt;
				currentRoom.liberal_policy_count = N["room"]["liberal_policy_count"].AsInt;

				rlc.SetRoomName();
	        }
	    }
	}

	IEnumerator GetTexture(RailsUser user, string uri) {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(uri);
        yield return request.Send();

        if(request.isNetworkError) {
            Debug.Log(request.error);
        }
        else {
            Texture2D temp = (Texture2D)((DownloadHandlerTexture)request.downloadHandler).texture;
            TextureScale.Bilinear(temp, 526, 526);
            user.avatar = Sprite.Create(temp, new Rect(0, 0, temp.width, temp.height), new Vector2(0.5f, 0.5f));
        }
    }

	IEnumerator UpdateRoomRequest(string uri, string bodyJsonString){
		var request = new UnityWebRequest(uri, "POST");
	    byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);
	    request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
	    request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
	    request.SetRequestHeader("Content-Type", "application/json");

	    yield return request.Send();

	    // process results
        var N = JSON.Parse(request.downloadHandler.text);

        //BUILD CURRENT ROOM 
        if(currentRoom.last_updated != N["last_updated"]){
            currentRoom.last_updated = N["last_updated"];
            currentRoom.chancellor_id = N["room"]["chancellor_id"].AsInt;
			currentRoom.president_id = N["room"]["president_id"].AsInt;
			currentRoom.disorder_count = N["room"]["disorder_count"].AsInt;
			currentRoom.fascist_policy_count = N["room"]["fascist_policy_count"].AsInt;
			currentRoom.liberal_policy_count = N["room"]["liberal_policy_count"].AsInt;

        }
        
        if(N["players"].Count > currentRoom.users.Count){//a new user joined
        	List<RailsUser> newUsers = new List<RailsUser>();
        	for(int k  = 0; k < N["players"].Count; k++){
        		RailsUser newUser = new RailsUser();
        		newUser.name = N["players"][k]["name"];
        		newUser.id = N["players"][k]["id"];
        		StartCoroutine(GetTexture(newUser, (url + N["players"][k]["avatar"])));
        		newUsers.Add(newUser);
        	}
        	currentRoom.users = newUsers;
        }
	}

	public void GetNewRoom(RoomLobbyController rlc){
		string uri = url + "/rooms/new.json";
		StartCoroutine(GetRoomRequest(uri, rlc));
	}

	public void CheckForUpdatesRoomLobby(){
		int id = 0;
		if(testRoomId != -1){
			id = testRoomId;
		}else{
			id = currentRoom.id;
		}
		string uri = url + "/rooms/" + id + "/check_for_updates.json";
		string body = "{}";
		StartCoroutine(UpdateRoomRequest(uri, body));
	}

	// public void CheckForUpdatesBoard
}
