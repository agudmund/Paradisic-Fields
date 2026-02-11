using UnityEngine;

public class Grabbable : MonoBehaviour
{
    Rigidbody rb;
    Transform grabSpot;

    public void Grab(Transform _grabSpot){
        this.grabSpot = _grabSpot;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void Drop(){
        grabSpot = null;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
    }

    void FixedUpdate(){
        if(grabSpot!=null){
            rb = GetComponent<Rigidbody>();
            float lerpSpeed = 10f;
            Vector3 newPos = Vector3.Lerp(transform.position,grabSpot.position,Time.deltaTime * lerpSpeed);
            rb.MovePosition(newPos);   
        }
    }
    
}
