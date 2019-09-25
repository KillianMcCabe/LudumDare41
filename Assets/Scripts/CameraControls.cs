using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour
{
    [SerializeField]
    private GameObject follow = null;

    float maxDistance = 24; // TODO: make scroll wheel update this val
    Vector3 focusOffset;
    Quaternion cameraRotation;

    // Use this for initialization
    void Start()
    {
        focusOffset = new Vector3(-1, .5f, 0) * maxDistance;

        Vector3 vectorToPlayer = follow.transform.position - transform.position;
        transform.position = transform.position = follow.transform.position + (Quaternion.LookRotation(Vector3.forward) * focusOffset);
        transform.rotation = Quaternion.LookRotation(vectorToPlayer.normalized);
    }

    private float xVelocity = 0f;
    private float yVelocity = 0f;
    private float smoothTime = 0.2f;
    private float horizontal = 0;
    private float vertical = 0;

    // Update is called once per frame
    void LateUpdate()
    {
        if (GameController.instance.GamePaused)
        {
            return;
        }

        float horizontalInput = 0;
        float verticalInput = 0;

        horizontalInput = Input.GetAxis("CameraHorizontal");
        verticalInput = -Input.GetAxis("CameraVertical");

        horizontal = Mathf.SmoothDamp(0, horizontalInput, ref xVelocity, smoothTime);
        vertical = Mathf.SmoothDamp(0, verticalInput, ref yVelocity, smoothTime);

        focusOffset = Quaternion.AngleAxis(horizontal * Settings.MouseSpeed * Time.deltaTime, Vector3.up) * focusOffset;

        Vector3 newFocusOffset = Quaternion.AngleAxis(vertical * Settings.MouseSpeed * Time.deltaTime, transform.right) * focusOffset;
        if (Vector3.Dot(Vector3.up, newFocusOffset.normalized) <= 0.99f && Vector3.Dot(Vector3.up, newFocusOffset.normalized) >= -0.85f)
        {
            focusOffset = newFocusOffset;
        }

        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("Terrain");
        if (Physics.SphereCast(follow.transform.position, 1f, focusOffset, out hit, focusOffset.magnitude, layerMask))
        {
            transform.position = hit.point + new Vector3(0, 0.2f, 0);
            // transform.position = Vector3.MoveTowards(transform.position, hit.point + new Vector3(0, 0.2f, 0), 40 * Time.deltaTime);
        }
        else
        {
            transform.position = follow.transform.position + focusOffset;
            // transform.position = Vector3.MoveTowards(transform.position, follow.transform.position + focusOffset, 40 * Time.deltaTime);
        }
        transform.LookAt(follow.transform);
    }
}