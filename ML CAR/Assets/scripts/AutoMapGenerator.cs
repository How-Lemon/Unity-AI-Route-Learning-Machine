using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMapGenerator : MonoBehaviour
{
    [Header("Editor Settings")]
    public GameObject checkPoint, canvas, road;
    public Material normal, start, goal;

    [Header("Road Generation Settings")]
    public int checkPoints = 10;
    public float maxLength = 30, minLength = 10;
    public float maxFanAngle = 45;
    public float distance = 100;
    public float roadWidth = 10;
    public bool ordered = true;
    public bool done = false;

    private List<GameObject> checkPointCollection;
    private List<GameObject> roadCollection;
    private int remainingCP;
    private Vector3 activeGenerationPoint;
    private float windingAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        checkPointCollection = new List<GameObject>();
        roadCollection = new List<GameObject>();
        remainingCP = checkPoints;
        activeGenerationPoint = Vector3.zero;
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("car"))
        {
            item.GetComponent<CarAgent>().map = GetComponent<Map>();
        }
        //GameObject.FindGameObjectWithTag("monitor").GetComponent<StatMonitor>().map = GetComponent<Map>();
    }

    // Update is called once per frame
    void Update()
    {
        Event currentEvent = Event.current;
        if (remainingCP > 0)
        {
            generateCheckPoint();

            remainingCP--;
        }
        else
        {
            EnterPlayMode();

        }

    }

    void generateCheckPoint()
    {

        GameObject ck = Instantiate(checkPoint, activeGenerationPoint, new Quaternion(), canvas.transform);
        if (checkPointCollection.Count == 0)
        {
            ck.transform.localScale = new Vector3(roadWidth, roadWidth, roadWidth);
            ck.GetComponent<Renderer>().material = start;
        }
        checkPointCollection.Add(ck);
        generateRoad_ordered();
        activeGenerationPoint = GetNextCKPostion();
    }

    void generateRoad_ordered()
    {
        if (checkPointCollection.Count <= 1) return;
        //for (int i = 1; i < checkPointCollection.Count; i++)
        //{
        //    GameObject root = checkPointCollection[i - 1], head = checkPointCollection[i];
        GameObject root = checkPointCollection[checkPointCollection.Count - 2], head = checkPointCollection[checkPointCollection.Count - 1];
        float distance = Vector3.Magnitude(root.transform.position - head.transform.position);
        //FIND rotation of two points;
        float angle = Mathf.Atan2(root.transform.position.x - head.transform.position.x, root.transform.position.z - head.transform.position.z) * Mathf.Rad2Deg + 90;
        GameObject newRoad = Instantiate(road, root.transform.position, new Quaternion(), canvas.transform);
        newRoad.transform.eulerAngles = new Vector3(0, angle, 0);
        newRoad.GetComponent<RoadController>().length = distance;
        newRoad.GetComponent<RoadController>().width = roadWidth;
        head.transform.localScale = new Vector3(roadWidth, roadWidth, roadWidth);
        roadCollection.Add(newRoad);
        //}
    }
    void refreshActivePoint(Vector3 start, Vector3 direction)
    {

    }

    private Vector3 GetNextCKPostion()
    {
        if (checkPointCollection.Count == 1) return Vector3.forward * Random.Range(minLength, maxLength);
        Vector3 nowPosition = checkPointCollection[checkPointCollection.Count - 1].transform.position, lastPosition = checkPointCollection[checkPointCollection.Count - 2].transform.position;
        Vector3 direction = lastPosition - nowPosition;
        direction /= direction.magnitude;
        //Get a random direction
        float angle = Random.Range(-maxFanAngle, maxFanAngle);
        while (Mathf.Abs(angle + windingAngle) >= 100)
        {
            angle = Random.Range(-maxFanAngle, maxFanAngle);
        }// 
        windingAngle += angle;
        float rad = angle * Mathf.Deg2Rad;

        Vector3 sDir = -direction;
        sDir = new Vector3(Mathf.Cos(rad) * sDir.x - Mathf.Sin(rad) * sDir.z,
                           sDir.y,
                           Mathf.Sin(rad) * sDir.x + Mathf.Cos(rad) * sDir.z);
        float distance = Random.Range(minLength, maxLength);
        Vector3 newPos = sDir * distance + nowPosition;
        return newPos;
    }
    void EnterPlayMode()
    {
        Debug.Log("Generation Complete");
        GetComponent<Map>().enabled = true;
        Destroy(this);
    }
}
