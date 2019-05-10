using UnityEngine;
 using System.Collections;
 using System.Collections.Generic;
 
 public class deformer : MonoBehaviour {
 
     public float speed = 2f;
     public Mesh mesh;
     public Vector3[] verts;
     public float maxDistance;
     public GameObject explosion;

     private Rigidbody rigidbody;
     
     void Start() {
         mesh = this.GetComponent<MeshFilter>().mesh;
         verts = mesh.vertices;
         rigidbody = this.GetComponent<Rigidbody>();
         this.GetComponent<MeshFilter>().mesh = mesh;
     }
     
     void OnCollisionEnter(Collision other) {
         Debug.Log("Collided with " + other.gameObject.name);
         if (other.gameObject.GetComponent<jelly>() != null) {
             Vector3 colPosition = transform.InverseTransformPoint(other.contacts[0].point);
             movePoints(other.gameObject);
         }
     }
     
     public void movePoints(GameObject other) {
         Vector3[] otherVerts = other.GetComponent<jelly>().verts;
         float distance;
         for (int i=0; i<otherVerts.Length; i+=1) {
             distance = Vector2.Distance((rigidbody.position), other.transform.TransformPoint(otherVerts[i]));
             if (distance <= maxDistance) {
                  //edit the vertices
             }
         }
         other.GetComponent<jelly>().UpdateMesh(otherVerts);
     }
 
 }