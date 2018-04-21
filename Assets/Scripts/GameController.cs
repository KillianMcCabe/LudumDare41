using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public static GameController instance = null;

	[System.NonSerialized]
	public Turret[] turrets;

	// Use this for initialization
	void Start () {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(this);
			return;
		}
		
		turrets = GameObject.FindObjectsOfType<Turret>() as Turret[];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
