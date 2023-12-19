using UnityEngine;

// add some ambient music while the menu is on
// should add a couple of buttons to the menu then, while we are at it, for sound and music on and off

public class AudioCtrl : MonoBehaviour
{
    public bool music,sound;
    AudioSource aSource,mMusic;
    public AudioClip menu,menuClick,newGame,pageFlip;
    GameObject cam;
    AudioListener listen;
    public float pitchIndex;

    /// <summary>
    /// Runtime Essentials
    /// </summary>

    private void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        if(cam!=null)
        {
            listen = cam.GetComponent<AudioListener>();
            aSource = cam.GetComponent<AudioSource>();
        }
        music = RestoreMusicPrefs();
        sound = RestoreSoundPrefs();

        pitchIndex = 1;
    }

    private void Start()
    {
        MenuMusic();
    }

    /// <summary>
    /// Controls
    /// </summary>
    public void Play( AudioClip current, GameObject source=null, float pitch = 1, float volume = 1)
    {
        
        // Only run if sound preferences have sound turned on.
        if (sound)
        {
            // Determine wether or not to run on the camera or a gameobject
            GameObject target;
            if(source==null) target = cam;
            else target = source;

            aSource = target.GetComponent<AudioSource>();

            // If there is no audio source on the gameobject, add one.
            if (aSource == null)
            {
                aSource = target.AddComponent(typeof(AudioSource)) as AudioSource;
            }

            // Cap it at max 3 sources for now (overlap thing on multiple raycasts)
            AudioSource[] aSources = target.GetComponents<AudioSource>();
            if(aSources.Length<3)
            {
                if (aSource.isPlaying)
                {
                    aSource = target.AddComponent(typeof(AudioSource)) as AudioSource;
                }
            }
            aSource.clip = current;
            aSource.pitch = pitch;
            aSource.volume = volume;
            AdjustPitch(pitch);
            aSource.Play();
            Clean(target);
        }
    }

    void MenuMusic()
    {
        mMusic = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        mMusic.clip = menu;
        mMusic.volume = .215f;
        mMusic.loop = true;
        if (music) mMusic.Play();
    }

    /// <summary>
    /// Toggles
    /// </summary>
    public void ToggleSound()
    {
        if (sound)
        {
            PlayerPrefs.SetInt("sound", 1);
            listen.enabled = true;
            if (music)
            {
                mMusic.Play();
            }
        }
        else
        {
            PlayerPrefs.SetInt("sound", 0);
            listen.enabled = false;
            AudioSource[] aSources = GetComponents<AudioSource>();
            foreach(AudioSource aSource in aSources)
            {
                aSource.Stop();
            }
        }
    }
    public void ToggleMusic()
    {
        if (music)
        {
            PlayerPrefs.SetInt("music", 1);
            mMusic.Play();
        }
        else
        {
            PlayerPrefs.SetInt("music", 0);
            mMusic.Stop();
        }
    }

    /// <summary>
    /// Preferences
    /// </summary>
    bool RestoreMusicPrefs()
    {
        bool active;
        active = PlayerPrefs.GetInt("music") == 1 ? true : false;
        return active;
    }
    bool RestoreSoundPrefs()
    {
        bool active;
        active = PlayerPrefs.GetInt("sound") == 1 ? true : false; // default to on while there is no menu button
        active = true;
        return active;
    }

    /// <summary>
    /// Utilites
    /// </summary>
    void AdjustPitch(float pitch)
    {
        if (pitch == pitchIndex)
        {
            pitchIndex = pitchIndex + .1f;
        }
        if (pitchIndex > 1.2)
        {
            pitchIndex = .8f;
        }
    }
    void Clean(GameObject target)
    {
        AudioSource[] aSources = target.GetComponents<AudioSource>();
        foreach(AudioSource a in aSources)
        {
            if (!a.isPlaying)
            {
                a.volume = 0;
                a.enabled = false;
                Destroy(a);
            }
        }
        aSources = target.GetComponents<AudioSource>();
        if(aSources.Length==0) aSource = target.AddComponent(typeof(AudioSource)) as AudioSource;
    }
}
