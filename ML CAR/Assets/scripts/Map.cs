using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public GameObject[] checkPoints;
    public GameObject[] walls;
    public GameObject car, progressPointDB;
    public float trackLength
    {
        get
        {
            if (_trackLength != -1) return _trackLength;
            else
            {
                _trackLength = CalculateTrackLength();
                return _trackLength;
            }
        }
    }
    public int laps
    {
        get
        {
            return _laps;
        }
    }
    private float _trackLength = -1, lapProgress = 0;
    private int _laps;

    private List<float> segmentLength;

    private int maxCheckPoint, checkPointPassed = 0;
    // Start is called before the first frame update
    void Start()
    {
        segmentLength = new List<float>();
        if (checkPoints.Length == 0)
        {
            checkPoints = GameObject.FindGameObjectsWithTag("checkPoint");
        }
        if (walls.Length == 0)
        {
            walls = GameObject.FindGameObjectsWithTag("wall");
        }
        foreach (GameObject go in checkPoints)
        {
            go.GetComponent<Collider>().isTrigger = true;
            go.GetComponent<CheckPoint>().map = this;
            go.SetActive(false);
        }
        foreach (GameObject go in walls)
        {
            go.GetComponent<Wall>().map = this;
            go.GetComponent<Wall>().SetPlayMode();
        }
        if (car == null)
        {
            car = GameObject.FindGameObjectWithTag("car");
            car.transform.position = checkPoints[0].transform.position;
            car.transform.rotation = checkPoints[0].transform.rotation;
        }

        checkPoints[0].SetActive(true);
        maxCheckPoint = checkPoints.Length;
        _trackLength = CalculateTrackLength();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
        GetDistance();
    }

    public void Failed()
    {
        Debug.Log("RUN FAILED!!!");
    }

    public void Success()
    {
        Debug.Log("YAY");
        _laps++;
        Reset();
    }

    public void Reset()
    {
        checkPoints[checkPointPassed - 1].SetActive(false);
        checkPoints[0].SetActive(true);
        checkPointPassed = 0;
        car.GetComponent<CarAgent>().Reset();
        car.transform.position = checkPoints[0].transform.position;
        car.transform.rotation = checkPoints[0].transform.rotation;

    }

    public void Pass(GameObject checkPoint)
    {
        if (++checkPointPassed >= maxCheckPoint)
        {
            Success();
        }
        else
        {
            checkPoints[checkPointPassed - 1].SetActive(false);
            checkPoints[checkPointPassed].SetActive(true);
            if (checkPointPassed > 1) lapProgress += segmentLength[checkPointPassed - 2];
        }
    }

    public float GetDistance()
    {//Get the distance Traveled
        if (checkPointPassed >= maxCheckPoint || checkPointPassed == 0)
        {
            return lapProgress;
        };
        //Get current segment
        Vector3 now = checkPoints[checkPointPassed - 1].transform.position, next = checkPoints[checkPointPassed].transform.position;
        Vector3 progressPoint = ClosestPointOnLine(now, next, car.transform.position);
        float output = 0;
        if (progressPoint == now && checkPointPassed > 1)
        {
            progressPoint = ClosestPointOnLine(checkPoints[checkPointPassed - 2].transform.position, checkPoints[checkPointPassed - 1].transform.position, car.transform.position);
            if (progressPoint == checkPoints[checkPointPassed - 2].transform.position) Failed();
            output = lapProgress - segmentLength[checkPointPassed - 2] + Vector3.Magnitude(progressPoint - checkPoints[checkPointPassed - 2].transform.position);
        }
        else
        {
            progressPoint = ClosestPointOnLine(now, next, car.transform.position);
            output = lapProgress + Vector3.Magnitude(progressPoint - now);
        }
        progressPointDB.transform.position = progressPoint;
        return output;
    }
    private float CalculateTrackLength()
    {
        float output = 0f;
        for (int i = 1; i < maxCheckPoint; i++)
        {
            GameObject now = checkPoints[i - 1], next = checkPoints[i];
            float seg = Vector3.Magnitude(next.transform.position - now.transform.position);
            output += seg;

            segmentLength.Add(seg);
        }
        return output;
    }

    private Vector3 ClosestPointOnLine(Vector3 vA, Vector3 vB, Vector3 vPoint)
    {
        Vector3 vVector1 = vPoint - vA;
        Vector3 vVector2 = (vB - vA).normalized;

        float d = Vector3.Distance(vA, vB);
        float t = Vector3.Dot(vVector2, vVector1);

        if (t <= 0)
            return vA;

        if (t >= d)
            return vB;

        Vector3 vVector3 = vVector2 * t;

        Vector3 vClosestPoint = vA + vVector3;

        return vClosestPoint;
    }
}
