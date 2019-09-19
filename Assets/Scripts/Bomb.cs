using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Enemy
{
    float jumpRate = 3; // lower is faster
    float jumpStrength = 10;
    float jumpHeight = 1.5f;
    float timeSinceJumped = 0;
    float turnSpeed = 40;

    protected override void Move(Vector3 desiredVelocity)
    {
        // Vector3 towardsTarget = target.transform.position - transform.position;
        // towardsTarget.Normalize();

        desiredVelocity.Normalize();

        // handle movement
        if (IsGrounded())
        {
            if (timeSinceJumped > jumpRate)
            {
                Vector3 jumpVector = transform.forward + Vector3.up * jumpHeight;
                rb.AddForce(jumpVector * jumpStrength, ForceMode.Impulse);
                timeSinceJumped = 0;
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(desiredVelocity, Vector3.up), turnSpeed * Time.deltaTime);
        }

        timeSinceJumped += Time.deltaTime;
    }

    void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }
}
