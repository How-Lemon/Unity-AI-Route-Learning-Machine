using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called before the first frame update
    public Map map;
    public List<GameObject> exclude;
    public bool editMode = true;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (editMode){ rb.WakeUp();Debug.Log("DANG");};
    }
    void OnCollisionEnter(Collision collision)
    {
        if (editMode)
        {
            foreach (GameObject go in exclude)
            {
                if (collision.gameObject == go) return;
            }
            if (collision.gameObject.tag == "road")
            {
                Destroy(gameObject.transform.parent.gameObject);
            }
        }else{
            if (collision.gameObject.tag == "car")
            {
                //map.Failed();
            }
        }

    }
    public void SetPlayMode(){
        editMode = false;
        Destroy(GetComponent<Rigidbody>());

    }
}
