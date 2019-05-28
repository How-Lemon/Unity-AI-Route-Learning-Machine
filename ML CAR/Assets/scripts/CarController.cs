using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{
    public GameObject skidMark;
    public Transform cameraPoint, mainCamera;
    public float speed;
    public float ABSThreshold = 0.3f, RPMThreshold = 100;
    public float carSpeed
    {
        get
        {
            if (rb)
            {
                return Vector3.Magnitude(rb.velocity);
            }
            else
            {
                return 0;
            }
        }
    }
    public float trailDistance
    {
        get
        {
            return _trailDistance;
        }
    }
    private Rigidbody rb;
    private float startTime;
    public GameObject[] skidMarks = new GameObject[4];
    private WheelCollider[] wc = new WheelCollider[4];
    private GameObject[] slipping = new GameObject[4];
    private bool[] breakable = new bool[4], slipWheel = new bool[4];
    private float BreakForce = 0, EngineForce = 0;

    private float _trailDistance = 0;
    void Start()
    {
        BreakForce = GetComponent<CarAgent>().BreakForce;
        EngineForce = GetComponent<CarAgent>().EngineForce;
        rb = gameObject.GetComponent<Rigidbody>();
        startTime = Time.time;
        wc[0] = GetComponent<CarAgent>().wheelBL;
        wc[1] = GetComponent<CarAgent>().wheelBR;
        wc[2] = GetComponent<CarAgent>().wheelFL;
        wc[3] = GetComponent<CarAgent>().wheelFR;
    }

    void FixedUpdate()
    {
        float dist = Vector3.Distance(mainCamera.position, cameraPoint.position);
        float fracJourney = speed * dist * dist / 10;
        mainCamera.position = Vector3.Lerp(mainCamera.position, cameraPoint.position, fracJourney);
    }

    void Update()
    {
        UpdateTrailDistance();
        CheckWheel();
    }
    void UpdateTrailDistance()
    {
        _trailDistance += Vector3.Magnitude(rb.velocity);
    }

    void CheckWheel()
    {
        for (int i = 0; i < 4; i++)
        {
            WheelCollider wheel = wc[i];
            WheelHit hit;
            bool pass = false;
            if (wheel.GetGroundHit(out hit))
            {

                if (Mathf.Abs(hit.forwardSlip) > 0.5 || Mathf.Abs(hit.sidewaysSlip) > 0.5)
                {
                    //pass = true;
                }
                if(Mathf.Abs(hit.forwardSlip) > 0.6){
                    slipWheel[i] = true;
                }else{
                    slipWheel[i] = false;
                }
                if (Mathf.Abs(hit.forwardSlip) > ABSThreshold)
                {
                    breakable[i] = false;
                }
                else
                {
                    breakable[i] = true;
                }
            }
            if (pass)
            {
                if (!slipping[i])
                {
                    GameObject slip = Instantiate(skidMark, hit.point, Quaternion.identity, wheel.gameObject.transform);
                    slipping[i] = slip;
                    slip.transform.Rotate(90, 0, 0);
                }
                else
                {
                    slipping[i].transform.position = new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z);
                    //slipping[i].transform.eulerAngles = Vector3.down;
                }

            }
            else
            {
                if (slipping[i])
                {
                    slipping[i].transform.SetParent(null);
                    slipping[i] = null;
                }
            }
        }
    }

    public void PushBreak(float force)
    {
        wc[0].motorTorque = 0;
        wc[1].motorTorque = 0;
        
        for (int i = 0; i < 4; i++)
        {
            if (breakable[i] || wc[i].rpm < RPMThreshold)
            {
                //Debug.Log(wc[i].rpm);
                wc[i].brakeTorque = BreakForce * force * -100;
            }else{
                wc[i].brakeTorque = 0;
            }
        }
    }
    public void PushGas(float force){
        if(!slipWheel[0])wc[0].motorTorque = EngineForce * force;
        if(!slipWheel[1])wc[1].motorTorque = EngineForce * force;
        wc[2].brakeTorque = 0;
        wc[3].brakeTorque = 0;
    }


}