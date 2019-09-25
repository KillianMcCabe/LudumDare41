using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Camera _cam;
    private CharacterController _controller;
    private Collider _collider = null;

    private const float MoveSpeed = 12f;
    private const float InteractionRange = 8f;
    private const float Mass = 3.0f; // defines the character mass
    private const float ThrowItemForce = 24.0f;

    private List<Item> itemsOverlapping;

    private Vector3 impact = Vector3.zero;
    private Vector3 movement = Vector3.zero;

    public GameObject flirtText;
    public GameObject giveGiftText;
    public Transform giftHoldTransform;

    private Turret turretInInteractionRange = null;

    private Item giftHeld = null;
    private float giftPickUpRange = 5;

    Animator _animator;

    // Use this for initialization
    void Awake()
    {
        _cam = Camera.main;
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();

        itemsOverlapping = new List<Item>();
    }

    // Update is called once per frame
    void Update()
    {
        // apply the impact force:
        if (impact.magnitude > 0.2)
        {
            _controller.Move(impact * Time.deltaTime);
        }
        else
        {
            HandleMovementInput();

            // Apply gravity
            _controller.Move(Physics.gravity * Time.deltaTime);
        }

        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);

        flirtText.SetActive(false);
        giveGiftText.SetActive(false);
        turretInInteractionRange = null;
        foreach (Turret t in GameController.instance.turrets)
        {
            if (t.isAlive && Vector3.Distance(transform.position, t.transform.position) < InteractionRange)
            {
                turretInInteractionRange = t;
                if (t.isFlirtable)
                {
                    flirtText.SetActive(true);
                }
                if (giftHeld != null)
                {
                    giveGiftText.SetActive(true);
                }
                break;
            }
        }

        HandleActionInputs();
        _animator.SetBool("itemHeld", giftHeld != null);

        Debug.Log("" + itemsOverlapping.Count);
    }

    void HandleMovementInput()
    {
        movement = Vector3.zero;

        // read input
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        // calculate movement vectors
        Vector3 verticalVector = Vector3.Cross(_cam.transform.right, Vector3.up);
        Vector3 horizontalVector = (new Vector3(_cam.transform.right.x, 0, _cam.transform.right.z));

        movement += (verticalVector * vertical);
        movement += (horizontalVector * horizontal);
        movement = Vector3.ClampMagnitude(movement, 1);

        if (movement.magnitude > 0)
        {
            // turn in direction of movement input
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(movement), 360 * Time.deltaTime);
            if (Vector3.Dot(transform.forward, movement.normalized) > .9f)
            {
                _controller.Move(movement * Time.deltaTime * MoveSpeed);
            }
        }
    }

    void HandleActionInputs()
    {
        if (Input.GetButtonDown("DropItem") && giftHeld != null)
        {
            float throwAngle = -25f;
            Vector3 throwVector = Quaternion.AngleAxis(throwAngle, transform.right) * transform.forward;
            giftHeld.Throw(throwVector * ThrowItemForce);
            giftHeld = null;

            if (itemsOverlapping.Count > 0)
            {
                PickedUpItem(itemsOverlapping[0]);
                itemsOverlapping.RemoveAt(0);
            }
        }

        if (turretInInteractionRange != null)
        {
            if (Input.GetButtonDown("Interact") && giftHeld != null)
            {
                turretInInteractionRange.GiveItem(giftHeld);
                Destroy(giftHeld.gameObject);
                giftHeld = null;

                if (itemsOverlapping.Count > 0)
                {
                    PickedUpItem(itemsOverlapping[0]);
                    itemsOverlapping.RemoveAt(0);
                }
            }
            if (Input.GetButtonDown("Flirt") && turretInInteractionRange.isFlirtable)
            {
                turretInInteractionRange.Flirt();
            }
        }
    }

    // call this function to add an impact force:
    void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / Mass;
    }

    private void PickedUpItem(Item item)
    {
        giftHeld = item;
        giftHeld.IsHeld = true;
        giftHeld.transform.SetParent(giftHoldTransform, false);
        giftHeld.transform.localPosition = new Vector3(0, 0, 0);
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
            if (giftHeld == null)
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
                itemsOverlapping.Add(item);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Gift")
        {
            Item item = other.gameObject.GetComponent<Item>();
            itemsOverlapping.Remove(item);
        }
    }
}
