using System.Collections;
using UnityEngine;
using TMPro;

public class NotificatCtrl : MonoBehaviour
{
    PlayerCtrl player;
    public GameObject banner;
    public GameObject[] stack;
    public NotifiCat page;
    TextMeshPro body;

    GameObject rez;

    Vector3 offset(Transform loc){
        // Count active panels and offset them slightly forward so they stack
        stack = GameObject.FindGameObjectsWithTag("msgPanel");
        float posZ = loc.position.z + (stack.Length / 50f);
        Vector3 _offset = new Vector3(loc.position.x,loc.position.y,posZ);
        return _offset;
    }

    public void Notificat(string _header, string _body, Transform loc){
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
        rez = GameObject.Instantiate(banner, offset(loc), player.notebook.transform.rotation, player.notebook);
        page = rez.GetComponent<NotifiCat>();

        StartCoroutine(page.Meow(_header,_body));
    }
}
