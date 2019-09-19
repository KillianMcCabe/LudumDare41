using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{

    protected Turret target;
    protected Rigidbody rb;
    NavMeshAgent agent;

    protected float distToGround = 0;

    protected float damage = 30;

    protected float _health = 100;
    protected float maxHealth = 100;
    public bool isAlive = true;

    float chanceToDropItem = 0.3f;

    GameObject explosionPrefab;

    protected GameObject healthBar;
    GameObject healthIndicator;

    protected float health
    {
        get { return _health; }
        set
        {
            _health = value;
            if (_health > maxHealth)
                _health = maxHealth;
            healthIndicator.transform.localScale = new Vector3(_health / maxHealth, 1, 1);
            if (_health < 0)
            {
                isAlive = false;
                Destroy(gameObject);
                GameController.instance.CheckIfGameOver();
            }
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
        agent = GetComponent<NavMeshAgent>();

        explosionPrefab = Resources.Load("Prefabs/Boom") as GameObject;

        healthBar = Instantiate(Resources.Load("Prefabs/HealthBar") as GameObject, Vector3.zero, Quaternion.identity, transform);
        healthBar.transform.localPosition = new Vector3(0, 2, 0);
        healthIndicator = healthBar.transform.Find("Health").gameObject;
    }

    protected void LocateNewTarget()
    {
        float closestDist = 10000;

        foreach (Turret t in GameController.instance.turrets)
        {
            if (t.isAlive)
            {
                float dist = Vector3.Distance(t.transform.position, transform.position);
                if (dist < closestDist)
                {
                    target = t.GetComponent<Turret>();
                    closestDist = dist;

                    if (agent != null)
                    {
                        agent.SetDestination(target.transform.position);
                    }
                    else
                    {
                        Debug.LogError("why is agent null?");
                    }
                }
            }
        }
    }

    public bool TakeDamage(float dmg)
    {
        health -= dmg;
        if (health < 0)
        {
            isAlive = false;
            GameController.instance.EnemyCount--;
            if (Random.Range(0f, 1f) < chanceToDropItem)
            {
                GameObject randomGift = GameController.instance.gifts[Random.Range(0, GameController.instance.gifts.Length)].gameObject;
                Instantiate(randomGift, transform.position, Random.rotation);
            }
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (target == null || !target.isAlive)
        {
            LocateNewTarget();
        }
    }

    void FixedUpdate()
    {
        if (target != null && target.isAlive)
        {
            // Move(agent.desiredVelocity);

            // Vector3 towardsTarget = target.transform.position - transform.position;
            // towardsTarget.Normalize();

            // Debug.DrawLine(transform.position, transform.position + towardsTarget, Color.green);
            // Debug.DrawLine(transform.position, transform.position + agent.desiredVelocity, Color.red);
        }
    }

    protected virtual void Move(Vector3 desiredVelocity)
    {

    }

    protected virtual void Attack(Turret t)
    {
        if (t != null)
        {
            t.health -= damage;
        }
        else
        {
            Debug.LogWarning("Bomb exploded on something that wasn't a turret??..");
        }
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
        GameController.instance.EnemyCount--;
    }

    protected bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Friendly")
        {
            Turret t = other.gameObject.GetComponent<Turret>();
            Attack(t);
        }
    }
}
