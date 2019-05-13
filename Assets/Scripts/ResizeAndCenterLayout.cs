using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizeAndCenterLayout : MonoBehaviour {

	public float percentWidth, percentHeight;
	public RectTransform parentRectObj;
	public VerticalLayoutGroup VLG;

	private Rect parentRect;

	// Use this for initialization
	void Start () {
		parentRect = parentRectObj.rect;
		int verticalPadding = (int)(parentRect.height * (1f-percentHeight))/2;
		int horizontalPadding = (int)(parentRect.width * (1f-percentWidth)) / 2;

		VLG.padding.left = horizontalPadding;
		VLG.padding.right = horizontalPadding;
		VLG.padding.top = verticalPadding;
		VLG.padding.bottom = verticalPadding;
	}
}
