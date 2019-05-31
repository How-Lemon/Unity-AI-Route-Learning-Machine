using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAcademy : MonoBehaviour
{

    public GameObject MapGenerator;
    public GameObject Car;
    public int CarCount = 10;
    public int lapCount = 10;
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
        GameObject mg = Instantiate(MapGenerator);
        for(int i = 0; i<CarCount;i++){
            Instantiate(Car);
        }
        mg.GetComponent<Map>().Start();
    }
}
