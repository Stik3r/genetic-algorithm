using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor; // is this wheel attached to motor?
        public bool steering; // does this wheel apply steer angle?
    }

    public List<AxleInfo> axleInfos; // the information about each individual axle
    public float maxMotorTorque; // maximum torque the motor can apply to wheel
    public float maxSteeringAngle; // maximum steer angle the wheel can have
    public float maxBrakTorque;

    private bool _braking;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _braking = true;
        }
        else
        {
            _braking = false;
        }
    }
    public void FixedUpdate()
    {
        float motor;
        if (!_braking)
        {
            motor = maxMotorTorque * Input.GetAxis("Vertical");
        }
        else
        {
            motor = 0f;
        }
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

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
            if (_braking)
            {
                axleInfo.leftWheel.brakeTorque = maxBrakTorque;
                axleInfo.rightWheel.brakeTorque = maxBrakTorque;
            }
            else
            {
                axleInfo.leftWheel.brakeTorque = 0;
                axleInfo.rightWheel.brakeTorque = 0;
            }
        }
    }
}
