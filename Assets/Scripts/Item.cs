using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	public string label = "";

	GameObject itemGlowEffect;

	// Use this for initialization
	void Start () {
		itemGlowEffect = Instantiate(Resources.Load("Prefabs/ItemGlow")) as GameObject;
		itemGlowEffect.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
	}
	
	public void DisableGlow() {
		itemGlowEffect.SetActive(false);
	}
}
