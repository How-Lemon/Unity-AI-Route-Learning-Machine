using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RangeFinder : MonoBehaviour
{
    public float maxRange;
    public LayerMask layerMask;
    public float range{
        get{
            return  _range;
        }
    }
    public bool detected{
        get{
            return _range != maxRange;
        }
    }
    private float _range = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, maxRange, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
            _range = hit.distance;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * maxRange, Color.yellow);
            _range = maxRange;
        }
    }

    
}

