using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour {

	public SpriteRenderer fg;
	public SpriteRenderer bg;
	public SpriteRenderer text;

	
	Color fgColorFocus = Color.white;
	Color bgColorFocus = Color.black;

	Color fgColor = Color.black;
	Color bgColor = Color.white;
	

	void OnMouseEnter() {
		fg.color = fgColorFocus;
		text.color = fgColorFocus;
		bg.color = bgColorFocus;
	}

	void OnMouseExit() {
		fg.color = fgColor;
		text.color = fgColor;
		bg.color = bgColor;
	}

	void OnMouseDown() {
		Application.Quit();
	}
	
}
