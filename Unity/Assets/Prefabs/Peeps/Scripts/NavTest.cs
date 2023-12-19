using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavTest : MonoBehaviour
{
    // NavMeshAgent nav;
    // GameObject cleo;
    // GameObject[] targets;
    
    // [Header("Presets")]
    // [SerializeField] MeshRenderer statusBubble;
    // [SerializeField] GameObject imagePlane;
    // [SerializeField] Texture[] peeps;
    // [SerializeField] Texture[] colors;
    // [SerializeField] GameObject noteDropOffPoint;
    // [SerializeField] GameObject pickupPoint;

    // [Header("Stats")]
    // [SerializeField] GameObject target;
    // [SerializeField] GameObject pickup;
    // [SerializeField] float distance;
    
    // [SerializeField] float heartTimerLimit = 20f;
    // [SerializeField] float heartTimer;
    // [SerializeField] bool status;
    // [SerializeField] bool debug;

    // [SerializeField] bool heartBroken;
    // [SerializeField] bool playerContact;
    // [SerializeField] bool notificatPickup;
    // [SerializeField] bool notificatDropoff;
    // [SerializeField] bool puffContact;

    // void Awake()
    // {
    //     debug = false;
    //     if(debug==true) status=true;
    //     nav = GetComponent<NavMeshAgent>();
    //     cleo = GameObject.FindGameObjectWithTag("Player");
    //     // noteDropOffPoint = GameObject.FindGameObjectWithTag("noteDropOffPoint");
    //     Texture peep = peeps[Random.Range(0,peeps.Length)];
    //     transform.name = peep.name;
    //     imagePlane.GetComponent<MeshRenderer>().material.mainTexture = peep;
    //     heartTimer  = heartTimerLimit;
    // }
    // void Start()
    // {   
    //     nav.speed = 0;
    //     GetTarget();
    // }
    // void Update()
    // {   
    //     // if(status==true) statusBubble.enabled = true;
    //     // else statusBubble.enabled = false;
    //     // distance = nav.remainingDistance;
    //     // SetColors();
    //     // ResetHeart();
    //     // if(target!=null)
    //     // {
    //     //     if(CheckTarget("Heart", heartBroken, 1.6f)==true)
    //     //     {
    //     //         HandleHeart();
    //     //         return;
    //     //     }
    //     //     if(CheckTarget("msgPanel", notificatPickup, 0.2f)==true)
    //     //     {
    //     //         HandleNotificat();
    //     //         return;
    //     //     }
    //     //     if(CheckTarget("noteDropOffPoint", notificatDropoff, 2f)==true)
    //     //     {
    //     //         HandleNotificatDropoff();
    //     //         return;
    //     //     }
    //     //     if(CheckTarget("Player", playerContact, 9f)==true)
    //     //     {
    //     //         HandlePlayer();
    //     //         return;
    //     //     }
    //     //     if(CheckTarget("Puff", puffContact, 1f)==true)
    //     //     {
    //     //         HandlePuff();
    //     //         return;
    //     //     }
    //     // }
    // }
    // void HandlePuff()
    // {
    //     puffContact = true;
    //     if(debug==true) Debug.Log($"{transform.name} is near a puff.");
    //     nav.speed = 0;
    //     pickup = target;
    //     Grabbable grabz = pickup.GetComponent<Grabbable>();
    //     grabz.Grab(pickupPoint.transform);
    //     SetTarget(noteDropOffPoint);
    //     puffContact = false;
    // }
    // void HandleNotificatDropoff()
    // {
    //     notificatDropoff = true;
    //     if(debug==true) Debug.Log($"{transform.name} is near notificat dropoff.");
    //     nav.speed = 0;
    //     target = null;
    //     Grabbable grabz = pickup.GetComponent<Grabbable>();
    //     grabz.Drop();
    //     pickup.gameObject.tag = "Untagged";
    //     pickup = null;
    //     StartCoroutine(WaitForNewTarget(3f));
    // }
    // void HandleNotificat()
    // {
    //     notificatPickup = true;
    //     if(debug==true) Debug.Log($"{transform.name} is near notificat.");
    //     nav.speed = 0;
    //     pickup = target;
    //     Grabbable grabz = pickup.GetComponent<Grabbable>();
    //     grabz.Grab(pickupPoint.transform);
    //     SetTarget(noteDropOffPoint);
    //     notificatPickup = false;
    // }
    // bool CheckTarget(string _target, bool _condition, float _distance)
    // {
    //     if(target==null) return false;
    //     if(target.gameObject.tag != _target) return false;
    //     if(_condition==true) return false;
    //     else return nav.remainingDistance < (_distance) == true;
    // }
    // void HandlePlayer()
    // {
    //     playerContact = true;
    //     if(debug==true) Debug.Log($"{transform.name} is near last known player location.");
    //     nav.speed = 0;
    //     target = null;
    //     StartCoroutine(WaitForNewTarget(3f));
    // }
    // void HandleHeart()
    // {
    //     heartBroken = true;
    //     if(debug==true) Debug.Log($"{transform.name} is handling the heart situation.");
    //     target.GetComponent<HeartCtrl>().Hit();
    //     nav.speed = 0;
    //     target = null;
    //     heartTimer = heartTimerLimit;
    //     StartCoroutine(WaitForNewTarget(5f));
    // }
    // void ResetHeart()
    // {
    //     if(heartBroken==true)
    //     {
    //         heartTimer = heartTimer - Time.deltaTime;
    //         if(heartTimer<0)
    //         {
    //             heartTimer = heartTimerLimit;
    //             heartBroken = false;
    //         }
    //     }
    // }
    // IEnumerator WaitForNewTarget(float seconds)
    // {
    //     if(debug==true) Debug.Log($"{transform.name} is idling near target.");
    //     yield return new WaitForSeconds(seconds);
    //     playerContact = false;
    //     notificatDropoff = false;
    //     GetTarget();
    // }
    // void SetColors()
    // {
    //     if(target==null) statusBubble.material.mainTexture = colors[0];
    //     else
    //     {
    //         if(target.gameObject.tag == "Heart") statusBubble.material.mainTexture = colors[1];
    //         if(target.gameObject.tag == "msgPanel") statusBubble.material.mainTexture = colors[2];
    //         if(target.gameObject.tag == "Player") statusBubble.material.mainTexture = colors[3];
    //     } 
    // }
    // void GetTarget()
    // {   
    //     if(debug==true) Debug.Log($"{transform.name} is looking for targets.");
    //     targets = new GameObject[0];
    //     if(heartBroken!=true)
    //     {
    //         targets = GameObject.FindGameObjectsWithTag("Heart");
    //         if(targets.Length>0)
    //         {
    //             if(debug==true) Debug.Log($"{transform.name} Found Hearts.");
    //             target = targets[Random.Range(0,targets.Length)];
    //             SetTarget(target);
    //             return;
    //         }
    //     }
    //     else
    //     {
    //         int seed = Random.Range(0,1);
    //         // int seed = 1;
    //         if(seed==1)
    //         {
    //             targets = GameObject.FindGameObjectsWithTag("msgPanel");
    //             if(targets.Length>0)
    //             {
    //                 if(debug==true) Debug.Log($"{transform.name} Found Notificat Panels."); 
    //                 target = targets[Random.Range(0,targets.Length)];
    //                 NotifiCat page = target.GetComponent<NotifiCat>();
    //                 if(debug==true) Debug.Log($"{transform.name} Check: {page.detached}");
    //                 if(page.detached) SetTarget(target);
    //                 else GetTarget();
    //             }
    //             else SetTarget(cleo);
    //         }
    //         else
    //         {
    //             targets = GameObject.FindGameObjectsWithTag("Puff");
    //             if(targets.Length>0)
    //             {
    //                 if(debug==true) Debug.Log($"{transform.name} Found some Puffs."); 
    //                 target = targets[Random.Range(0,targets.Length)];
    //                 PuffCtrl puff = target.GetComponent<PuffCtrl>();
    //                 if(debug==true) Debug.Log($"{transform.name} Check: {puff.detached}");
    //                 if(puff.detached) SetTarget(target);
    //                 else StartCoroutine(WaitForNewTarget(heartTimerLimit));
    //             }
    //             else SetTarget(cleo);
    //         }
    //     }
    // }
    // void SetTarget(GameObject _target)
    // {
    //     if(debug==true) Debug.Log($"{transform.name} Target is {_target}.");
    //     target = _target;
    //     nav.SetDestination(target.GetComponent<Transform>().position);
    //     nav.speed = Random.Range(1,3);
    // }
}
