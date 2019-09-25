using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const float MoveSpeed = 12f;
    private const float InteractionRange = 8f;
    private const float Mass = 3.0f; // defines the character mass
    private const float ThrowItemForce = 25.0f;
    private const float ThrowItemAngle = -20.0f;

    [Header ("External Components")]

    [SerializeField]
    private GameObject _giveGiftInputHint = null;

    [SerializeField]
    private GameObject _throwGiftInputHint = null;

    [SerializeField]
    private GameObject _flirtInputHint = null;

    [Header ("Sub-components")]

    [SerializeField]
    private Transform giftHoldTransform;

    private Camera _cam;
    private CharacterController _controller;
    private Collider _collider = null;

    private List<Item> _itemsOverlapping;

    private Vector3 _impact = Vector3.zero;
    private Vector3 _movement = Vector3.zero;

    private Turret _turretInInteractionRange = null;

    private Item _giftHeld = null;

    private Animator _animator;

    // Use this for initialization
    void Awake()
    {
        _cam = Camera.main;
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();

        _itemsOverlapping = new List<Item>();
    }

    // Update is called once per frame
    void Update()
    {
        // check if player is still affected by impact
        if (_impact.magnitude > 0.2)
        {
            // apply the impact force:
            _controller.Move(_impact * Time.deltaTime);
        }
        else
        {
            HandleMovementInput();

            // Apply gravity
            _controller.Move(Physics.gravity * Time.deltaTime);
        }

        // consumes the impact energy each cycle:
        _impact = Vector3.Lerp(_impact, Vector3.zero, 5 * Time.deltaTime);

        // find closest turret within interactable range
        _turretInInteractionRange = null;
        foreach (Turret t in GameController.instance.turrets)
        {
            if (t.isAlive && Vector3.Distance(transform.position, t.transform.position) < InteractionRange)
            {
                _turretInInteractionRange = t;
                break;
            }
        }

        _throwGiftInputHint.SetActive(_giftHeld);
        _flirtInputHint.SetActive(_turretInInteractionRange != null && _turretInInteractionRange.IsFlirtable && !_turretInInteractionRange.HasFullHealth);
        _giveGiftInputHint.SetActive(_turretInInteractionRange != null && _giftHeld);

        HandleActionInputs();
        _animator.SetBool("itemHeld", _giftHeld != null);
    }

    void HandleMovementInput()
    {
        _movement = Vector3.zero;

        // read input
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        // calculate movement vectors
        Vector3 verticalVector = Vector3.Cross(_cam.transform.right, Vector3.up);
        Vector3 horizontalVector = (new Vector3(_cam.transform.right.x, 0, _cam.transform.right.z));

        _movement += (verticalVector * vertical);
        _movement += (horizontalVector * horizontal);
        _movement = Vector3.ClampMagnitude(_movement, 1);

        if (_movement.magnitude > 0)
        {
            // turn in direction of movement input
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_movement), 360 * Time.deltaTime);
            if (Vector3.Dot(transform.forward, _movement.normalized) > .9f)
            {
                _controller.Move(_movement * Time.deltaTime * MoveSpeed);
            }
        }
    }

    void HandleActionInputs()
    {
        if (Input.GetButtonDown("DropItem") && _giftHeld != null)
        {
            Vector3 throwVector = Quaternion.AngleAxis(ThrowItemAngle, transform.right) * transform.forward;
            _giftHeld.Throw(throwVector * ThrowItemForce);
            _giftHeld = null;

            if (_itemsOverlapping.Count > 0)
            {
                Item item = _itemsOverlapping[0];
                Debug.Log("found " + item.Label + " at feet");
                PickedUpItem(item);
                _itemsOverlapping.RemoveAt(0);
            }
        }

        if (_turretInInteractionRange != null)
        {
            if (Input.GetButtonDown("Interact") && _giftHeld != null)
            {
                _turretInInteractionRange.GiveItem(_giftHeld);
                Destroy(_giftHeld.gameObject);
                _giftHeld = null;

                if (_itemsOverlapping.Count > 0)
                {
                    PickedUpItem(_itemsOverlapping[0]);
                    _itemsOverlapping.RemoveAt(0);
                }
            }
            if (Input.GetButtonDown("Flirt") && _turretInInteractionRange.IsFlirtable && !_turretInInteractionRange.HasFullHealth)
            {
                _turretInInteractionRange.Flirt();
            }
        }
    }

    // call this function to add an impact force:
    void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        _impact += dir.normalized * force / Mass;
    }

    private void PickedUpItem(Item item)
    {
        _giftHeld = item;
        _giftHeld.IsHeld = true;
        _giftHeld.transform.SetParent(giftHoldTransform, false);
        _giftHeld.transform.localPosition = new Vector3(0, 0, 0);
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
            if (_giftHeld == null)
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
