using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CameraObserver : MonoBehaviour
{
    Vector3 position;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        int cc =  0;
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("car"))
        {
            cc++;
            position += item.transform.position;
        }
        position /= cc;
        gameObject.transform.position = new Vector3(position.x, gameObject.transform.position.y, position.z);
    }
}
