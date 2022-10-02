using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    CachedSound item;
    public AudioImporter importer;

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
            var item = CachedItems.cachedSounds.Find(x => x.name == name && x.type == type).audioClip;
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
                    I.AddAudioSource(true);
                    audio = I.nextCameraAudioSource;
                    audio.PlayOneShot(item, volume01 * Volume01);
                }
                return audio;//I.cameraAudioSources.Find(x => x.clip == item);
            }
            if (I.nextAudioSource != null)
            {
                var s = I.AddAudioSource();
                s.transform.position = location;

                I.nextAudioSource.PlayOneShot(item, volume01 * Volume01);
                return s;
            }
            else
            {
                var s = I.AddAudioSource();
                s.transform.position = location;

                s.PlayOneShot(item, volume01 * Volume01);
                return s;
            }
        }
        else
        {
            I.doneCaching = false;
            StartCoroutine(I.CacheAudio(name, type));
            while(!I.doneCaching) { }
            I.doneCaching = true;
            return I.PlaySound(name, type, volume01, location, isUI);
        }
    }

    protected void Update()
    {
        transform.position = I.mainCam.transform.position; // i think
    }

    static void Load(string path)
    {
        I.importer.Import(path);
    }

    bool doneCaching = false;

    public IEnumerator CacheAudio(string name, string type)
    {
        Load(Application.persistentDataPath + "\\audio\\" + type + "\\" + name);
        //yield return new WaitUntil(() => I.importer.isInitialized);
        I.Create(name, type);
        yield return null;
    }

    void Create(string name, string type)
    {
        Debug.Log($"here");
        AudioClip clip = I.importer.audioClip;
        I.item = new CachedSound(clip, name, type);
        if (type == "UI")
            UIManager.UISounds.Add(item);
        I.doneCaching = true;
        cachedSounds.Add(I.item);
    }

    AudioSource AddAudioSource(bool camera=false)
    {
        if(!camera)
        { 
            GameObject o = new GameObject("AudioSource");
            var s = o.AddComponent<AudioSource>();
            I.audioSources.Add(s);
            return s;
        }
        else
        {
            var s = I.mainCam.gameObject.AddComponent<AudioSource>();
            I.cameraAudioSources.Add(s);
            return s;
        }
    }
}
