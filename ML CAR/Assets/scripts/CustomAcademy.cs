using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAcademy : MonoBehaviour
{

    public GameObject MapGenerator;
    public GameObject Car;
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
        if(GameObject.FindGameObjectWithTag("mapGenerator").GetComponent<Map>().Passed >= lapCount){
            ResetPlayGround();
        }
    }

    void ResetPlayGround(){
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("car"))
        {
            DestroyImmediate(item);
        }
        Destroy(GameObject.FindGameObjectWithTag("mapGenerator"));
        mg = Instantiate(MapGenerator);
        StartCoroutine("WaitForGeneration");
    }
    IEnumerator WaitForGeneration(){
        while(mg.GetComponent<Map>().enabled == false){
            yield return 0;
        }
        
        for(int i = 0; i<CarCount;i++){
            GameObject car = Instantiate(Car);
            car.GetComponent<CarAgent>().map = mg.GetComponent<Map>();
        }
        mg.GetComponent<Map>().enabled = true;
        //mg.GetComponent<Map>().StartMap();
        
    }
}
