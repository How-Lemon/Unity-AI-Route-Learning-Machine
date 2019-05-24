using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public GameObject[] checkPoints;
    public GameObject[] walls;
    public GameObject progressPointDB;
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
    public int GetMaxCheckPointIndex
    {
        get
        {
            return maxCheckPoint - 1;
        }
    }
    private float _trackLength = -1, lapProgress = 0;
    private int _laps;
    private GameObject[] cars;
    private List<float> segmentLength;

    private int maxCheckPoint, checkPointPassed = 0;
    // Start is called before the first frame update
    void Start()
    {
        segmentLength = new List<float>();
        if (checkPoints.Length == 0)// Get ALL checkpoints
        {
            checkPoints = GameObject.FindGameObjectsWithTag("checkPoint");
        }
        if (walls.Length == 0)//Get All WALLS
        {
            walls = GameObject.FindGameObjectsWithTag("wall");
        }
        for (int i = 0; i < checkPoints.Length; i++)//Set all checkpoints
        {
            checkPoints[i].GetComponent<Collider>().isTrigger = true;
            checkPoints[i].GetComponent<MeshRenderer>().enabled = false;
            checkPoints[i].GetComponent<CheckPoint>().map = this;
            checkPoints[i].GetComponent<CheckPoint>().order = i;
            checkPoints[i].GetComponent<CheckPoint>().isLast = (i == (checkPoints.Length - 1));
            checkPoints[i].SetActive(true);
        }
        foreach (GameObject go in walls)//Activate All Walls
        {
            go.GetComponent<Wall>().map = this;
            go.GetComponent<Wall>().SetPlayMode();
        }
        cars = GameObject.FindGameObjectsWithTag("car");
        foreach (GameObject car in cars)
        {
            car.GetComponent<CarAgent>().map = this;
            car.GetComponent<CarAgent>().carStartPos = checkPoints[0].transform.position;
            car.GetComponent<CarAgent>().carStartRotation = checkPoints[0].transform.rotation;
            car.GetComponent<CarAgent>().Reset();
            //car.transform.position = checkPoints[0].transform.position;
            //car.transform.rotation = checkPoints[0].transform.rotation;
        }
        maxCheckPoint = checkPoints.Length;
        _trackLength = CalculateTrackLength();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void CallCarFailed(GameObject car)
    {

    }

    public void PassGoal(GameObject car)
    {//Reset the position of the last car
        car.transform.position = checkPoints[0].transform.position;
        car.transform.rotation = checkPoints[0].transform.rotation;
        car.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public float GetCarProgress(Vector3 position, int recentCheckPoint)//Get Current Progress in lap; return lap length if is at goal
    {
        float output = 0.0f;
        if (recentCheckPoint >= maxCheckPoint)
        {
            return trackLength;
        }
        //Get current segment
        Vector3 now = checkPoints[recentCheckPoint].transform.position, next = checkPoints[recentCheckPoint + 1].transform.position;
        Vector3 progressPoint = ClosestPointOnLine(now, next, position);
        if (progressPoint == now && recentCheckPoint >= 1)
        {
            progressPoint = ClosestPointOnLine(checkPoints[recentCheckPoint - 1].transform.position, checkPoints[recentCheckPoint].transform.position, position);
            output = GetLapProgress(recentCheckPoint) + Vector3.Magnitude(progressPoint - checkPoints[recentCheckPoint - 1].transform.position);
        }
        else
        {
            progressPoint = ClosestPointOnLine(now, next, position);
            output = GetLapProgress(recentCheckPoint + 1) + Vector3.Magnitude(progressPoint - now);
        }
        progressPointDB.transform.position = progressPoint;
        return output;
    }

    private float[] _tlLength = null;
    private float GetLapProgress(int checkPoint)
    {
         if (_tlLength == null)
        {
            _tlLength = new float[maxCheckPoint];
            for (int i = 0; i < _tlLength.Length; i++)
            {
                _tlLength[i] = -1f;
            }
            _tlLength[0] = 0f;
        }
        if (checkPoint <= 0) return 0f;
        float output = 0f;
        Debug.Log(checkPoint);
        if (_tlLength[checkPoint - 1] == -1f)
        {
            for (int i = 0; i < checkPoint - 1; i++)
            {
                output += segmentLength[i];
            }
            _tlLength[checkPoint - 1] = output;
        }
        else
        {
            output = _tlLength[checkPoint - 1];
        }

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
