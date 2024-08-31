using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentBreakForce, currentMotorForce, currentVelocity;
    private bool isBreaking;
    private Rigidbody rb;

    // Settings
    [SerializeField] private float torque, maxSteerAngle, flipVelocity;
    [SerializeField] private Vector3 defaultPosition;
    [SerializeField] private bool debugState = false;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Controls
        GetInput();
        HandleMotor();
        HandleBraking();
        HandleSteering();
        UpdateWheels();
        currentVelocity = rb.velocity.sqrMagnitude;

        if (debugState)
        {
            Debug.Log("Torque Force: " + torque +
          ", Steering Angle: " + maxSteerAngle +
          ", Flip Velocity: " + flipVelocity +
          ", Current Velocity: " + currentVelocity +
          ", Current Motor Force: " + currentMotorForce +
          ", Current Break Force: " + currentBreakForce +
          ", Braking: " + isBreaking +
          ", Vertical Input: " + verticalInput +
          ", Horizontal Input: " + horizontalInput);
        }
    }

    private void GetInput()
    {
        // Reset Player Position
        if (Input.GetKey(KeyCode.R))
        {
            ResetPlayerPosition();
        }

        // Steering Input
        horizontalInput = Input.GetAxis("Horizontal");

        // Acceleration Input
        verticalInput = Input.GetAxis("Vertical");

        // isInputting
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        currentMotorForce = torque * verticalInput;

        rearLeftWheelCollider.motorTorque = rearRightWheelCollider.motorTorque = currentMotorForce;
    }

    private void HandleBraking()
    {
        currentBreakForce = isBreaking ? torque : 0f;

        frontLeftWheelCollider.brakeTorque = frontRightWheelCollider.brakeTorque = rearRightWheelCollider.brakeTorque = rearLeftWheelCollider.brakeTorque = currentBreakForce;
    }

    private void HandleSteering()
    {
        float velX = rb.velocity.x;
        float velZ = rb.velocity.z;
        Vector2 velXZ = new Vector2(velX, velZ);
        currentVelocity = velXZ.sqrMagnitude;

        if (currentSteerAngle == maxSteerAngle && currentVelocity >= flipVelocity)
        {
            ForceAppPointDistanceSetter(-2f);
        }
        else
        {
            ForceAppPointDistanceSetter(0f);
        }

        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private string ResetPlayerPosition()
    {
        GameObject player = GameObject.FindWithTag("Player");
        player.transform.position = defaultPosition;
        player.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        frontLeftWheelCollider.brakeTorque = frontRightWheelCollider.brakeTorque = rearLeftWheelCollider.brakeTorque = rearRightWheelCollider.brakeTorque = torque;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        Invoke(UnfreezePlayer(rb), 0.5f);

        return "freeze";
    }

    private string UnfreezePlayer(Rigidbody rb)
    {
        rb.constraints = RigidbodyConstraints.None;
        return "unfreeze";
    }
    private void ForceAppPointDistanceSetter(float val)
    {
        foreach (var wheel in GameObject.FindGameObjectsWithTag("WheelsCollider"))
        {
            wheel.GetComponent<WheelCollider>().forceAppPointDistance = val;
        }
    }


}