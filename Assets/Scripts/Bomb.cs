using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

	Turret target;
	Rigidbody rb;

	float jumpRate = 3; // lower is faster
	float jumpStrength = 5;
	float jumpHeight = 1.5f;
	float turnSpeed = 40;

	float distToGround = 0;
	float timeSinceJumped = 0;
	float damage = 30;

	float health = 100;
	public bool isAlive = true;

	float chanceToDropItem = 0.3f;

	GameObject explosionPrefab;

	void Awake() {
		rb = GetComponent<Rigidbody>();
		distToGround = GetComponent<Collider>().bounds.extents.y;

		explosionPrefab = Resources.Load("Prefabs/Boom") as GameObject;
	}

	void LocateNewTarget() {
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

	public bool Damage(float dmg) {
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
	void Update () {
		if (target != null && target.isAlive) {
			Vector3 towardsTarget = target.transform.position - transform.position;
			towardsTarget.Normalize();

			// handle movement
			if (IsGrounded()) {
				if (timeSinceJumped > jumpRate) {
					Vector3 jumpVector = transform.forward + Vector3.up * jumpHeight;
					rb.AddForce(jumpVector * jumpStrength, ForceMode.Impulse);
					timeSinceJumped = 0;
				}
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(towardsTarget, Vector3.up), turnSpeed * Time.deltaTime);
			}
		} else {
			LocateNewTarget();
		}
		
		timeSinceJumped += Time.deltaTime;
	}

	void LateUpdate()
	{
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
	}

	bool IsGrounded() {
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
	}

	void OnCollisionEnter(Collision other)
	{
		if (!isAlive) {
			Debug.LogWarning("Collided after death?? GHOST BOMB! =o");
		}
		if (other.gameObject.tag == "Friendly") {
			Turret t = other.gameObject.GetComponent<Turret>();
			if (t != null) {
				t.health -= damage;
			} else {
				Debug.LogWarning("Bomb exploded on something that wasn't a turret??..");
			}
			Instantiate(explosionPrefab, transform.position, Quaternion.identity);
			Destroy(gameObject);
			GameController.instance.EnemyCount--;
		}
	}

}
