using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

	float range = 40;
	float turnSpeed = 40;
	float gunTurnSpeed = 40;
	float dps = 50;

	Bomb target;

	public GameObject gun;
	public GameObject gunEffect;
	public Transform bowSlot;
	public Transform flowerSlot;

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
                target = obj.GetComponent<Bomb>();
				bestDistance = dist;
            }
        }
	}

	// Update is called once per frame
	void Update () {
		if (target != null) {
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
					bool killedIt = target.Damage(dps * Time.deltaTime);
					if (killedIt) {
						target = null;
						gunEffect.SetActive(false);
					}
				} else {
					gunEffect.SetActive(false);
				}
			}
		} else {
			FindClosestTargetWithinRange();
		}
	}

	public void AddAccessory(GameObject accessory) {
		GameObject go = Instantiate(accessory);
		if (accessory.name.StartsWith("Bow")) {
			go.transform.SetParent(bowSlot.transform, false);
		} else {
			go.transform.SetParent(flowerSlot.transform, false);
		}
	}

	void LateUpdate()
	{
		// confine gun rotation
		gun.transform.localEulerAngles = new Vector3(gun.transform.localEulerAngles.x, 0, 0);
	}
}
