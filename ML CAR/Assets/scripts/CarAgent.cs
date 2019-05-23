using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAgent : Agent
{

    [Header("Car Agent Settings")]
    public GameObject target;
    public float CarWeight = 100;
    public float EngineForce = 1f;
    public float BreakForce = 1f;
    public float MaxSpeed = 2.5f;
    public float TurnSpeed = 1f;
    public float forwardInput{
        get{
            return forwardAction;
        }
    }

    public float sideInput{
        get{
            return sideAction;
        }
    }
    
    public WheelCollider wheelBL, wheelBR, wheelFL, wheelFR;
    public Transform Root;
    public Transform RootL, RootR;

    public GameObject rangeFinderContainer;
    
    [HideInInspector]
    public int checkPointPassed = 0;

    Vector3 carStartPos;
    private Rigidbody carRigidbody;
    private float wheelTurned = 0.0f;
    private Transform origL, origR;
    private float forwardAction,sideAction;
    private CarController cc;

    private RayPerception3D rayPerception;
    private RangeFinder[] rangeFinders;
    private WheelCollider[] wcs;

    private void Start()
    {
        rangeFinders = rangeFinderContainer.GetComponentsInChildren<RangeFinder>();
        
        cc = GetComponent<CarController>();
        wcs = new WheelCollider[]{wheelBL, wheelBR, wheelFL, wheelFR};
        carRigidbody = gameObject.GetComponent<Rigidbody>();
        carStartPos = gameObject.transform.position;
        carRigidbody.mass = CarWeight;
        rayPerception = GetComponent<RayPerception3D>();
    }

    public override void CollectObservations()
    {
        //Rangefinder detected Ranges
        foreach (RangeFinder item in rangeFinders)// 7 items
        {
            AddVectorObs(item.range);
        }
        //Car Speed
        AddVectorObs(cc.carSpeed);
        //Wheel Slip
        foreach (WheelCollider wheel in wcs) // 8 items
        {
            WheelHit hit;
            wheel.GetGroundHit(out hit);
            AddVectorObs(hit.forwardSlip);
            AddVectorObs(hit.sidewaysSlip);
        }
        // Add velocity observation
        Vector3 localVelocity = transform.InverseTransformDirection(carRigidbody.velocity);
        AddVectorObs(localVelocity.x); // 1 item
        AddVectorObs(localVelocity.z); // 1 item
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (brain.brainParameters.vectorActionSpaceType == SpaceType.continuous)
        {

            forwardAction = vectorAction[0];
            sideAction = vectorAction[1];
            if (forwardAction == 1)
            {
                cc.PushGas(forwardAction);
                //PushGas(forwardAction);
                //gameObject.transform.position += new Vector3(actionX, 0, 0);
            }else if(forwardAction == 2){

                cc.PushBreak(-1);
                //PushBrake(forwardAction);
            }else if(forwardAction == 0){
                ResetMotor();
            }

            if (sideAction != 0)
            {
                if (sideAction == 1)
                {
                    TurnWheel(sideAction);
                }
                else
                {
                    TurnWheel(-1);
                }
                
            }
            else {
                ResetTurn();
            }
        }

        if (GetCumulativeReward() <= -5f)
        {
            Done();
            Reset();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("checkPoint"))
        {
            Debug.Log("go through check point");
            Debug.Log(GetCumulativeReward());
            AddReward(.5f);
        }
        else if (collision.gameObject.CompareTag("wall"))
        {
            AddReward(-.01f);
        }
        else
        {
            AddReward(-.001f);
        }
    }

    private void PushGas()
    {
       
        PushGas(1f);
    }

    private void PushGas(float force)
    {
        Debug.Log("VROOM");
        //carRigidbody.AddRelativeForce(Vector3.forward * EngineForce * force * 100);
        wheelBL.motorTorque = EngineForce * force;
        wheelBR.motorTorque = EngineForce * force;
        wheelBL.brakeTorque = 0;
        wheelBR.brakeTorque = 0;
    }

    private void PushBrake(float force)
    {
        Debug.Log("WEEZE");
        wheelBL.motorTorque = 0;
        wheelBR.motorTorque = 0;
        wheelBL.brakeTorque = BreakForce * force * -100;
        wheelBR.brakeTorque = BreakForce * force * -100;
        wheelFL.brakeTorque = BreakForce * force * -100;
        wheelFR.brakeTorque = BreakForce * force * -100;
    }
    public void Reset(){
        PushBrake(1);
        ResetMotor();
        ResetTurn();
        carRigidbody.velocity = Vector3.zero;
        AgentReset();
    }

    private void ResetMotor(){
        wheelBL.brakeTorque = 0;
        wheelBR.brakeTorque = 0;
        wheelFL.brakeTorque = 0;
        wheelFR.brakeTorque = 0;
        wheelBL.motorTorque = 0;
        wheelBR.motorTorque = 0;
       
    }
    private void ResetTurn(){
        wheelFL.steerAngle = 0;
        wheelFR.steerAngle = 0;
        RootL.transform.eulerAngles = new Vector3(RootL.parent.transform.eulerAngles.x,RootL.parent.transform.eulerAngles.y,RootL.parent.transform.eulerAngles.z);
        RootR.transform.eulerAngles = new Vector3(RootL.parent.transform.eulerAngles.x,RootL.parent.transform.eulerAngles.y,RootL.parent.transform.eulerAngles.z);
    }

    private void TurnWheel(float force){
        wheelFL.steerAngle = TurnSpeed * force;
        wheelFR.steerAngle = TurnSpeed * force;
        RootL.transform.eulerAngles = new Vector3(RootL.parent.transform.eulerAngles.x,RootL.parent.transform.eulerAngles.y + TurnSpeed * force,RootL.parent.transform.eulerAngles.z);
        RootR.transform.eulerAngles = new Vector3(RootL.parent.transform.eulerAngles.x,RootL.parent.transform.eulerAngles.y+TurnSpeed * force,RootL.parent.transform.eulerAngles.z);

    }

     public override void AgentReset()
    {
        //gameObject.transform.position = carStartPos;
    }
}
