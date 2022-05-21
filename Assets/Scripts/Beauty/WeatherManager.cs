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
    public enum WeatherType { Snow, Rain, Thunderstorm, Clear }
    [SerializeField] private ParticleSystem rainPS;
    [SerializeField] private ParticleSystem snowPS;
    [SerializeField] private ParticleSystem lightningPS;

    Camera maincam;
    public static WeatherType currentWeather = WeatherType.Clear;

    [Range(-100,150)] public float currentTemperature;
    [Range(-15f, 15f)] public float temperatureChange;

    [Range(35,90)] public int minLengthSeconds;

    public bool isWeather;
    [SerializeField] private WindZone wind;
    int currentTime;

    List<Weather> sortedWeather = new List<Weather>();

    protected void Awake()
    {
        maincam = Camera.main;
        startWeather(WeatherType.Clear);
    }

    protected void Start() 
    { 
        currentTemperature = MapGenerator.currentBiome.locationData.averageTemperature;

        sortedWeather = new List<Weather>(MapGenerator.currentBiome.weatherFrequencies);
        sortedWeather.OrderBy(x => x.frequency);
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
            transform.position = new Vector2(maincam.transform.position.x,
                    maincam.transform.position.y + maincam.rect.yMax + 50);
            currentTime++;
        }
    }

    void startWeather(WeatherType type) // for transitions, just place the new one the delete the old one
    {
        switch(type)  // todo: this is pretty bad. use a list instead? ok let's be real im too lazy
        {
            case WeatherType.Clear:
                rainPS.Stop(true);
                snowPS.Stop(true);
                break;
            case WeatherType.Rain:
                if (currentTemperature <= 32)
                    goto case WeatherType.Snow; // i finally got to use this :)
                snowPS.Stop(false);
                rainPS.Play(false);
                break;
            case WeatherType.Snow:
                rainPS.Stop(false);
                snowPS.Play(false);
                break;
            case WeatherType.Thunderstorm:
                rainPS.Play(false);
                lightningPS.Play(false);
                snowPS.Stop(false);
                break;
        }

        //var scale = ps.shape.scale; // this shit doesn't work but this /SHOULD/ work on all monitors
        //scale.x = maincam.rect.width + (wind.windTurbulence * ps.externalForces.multiplier); // wind
        //scale.y = 50000;
    }

    protected void OnValidate()
    {
        startWeather(currentWeather);
        minLengthSeconds *= 50; // because peepee
    }
}
