using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CustomAcademy : MonoBehaviour
{

    public GameObject MapGenerator;
    public GameObject StatMonitor;
    public GameObject Car;
    public Transform chaseCamera;
    public int CarCount = 10;

    public int lapCount = 10;
    GameObject mg;
    // Start is called before the first frame update
    void Start()
    {
        ResetPlayGround();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (GameObject.FindGameObjectWithTag("mapGenerator").GetComponent<Map>().Passed >= lapCount)
        {
            ResetPlayGround();
        }else{
            FindBestCar();
        }
        //Target The car with most Progress
    }

    void ResetPlayGround()
    {
        StatMonitor.SetActive(false);
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("car"))
        {
            DestroyImmediate(item);
        }
        Destroy(GameObject.FindGameObjectWithTag("mapGenerator"));
        mg = Instantiate(MapGenerator);
        StartCoroutine("WaitForGeneration");
    }
    IEnumerator WaitForGeneration()
    {
        while (mg.GetComponent<Map>().enabled == false)
        {
            yield return 0;
        }

        for (int i = 0; i < CarCount; i++)
        {
            GameObject car = Instantiate(Car);
            car.GetComponent<CarAgent>().map = mg.GetComponent<Map>();
            car.GetComponent<CarAgent>().ca = this;
        }
        mg.GetComponent<Map>().enabled = true;
        //mg.GetComponent<Map>().StartMap();

    }

    public void ResetCar(Vector3 position, Quaternion rotation)
    {

        GameObject car = Instantiate(Car, position, rotation); ;
        car.GetComponent<CarAgent>().map = mg.GetComponent<Map>();
        car.GetComponent<CarAgent>().ca = this;
    }

    private void SetMonitorToCar(GameObject car)
    {
        if (!StatMonitor.activeSelf)
        {
            StatMonitor.SetActive(true);
        }
        StatMonitor sm = StatMonitor.GetComponent<StatMonitor>();
        sm.car = car.GetComponent<CarAgent>();
        sm.CarController = car.GetComponent<CarController>();
        sm.rangeFindersContainer = car.transform.Find("Detectors").gameObject;
    }
    private void SetCameraToCar(GameObject car){
        UnityEngine.Animations.ConstraintSource cs = new UnityEngine.Animations.ConstraintSource();
        cs.sourceTransform = car.transform;
        cs.weight = 1;
        car.GetComponent<CarController>().mainCamera = chaseCamera;
        chaseCamera.gameObject.GetComponent<LookAtConstraint>().SetSource(0, cs);
    }
    private void FindBestCar(){
        GameObject nowBestCar = null;
        float bestProgress = 0;
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("car"))
        {
            if(item.GetComponent<CarAgent>().carPorgress > bestProgress){
                bestProgress = item.GetComponent<CarAgent>().carPorgress;
                nowBestCar = item;
            }
        }
        if(nowBestCar){
            SetMonitorToCar(nowBestCar);
             SetCameraToCar(nowBestCar);
        }
        
    }
}
