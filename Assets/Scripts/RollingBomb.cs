using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBomb : Enemy {

    float speed = 2;

    void Start ()  {
        health = 50;
    }

	protected override void Move() {
	
		Vector3 towardsTarget = target.transform.position - transform.position;
		towardsTarget.Normalize();

		// handle movement
		if (IsGrounded()) {
            rb.AddForce(towardsTarget * speed);
		}
	}
}
