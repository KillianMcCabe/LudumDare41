using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	protected Turret target;
	protected Rigidbody rb;

	protected float distToGround = 0;
	
	float damage = 30;

	protected float health = 100;
	public bool isAlive = true;

	float chanceToDropItem = 0.25f;

	GameObject explosionPrefab;

	void Awake() {
		rb = GetComponent<Rigidbody>();
		distToGround = GetComponent<Collider>().bounds.extents.y;

		explosionPrefab = Resources.Load("Prefabs/Boom") as GameObject;
	}

	protected void LocateNewTarget() {
		float closestDist = 10000;

		foreach (Turret t in GameController.instance.turrets) {
			if (t.isAlive) {
				float dist = Vector3.Distance(t.transform.position, transform.position);
				if (dist < closestDist) {
					target = t.GetComponent<Turret>();
					closestDist = dist;
				}
			}
		}
	}
	// Use this for initialization
	void Start () {

	}

	public bool TakeDamage(float dmg) {
		health -= dmg;
		if (health < 0) {
			isAlive = false;
			GameController.instance.EnemyCount--;
			if (Random.Range(0f, 1f) < chanceToDropItem) {
				GameObject randomGift = GameController.instance.gifts[Random.Range(0, GameController.instance.gifts.Length)].gameObject;
				Instantiate(randomGift, transform.position, Random.rotation);
			}
			Destroy(gameObject);
			return true;
		}
		return false;
	}
	
	// Update is called once per frame
	protected void Update () {
		if (target == null || !target.isAlive) {
			LocateNewTarget();
		}
	}

	void FixedUpdate() {	
		if (target != null && target.isAlive) {
			Move();
		}
	}

	protected virtual void Move() {

	}

	protected virtual void Attack(Turret t) {
		if (t != null) {
			t.health -= damage;
		} else {
			Debug.LogWarning("Bomb exploded on something that wasn't a turret??..");
		}
		Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		Destroy(gameObject);
		GameController.instance.EnemyCount--;
	}

	protected bool IsGrounded() {
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "Friendly") {
			Turret t = other.gameObject.GetComponent<Turret>();
			Attack(t);
		}
	}
}
