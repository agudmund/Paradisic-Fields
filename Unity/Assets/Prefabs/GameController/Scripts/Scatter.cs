using UnityEngine;
// while min num keep trying to make new ones, with max attempts.
// padding determined by number of objects to scatter

public class Scatter : MonoBehaviour
{
    [SerializeField] LayerMask colliderMask;
    [SerializeField] BoxCollider area;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] Quaternion rootRotation;
    [SerializeField] bool randomRotation;
    [SerializeField] float offsetY = 1;
    [SerializeField] float spread = 2;
    [SerializeField] int number = 5;
    [SerializeField] string objectName = "Thing";
    [SerializeField] float scatterClamp;
    
    GameCtrl ctrl;

    void Awake()
    {
        ctrl = GetComponent<GameCtrl>();
        Spawn(); 
    }

    public void Spawn()
    {
        GameObject parentGameObject = new GameObject(objectName);
        for (int i = 0; i < number; i++)
        {
            // Get a Random point from the area with a offset on the y axis.
            Vector3 pos = ctrl.RandomPosition(area, offsetY,scatterClamp);
            Collider[] hitColliders = Physics.OverlapSphere(pos, spread, colliderMask);
            if (hitColliders.Length > 0) continue;

            // Spawn a new GameObject
            int seed = Random.Range(0,prefabs.Length);
            GameObject current = Instantiate(prefabs[seed], pos, rootRotation) as GameObject;
            current.name = $"{i.ToString("D3")}:{prefabs[seed].transform.name}";
            if(parentGameObject!=null) current.transform.parent = parentGameObject.transform;
            if(randomRotation) current.transform.Rotate(0,Random.Range(0,360),0);
        }
    }
}