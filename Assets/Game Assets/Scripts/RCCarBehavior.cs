using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCCarBehavior : MonoBehaviour {
    public GameObject wheelFrontLeft;
    public GameObject wheelFrontRight;
    public GameObject wheelRearLeft;
    public GameObject wheelRearRight;

    public GameObject markerFrontLeft;
    public GameObject markerFrontRight;
    public GameObject markerlRearLeft;
    public GameObject markerRearRight;

    public float wheelAdjustSpeed = 1f;
    public float maxSpeed = 0.5f;
    public float acceleration = 0.01f;

    private GameObject[] wheels;
    private GameObject[] markers;

    private float wheelHeight;
    private float bodyHeight;

    private float speed = 0f;

    // Use this for initialization
    void Start() {
        wheels = new GameObject[] { wheelFrontLeft, wheelFrontRight, wheelRearLeft, wheelRearRight };
        markers = new GameObject[] { markerFrontLeft, markerFrontRight, markerlRearLeft, markerRearRight };
        wheelHeight = wheelFrontLeft.transform.GetComponent<Collider>().bounds.extents.y;
        bodyHeight = transform.GetComponent<Collider>().bounds.extents.y;
    }

    // Update is called once per frame
    void Update () {
        updateBodyToSlope();
        updateWheelsToSlope();
        updateMovement();
    }

    void updateWheelsToSlope()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            GameObject wheel = wheels[i];

            Vector3 castPosition = markers[i].transform.position + Vector3.up * 0.25f;
            Debug.DrawRay(castPosition, Vector3.down, Color.yellow);

            Ray raycast = new Ray(castPosition, Vector3.down);
            RaycastHit hitInfo;
            if (Physics.Raycast(raycast, out hitInfo, Mathf.Infinity, ~(1 << 8)))
            {
                Vector3 target = hitInfo.point + new Vector3(0, wheelHeight, 0);
                float step = wheelAdjustSpeed * Time.deltaTime;
                //wheel.transform.Translate(new Vector3(0, Vector3.Distance(wheel.transform.position, hitInfo.point), 0));
                wheel.transform.position = Vector3.MoveTowards(wheel.transform.position, target, step);

                if (wheel == wheelFrontLeft || wheel == wheelRearLeft)
                    wheel.transform.Rotate(new Vector3(0, 0, speed * 10000 * Time.deltaTime));
                else
                    wheel.transform.Rotate(new Vector3(0, 0, -speed * 10000 * Time.deltaTime));

                //wheel.transform.rotation = Quaternion.FromToRotation(transform.up, hitInfo.normal) * transform.rotation;
            }
        }
    }

    void updateBodyToSlope()
    {
        Vector3 castPosition = transform.position + Vector3.up * 0.25f;
        Debug.DrawRay(castPosition, Vector3.down, Color.blue);

        Ray raycast = new Ray(castPosition, Vector3.down);
        RaycastHit hitInfo;
        if (Physics.Raycast(raycast, out hitInfo, Mathf.Infinity, ~(1 << 8)))
        {
            Vector3 target = hitInfo.point + new Vector3(0, bodyHeight, 0);
            float step = wheelAdjustSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, target, step);

            Quaternion newRotation = Quaternion.FromToRotation(transform.up, hitInfo.normal) * transform.rotation;
            //transform.rotation = newRotation;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, 1f);
        }
    }

    void updateMovement()
    {
        speed += acceleration * Input.GetAxis("Vertical");
        speed = Mathf.Min(maxSpeed, speed);

        speed = speed * (1f - 0.005f);

        transform.Translate(new Vector3(speed, 0, 0));
        transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * (speed * 50f), 0));
        wheelFrontLeft.transform.localEulerAngles = new Vector3(wheelFrontLeft.transform.localEulerAngles.x, 180f + Input.GetAxis("Horizontal") * 45f, wheelFrontLeft.transform.localEulerAngles.z);
        wheelFrontRight.transform.localEulerAngles = new Vector3(wheelFrontRight.transform.localEulerAngles.x, Input.GetAxis("Horizontal") * 45f, wheelFrontRight.transform.localEulerAngles.z);

    }
}
