using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJoint : MonoBehaviour
{
    public LayerMask layerMask;
    public GameObject JointWall;
    public Wall wall;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!wall.editMode){
            Destroy(gameObject);
        }
        // Bit shift the index of the layer (8) to get a bit mask
        //int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 1000, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * hit.distance, Color.red);
            Debug.Log("Did Hit");
            GameObject joint = Instantiate(JointWall, hit.point,new Quaternion());
            joint.transform.SetParent(transform.parent);
            Destroy(hit.transform.gameObject);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * 1000, Color.red);
            Debug.Log("Did not Hit");
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * hit.distance, Color.red);
            Debug.Log("Did Hit");
            GameObject joint = Instantiate(JointWall, hit.point,new Quaternion());
            joint.transform.SetParent(transform.parent);
            Destroy(gameObject);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * 1000, Color.red);
            Debug.Log("Did not Hit");
        }
 
    }
}
