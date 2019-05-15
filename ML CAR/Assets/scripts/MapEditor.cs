using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditor : MonoBehaviour
{
    [Header("Editor Settings")]
    public Camera mainCamera;
    public GameObject checkPoint, canvas, road;
    public Material normal, start, goal;
    public LayerMask layerMask;

    [Header("Road Generation Settings")]
    public float distance = 100;
    public float roadWidth = 10;
    public bool ordered = true;

    private List<GameObject> checkPointCollection;
    private List<GameObject> roadCollection;

    // Start is called before the first frame update
    void Start()
    {
        checkPointCollection = new List<GameObject>();
        roadCollection = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        Event currentEvent = Event.current;

        if (Input.GetMouseButtonDown(0))
            checkClicked();

    }

    void checkClicked()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.nearClipPlane);
        Vector3 camPosition = mainCamera.transform.position;
        Vector3 clickPosition = mainCamera.ScreenToWorldPoint(mousePos);
        Vector3 direction = Vector3.Normalize(camPosition - mousePos);
        //raycast thing
        RaycastHit hit;
        if (Physics.Raycast(clickPosition, transform.TransformDirection(Vector3.down), out hit, distance, layerMask))
        {
            Debug.DrawRay(clickPosition, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            Debug.Log("Did Hit, do nothing for now");
        }
        else
        {
            Debug.DrawRay(clickPosition, transform.TransformDirection(Vector3.down) * distance, Color.white);
            Debug.Log("Did not Hit, Creating CheckPoint");
            Vector3 hitPosition = new Vector3(clickPosition.x, clickPosition.y - distance, clickPosition.z);
            GameObject ck = Instantiate(checkPoint, hitPosition, new Quaternion(), canvas.transform);
            if (checkPointCollection.Count == 0)
            {
                ck.transform.localScale = new Vector3(roadWidth, roadWidth, roadWidth);
                ck.GetComponent<Renderer>().material = start;
            }
            checkPointCollection.Add(ck);
        }
        if (ordered) generateRoad_ordered();
        else generateRoad_auto();
    }

    void generateRoad_ordered()
    {
        if(checkPointCollection.Count <= 1) return;
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

    void generateRoad_auto()
    {

    }
}
