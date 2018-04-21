using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	Camera cam;
	CharacterController controller;

	float moveSpeed = 10f;
    float flirtRange = 8f;
	Vector3 movement = Vector3.zero;

    public GameObject interactionText;

    private Turret turretInFlirtRange = null;

    // Use this for initialization
    void Start () {
        cam = Camera.main;
        controller = GetComponent<CharacterController>();
    }
	
	// Update is called once per frame
	void Update () {
        HandleMovementInput();

        interactionText.SetActive(false);
        turretInFlirtRange = null;
        foreach (Turret t in GameController.instance.turrets) {
            if (t.isFlirtable && Vector3.Distance(transform.position, t.transform.position) < flirtRange) {
                turretInFlirtRange = t;
                interactionText.SetActive(true);
                break;
            }
        }

		HandleActionInputs();
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
        if (turretInFlirtRange != null && Input.GetButtonDown("Interact"))
        {
			turretInFlirtRange.Flirt();
        }
    }
}
