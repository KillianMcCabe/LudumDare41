using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

	GameObject target;
	Rigidbody rb;

	float jumpRate = 3; // lower is faster
	float jumpStrength = 4;
	float jumpHeight = 1.5f;
	float turnSpeed = 40;

	float distToGround = 0;
	float timeSinceJumped = 0;

	float health = 100;
	public bool isAlive = true;

	GameObject explosionPrefab;

	void Awake() {
		rb = GetComponent<Rigidbody>();
		distToGround = GetComponent<Collider>().bounds.extents.y;

		explosionPrefab = Resources.Load("Prefabs/Boom") as GameObject;
	}

	// Use this for initialization
	void Start () {
		float closestDist = 10000;

		foreach (Turret t in GameController.instance.turrets) {
			float dist = Vector3.Distance(t.transform.position, transform.position);
			if (dist < closestDist) {
				target = t.gameObject;
				closestDist = dist;
			}
		}
	}

	public bool Damage(float dmg) {
		health -= dmg;
		if (health < 0) {
			isAlive = false;
			GameController.instance.EnemyCount --;
			Destroy(gameObject);
			return true;
		}
		return false;
	}
	
	// Update is called once per frame
	void Update () {
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
		if (other.gameObject.tag == "Friendly") {
			Instantiate(explosionPrefab, transform.position, Quaternion.identity);
			Destroy(gameObject);
		}
	}

}
