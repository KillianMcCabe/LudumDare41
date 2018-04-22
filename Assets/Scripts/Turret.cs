using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

	float range = 30;
	float turnSpeed = 30;
	float gunTurnSpeed = 30;
	float dps = 30;

	Enemy target;

	public GameObject gun;
	public GameObject gunEffect;
	public Transform bowSlot;
	public Transform flowerSlot;
	public GameObject healthIndicator;

	GameObject flirtParticleEffect;
	GameObject receivedGiftParticleEffect;
	GameObject receivedGoodGiftParticleEffect;
	GameObject receivedBadGiftParticleEffect;

	public bool isFlirtable = false;
	float timeSinceFlirted = 0;
	float flirtCD = 10;

	float maxHealth = 100;
	float _health = 100;
	float healthGainedFromFlirting = 30f;
	public bool isAlive;

	string likes = "";
	string dislikes = "";

	public float health
	{
		get { return _health; }
		set
		{
			_health = value;
			if (_health > maxHealth)
				_health = maxHealth;
			healthIndicator.transform.localScale = new Vector3(_health/maxHealth, 1, 1);
			if (_health < 0) {
				isAlive = false;
				Destroy(gameObject);
				GameController.instance.CheckIfGameOver();
			}
		}
	}

	void Awake() {
		flirtParticleEffect = Resources.Load("Prefabs/FlirtParticleEffect") as GameObject;
		receivedGiftParticleEffect = Resources.Load("Prefabs/ReceivedGiftParticleEffect") as GameObject;
		receivedGoodGiftParticleEffect = Resources.Load("Prefabs/ReceivedGoodGiftParticleEffect") as GameObject;
		receivedBadGiftParticleEffect = Resources.Load("Prefabs/ReceivedBadGiftParticleEffect") as GameObject;

		isAlive = true;
	}

	// Use this for initialization
	void Start () {
		FindClosestTargetWithinRange();
		gunEffect.SetActive(false);
	}
	
	void FindClosestTargetWithinRange() { // TODO: optimize
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy");

		float bestDistance = range;
		target = null;
        foreach (GameObject obj in objs)
        {
			float dist = Vector3.Distance(transform.position, obj.transform.position);
            if (dist < bestDistance)
            {
                target = obj.GetComponent<Enemy>();
				bestDistance = dist;
            }
        }
	}

	public void GiveItem(Item item) {

		float multiplier;
		if (item.label == likes) {
			multiplier = 1.5f;
			Instantiate(receivedGoodGiftParticleEffect, transform.position, Quaternion.identity);
		} else if (item.label == dislikes) {
			multiplier = -0.5f;
			Instantiate(receivedBadGiftParticleEffect, transform.position, Quaternion.identity);
		} else {
			multiplier = 1;
			Instantiate(receivedGiftParticleEffect, transform.position, Quaternion.identity);
		}

		maxHealth += Random.Range(1.0f, 3.5f) * multiplier;
		health += Random.Range(1f, 3.5f) * multiplier;
		healthGainedFromFlirting += Random.Range(1f, 2.5f) * multiplier;
		range += Random.Range(1f, 3.5f) * multiplier;
		turnSpeed += Random.Range(4f, 8f) * multiplier;
		gunTurnSpeed += Random.Range(4f, 8f) * multiplier;
		dps += Random.Range(1f, 3.5f) * multiplier;
	}

	// Update is called once per frame
	void Update () {
		if (target != null && target.isAlive) {
			// Turn towards target
			Vector3 towardsTarget = target.transform.position - transform.position;
			towardsTarget = new Vector3(towardsTarget.x, 0, towardsTarget.z);
			towardsTarget.Normalize();
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(towardsTarget, Vector3.up), turnSpeed * Time.deltaTime);

			if (Vector3.Dot(towardsTarget, transform.forward) > 0.9) {
				// Aim gun at target
				Vector3 gunTowardsTarget = target.transform.position - gun.transform.position;
				gunTowardsTarget.Normalize();
				gun.transform.rotation = Quaternion.RotateTowards(gun.transform.rotation, Quaternion.LookRotation(gunTowardsTarget, Vector3.up), gunTurnSpeed * Time.deltaTime);

				if (Vector3.Dot(towardsTarget, transform.forward) > 0.9) {
					gunEffect.SetActive(true);
					target.TakeDamage(dps * Time.deltaTime);
				} else {
					gunEffect.SetActive(false);
				}
			} else {
				gunEffect.SetActive(false);
			}
		} else {
			gunEffect.SetActive(false);
			FindClosestTargetWithinRange();
		}

		timeSinceFlirted += Time.deltaTime;
		if (timeSinceFlirted > flirtCD) {
			isFlirtable = true;
		}
	}

	public void Flirt() {
		health += healthGainedFromFlirting;
		Instantiate(flirtParticleEffect, transform.position, Quaternion.identity);
		isFlirtable = false;
		timeSinceFlirted = 0;
	}

	public void AddAccessory(GameObject accessory) {
		GameObject go = Instantiate(accessory);
		if (accessory.name.StartsWith("Bow")) {
			go.transform.SetParent(bowSlot.transform, false);
		} else {
			go.transform.SetParent(flowerSlot.transform, false);
		}
	}

	public void SetLike(string label) {
		likes = label;
	}

	public void SetDislike(string label) {
		dislikes = label;
	}

	void LateUpdate()
	{
		// confine gun rotation
		gun.transform.localEulerAngles = new Vector3(gun.transform.localEulerAngles.x, 0, 0);
	}
}
