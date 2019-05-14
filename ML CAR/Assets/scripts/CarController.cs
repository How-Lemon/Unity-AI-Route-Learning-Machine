using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{
    public GameObject skidMark;
    public Transform cameraPoint, mainCamera;
    public float speed;
    private Rigidbody rb;
    private float startTime;
    public GameObject[] skidMarks = new GameObject[4];
    private WheelCollider[] wc = new WheelCollider[4];
    private GameObject[] slipping = new GameObject[4];
    void Start()
    {
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
        for (int i = 0; i < 4; i++)
        {
            WheelCollider wheel = wc[i];
            WheelHit hit;
            bool pass = false;
            if (wheel.GetGroundHit(out hit))
            {

                if (hit.forwardSlip > 0.5 || hit.sidewaysSlip > 0.5)
                {
                    pass = true;
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
                    slipping[i].transform.position = hit.point;
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


}