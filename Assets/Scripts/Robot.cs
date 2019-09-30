using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Robot : MonoBehaviour
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
    private NavMeshAgent _agent = null;

    private List<Item> _itemsOverlapping;

    private Vector3 _impact = Vector3.zero;
    private Vector3 _movement = Vector3.zero;

    public Turret TurretInInteractionRange {get; private set;} = null;
    public Item GiftHeld  {get; private set;} = null;

    private Animator _animator;

    // Use this for initialization
    void Awake()
    {
        _cam = Camera.main;
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
        _agent = GetComponent<NavMeshAgent>();

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
        foreach (Turret t in GameController.instance.turrets)
        {
            if (t.isAlive && Vector3.Distance(transform.position, t.transform.position) < InteractionRange)
            {
                TurretInInteractionRange = t;
                break;
            }
        }

        HandleActionInputs();
        _animator.SetBool("itemHeld", GiftHeld != null);
    }

    public void MoveTo(Vector3 position)
    {
        if (_agent != null)
        {
            _agent.SetDestination(position);
        }
    }

    void HandleActionInputs()
    {
        if (Input.GetButtonDown("DropItem") && GiftHeld != null)
        {
            Vector3 throwVector = Quaternion.AngleAxis(ThrowItemAngle, transform.right) * transform.forward;
            GiftHeld.Throw(throwVector * ThrowItemForce);
            GiftHeld = null;

            if (_itemsOverlapping.Count > 0)
            {
                Item item = _itemsOverlapping[0];
                Debug.Log("found " + item.Label + " at feet");
                PickedUpItem(item);
                _itemsOverlapping.RemoveAt(0);
            }
        }

        if (TurretInInteractionRange != null)
        {
            if (Input.GetButtonDown("Interact") && GiftHeld != null)
            {
                TurretInInteractionRange.GiveItem(GiftHeld);
                Destroy(GiftHeld.gameObject);
                GiftHeld = null;

                if (_itemsOverlapping.Count > 0)
                {
                    PickedUpItem(_itemsOverlapping[0]);
                    _itemsOverlapping.RemoveAt(0);
                }
            }
            if (Input.GetButtonDown("Flirt") && TurretInInteractionRange.IsFlirtable && !TurretInInteractionRange.HasFullHealth)
            {
                TurretInInteractionRange.Flirt();
            }
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
        GiftHeld = item;
        GiftHeld.IsHeld = true;
        GiftHeld.transform.SetParent(giftHoldTransform, false);
        GiftHeld.transform.localPosition = new Vector3(0, 0, 0);
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
            if (GiftHeld == null)
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
