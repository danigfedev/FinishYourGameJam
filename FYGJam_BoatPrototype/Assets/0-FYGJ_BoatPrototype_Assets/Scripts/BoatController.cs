using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles boat control (motion and steering)
/// </summary>
[RequireComponent(typeof(BuoyancyHandler))] //A boat must be buoyant in order to be controlled
public class BoatController : MonoBehaviour
{
    //Idea: put all this properties ina Scriptable Object
    [Header("Boat steering")]
    public bool steerOnIdle = false;
    public float maxSteerSpeed = 50f; //Max Steer power (change name later)
    [Tooltip("Time to reach max steering speed")]
    public float steerAccelerationTime = 0.5f;//seconds
    [Tooltip("Time to decelerate from max steering speed")]
    public float steerDecelerationTime = 1f;//seconds
    
    
    [Header("Boat motion")]
    public float maxForwardSpeed = 10f;
    public float maxReverseSpeed = 5f;
    [Tooltip("Time to reach max steering speed")]
    public float motionAccelerationTime = 0.5f;//seconds
    [Tooltip("Time to decelerate from max steering speed")]
    public float motionDecelerationTime = 1f;//seconds

    public float motionDecelerationTimeOnAir = 1.5f;

    /*
    [Header("Forces approach")]
    //Forces approach
    //TODO remove this if not using forces approach
    public Transform motor;
    public float Power = 5f;
    public float Drag = 0.1f;
    */

    //Private fields ===========================
    private BuoyancyHandler buoyancyHandler;
    private Rigidbody rigidBody;
    //steer smoothing control
    private float steerSpeed = 0;
    private float currentSteerSpeed = 0;
    //motion smoothing control
    private float motionSpeed = 0;
    private float currentMotionSpeed = 0;

    private void OnValidate()
    {
        buoyancyHandler = GetComponent<BuoyancyHandler>();
        buoyancyHandler.isBoat = true;
    }

    private void Awake()
    {
        buoyancyHandler.Initialize(/*true*/);
        rigidBody = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        buoyancyHandler.isBoat = false; //In case boat controller is destroyed in Play mode
    }

    private void FixedUpdate()
    {
        HandleBoatControl();
        buoyancyHandler.HandleBuoyancy();
    }

    float timeOnAir = 0;
    int onWater = 1;
    private void HandleBoatControl()
    {
        //0-Check if floating
        if (rigidBody.drag == buoyancyHandler.airDrag)
            timeOnAir += Time.fixedDeltaTime;
        else
        {
            timeOnAir = 0;
            onWater = 1;
        }
        
        if (timeOnAir > 0.5f)//Out of water offset to start behaving like that
            onWater = 0;


        //1-Moving =============================

        int motion = 0;
        int motionFactor = 1;
        if (Input.GetKey(KeyCode.W))
        {
            motion = 1;
            motionFactor *= motion;
        }
        if (Input.GetKey(KeyCode.S))
        {
            motion = -1;
            motionFactor *= motion;
        }

        float motionTime;
        if (onWater == 0) //If on air, apply a different deceleration resistance (time)
            motionTime = motionDecelerationTimeOnAir;
        else
            motionTime = Mathf.Abs(motion) > 0 ? motionAccelerationTime : motionDecelerationTime;
        motionSpeed = Mathf.SmoothDamp(motionSpeed, onWater*motion * maxForwardSpeed, ref currentMotionSpeed, motionTime);
        if (Mathf.Abs(motionSpeed) > 0.05f)//moption threshold
        {
            //Two aproaches: a) forward direction or b) horizontal forward direction
            //a)Not bad, but a sometimes behaving extrange while jumping on air
            //rigidBody.MovePosition(transform.position + transform.forward /** motion * maxForwardSpeed*/* motionSpeed * Time.fixedDeltaTime);
            //b) Seems OK, even when jumping. Applied for now until further testing in a more polished scenario
            Vector3 horizontalForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1));
            rigidBody.MovePosition(transform.position + horizontalForward /** motion * maxForwardSpeed*/* motionSpeed * Time.fixedDeltaTime);
        }


        //2-Steering =============================

        //float turn = Input.GetAxis("Horizontal");
        int steer = 0;
        if (Input.GetKey(KeyCode.A))
            steer = -1;
        if (Input.GetKey(KeyCode.D))
            steer = 1;

        motionFactor = (Mathf.Abs(motionSpeed) > 0.05f || steerOnIdle) ? motionFactor : 0;
        float steerTime = Mathf.Abs(steer) > 0 ? steerAccelerationTime : steerDecelerationTime;
        //Note: Have into account motion orientation to flip horizontal axes when going backwards
        steerSpeed = Mathf.SmoothDamp(steerSpeed, onWater * steer * motionFactor * maxSteerSpeed, ref currentSteerSpeed, steerTime);
        if (Mathf.Abs(steerSpeed) > 0.05f)//steering threshold
        {
            Vector3 m_EulerAngleVelocity = transform.up /* steer*/ * steerSpeed; //Vector3.up
            Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
            rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
        }
    }
}
