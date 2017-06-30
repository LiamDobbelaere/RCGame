using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCCarAI : MonoBehaviour {
    // These variables allow the script to power the wheels of the car.
    public WheelCollider FrontLeftWheel;
    public WheelCollider FrontRightWheel;
    public WheelCollider RearLeftWheel;
    public WheelCollider RearRightWheel;

    public float vehicleCenterOfMass = 0.0f;
    public float steeringSharpness = 12.0f;
    public float requiredMagnitude = 1.0f;

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

    public float speedMultiplier = 300f;

	// Use this for initialization
	void Start () {
        GetWaypoints();
    }

    // Update is called once per frame
    void FixedUpdate () {
        NavigateTowardsWaypoint();

        // finally, apply the values to the wheels.	The torque applied is divided by the current gear, and
        // multiplied by the calculated AI input variable.
        FrontLeftWheel.motorTorque = inputTorque * speedMultiplier;
        FrontRightWheel.motorTorque = inputTorque * speedMultiplier;

        // the steer angle is an arbitrary value multiplied by the calculated AI input.
        FrontLeftWheel.steerAngle = (steeringSharpness) * inputSteer;
        FrontRightWheel.steerAngle = (steeringSharpness) * inputSteer;

        ApplyLocalPositionToVisuals(FrontLeftWheel);
        ApplyLocalPositionToVisuals(RearLeftWheel);

        ApplyLocalPositionToVisuals(FrontRightWheel);
        ApplyLocalPositionToVisuals(RearRightWheel);
    }

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
}
