using UnityEngine;

public class PuffCtrl : MonoBehaviour
{
    NotificatCtrl msg;
    [SerializeField] TreeCtrl tree;
    float puffHealth = 4f;
    public bool detached;
    public AudioClip[] hitSounds;
    AudioClip boom;
    Rigidbody rb;

    void Awake(){
        msg = GameObject.FindGameObjectWithTag("Notificat").GetComponent<NotificatCtrl>();
        boom = hitSounds[Random.Range(0,hitSounds.Length)];
    }

    public void deezNuts(){
        Health();
        tree.Play(boom);
        if(detached){
            rb.AddTorque(new Vector3(Random.Range(-30,30),0,Random.Range(-30,30)));
        }
    }

    void OnCollisionEnter(Collision other){
        tree.Play(boom);
    }

    void Health(){

        puffHealth -= 1;
        if( puffHealth<1 && !detached ){
            detached = true;
            puffHealth = 0;
            rb = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
            rb.AddTorque(new Vector3(Random.Range(-30,30),0,Random.Range(-30,30)));
        }
    }
}
