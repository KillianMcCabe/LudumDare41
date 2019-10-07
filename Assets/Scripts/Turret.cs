using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    Unknown,
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

    [SerializeField]
    private StatData[] _statDatas = null;

    private Dictionary<StatData, int> _statLevels = new Dictionary<StatData, int>();

    private float _health = 0;

    private Enemy target;

    public GameObject gun;
    public GameObject gunEffect;
    public Transform bowSlot;
    public Transform flowerSlot;
    public GameObject healthIndicator;

    private GameObject flirtParticleEffect;
    private GameObject receivedGiftParticleEffect;
    private GameObject receivedGoodGiftParticleEffect;
    private GameObject receivedBadGiftParticleEffect;

    public string Name {get; set;}
    public int AvailableStatPoints {get; set;} = 0;
    public bool HasFullHealth {get; private set;} = false;
    public bool isAlive {get; private set;} = true;
    public bool FoundLike {get; private set;} = false;
    public bool FoundDislike {get; private set;} = false;

    public string Likes {get; set;}
    public string Dislikes {get; set;}

    public float MaxHealth { get; private set; } = 0;

    public float Health
    {
        get { return _health; }
        set
        {
            _health = value;

            if (_health > MaxHealth)
            {
                _health = MaxHealth;
            }

            HasFullHealth = _health == MaxHealth;

            if (!float.IsNaN(_health))
                healthIndicator.transform.localScale = new Vector3(_health / MaxHealth, 1, 1);

            if (OnChange != null)
            {
                OnChange.Invoke();
            }

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

        foreach (StatData stat in _statDatas)
        {
            _statLevels.Add(stat, 1);
        }
    }

    // Use this for initialization
    void Start()
    {
        FindClosestTargetWithinRange();
        gunEffect.SetActive(false);

        RecalculateMaxHealth();
        _health = MaxHealth;
    }

    private float CalculateDPS()
    {
        const float BaseDPS = 30f;
        const float DPSPerDamageLevel = 5f;
        return BaseDPS + (GetStatLevel(StatData.GetDamageStatData()) * DPSPerDamageLevel);
    }

    private float CalculateFiringRange()
    {
        const float BaseRange = 18f;
        const float RangePerRangeLevel = 4f;
        return BaseRange + (GetStatLevel(StatData.GetRangeStatData()) * RangePerRangeLevel);
    }

    private float CalculateFlirtHealthGain()
    {
        return 30f + (GetStatLevel(StatData.GetRomanceStatData()) * 5f);
    }

    private void RecalculateMaxHealth()
    {
        const float BaseHP = 90f;
        const float HPPerFortitudeLevel = 10f;

        float prevMaxHealth = MaxHealth;
        MaxHealth = BaseHP + (GetStatLevel(StatData.GetFortitudeStatData()) * HPPerFortitudeLevel);

        float proportionalHealthChange = MaxHealth / prevMaxHealth;

        Health = Health * proportionalHealthChange;
    }

    private float CalculateTurnSpeed()
    {
        return 34f + (GetStatLevel(StatData.GetMobilityStatData()) * 6f);
    }

    private float CalculateHealthRegenRate()
    {
        return (MaxHealth / 100f); // 1% of max hp
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
        if (item.Label == Likes)
        {
            AvailableStatPoints += 4;
            Instantiate(receivedGoodGiftParticleEffect, transform.position, Quaternion.identity);
            GameController.Instance.DisplayHint("That was the perfect gift for that tower!");
            FoundLike = true;
        }
        else if (item.Label == Dislikes)
        {
            AvailableStatPoints += 0;
            Instantiate(receivedBadGiftParticleEffect, transform.position, Quaternion.identity);
            GameController.Instance.DisplayHint("I don't think that was the right gift for that tower");
            FoundDislike = true;
        }
        else
        {
            AvailableStatPoints += 2;
            Instantiate(receivedGiftParticleEffect, transform.position, Quaternion.identity);
        }

        _level += AvailableStatPoints;

        if (OnChange != null)
        {
            OnChange.Invoke();
        }
    }

    private void Update()
    {
        RecalculateMaxHealth();

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

                    target.TakeDamage(CalculateDPS() * Time.deltaTime);
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

            // health regen
            _health += CalculateHealthRegenRate() * Time.deltaTime;
        }
        else
        {
            gunEffect.SetActive(false);
            FindClosestTargetWithinRange();
        }
    }

    private void LateUpdate()
    {
        // confine gun rotation
        gun.transform.localEulerAngles = new Vector3(gun.transform.localEulerAngles.x, 0, 0);
    }

    public void Flirt()
    {
        Health += CalculateFlirtHealthGain();
        Instantiate(flirtParticleEffect, transform.position, Quaternion.identity);
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

    public int GetStatLevel(StatData statType)
    {
        if (_statLevels.ContainsKey(statType))
        {
            return _statLevels[statType];
        }
        else
        {
            Debug.LogError("Unhandled StatType \"" + statType + "\"");
            return -1;
        }
    }

    public void SetStatLevel(StatData statType, int level)
    {
        if (_statLevels.ContainsKey(statType))
        {
            _statLevels[statType] = level;
        }
        else
        {
            Debug.LogError("Unhandled StatType \"" + statType + "\"");
        }
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
