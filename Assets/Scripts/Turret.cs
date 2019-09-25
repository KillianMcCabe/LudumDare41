using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum StatType
{
    Range,
    Speed,
    Damage,
    Fortitude,
    Romance
}

public class Turret : MonoBehaviour
{
    public System.Action OnDeath;
    public System.Action OnChange; // e.g. found like / dislike, gained stats etc

    private int _level = 1;
    public int Level
    {
        get { return _level; }
    }

    // TODO: create DpsBaseValue and DpsPerStatPoint
    [System.NonSerialized]
    public float StatRange = 1;

    [System.NonSerialized]
    public float StatTurnSpeed = 1;

    [System.NonSerialized]
    public float StatDps = 1;

    [System.NonSerialized]
    public float StatFortitude = 1;

    [System.NonSerialized]
    public float StatRomance = 1;

    public const float DpsBaseValue = 30f;
    public const float DpsPerStatPoint = 5f;

    private float _health = 100;

    private Enemy target;

    public GameObject gun;
    public GameObject gunEffect;
    public Transform bowSlot;
    public Transform flowerSlot;
    public GameObject healthIndicator;

    GameObject flirtParticleEffect;
    GameObject receivedGiftParticleEffect;
    GameObject receivedGoodGiftParticleEffect;
    GameObject receivedBadGiftParticleEffect;

    public string Name {get; set;}

    public bool isFlirtable = false;
    float timeSinceFlirted = 0;
    float flirtCD = 10;
    public bool isAlive;

    public bool FoundLike {get; private set;}
    public bool FoundDislike {get; private set;}

    public string Likes {get; set;}
    public string Dislikes {get; set;}

    public float health
    {
        get { return _health; }
        set
        {
            _health = value;
            float maxHealth = CalculateMaxHealth();
            if (_health > maxHealth)
                _health = maxHealth;
            healthIndicator.transform.localScale = new Vector3(_health / maxHealth, 1, 1);
            if (_health < 0)
            {
                isAlive = false;
                Destroy(gameObject);
                if (OnDeath != null)
                {
                    OnDeath.Invoke();
                }
            }
        }
    }

    void Awake()
    {
        flirtParticleEffect = Resources.Load("Prefabs/FlirtParticleEffect") as GameObject;
        receivedGiftParticleEffect = Resources.Load("Prefabs/ReceivedGiftParticleEffect") as GameObject;
        receivedGoodGiftParticleEffect = Resources.Load("Prefabs/ReceivedGoodGiftParticleEffect") as GameObject;
        receivedBadGiftParticleEffect = Resources.Load("Prefabs/ReceivedBadGiftParticleEffect") as GameObject;

        isAlive = true;

        FoundLike = false;
        FoundDislike = false;
    }

    // Use this for initialization
    void Start()
    {
        FindClosestTargetWithinRange();
        gunEffect.SetActive(false);
    }

    private float CalculateFiringRange()
    {
        return 30f + (StatRange * 5f);
    }

    private float CalculateDps()
    {
        return DpsBaseValue + (StatDps * DpsPerStatPoint);
    }

    private float CalculateFlirtHealthGain()
    {
        return 30f + (StatRomance * 5f);
    }

    private float CalculateMaxHealth()
    {
        return 100f + (StatFortitude * 10f);
    }

    private float CalculateTurnSpeed()
    {
        return 34f + (StatTurnSpeed * 5f);
    }

    void FindClosestTargetWithinRange()
    { // TODO: optimize
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Enemy");

        float bestDistance = CalculateFiringRange();
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

    public void GiveItem(Item item)
    {
        int statPoints;
        if (item.Label == Likes)
        {
            statPoints = 4;
            Instantiate(receivedGoodGiftParticleEffect, transform.position, Quaternion.identity);
            GameController.instance.DisplayHint("That was the perfect gift for that tower!");
            FoundLike = true;
        }
        else if (item.Label == Dislikes)
        {
            statPoints = 0;
            Instantiate(receivedBadGiftParticleEffect, transform.position, Quaternion.identity);
            GameController.instance.DisplayHint("I don't think that was the right gift for that tower");
            FoundDislike = true;
        }
        else
        {
            statPoints = 2;
            Instantiate(receivedGiftParticleEffect, transform.position, Quaternion.identity);
        }

        _level += statPoints;

        // randomly distribute stat points
        while (statPoints > 0)
        {
            StatType statType = (StatType)Random.Range(0, System.Enum.GetValues(typeof(StatType)).Length);
            switch (statType)
            {
                case StatType.Range:
                    StatRange ++;
                    break;
                case StatType.Fortitude:
                    StatFortitude ++;
                    break;
                case StatType.Speed:
                    StatTurnSpeed ++;
                    break;
                case StatType.Damage:
                    StatDps ++;
                    break;
                case StatType.Romance:
                    StatRomance ++;
                    break;
                default:
                    Debug.LogError("Unhandled StatType \"" + statType + "\"");
                    break;
            }
            statPoints--;
        }

        if (OnChange != null)
        {
            OnChange.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null && target.isAlive)
        {
            // Turn towards target
            Vector3 towardsTarget = target.transform.position - transform.position;
            towardsTarget = new Vector3(towardsTarget.x, 0, towardsTarget.z);
            towardsTarget.Normalize();
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(towardsTarget, Vector3.up), CalculateTurnSpeed() * Time.deltaTime);

            if (Vector3.Dot(towardsTarget, transform.forward) > 0.9)
            {
                // Aim gun at target
                Vector3 gunTowardsTarget = target.transform.position - gun.transform.position;
                gunTowardsTarget.Normalize();
                gun.transform.rotation = Quaternion.RotateTowards(gun.transform.rotation, Quaternion.LookRotation(gunTowardsTarget, Vector3.up), CalculateTurnSpeed() * Time.deltaTime);

                if (Vector3.Dot(towardsTarget, transform.forward) > 0.9)
                {
                    gunEffect.SetActive(true);
                    target.TakeDamage(CalculateDps() * Time.deltaTime);
                }
                else
                {
                    gunEffect.SetActive(false);
                }
            }
            else
            {
                gunEffect.SetActive(false);
            }
        }
        else
        {
            gunEffect.SetActive(false);
            FindClosestTargetWithinRange();
        }

        timeSinceFlirted += Time.deltaTime;
        if (timeSinceFlirted > flirtCD)
        {
            isFlirtable = true;
        }
    }

    public void Flirt()
    {
        health += CalculateFlirtHealthGain();
        Instantiate(flirtParticleEffect, transform.position, Quaternion.identity);
        isFlirtable = false;
        timeSinceFlirted = 0;
    }

    public void AddAccessory(GameObject accessory)
    {
        GameObject go = Instantiate(accessory);
        if (accessory.name.StartsWith("Bow"))
        {
            go.transform.SetParent(bowSlot.transform, false);
        }
        else
        {
            go.transform.SetParent(flowerSlot.transform, false);
        }
    }

    public void SetLike(string label)
    {
        Likes = label;
    }

    public void SetDislike(string label)
    {
        Dislikes = label;
    }

    void LateUpdate()
    {
        // confine gun rotation
        gun.transform.localEulerAngles = new Vector3(gun.transform.localEulerAngles.x, 0, 0);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Gift")
        {
            Item item = other.gameObject.GetComponent<Item>();
            GiveItem(item);
            GameObject.Destroy(item.gameObject);
        }
    }
}
