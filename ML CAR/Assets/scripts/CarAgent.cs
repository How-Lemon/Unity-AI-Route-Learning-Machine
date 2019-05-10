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
    
    public WheelCollider wheelBL, wheelBR, wheelFL, wheelFR;
    public Transform Root;
    public Transform RootL, RootR;
    Vector3 carStartPos;
    private Rigidbody carRigidbody;
    private float wheelTurned = 0.0f;
    private Transform origL, origR;

    private void Start()
    {
        carRigidbody = gameObject.GetComponent<Rigidbody>();
        carStartPos = gameObject.transform.position;
        carRigidbody.mass = CarWeight;
    }

    public override void CollectObservations()
    {
        AddVectorObs(gameObject.transform.position.x);
        AddVectorObs(gameObject.transform.position.z);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (brain.brainParameters.vectorActionSpaceType == SpaceType.continuous)
        {

            float forwardAction = vectorAction[1];
            float sideAction = vectorAction[0];
            if (forwardAction > 0)
            {
                PushGas(forwardAction);
                //gameObject.transform.position += new Vector3(actionX, 0, 0);
            }else if(forwardAction < 0){
                PushBrake(forwardAction);
            }else if(forwardAction == 0){
                ResetMotor();
            }

            if (sideAction != 0)
            {
                TurnWheel(sideAction);
            }else{
                ResetTurn();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("target"))
        {
            Debug.Log("get target");
            Done();
            AddReward(1f);
        }
        else if (collision.gameObject.CompareTag("wall"))
        {
            AddReward(-.01f);
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
        gameObject.transform.position = carStartPos;
    }
}
