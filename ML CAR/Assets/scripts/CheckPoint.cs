using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Map map;
    public int order;

    public bool isLast = false;
    private int _order = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // Change the cube color to green.
        if(other.gameObject.tag == "car"){
            //map.Pass(gameObject);
            other.gameObject.GetComponent<CarAgent>().PassCheckPoint(order);
            if(isLast){
                map.PassGoal(other.gameObject);
            }  
        }
    }
}
