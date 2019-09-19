using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBomb : Enemy
{
    float speed = 2;

    void Start()
    {
        maxHealth = 50;
        health = 50;

        damage = 25;
    }

    new private void Update()
    {
        base.Update();

        healthBar.transform.position = transform.position + new Vector3(0, 2, 0);
    }

    protected override void Move(Vector3 desiredVelocity)
    {

        Vector3 towardsTarget = target.transform.position - transform.position;
        towardsTarget.Normalize();

        // handle movement
        if (IsGrounded())
        {
            rb.AddForce(towardsTarget * speed);
        }
    }
}
