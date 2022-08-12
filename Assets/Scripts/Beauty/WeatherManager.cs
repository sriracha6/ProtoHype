using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;
using Nature;

/// <summary>
/// im very stupid, just make this position itself right above the camera instead of the map
/// </summary>
public class WeatherManager : MonoBehaviour
{
    public static WeatherManager I = null;

    public enum WeatherType { Snow=2, Rain=1, Thunderstorm=3, Clear=0 }
    [SerializeField] private ParticleSystem rainPS;
    [SerializeField] private ParticleSystem snowPS;
    [SerializeField] private ParticleSystem lightningPS;

    Camera maincam;
    public static WeatherType currentWeather = WeatherType.Clear;
    public static WeatherType weatherQueue = WeatherType.Clear;

    [Range(-100,150)] public float currentTemperature;
    [Range(-15f, 15f)] public float temperatureChange;

    [Range(35,90)] public int minLengthSeconds;

    public bool isWeather;
    [SerializeField] private WindZone wind;
    int currentTime;

    List<Weather> sortedWeather = new List<Weather>();
    AudioSource rainSound;

    protected void Awake()
    {
        maincam = Camera.main;
        if (I == null)
        {
            startWeather(WeatherType.Clear);
            isWeather = false;
            I = this;
        }
        else if (Menus.I.inBattle)
        {
            I.rainPS = rainPS;
            I.lightningPS = lightningPS;
            I.snowPS = snowPS;
            I.wind = wind;
            I.maincam = Camera.main;
            I.temperatureChange = temperatureChange;
            I.minLengthSeconds = minLengthSeconds;

            I.startWeather(weatherQueue);
        }
    }

    protected void Start() 
    {
        if (MapGenerator.I.isTestMap)
        {
            isWeather = false;
            return;
        }
        if (Menus.I.inBattle)
        {
            currentTemperature = MapGenerator.I.currentBiome.locationData.averageTemperature + Random.Range(-15, 16);
            //currentTemperature = 31;

            if (currentTemperature <= 32)
                MapGenerator.I.water.GetComponent<MeshRenderer>().material = WCMngr.I.iceMat;

            if (currentTemperature >= 90)
            {
                var g = Instantiate(WCMngr.I.heatDistortBlock);
                g.transform.localScale = new Vector3(MapGenerator.I.mapWidth*2, MapGenerator.I.mapHeight*2);
                g.transform.position = Vector3.zero;
            }
            sortedWeather = new List<Weather>(MapGenerator.I.currentBiome.weatherFrequencies);
            sortedWeather.OrderBy(x => x.frequency);
        }
    }

    protected void FixedUpdate()
    {
        if (currentTime >= minLengthSeconds)
        {
            float num = Random.Range(0f, 100f);

            for(int i = 0; i < sortedWeather.Count; i++)
            {
                if (num <= sortedWeather[i].frequency)
                    startWeather(sortedWeather[i].type);
            }

            currentTime = 0;
            minLengthSeconds += Random.Range(-minLengthSeconds,minLengthSeconds); // 50 calls per second
            minLengthSeconds = Mathf.Clamp(minLengthSeconds, 3000, 50000);
            //currentTemperature = Mathf.Clamp(currentTemperature, -100, 150) + Mathf.Sin(Time.time) * temperatureChange;
            isWeather = true; // we put this here so that pausing doesn't scerw things up
        }
        if (isWeather)
        {
            if (rainPS != null)
            {
                rainPS.transform.localScale = new Vector3(maincam.rect.width*20, 1, 1); 
                snowPS.transform.localScale = new Vector3(maincam.rect.width*20, 1, 1);
                lightningPS.transform.localScale = new Vector3(maincam.rect.width*20, 1, 1);
            }

            transform.position = new Vector2(maincam.transform.position.x + maincam.rect.width,
                    maincam.transform.position.y + maincam.rect.yMax + 50);
            currentTime++;
        }
    }

    void startWeather(WeatherType type) // for transitions, just place the new one the delete the old one
    {
        if (rainPS == null || snowPS == null && type == WeatherType.Clear) return;
        switch(type)
        {
            case WeatherType.Clear:
                rainPS.Stop(true);
                snowPS.Stop(true);
                if (rainSound != null)
                {
                    rainSound.Stop();
                    rainSound.loop = false;
                }
                break;
            case WeatherType.Rain:
                if (currentTemperature <= 32)
                    goto case WeatherType.Snow; // i finally got to use this :)
                rainSound = SFXManager.I.PlaySound("rain.mp3", "Weather", 1f, Vector2.zero, true);
                rainSound.loop = true;
                snowPS.Stop(false);
                rainPS.Play(false);
                break;
            case WeatherType.Snow:
                rainPS.Stop(false);
                snowPS.Play(false);
                if (rainSound != null)
                {
                    rainSound.Stop();
                    rainSound.loop = false;
                }
                break;
            case WeatherType.Thunderstorm:
                rainPS.Play(false);
                lightningPS.Play(false);
                snowPS.Stop(false);
                rainSound = SFXManager.I.PlaySound("rain.mp3", "Weather", 1f, Vector2.zero, true);
                rainSound.loop = true;
                break;
        }

        //var scale = ps.shape.scale; // this shit doesn't work but this /SHOULD/ work on all monitors
        //scale.x = maincam.rect.width + (wind.windTurbulence * ps.externalForces.multiplier); // wind
        //scale.y = 50000;
    }
}
