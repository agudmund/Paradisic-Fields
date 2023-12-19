using UnityEngine;
using System.Collections;
using TMPro;

public class NotifiCat : MonoBehaviour
{
    public GameObject header;
    public GameObject body;
    public bool detached;
    public bool active;

    TextMeshPro head;
    TextMeshPro bod;

    float maxExistenceTime = 10f;

    void Awake()
    {
        active = true;
        // Get the components of the new message panel.
        head = header.GetComponent<TextMeshPro>();
        bod = body.GetComponent<TextMeshPro>();
    }

    public IEnumerator Meow(string _header, string _bod)
    {
        head.text = _header;
        string message = "";
        for(int i=0;i<_bod.Length ;i++){
            message += _bod[i].ToString();
            bod.text = message;
            yield return new WaitForSeconds(0.01f);
        }
        StartCoroutine(Activate());
        
    }
    IEnumerator Activate()
    {
        yield return new WaitForSeconds( 1f );
        Drop();
    }

    public void Drop()
    {
        active = false;
        detached = true;
        transform.parent = null;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddTorque(new Vector3(Random.Range(0,30),0,Random.Range(-30,30)));
    }

    IEnumerator Vanish(){
        yield return new WaitForSeconds( maxExistenceTime );
        Destroy(gameObject);
    }
}
