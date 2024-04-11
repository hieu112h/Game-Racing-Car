using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class CarControll : MonoBehaviour
{
    public InputManager manager;
    public List<WheelCollider> throttleWheels;
    public List<WheelCollider> steeringWheels;
    public float strength = 2000f;
    public float maxTune = 20f;
    void Start()
    {
        manager = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void FixUpdate()
    {
        foreach(WheelCollider wheel in throttleWheels)
        {
            wheel.motorTorque = strength * Time.deltaTime * manager.throttle;
        }
        foreach(WheelCollider wheel in steeringWheels)
        {
            wheel.steerAngle = maxTune * Time.deltaTime;
        }
    }
}
