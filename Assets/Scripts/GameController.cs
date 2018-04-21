using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public static GameController instance = null;

	[System.NonSerialized]
	public Turret[] turrets;

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
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
