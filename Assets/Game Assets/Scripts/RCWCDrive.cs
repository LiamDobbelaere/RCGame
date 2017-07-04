using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCWCDrive : MonoBehaviour {
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;

    public string inputHaxis = "HorizontalP1";
    public string inputVaxis = "VerticalP1";

    public bool isComputer = false;

    private Rigidbody rigidbody;

    /* AI vars */
    public float steeringSharpness = 12.0f;
    public float requiredMagnitude = 2.0f;

    // Here's all the variables for the AI, the waypoints are determined in the "GetWaypoints" function.
    // the waypoint container is used to search for all the waypoints in the scene, and the current
    // waypoint is used to determine which waypoint in the array the car is aiming for.
    public GameObject waypointContainer;
    private Transform[] waypoints;
    private int currentWaypoint = 0;

    // input steer and input torque are the values substituted out for the player input. The 
    // "NavigateTowardsWaypoint" function determines values to use for these variables to move the car
    // in the desired direction.
    private float inputSteer = 0.0f;
    private float inputTorque = 0.0f;

    public float speedMultiplier = 10000f;

    public void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        rigidbody.centerOfMass = new Vector3(rigidbody.centerOfMass.x, rigidbody.centerOfMass.y - rigidbody.centerOfMass.y * 0.1f, rigidbody.centerOfMass.z);

        if (isComputer) GetWaypoints();
    }

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;

        //if (collider.gameObject.name.Equals("WheelFR") || collider.gameObject.name.Equals("WheelRR"))
        //    rotation.eulerAngles = new Vector3(-rotation.eulerAngles.x, rotation.eulerAngles.y + 180f, rotation.eulerAngles.z);

        visualWheel.transform.rotation = rotation;
    }

    public void FixedUpdate()
    {
        if (isComputer)
        {
            NavigateTowardsWaypoint();

            foreach (AxleInfo axleInfo in axleInfos)
            {
                if (axleInfo.steering)
                {
                    axleInfo.leftWheel.steerAngle = (steeringSharpness) * inputSteer;
                    axleInfo.rightWheel.steerAngle = (steeringSharpness) * inputSteer;
                }

                if (axleInfo.motor)
                {
                    axleInfo.leftWheel.motorTorque = inputTorque * speedMultiplier;
                    axleInfo.rightWheel.motorTorque = inputTorque * speedMultiplier;
                }

                ApplyLocalPositionToVisuals(axleInfo.leftWheel);
                ApplyLocalPositionToVisuals(axleInfo.rightWheel);
            }
        } else {
            float motor = maxMotorTorque * Input.GetAxis(inputVaxis);
            float steering = maxSteeringAngle * Input.GetAxis(inputHaxis);

            foreach (AxleInfo axleInfo in axleInfos)
            {
                if (axleInfo.steering)
                {
                    axleInfo.leftWheel.steerAngle = steering;
                    axleInfo.rightWheel.steerAngle = steering;
                }
                if (axleInfo.motor)
                {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;
                }
                ApplyLocalPositionToVisuals(axleInfo.leftWheel);
                ApplyLocalPositionToVisuals(axleInfo.rightWheel);
            }
        }
    }

    /* AI SUBROUTINES */
    void GetWaypoints()
    {
        // Now, this function basically takes the container object for the waypoints, then finds all of the transforms in it,
        // once it has the transforms, it checks to make sure it's not the container, and adds them to the array of waypoints.
        Transform[] potentialWaypoints = waypointContainer.GetComponentsInChildren<Transform>();

        int actCount = 0;

        foreach (Transform potentialWaypoint in potentialWaypoints)
        {
            if (potentialWaypoint != waypointContainer.transform)
            {
                actCount++;
            }
        }

        Transform[] newSet = new Transform[actCount];

        actCount = 0;

        foreach (Transform potentialWaypoint in potentialWaypoints)
        {
            if (potentialWaypoint != waypointContainer.transform)
            {
                newSet[actCount] = potentialWaypoint;
                actCount++;
            }
        }

        waypoints = newSet;
    }

    void NavigateTowardsWaypoint()
    {
        // now we just find the relative position of the waypoint from the car transform,
        // that way we can determine how far to the left and right the waypoint is.
        Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(new Vector3(
                                                    waypoints[currentWaypoint].position.x,
                                                    transform.position.y,
                                                    waypoints[currentWaypoint].position.z));


        // by dividing the horizontal position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
        inputSteer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;

        // now we do the same for torque, but make sure that it doesn't apply any engine torque when going around a sharp turn...
        if (Mathf.Abs(inputSteer) < 0.5)
        {
            inputTorque = RelativeWaypointPosition.z / RelativeWaypointPosition.magnitude - Mathf.Abs(inputSteer);
        }
        else
        {
            inputTorque = 0.0f;
        }

        // this just checks if the car's position is near enough to a waypoint to count as passing it, if it is, then change the target waypoint to the
        // next in the list.
        Debug.LogWarning(RelativeWaypointPosition.magnitude);
        if (RelativeWaypointPosition.magnitude < requiredMagnitude)
        {
            currentWaypoint++;

            if (currentWaypoint >= waypoints.Length)
            {
                currentWaypoint = 0;
            }
        }

        Debug.Log(currentWaypoint);
    }

    /*Audio*/

    public AudioSource driveSound;
    public AudioSource steerSound;

    public float lowPitch = 0.9f;
    public float highPitch = 1.5f;

    private float speedToRevs = 0.01f;
    private bool isSteering = false;

    // Update is called once per frame
    void Update()
    {
        float forwardSpeed = transform.InverseTransformDirection(rigidbody.velocity).z;
        float engineRevs = lowPitch + (Mathf.Abs(forwardSpeed) / 20);
        driveSound.volume = 0.1f + Mathf.Abs(forwardSpeed) / 10;
        driveSound.pitch = Mathf.Clamp(engineRevs, lowPitch, highPitch);

        if (Mathf.Abs(Input.GetAxis(inputHaxis)) > 0.2f && !isSteering)
        {
            isSteering = true;
            steerSound.pitch = Random.Range(0.7f, 1.1f);
            if (!steerSound.isPlaying) steerSound.Play();
        }

        if (Mathf.Abs(Input.GetAxis(inputHaxis)) < 0.1f && isSteering)
        {
            isSteering = false;
        }
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}
