using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testAgent : Agent {
    public GameObject target;
    Vector3 ballStartPos;

    private void Start()
    {
        ballStartPos = gameObject.transform.position;
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
            var actionX = 0.1f * Mathf.Clamp(vectorAction[0], -1f, 1f);
            var actionZ = 0.1f * Mathf.Clamp(vectorAction[1], -1f, 1f);

            var position_x = gameObject.transform.position.x + actionX;
            var position_z = gameObject.transform.position.z + actionZ;

            if (position_x < 2f && position_x > -1.95f)
            {
                gameObject.transform.position += new Vector3(actionX, 0, 0);
            }

            if (position_z < -2.8f && position_z > -6.5f)
            {
                gameObject.transform.position += new Vector3(0, 0, actionZ);
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

    public override void AgentReset()
    {
        gameObject.transform.position = ballStartPos;
    }
}
