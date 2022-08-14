using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using static CachedItems;

public class SFXManager : MonoBehaviour
{
    public static SFXManager I;
    readonly List<AudioSource> audioSources = new List<AudioSource>();
    Camera mainCam;

    public const int MAXAUDIODISTANCE = 100;

    public static float Volume01 = 0.5f;
    public int samplerate = 44100;

    AudioSource nextAudioSource { get { return I.audioSources.Find(x => !x.isPlaying); } }

    readonly List<AudioSource> cameraAudioSources = new List<AudioSource>();
    AudioSource nextCameraAudioSource { get { return I.cameraAudioSources.Find(x => !x.isPlaying); } }

    // Start is called before the first frame update
    protected void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += delegate 
        {
            foreach(AudioSource s in I.cameraAudioSources)
                Destroy(s);
            I.cameraAudioSources.Clear();
            foreach (AudioSource s in I.audioSources)
                Destroy(s.gameObject);
            I.audioSources.Clear();

            I.mainCam = Camera.main; 
        };
        I.mainCam = Camera.main;
    }

    public AudioSource PlaySound(string name, string type, float volume01, Vector2 location, bool isUI, bool overrideDistanceCheck=false)
    {
        Vector2 offset = location - (Vector2)I.mainCam.transform.position;
        float sqrLen = offset.sqrMagnitude;

        if (!isUI && !overrideDistanceCheck && sqrLen < (volume01 * volume01) * (MAXAUDIODISTANCE * MAXAUDIODISTANCE))
            return null;

        if (CachedItems.cachedSounds.Exists(x => x.name == name))
        {
            var item = CachedItems.cachedSounds.Find(x => x.name == name).audioClip;
            if (isUI)
            {
                AudioSource audio;
                if (I.nextCameraAudioSource != null)
                {
                    audio = I.nextCameraAudioSource;
                    audio.PlayOneShot(item, volume01 * Volume01);
                }
                else
                {
                    audio = I.AddAudioSource(true);
                    audio.PlayOneShot(item, volume01 * Volume01);
                }
                return I.cameraAudioSources.Find(x => x.clip == item);
            }
            if (I.nextAudioSource != null)
            {
                var s = I.AddAudioSource();
                s.transform.position = location;

                I.nextAudioSource.PlayOneShot(item, volume01 * Volume01);
            }
            else
            {
                var s = I.AddAudioSource();
                s.transform.position = location;

                s.PlayOneShot(item, volume01 * Volume01);
            }
        }
        else
        {
            I.CacheAudio(name, type);
            I.PlaySound(name, type, volume01, location, isUI);
        }
        return audioSources.Find(x=>x.clip.name==name);
    }

    protected void Update()
    {
        transform.position = I.mainCam.transform.position; // i think
    }

    public CachedSound CacheAudio(string name, string type)
    {
        // todo: mods
        byte[] data = XMLLoader.Loaders.LoadBytes(Application.persistentDataPath + "\\audio\\" + type + "\\" + name);
        AudioClip clip = AudioClip.Create(name, data.Length, 1, I.samplerate, false);
        float[] samples = new float[clip.samples * clip.channels];
        for (int i = 0; i < data.Length; i++)
            samples[i] = data[i] / 255f; // wtf?

        clip.SetData(samples, 0);


        CachedSound item = new CachedItems.CachedSound(clip, name);
        cachedSounds.Add(item);
        if (type == "UI")
            UIManager.UISounds.Add(item);
        return item;
    }

    AudioSource AddAudioSource(bool camera=false)
    {
        if(!camera)
        { 
            GameObject o = new GameObject("AudioSource");
            var s = o.AddComponent<AudioSource>();
            audioSources.Add(s);
            return s;
        }
        else
        {
            var s = I.mainCam.gameObject.AddComponent<AudioSource>();
            cameraAudioSources.Add(s);
            return s;
        }
    }
}
