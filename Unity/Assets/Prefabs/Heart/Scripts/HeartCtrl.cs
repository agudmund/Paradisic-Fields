using UnityEngine;
using System.Collections;

public class HeartCtrl : MonoBehaviour
{
    [SerializeField] AudioClip[] hitSounds;
    [SerializeField] AudioClip bumpSound;
    [SerializeField] float puffHealth;
    AudioCtrl aCtrl;
    AudioClip boom;
    Rigidbody rb;

    public bool damaged;
    public bool detached;

    void Awake()
    {
        aCtrl = GameObject.FindGameObjectWithTag("GameController").GetComponent<AudioCtrl>();
        boom = hitSounds[Random.Range(0,hitSounds.Length)];
    }

    public void Hit()
    {
        aCtrl.Play(boom, gameObject, Random.Range(0.7f,1.4f));
        puffHealth--;
        damaged = puffHealth < 6f; 

        if(damaged && !detached){
            transform.parent.GetComponent<HeartMainCtrl>().damaged = true;
            rb = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
            rb.useGravity = true;
            rb.AddTorque(new Vector3(Random.Range(-30,30),0,Random.Range(-30,30)));
            detached = true;
        }
        if(puffHealth <= 1)
        {
            Destroy(gameObject);
        }
    }

    public void deezNuts(){
        aCtrl.Play(boom, gameObject, Random.Range(0.7f,1.4f));
        if(detached){
            rb.AddTorque(new Vector3(Random.Range(-100,100),0,Random.Range(-100,100)));
        }
    }

    void OnCollisionEnter(Collision other){
       aCtrl.Play(bumpSound, gameObject);
    }
}
