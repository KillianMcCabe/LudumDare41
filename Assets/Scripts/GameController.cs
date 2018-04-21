using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public static GameController instance = null;

	[System.NonSerialized]
	public Turret[] turrets;

	public GameObject[] accessories;

	// public Bomb[] bombs;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(this);
			return;
		}

		turrets = GameObject.FindObjectsOfType<Turret>() as Turret[];
		// bombs = GameObject.FindObjectsOfType<Bomb>() as Bomb[];
	}

	// Use this for initialization
	void Start () {

		// Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < accessories.Length; t++ )
        {
            GameObject tmp  = accessories[t];
            int r = Random.Range(t, accessories.Length);
            accessories[t] = accessories[r];
            accessories[r] = tmp;
        }

		for (var i =0; i < turrets.Length && i < accessories.Length; i++) {
			GameObject accessory = accessories[i];
			turrets[i].AddAccessory(accessory);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
