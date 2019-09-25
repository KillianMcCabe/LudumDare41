using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBomb : Enemy
{
    float jumpRate = 0.8f; // lower is faster
    float jumpStrength = 14;
    float jumpHeight = 1.8f;
    float timeSinceJumped = 0;
    float turnSpeed = 32;
    float dps = 20;

    Turret eatingTurret = null;
    bool wasGroundedLastUpdate = false;

    void Start()
    {
        maxHealth = 500;
        health = 500;
    }

    new void Update()
    {
        base.Update();

        if (eatingTurret != null)
        {
            if (eatingTurret.isAlive)
            {
                eatingTurret.health -= dps * Time.deltaTime;
            }
            else
            {
                eatingTurret = null;
            }
        }
    }

    protected override void Move(Vector3 desiredVelocity)
    {

        Vector3 towardsTarget = target.transform.position - transform.position;
        towardsTarget.Normalize();

        // handle movement
        if (IsGrounded())
        {
            if (!wasGroundedLastUpdate)
            {
                rb.velocity = Vector3.zero;
            }
            if (Vector3.Dot(transform.forward, towardsTarget) > 0.8)
            {
                if (timeSinceJumped > jumpRate)
                {
                    Vector3 jumpVector = transform.forward + Vector3.up * jumpHeight;
                    rb.AddForce(jumpVector * jumpStrength, ForceMode.Impulse);
                    timeSinceJumped = 0;
                }
            }
            wasGroundedLastUpdate = true;
        }
        else
        {
            wasGroundedLastUpdate = false;
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(towardsTarget, Vector3.up), turnSpeed * Time.deltaTime);

        timeSinceJumped += Time.deltaTime;
    }

    protected override void Attack(Turret t)
    {
        eatingTurret = t;
    }

    void LateUpdate()
    {
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }
}
