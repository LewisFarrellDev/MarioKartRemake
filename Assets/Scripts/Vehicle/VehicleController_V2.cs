// Script Created By Lewis Farrell - S15118289 //

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class VehicleController_V2 : NetworkBehaviour
{
    public GameObject[] raycastPoints;
    public float hoverHeight;
    public float hoverForce;
    public float minGravity;
    public float maxGravity;

    public float speed;
    public float handling;
    public float acceleration;
    public float airControl;

    Rigidbody rb;
    int layerMask;

    bool isControllable;
    bool isGrounded;
    float verticalFloat;
    float horizontalFloat;
    float rbMaxVelocity;
    float gravity;

    // For audio
    public AudioSource audioSource;
    public AudioSource collisionAudioSource;
    public AudioClip collisionSound;

    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Vehicle");
        layerMask = ~layerMask;

        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
            audioSource.pitch = 0.2f;

        rbMaxVelocity = (((speed + 100) / rb.drag) - (Time.fixedDeltaTime * (speed + 100))) / rb.mass;

        SetControllable(false);
        if (NetworkClient.active)
            RaceManager.EventStartRace += OnRaceStart;

    }

    void Reset()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = 1;
        rb.drag = 1;
        rb.angularDrag = 20;

        hoverHeight = 2;
        hoverForce = 400;
        minGravity = 25;
        maxGravity = 30;
        speed = 50;
        handling = 1f;
        acceleration = 0.5f;

        gravity = minGravity;
    }

    void Update()
    {
        if (hasAuthority)
        {
            float forwardVelocityNormalized = 0;
            // Get the forward velocity and multiply the horizonal axis by it. This prevents turning when the vehicle is moving forward
            float forwardVelocity = Vector3.Dot(rb.velocity, rb.transform.forward);
            forwardVelocityNormalized = forwardVelocity / rbMaxVelocity;


            // Change pitch of audio based on car speed
            if (audioSource != null)
            {
                float pitch = 3 * forwardVelocityNormalized;
                audioSource.pitch = Mathf.Clamp(pitch, 0.2f, 3);
            }

            if (isControllable)
            {
                if (isGrounded)
                {
                    horizontalFloat = Input.GetAxis("Horizontal");
                    verticalFloat = Mathf.Lerp(verticalFloat, Input.GetAxis("Vertical"), acceleration * Time.deltaTime);
                    horizontalFloat *= Mathf.Clamp(forwardVelocityNormalized * handling, -1, 1);
                }
                else
                    horizontalFloat = Input.GetAxis("Horizontal") * airControl;
            }
        }
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        float raycastLength = hoverHeight + 0.5f;

        // Check is grounded
        // Debug ray
        Debug.DrawRay(transform.position, -transform.up * (raycastLength + 1));
        if (Physics.Raycast(transform.position, -transform.up, out hit, raycastLength + 1, layerMask))
        {
            gravity = minGravity;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            gravity = Mathf.Lerp(gravity, maxGravity, 1 * Time.deltaTime);

            // Rotate car to upright position if it is not grounded
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 5 * Time.deltaTime);
        }

        // Iterate through "thrusters"
        foreach (GameObject raycastPoint in raycastPoints)
        {
            Debug.DrawRay(raycastPoint.transform.position, -raycastPoint.transform.up * raycastLength);
            if (Physics.Raycast(raycastPoint.transform.position, -raycastPoint.transform.up, out hit, raycastLength, layerMask))
            {
                // Apply force with an increasing scale factor depending on how close to the ground the ray was
                float force = hoverForce * (1.0f - (hit.distance / hoverHeight));
                rb.AddForceAtPosition(raycastPoint.transform.up * force, raycastPoint.transform.position);
            }
            else
            {
                // Pull the car back down if not touching anything
                rb.AddForceAtPosition(-raycastPoint.transform.up * gravity, raycastPoint.transform.position);
            }
        }

        // Rotate Car
        float angle = handling * horizontalFloat / 2;
        transform.Rotate(0, angle, 0);

        // Get true forward of camera
        Vector3 forwardDirection;
        GameObject camPos = transform.Find("CamPosition").gameObject;
        forwardDirection = -Vector3.Cross(transform.up, Camera.main.transform.right);

        // Move Forward
        rb.AddForce(forwardDirection * (speed * verticalFloat), ForceMode.Acceleration);

        // maintain direction only if grounded
        // otherwise fly in direction of velocity when not on the ground
        if (isGrounded)
            rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity.magnitude * forwardDirection, 1 * Time.deltaTime);
    }

    void OnRaceStart()
    {
        SetControllable(true);
    }

    public void SetControllable(bool controllable)
    {
        isControllable = controllable;

        if (isControllable == false)
        {
            verticalFloat = 0;
            horizontalFloat = 0;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            // Bounce the car away from the wall, helps to prevent the car from getting stuck
            rb.AddForce(contact.normal * 5, ForceMode.Impulse);
            StartCoroutine(playSFX(collisionSound));
        }
    }

    IEnumerator playSFX(AudioClip audioClip)
    {
        collisionAudioSource.clip = audioClip;
        collisionAudioSource.Play();
        yield return new WaitForSeconds(collisionAudioSource.clip.length);
    }

    public IEnumerator BoostSpeed(float speedIncrease, float Duration)
    {
        speed += speedIncrease;
        hoverForce *= 3;

        yield return new WaitForSeconds(Duration);
        speed -= speedIncrease;
        hoverForce /= 3;
    }

    public IEnumerator DisableVehicle(float duration)
    {
        SetControllable(false);

        Quaternion rotation = transform.rotation;
        float currentTime = Time.time;

        while (true)
        {
            float angle = 360 * Time.deltaTime;
            transform.Rotate(0, angle, 0);
            float currentTimeNew = Time.time;
            if (currentTimeNew >= (currentTime + duration))
                break;
            yield return null;
        }

        yield return new WaitForSeconds(0);
        transform.rotation = rotation;
        SetControllable(true);
    }

    public IEnumerator SlowSpeed(float speedDecrease, float duration)
    {
        speed -= speedDecrease;
        yield return new WaitForSeconds(duration);
        speed += speedDecrease;
    }
}
