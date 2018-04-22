using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	Camera cam;
	CharacterController controller;

	float moveSpeed = 10f;
    float interactionRange = 8f;
	Vector3 movement = Vector3.zero;

    public GameObject flirtText;
    public GameObject giveGiftText;

    private Turret turretInInteractionRange = null;

    public Transform giftHoldTransform;
    GameObject giftHeld = null;
    float giftPickUpRange = 5;

    Animator animator;

    // Use this for initialization
    void Start () {
        cam = Camera.main;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {

        HandleMovementInput();

        if (giftHeld == null) {
            FindClosestGiftWithinRange();
        }

        flirtText.SetActive(false);
        giveGiftText.SetActive(false);
        turretInInteractionRange = null;
        foreach (Turret t in GameController.instance.turrets) {
            if (t.isAlive && Vector3.Distance(transform.position, t.transform.position) < interactionRange) {
                turretInInteractionRange = t;
                if (t.isFlirtable) {
                    flirtText.SetActive(true);
                }
                if (giftHeld != null) {
                    giveGiftText.SetActive(true);
                }
                break;
            }
        }

		HandleActionInputs();
        animator.SetBool("itemHeld", giftHeld != null);
    }

    void FindClosestGiftWithinRange() {
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

        if (movement.magnitude > 0) {
            // turn in direction of movement input
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(movement), 360 * Time.deltaTime);
            if (Vector3.Dot(transform.forward, movement.normalized) > .9f) {
                controller.Move(movement * Time.deltaTime * moveSpeed);
            }
        }
    }

    void HandleActionInputs()
    {
        if (turretInInteractionRange != null)
        {
            if (Input.GetButtonDown("Interact") && giftHeld != null) {
                turretInInteractionRange.GiveItem(giftHeld.GetComponent<Item>());
                Destroy(giftHeld.gameObject);
                giftHeld = null;
            }
			if (Input.GetButtonDown("Flirt") && turretInInteractionRange.isFlirtable) {
                turretInInteractionRange.Flirt();
            }
        }
    }
}
