using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    public float shotForce = 1000;

    float heading;
    float pitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        heading = transform.rotation.eulerAngles.y;
        pitch = transform.rotation.eulerAngles.x;
    }

    private void Update()
    {
        // rotate camera
        float turn = Input.GetAxis("Mouse X");
        heading += turn;

        float tilt = Input.GetAxis("Mouse Y");
        pitch -= tilt;

        pitch = Mathf.Clamp(pitch, -90, 90);
        transform.localEulerAngles = new Vector3(pitch, heading);

        // move camera
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        forward *= Input.GetAxis("Vertical") * 0.05f;
        right *= Input.GetAxis("Horizontal") * 0.05f;

        transform.position += forward + right;

        // shoot ray bullet
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
            {
                RagdollController rdc = hit.collider.gameObject.GetComponentInParent<RagdollController>();
                
                if (rdc != null)
                    rdc.RagdollActive(true);

                if (hit.rigidbody != null)
                    hit.rigidbody.AddForceAtPosition(transform.forward * shotForce, hit.point);
            }
        }
    }
}