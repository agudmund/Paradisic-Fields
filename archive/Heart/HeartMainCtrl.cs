using UnityEngine;

public class HeartMainCtrl : MonoBehaviour
{
    [SerializeField] GameObject leftHeart;
    [SerializeField] GameObject rightHeart;
    [SerializeField] GameObject burst;

    public bool damaged;
    int rotateSpeed;
    float baseSize = 20f;

    void Awake() => rotateSpeed = Random.Range(20,70);

    void Update()
    {
        if(damaged)
        {
            if(leftHeart==null && rightHeart==null) burst.SetActive(true);

        }
        else
        {
            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
            float n = baseSize + Mathf.Sin(Time.time * 2f) * baseSize /30f;
            transform.localScale = Vector3.one * n;
        }
    }
}
