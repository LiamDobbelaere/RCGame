using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCWCSound : MonoBehaviour {
    public AudioSource driveSound;
    public AudioSource steerSound;

    public float lowPitch = 0.9f;
    public float highPitch = 1.5f;

    private float speedToRevs = 0.01f;
    private bool isSteering = false;

    Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        float forwardSpeed = transform.InverseTransformDirection(rigidbody.velocity).z;
        float engineRevs = lowPitch + (Mathf.Abs(forwardSpeed) / 20);
        driveSound.volume = 0.1f + Mathf.Abs(forwardSpeed) / 10;
        driveSound.pitch = Mathf.Clamp(engineRevs, lowPitch, highPitch);

        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.2f && !isSteering)
        {
            isSteering = true;
            steerSound.pitch = Random.Range(0.7f, 1.1f);
            if (!steerSound.isPlaying) steerSound.Play();
        }

        if (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f && isSteering)
        {
            isSteering = false;
        }
    }
}
