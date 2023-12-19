using UnityEngine;
using System.Collections;

// while(true) // while total encounter peeps is less than x

public class GameCtrl : MonoBehaviour
{
    public PlayerCtrl player;
    public int monies = 100;
    public GameObject tile;
    public GameObject pixel;
    public GameObject[] triggers;
    public GameObject[] triggerLights;

    public GameObject encounter;
    public int numberOfEncounters;

    public float totalplaytime = 0;
    float clockcounter = 0;
    public int hours = 0;
    public int minutes = 0;

    GameObject[] trees;

    public BoxCollider ground;

    void Awake(){
        // Turn off all triggers and trigger lights when the game starts.
        // foreach(GameObject light in triggerLights) light.SetActive(false);
        // foreach(GameObject trigger in triggers) trigger.SetActive(false);
        trees = GameObject.FindGameObjectsWithTag("Tree");
    }

    void Start(){
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        for(int i=0;i<numberOfEncounters ;i++)    
        {
            Instantiate(encounter,RandomPosition(ground, 1,3f),Quaternion.identity);
            yield return new WaitForSeconds(5f);
        }
    }

    public void Clock(){

        totalplaytime = totalplaytime + Time.deltaTime;
        clockcounter = clockcounter + Time.deltaTime;

        // Count increments of 15 minutes.
        int segment = 10;
        if(clockcounter>segment){
            minutes = minutes + 15;
            clockcounter = 0;
            // if(player.totalmotion>100)
            // {
            //     player.strength = player.strength + player.totalmotion/10000f;
            // }
            // player.currenttotalmotion = player.totalmotion;
            // player.totalmotion = 0f;

        }
        if (minutes>59){
            minutes = minutes - 60;
            hours = hours + 1;
            // Should of course be in sleepCtrl with the rest of them
            player.sleeplevel = player.sleeplevel + 1;
        }
        if (hours>24){
            hours = hours - 24;
        }
    }

    void FixedUpdate(){
        Clock();
    }

    public Vector3 RandomPosition(BoxCollider _boxCollider, float _yOffset, float _scatterClamp)
    // Returns a random position on a box colliders y plane.
    {
        float x = Random.Range(_boxCollider.bounds.min.x + _scatterClamp, _boxCollider.bounds.max.x - _scatterClamp);
        float y = _boxCollider.bounds.max.y + _yOffset;
        float z = Random.Range(_boxCollider.bounds.min.z + _scatterClamp, _boxCollider.bounds.max.z - _scatterClamp);

        return new Vector3(x,y,z);
    }

   
}
