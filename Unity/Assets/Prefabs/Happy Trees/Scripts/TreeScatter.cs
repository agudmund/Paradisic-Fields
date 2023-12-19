using UnityEngine;

public class TreeScatter : MonoBehaviour
{
    [SerializeField] int number;
    [SerializeField] GameObject[] prefab;
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] GameObject parentGameObject;

    void Awake()
    {
        Scatter();
    }

    void Scatter()
    {
        for (int i = 0; i < number; i++)
        {
            Vector3 pos = RandomPosition();
            Quaternion _rotation = Quaternion.Euler(0f, Random.Range(0,360), 0f);

            Collider[] hitColliders = Physics.OverlapSphere(pos, 2, LayerMask.GetMask("Object"));
            if (hitColliders.Length > 0)
            {
              continue;
            }

            GameObject heartObject = Instantiate(prefab[Random.Range(0,prefab.Length)], pos, _rotation) as GameObject;
            heartObject.name = "Tree" + i;
            heartObject.transform.parent = parentGameObject.transform;
        }
    }

    Vector3 RandomPosition()
    {
        float x = Random.Range(boxCollider.bounds.min.x, boxCollider.bounds.max.x);
        float y = boxCollider.bounds.max.y +2.2f;
        float z = Random.Range(boxCollider.bounds.min.z, boxCollider.bounds.max.z);

        return new Vector3(x,y,z);

    }
}