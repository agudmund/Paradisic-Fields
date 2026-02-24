using UnityEngine;

public class TreeCtrl : MonoBehaviour
{
    [SerializeField] GameObject[] Puffs;
    AudioSource aSource;

    void Awake(){
        aSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip current)
    {
        Clean();
        AudioSource[] aSources = GetComponents<AudioSource>();
        if(aSources.Length<3){
            if (aSource.isPlaying)
            {
                
                aSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            }
            aSource.clip = current;
            aSource.Play();
    }}

    void Clean()
    {
        AudioSource[] aSources = GetComponents<AudioSource>();
        foreach(AudioSource a in aSources)
        {
            if (!a.isPlaying)
            {
                a.volume = 0;
                a.enabled = false;
                Destroy(a);
            }
        }
        aSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
    }
   
}
