using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLayout : MonoBehaviour {

	public LayoutElement logoElem, topTextElem, avatarElem, nicknameElem, findGameElem;
	public VerticalLayoutGroup VLG;
	public float 	leftPercentage, rightPercentage, topPercentage, bottomPercentage, spacingPercentage,
					logoHeightPercentage, topTextHeightPercentage, avatarHeightPercentage, nicknameHeightPercentage, findGameHeightPercentage;
	
	private Rect canvasRect;

	// Use this for initialization
	void Start () {
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
}
