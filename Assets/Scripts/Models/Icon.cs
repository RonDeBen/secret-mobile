using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Icon : MonoBehaviour{

	public SpriteRenderer avatarSprender, borderSprender, tophatSprender, pinwheelSprender;
	public TextMeshPro tmp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAvatar(Sprite newSprite){
    	avatarSprender.sprite = newSprite;
    }

    public void SetName(string name){
    	tmp.SetText(name);
    }

    public void SetAlphaForAll(float newAlpha){
        Color c = Color.white;
        c.a = newAlpha;
        avatarSprender.color = c;
        borderSprender.color = c;
        tmp.color = c;
        // tophatSprender.color = c;
        // pinwheelSprender.color = c;
    }

    public void SetAlphaForTophat(float newAlpha){
        Color c = Color.white;
        c.a = newAlpha;
        tophatSprender.color = c;
    }

    public void SetAlphaForPinwheel(float newAlpha){
        Color c = Color.white;
        c.a = newAlpha;
        pinwheelSprender.color = c;
    }
}
