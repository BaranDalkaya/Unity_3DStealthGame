using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event System.Action OnReachedEndOfLevel;

    public float moveSpeed = 20f;
    public float smoothTime = .1f;
    public float turnSpeed = 8;

    float angle;
    float smoothInputMag;
    float smoothMoveVel;
    Vector3 velocity;

    Rigidbody rigidbody;
    bool disabled;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody> ();
        GuardCode.OnGuardSpotsPlayer += Disabled;
    }

    void Update()
    {
        Vector3 inputDirection = Vector3.zero;
        if (!disabled)
        {
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMag = Mathf.SmoothDamp(smoothInputMag, inputMagnitude, ref smoothMoveVel, smoothTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, turnSpeed * Time.deltaTime * inputMagnitude);

        velocity = transform.forward * moveSpeed * smoothInputMag * Time.deltaTime;
        //transform.eulerAngles = Vector3.up * angle
        //transform.Translate(transform.forward * moveSpeed * Time.deltaTime * smoothInputMag, Space.World);
    }

    void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + velocity);
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
    }

    void Disabled()
    {
        disabled = true;
    }

    void OnDestroy()
    {
        GuardCode.OnGuardSpotsPlayer -= Disabled;  
    }

    private void OnTriggerEnter(Collider hitCollider)
    {
        if (hitCollider.tag == "Finish")
        {
            Disabled();
            if (OnReachedEndOfLevel != null)
                OnReachedEndOfLevel();
        }
    }
}
 