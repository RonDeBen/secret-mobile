using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RequestManager : MonoBehaviour {

	public static RequestManager instance;
	public bool isLocalGame;
	private string url;
	private Sprite userSprite;

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
	
	// Update is called once per frame
	void Update () {
		
	}

	// IEnumerator GetRequest(string uri){
	//     bool requestFinished = false;
	//     bool requestErrorOccurred = false;

	//     UnityWebRequest request = UnityWebRequest.Get(uri);
	//     yield return request.Send();

	//     requestFinished = true;
	//     if (request.isError){
	//         Debug.Log("Something went wrong, and returned error: " + request.error);
	//         requestErrorOccurred = true;
	//     }
	//     else{
	//         // Show results as text

	//         if (request.responseCode == 200){
	//         }
	//         else{
	//             Debug.Log("Request failed (status:" + request.responseCode + ")");
	//             requestErrorOccurred = true;
	//         }
	//         if (!requestErrorOccurred){
	//             yield return null;
	//             // process results
	//         }
	//     }
	// }

	// IEnumerator PostRequest(string uri, string bodyJsonString){
	//     var request = new UnityWebRequest(uri, "POST");
	//     byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(bodyJsonString);
	//     request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
	//     request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
	//     request.SetRequestHeader("Content-Type", "application/json");

	//     yield return request.Send();
	//     Debug.Log("Response: " + request.downloadHandler.text);
	// }

	IEnumerator UploadUserStuff(Texture2D tex, string nickname, string uri)
    {
        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();

        //create sprite from tex, and save to userSprite
        userSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);;

        // Create a Web Form
        WWWForm form = new WWWForm();
        form.AddField("nickname", nickname);
        form.AddBinaryData("avatar", bytes, "avatar.png", "image/png");

        // Upload to a cgi script
        using (var w = UnityWebRequest.Post(uri, form))
        {
            yield return w.Send();
            if (w.isError) {
                print(w.error);
            }
            else {
                print("Finished Uploading Screenshot");
            }
        }
    }

    public void SendUserStuff(string nickname, Texture2D tex){
		string uri = url + "/users/login.json";
		StartCoroutine(UploadUserStuff(tex, nickname, uri));
	}
}
