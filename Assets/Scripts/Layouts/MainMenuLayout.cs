using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLayout : MonoBehaviour {

	public LayoutElement logoElem, topTextElem, avatarElem, nicknameElem, findGameElem;
	public VerticalLayoutGroup VLG;
	public float 	leftPercentage, rightPercentage, topPercentage, bottomPercentage, spacingPercentage,
					logoHeightPercentage, topTextHeightPercentage, avatarHeightPercentage, nicknameHeightPercentage, findGameHeightPercentage;
	public Text nicknameText;
	public Image avatarImage;
	private Texture2D avatarTex;
	public Texture2D dummy;
	private Rect canvasRect;

	// Use this for initialization
	void Start () {
		#if UNITY_EDITOR || UNITY_STANDALONE
			avatarImage.sprite = Sprite.Create(dummy, new Rect(0.0f, 0.0f, dummy.width, dummy.height), new Vector2(0.5f, 0.5f), 100.0f);;
		#endif
		Rect canvasRect = gameObject.GetComponent<RectTransform>().rect;
		float height = canvasRect.height;
		//left, right, top, bottom
		VLG.padding = new RectOffset((int)(height * leftPercentage), (int)(height * rightPercentage), (int)(height * topPercentage), (int)(height * bottomPercentage));
		VLG.spacing = (int)(height * spacingPercentage);

		logoElem.preferredHeight = height * logoHeightPercentage;
		topTextElem.preferredHeight = height * topTextHeightPercentage;
		avatarElem.preferredHeight = height * avatarHeightPercentage;
		nicknameElem.preferredHeight = height * nicknameHeightPercentage;
		findGameElem.preferredHeight = height * findGameHeightPercentage;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnAvatarButtonClicked(){
		if(NativeGallery.IsMediaPickerBusy()){
			return;
		}

		PickImage(512);
	}

	public void OnFindGameButtonClicked(){
		// RequestManager.instance.SendUserStuff(nicknameText.text, avatarTex);
	}

	private void PickImage( int maxSize )
	{
		NativeGallery.Permission permission = NativeGallery.GetImageFromGallery( ( path ) =>
		{
			Debug.Log( "Image path: " + path );
			if( path != null )
			{
				// Create Texture from selected image
				avatarTex = NativeGallery.LoadImageAtPath( path, maxSize );
				if( avatarTex == null )
				{
					Debug.Log( "Couldn't load sprite from " + path );
					return;
				}

				avatarImage.sprite = Sprite.Create(avatarTex, new Rect(0.0f, 0.0f, avatarTex.width, avatarTex.height), new Vector2(0.5f, 0.5f), 100.0f);
			}
		}, "Select a PNG image", "image/png", maxSize );
		Debug.Log( "Permission result: " + permission );
	}
}
