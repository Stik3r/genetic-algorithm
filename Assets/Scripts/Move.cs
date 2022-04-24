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
    public float motor;
    
    float steering;
    private bool _braking;
    float scaleRotate = 0;
    float scaleSpeed = 0;

    public void SetMoveValue(Directional directional, Directional rotate)
    {
        if(rotate == Directional.LEFT)
        {
            scaleRotate = scaleRotate == -1 ? scaleRotate : scaleRotate - 0.1f;
            steering = maxSteeringAngle * scaleRotate;
        }
        else
        {
            scaleRotate = scaleRotate == 1 ? scaleRotate : scaleRotate + 0.1f;
            steering = maxSteeringAngle * scaleRotate;
        }
        if(directional == Directional.FORWARD)
        {
            scaleSpeed = scaleSpeed == 1 ? scaleSpeed : scaleSpeed + 0.1f;
            motor = maxMotorTorque * 0.15f;
        }
        else
        {
            scaleSpeed = scaleSpeed == -1 ? scaleSpeed : scaleSpeed - 0.1f;
            motor = maxMotorTorque * 0.15f;
        }
    }
    public void MoveCar()
    {
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

    /*private void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
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
        }
    }*/
}
