using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class CarControll : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private Transform accelerationPoint;

    [Header("Suspension Settings")]
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness;
    [SerializeField] private float restLength;
    [SerializeField] private float springTravel;
    [SerializeField] private float wheelRadius;

    private int[] wheelsIsGround = new int[4];
    private bool IsGrounded = false;

    [Header("Input")]
    private float moveInput = 1;
    private float steerInput = 0;

    [Header("Car Settings")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float deceleration = 10f;

    private Vector3 currentCarLocalVelocity= Vector3.zero;
    private float carVelocityRatio = 0;
    
    //public InputManager manager;
    //public List<WheelCollider> throttleWheels;
    //public List<WheelCollider> steeringWheels;
    //public float strength = 2000f;
    //public float maxTune = 20f;
    void Start()
    {
        carRB=GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
    }
    private void GroundCheck()
    {
        int tempGroundWheels = 0;
        for(int i=0;i<wheelsIsGround.Length;i++)
        {
            tempGroundWheels += wheelsIsGround[i];
        }
        if(tempGroundWheels > 1)
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded=false;
        }
    }
    private void CalculateCarVelocity()
    {
        currentCarLocalVelocity = transform.InverseTransformDirection(carRB.velocity);
        carVelocityRatio = currentCarLocalVelocity.z / maxSpeed;
    }
    // Update is called once per frame
    public void Suspension()
    {
        //foreach(WheelCollider wheel in throttleWheels)
        //{
        //    wheel.motorTorque = strength * Time.deltaTime * manager.throttle;
        //}
        //foreach(WheelCollider wheel in steeringWheels)
        //{
        //    wheel.steerAngle = maxTune * Time.deltaTime;
        //}
        for(int i=0;i<rayPoints.Length;i++)
        {
            
            RaycastHit hit;
            float maxLength=restLength+springTravel;
            if (Physics.Raycast(rayPoints[i].position, -rayPoints[i].up,out hit, maxLength + wheelRadius, drivable))
            {
                wheelsIsGround[i] = 1;
                float currentSpringLength = hit.distance - wheelRadius;
                float springCompression = (restLength - currentSpringLength) / springTravel;
                float springVelocity = Vector3.Dot(carRB.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                float dampForce=damperStiffness*springCompression;
                float springForce= springStiffness * springCompression;
                float netForce = springForce - dampForce;
                carRB.AddForceAtPosition(netForce* rayPoints[i].up, rayPoints[i].position);
                Debug.DrawLine(rayPoints[i].position, hit.point, Color.red);
            }
            else
            {
                wheelsIsGround[i] = 0;
                Debug.DrawLine(rayPoints[i].position, rayPoints[i].position + (wheelRadius + maxLength) * -rayPoints[i].up, Color.red);
            }
        }
    }
    private void Movement()
    {
        if(IsGrounded)
        {
            Acceleration();
            Decelration();
        }
    }
    private void Acceleration()
    {
        carRB.AddForceAtPosition(acceleration * moveInput * transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }
    private void Decelration()
    {
        carRB.AddForceAtPosition(acceleration * moveInput * -transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }
}
