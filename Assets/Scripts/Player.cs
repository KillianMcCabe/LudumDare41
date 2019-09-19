using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    Camera cam;
    CharacterController controller;
    // Rigidbody _rigidbody;

    private const float MoveSpeed = 12f;
    private const float InteractionRange = 8f;
    private const float Mass = 3.0f; // defines the character mass

    Vector3 impact = Vector3.zero;
    Vector3 movement = Vector3.zero;

    public GameObject flirtText;
    public GameObject giveGiftText;

    private Turret turretInInteractionRange = null;

    public Transform giftHoldTransform;
    GameObject giftHeld = null;
    float giftPickUpRange = 5;

    Animator animator;

    // Use this for initialization
    void Start()
    {
        cam = Camera.main;
        controller = GetComponent<CharacterController>();
        // _rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // apply the impact force:
        if (impact.magnitude > 0.2)
        {
            controller.Move(impact * Time.deltaTime);
            // _rigidbody.AddForce(impact * Time.deltaTime);
        }
        else
        {
            HandleMovementInput();

            // Apply gravity
            controller.Move(Physics.gravity * Time.deltaTime);
        }

        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);

        if (giftHeld == null)
        {
            FindClosestGiftWithinRange();
        }

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
        animator.SetBool("itemHeld", giftHeld != null);
    }

    void FindClosestGiftWithinRange()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Gift"); // TODO: optimize
        foreach (GameObject obj in objs)
        {
            float dist = Vector3.Distance(transform.position, obj.transform.position);
            if (dist < giftPickUpRange)
            {
                giftHeld = obj;
                giftHeld.transform.SetParent(giftHoldTransform, false);
                giftHeld.transform.localPosition = new Vector3(0, 0, 0);
                Rigidbody giftRB = giftHeld.GetComponent<Rigidbody>();
                giftRB.useGravity = false;
                giftRB.isKinematic = false;
                giftHeld.GetComponent<Collider>().enabled = false;
                giftHeld.GetComponent<Item>().PickedUp();
                break;
            }
        }
    }

    void HandleMovementInput()
    {
        movement = Vector3.zero;

        // read input
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        // calculate movement vectors
        Vector3 verticalVector = Vector3.Cross(cam.transform.right, Vector3.up);
        Vector3 horizontalVector = (new Vector3(cam.transform.right.x, 0, cam.transform.right.z));

        movement += (verticalVector * vertical);
        movement += (horizontalVector * horizontal);
        movement = Vector3.ClampMagnitude(movement, 1);

        if (movement.magnitude > 0)
        {
            // turn in direction of movement input
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(movement), 360 * Time.deltaTime);
            if (Vector3.Dot(transform.forward, movement.normalized) > .9f)
            {
                controller.Move(movement * Time.deltaTime * MoveSpeed);
                // _rigidbody.AddForce(movement * MoveSpeed); -- too slidey
                // transform.position += movement * Time.deltaTime * MoveSpeed;
            }
        }
    }

    void HandleActionInputs()
    {
        if (turretInInteractionRange != null)
        {
            if (Input.GetButtonDown("Interact") && giftHeld != null)
            {
                turretInInteractionRange.GiveItem(giftHeld.GetComponent<Item>());
                Destroy(giftHeld.gameObject);
                giftHeld = null;
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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Vector3 dir = transform.position - other.transform.position;
            dir.y = 0;
            Debug.Log("Impact " + other.gameObject.name);
            AddImpact(dir, 40);
        }
    }
}
