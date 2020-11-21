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

    /*
    [Header("Forces approach")]
    //Forces approach
    //TODO remove this if not using forces approach
    public Transform motor;
    public float Power = 5f;
    public float Drag = 0.1f;
    */

    //Private fields ===========================
    private Rigidbody rigidBody;
    //steer smoothing control
    private float steerSpeed = 0;
    private float currentSteerSpeed = 0;
    //motion smoothing control
    private float motionSpeed = 0;
    private float currentMotionSpeed = 0;


    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //default direction
        //Vector3 forceDirection = transform.forward;

        //1-Moving =============================

        //Vector3 forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward);
        //Vector3 targetVel = Vector3.zero;
        int motion = 0;
        int motionFactor = 1;
        if (Input.GetKey(KeyCode.W))
        {
            motion = 1;
            motionFactor *= motion;
            //ApplyForceToReachVelocity(rigidBody, forward * MaxSpeed, Power);
        }
        if (Input.GetKey(KeyCode.S))
        {
            motion = -1;
            motionFactor *= motion;
            //ApplyForceToReachVelocity(rigidBody, forward * -MaxSpeed, Power);

        }

        float motionTime = Mathf.Abs(motion) > 0 ? motionAccelerationTime : motionDecelerationTime;
        motionSpeed = Mathf.SmoothDamp(motionSpeed, motion * maxForwardSpeed, ref currentMotionSpeed, motionTime);
        if (Mathf.Abs(motionSpeed) > 0.05f)//moption threshold
            rigidBody.MovePosition(transform.position + transform.forward /** motion * maxForwardSpeed*/* motionSpeed * Time.fixedDeltaTime);

        /*
        //Applying forces  
        //moving forward
        bool movingForward = Vector3.Cross(transform.forward, rigidBody.velocity).y < 0;
        //move in direction
        rigidBody.velocity = Quaternion.AngleAxis(Vector3.SignedAngle(rigidBody.velocity, (movingForward ? 1f : 0f) * transform.forward, Vector3.up) * Drag, Vector3.up) * rigidBody.velocity;
        */


        //2-Steering =============================

        //float turn = Input.GetAxis("Horizontal");
        int steer = 0;
        if (Input.GetKey(KeyCode.A))
            steer = -1;
        if (Input.GetKey(KeyCode.D))
            steer = 1;

        motionFactor = (rigidBody.velocity.magnitude > 0.05f || steerOnIdle) ? motionFactor : 0;
        float steerTime = Mathf.Abs(steer) > 0 ? steerAccelerationTime : steerDecelerationTime;
        //Note: Have into account motion orientation to flip horizontal axes when going backwards
        steerSpeed = Mathf.SmoothDamp(steerSpeed, steer * motionFactor * maxSteerSpeed, ref currentSteerSpeed, steerTime);
        if (Mathf.Abs(steerSpeed) > 0.05f)//steering threshold
        {
            Vector3 m_EulerAngleVelocity = transform.up /* steer*/ * steerSpeed; //Vector3.up
            Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
            rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);
        }

        //Applying forces 
        //(way 1)
        //rigidBody.AddRelativeTorque(transform.up * SteerPower * -1 * steer);
        //(way 2)
        //Vector3 steeringForce = SteerPower * steer * transform.right;
        //rigidBody.AddForceAtPosition(steeringForce, motor.position);
    }

    /*
    public void ApplyForceToReachVelocity(Rigidbody rigidbody, Vector3 velocity, float force = 1, ForceMode mode = ForceMode.Force)
    {
        if (force == 0 || velocity.magnitude == 0)
            return;

        velocity = velocity + velocity.normalized * 0.2f * rigidbody.drag;

        //force = 1 => need 1 s to reach velocity (if mass is 1) => force can be max 1 / Time.fixedDeltaTime
        force = Mathf.Clamp(force, -rigidbody.mass / Time.fixedDeltaTime, rigidbody.mass / Time.fixedDeltaTime);

        //dot product is a projection from rhs to lhs with a length of result / lhs.magnitude https://www.youtube.com/watch?v=h0NJK4mEIJU
        if (rigidbody.velocity.magnitude == 0)
        {
            rigidbody.AddForce(velocity * force, mode);
        }
        else
        {
            var velocityProjectedToTarget = (velocity.normalized * Vector3.Dot(velocity, rigidbody.velocity) / velocity.magnitude);
            rigidbody.AddForce((velocity - velocityProjectedToTarget) * force, mode);
        }
    }
    */
}
