using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadController : MonoBehaviour
{
    [Header("Road Attribute")]
    public float width = 1;
    public float length = 1;
    public GameObject road, joint;
    private float testvalue = 3;
    // Start is called before the first frame update
    void Start()
    {
        if (length <= 0)
        {
            Debug.LogError("Length Should Not Be ZERO!");
            return;
        }
        road.transform.localScale = new Vector3(length, 1, width);
        joint.transform.localScale = new Vector3(width / length, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetRoadAttribute(float width, float length, float rootExtension, float headExtension)
    {
        this.width = width;
        this.length = length;
        this.Start();
    }



}
