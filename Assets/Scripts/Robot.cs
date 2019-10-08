using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : Mob
{
    private const float MoveSpeed = 12f;
    private const float InteractionRange = 8f;
    private const float Mass = 3.0f; // defines the character mass
    private const float ThrowItemForce = 25.0f;
    private const float ThrowItemAngle = -20.0f;

    [Header ("Sub-components")]

    [SerializeField]
    private Transform giftHoldTransform = null;

    private Camera _cam = null;
    private Collider _collider = null;

    private List<Item> _itemsOverlapping;

    private Vector3 _impact = Vector3.zero;
    private Vector3 _movement = Vector3.zero;

    public Turret TurretInInteractionRange {get; private set;} = null;
    public Item HoldingItem {get; private set;} = null;

    private Animator _animator;

    // Use this for initialization
    public override void Awake()
    {
        base.Awake();

        _cam = Camera.main;
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();

        _itemsOverlapping = new List<Item>();
    }

    // Update is called once per frame
    void Update()
    {
        // check if player is still affected by impact
        // if (_impact.magnitude > 0.2)
        // {
        //     // apply the impact force:
        //     _controller.Move(_impact * Time.deltaTime);
        // }

        // consumes the impact energy each cycle:
        // _impact = Vector3.Lerp(_impact, Vector3.zero, 5 * Time.deltaTime);

        // find closest turret within interactable range
        TurretInInteractionRange = null;
        foreach (Turret t in GameController.Instance.Turrets)
        {
            if (t.isAlive && Vector3.Distance(transform.position, t.transform.position) < InteractionRange)
            {
                TurretInInteractionRange = t;
                break;
            }
        }

        _animator.SetBool("itemHeld", HoldingItem != null);
    }

    public void GiveGift()
    {
        if (TurretInInteractionRange == null)
        {
            Debug.LogWarning("TurretInInteractionRange is null!");
            return;
        }
        if (HoldingItem == null)
        {
            Debug.LogWarning("HoldingItem is null!");
            return;
        }

        TurretInInteractionRange.GiveItem(HoldingItem);
        Destroy(HoldingItem.gameObject);
        HoldingItem = null;

        CheckForItemUnderFeet();
    }

    public void ThrowCurrentlyHeldItem()
    {
        Vector3 throwVector = Quaternion.AngleAxis(ThrowItemAngle, transform.right) * transform.forward;
        HoldingItem.Throw(throwVector * ThrowItemForce);
        HoldingItem = null;

        CheckForItemUnderFeet();
    }

    /// <summary>
    /// Pick up an item if currently overlapping an item
    /// </summary>
    private void CheckForItemUnderFeet()
    {
        if (_itemsOverlapping.Count > 0)
        {
            PickedUpItem(_itemsOverlapping[0]);
            _itemsOverlapping.RemoveAt(0);
        }
    }

    // call this function to add an impact force:
    void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        // _impact += dir.normalized * force / Mass;
        // TODO: reimplement impact
    }

    private void PickedUpItem(Item item)
    {
        HoldingItem = item;
        HoldingItem.IsHeld = true;
        HoldingItem.transform.SetParent(giftHoldTransform, false);
        HoldingItem.transform.localPosition = new Vector3(0, 0, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Vector3 dir = transform.position - other.transform.position;
            dir.y = 0;
            AddImpact(dir, 40);
        }
        else if (other.gameObject.tag == "Gift")
        {
            if (HoldingItem == null)
            {
                Item item = other.gameObject.GetComponent<Item>();
                if (item.CanBePickedUp)
                {
                    PickedUpItem(item);
                }
            }
            else
            {
                Item item = other.gameObject.GetComponent<Item>();
                _itemsOverlapping.Add(item);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Gift")
        {
            Item item = other.gameObject.GetComponent<Item>();
            _itemsOverlapping.Remove(item);
        }
    }
}
