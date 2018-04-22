using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	public string label = "";
	float t = 0;
	float duration = 60;
	bool isHeld = false;
	GameObject itemGlowEffect;

	// Use this for initialization
	void Start () {
		itemGlowEffect = Instantiate(Resources.Load("Prefabs/ItemGlow")) as GameObject;
		itemGlowEffect.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
	}

	void Update() {
		if (!isHeld) {
			if (t > duration) {
				Destroy(itemGlowEffect);
				Destroy(gameObject);
			}
			t += Time.deltaTime;
		}
	}
	
	public void PickedUp() {
		isHeld = true;
		Destroy(itemGlowEffect);
	}
}
