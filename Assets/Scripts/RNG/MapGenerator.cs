using RNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MeshGenerator;
using static TextureGenerator;
using static WaterFeature;
using static TilemapPlace;
using Nature;
using PawnFunctions;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Place }

    public static MapGenerator I = null;

    [Header("General Settings")]
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public bool autoUpdate;
    public DrawMode drawMode;
    //public TerrainType[] terrainTypes;
    [Space]
    [Range(0f,1f)]public float shallowSeaLevel;
    [Range(0f,1f)]public float deepSeaLevel;
    [Range(0f, 1f)] public float mountainHeight;
    [Space]
    [Header("Noise Settings")]
    public string seed;
    [Space]
    public Vector2 offset;
    public int octaves;
    [Range(0f,1f)]
    public float persistence;
    public float lacunarity;
    public int erosionAmount;
    public float meshHeightMultiplier;
    [Space]
    [Header("Trees")]
    public string treesSeed;
    [Range(0f,10f)] public float radius;
    public int rejectionSamples = 30;
    [Range(0f,1f)] public float treeHeight;

    [Space][Space][Space]
    [Header("Components")]
    [SerializeField] CameraMove mapBounds;
    public Texture2D currentTexture;
    [SerializeField] private PositionSun sun;
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private GameObject treeFab; // GET IT??? TREEFAB???? AHAAHAHAHAHAHAAHAHAHAHAHA

    public int riverCount;
    public int seasidesCount;

    public float riverPerturbation;

    public int minRiverWidth;
    public int maxRiverWidth;

    public bool isTestMap = false; // todo : should be false by default. lol
    public Biome currentBiome;

    public static float[,] noiseMap;
    public static List<TerrainType> terrainTypes;

    public System.Random rand;

    protected void Awake()
    {
        if (!isTestMap)
        {
            currentBiome = Biome.Get("Plains");
            treeHeight = currentBiome.plantDensity;
        }

        // -------------------------
        //seed = Random.Range(int.MinValue, int.MaxValue).ToString();
        //treesSeed = Random.Range(int.MinValue, int.MaxValue).ToString();
        DontDestroyOnLoad(gameObject);
        if (I == null) // Main Menu
            I = this;
        else if (I != null)// We have loaded the battle scene. We aren't on the main menu.
        {
            I.mapBounds = mapBounds;
            I.sun = sun;
            drawMode = DrawMode.Place;
            DrawMap();
        }
    }

    List<Vector2> treePoints = new List<Vector2>();

    protected void Start()
    {
        if (isTestMap)
        {
            generateTestRoom(100, 100);
            mapBounds.resizeBounds(100, 100);
        }
    }

    public long ParseSeed(string seed)
    {
        if (long.TryParse(seed, out long longseed))
            return longseed;

        long s = 0;
        foreach(char c in seed)
        {
            s += c.GetHashCode();
        }
        System.Random seeder = new System.Random((int)s);
        return LongRandom(long.MinValue, long.MaxValue, seeder);
    }

    long LongRandom(long min, long max, System.Random rand)
    {
        long result = rand.Next((int)(min >> 32), (int)(max >> 32));
        result = (result << 32);
        result = result | (long)rand.Next((int)min, (int)max);
        return result;
    }

    public void GenMap()
    {
        terrainTypes = currentBiome.terrainFrequencies.terrain;
        erosionAmount = Mathf.CeilToInt(mapWidth / 100);

        //long _seed = ParseSeed(seed);
        if (!int.TryParse(seed, out int _seed))
            _seed = seed.GetHashCode();

        rand = new System.Random(_seed);

        noiseMap = RandomMap.genNoise(mapWidth, mapHeight, _seed, noiseScale, octaves, persistence, lacunarity, offset)
            .carveRiver(deepSeaLevel, rand, riverPerturbation, false)
            .carveWaterBody(deepSeaLevel, rand, false);

        for (int i = 0; i < riverCount; i++)
            noiseMap.carveRiver(deepSeaLevel, rand, riverPerturbation, always:true);
        for (int i = 0; i < seasidesCount; i++)
            noiseMap.carveWaterBody(deepSeaLevel, rand, overrideRandom: true);

        //noiseMap.erodeNearWater(shallowSeaLevel, erosionAmount);

        new PathfindExtra();
        DrawMap();
    }

    private void DrawMap()
    {
        if (drawMode == DrawMode.NoiseMap)
        {
            currentTexture = TextureGenerator.textureFromHeightMap(noiseMap);
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            int width = noiseMap.GetLength(0);
            int height = noiseMap.GetLength(1);

            Color[] colorMap = new Color[mapWidth * mapHeight];

            TerrainType[] tts = terrainTypes.OrderBy(x => x.height).ToArray();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float currentHeight = noiseMap[x, y];
                    for (int i = 0; i < terrainTypes.Count; i++)
                    {
                        if (currentHeight <= tts[i].height)
                        {
                            colorMap[y * mapWidth + x] = tts[i].color;
                            break;
                        }
                    }
                }
            }
            currentTexture = TextureGenerator.texturePreviewFromMap(colorMap, mapWidth, mapHeight);
        }
        else if (drawMode == DrawMode.Place)
        {
            TilemapPlace.UpdateTilemap(noiseMap, terrainTypes.ToArray(), true, currentBiome); // for those wondering, this line cost me 7 days of work. because i forgot to put in the terraintypes of the current biome instead of the testing one in the unity editor.
            generateWater();
            mapBounds.resizeBounds(mapWidth, mapHeight);
            //TilemapPlace.Instance.placeTrees(generateTrees(), currentBiome.flora, rand, treeFab);
        }
    }

    private void generateWater()
    {
        foreach(Transform c in WCMngr.I.groundTilemap.transform)
            Destroy(c.gameObject); // kill kids (NOT LIKE THAT)
        
        GameObject water = Instantiate(waterPrefab, WCMngr.I.groundTilemap.transform);

        Mesh m = water.GetComponent<MeshFilter>().sharedMesh;
        Vector3 meshSize = m.bounds.size * 2;
        
        float xScale = (mapWidth * 2) / meshSize.x;
        float zScale = (mapHeight * 2) / meshSize.z;

        water.transform.localScale = new Vector3(xScale*2, 1, zScale*2);
        water.transform.position = new Vector3(mapWidth+1, mapHeight+1, 11);
    }

    public static void generateTestRoom(int width, int height)
    {
        I.isTestMap = true;
        GameObject go = Instantiate(WCMngr.I.testMat);
        go.transform.position = new Vector3(width/2,height/2,0);
        go.transform.localScale = new Vector3(width, height, 1);

        I.mapWidth = width;
        I.mapHeight = height;

        TilemapPlace.tilemap = new TerrainType[width, height];
        for(int x = 0; x < width; x++)
            for(int y = 0; y < height; y++)
                TilemapPlace.tilemap[x, y] = TerrainType.Get("Test Tile");

        new PathfindExtra();
    }

    private void OnValidate() // i love this
    {
        if (mapWidth<1)
            mapWidth = 1;
        if(mapHeight<1)
            mapHeight = 1;
        if(lacunarity<1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
    }

    // todo: this is so ineffcient for some reasonn
    /// <summary>
    /// this shit fucking sucks. im not doing this. doesnt really fit the game anyway. maybe another update bc fuck
    /// </summary>
    /// <returns></returns>
    public List<Vector3Int> generateTrees() // i promies this is the most inefficient algorithm in the entire project
    {
        float[,] treeNoiseMap = RandomMap.genNoise(mapWidth, mapHeight, treesSeed.GetHashCode().GetHashCode(), noiseScale, octaves, persistence, lacunarity, offset);
        treePoints = TreeSampling.generatePoints(radius, new Vector2(mapWidth,mapHeight), rejectionSamples);
        //Debug.Break();
        List<Vector3Int> points = new List<Vector3Int>();

        for (int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight;y++)
            {
                if(treeNoiseMap[x, y] <= treeHeight && treePoints.Contains(new Vector2(x,y)) 
                    )//&& Player.tmap.GetTile(new Vector3Int(x,y,0)) != null) // sex moment
                {
                    points.Add(new Vector3Int(x,y,0));
                }
            }
        }
        return points;
    }
}
