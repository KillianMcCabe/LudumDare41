using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour {

    public GameObject follow;
	float maxDistance = 24; // TODO: scroll wheel updates this val
    Vector3 focusOffset;
    Quaternion cameraRotation;
    
    float speed = 1.5f;

	// Use this for initialization
	void Start () {
        focusOffset = new Vector3(-1, .5f, 0) * maxDistance;

        Vector3 vectorToPlayer = follow.transform.position - transform.position;
        transform.position = transform.position = follow.transform.position + (Quaternion.LookRotation(Vector3.forward) * focusOffset);
        transform.rotation = Quaternion.LookRotation(vectorToPlayer.normalized);
    }
	
	// Update is called once per frame
	void LateUpdate () {
		
		float horizontal = 0;
        float vertical = 0;
		if (Input.GetButton("Fire3")) {
			horizontal = Input.GetAxis("CameraHorizontal");
        	vertical = -Input.GetAxis("CameraVertical");
		}

        focusOffset = Quaternion.AngleAxis(horizontal * speed * Time.deltaTime, Vector3.up) * focusOffset;

        Vector3 newFocusPole = Quaternion.AngleAxis(vertical * speed * Time.deltaTime, transform.right) * focusOffset;
        if (Vector3.Dot(Vector3.up, newFocusPole.normalized) <= 0.99f && Vector3.Dot(Vector3.up, newFocusPole.normalized) >= -0.85f) {
            focusOffset = newFocusPole;
        }
        
		RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Terrain");
        if (Physics.SphereCast(follow.transform.position, 1f, focusOffset, out hit, focusOffset.magnitude, layerMask)) {
            transform.position = hit.point + new Vector3(0, 0.2f, 0);
        } else {
            transform.position = follow.transform.position + focusOffset;
        }
        transform.LookAt(follow.transform);
    }
}