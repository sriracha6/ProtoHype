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

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh }

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
    public string seed; // todo: int parse this as a string so you can put in seeds like "poopy" and "sex"
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

    public static int mapW;
    public static int mapH;

    public static Biome currentBiome;

    protected void Awake()
    {
        mapW = mapWidth;
        mapH = mapHeight;

        currentBiome = BiomeManager.Get("Plains");
        treeHeight = currentBiome.plantDensity;

        // -------------------------
        seed = Random.Range(int.MinValue, int.MaxValue).ToString();
        treesSeed = Random.Range(int.MinValue, int.MaxValue).ToString();
    }

    List<Vector2> treePoints = new List<Vector2>();

    protected void Start()
    {
        generateTestRoom(100, 100);
        mapBounds.resizeBounds(100, 100);
    }

    public void GenMap()
    {
        List<TerrainType> terrainTypes = currentBiome.terrainFrequencies.terrain;
        erosionAmount = Mathf.CeilToInt(mapWidth / 100);

        mapH = mapHeight;
        mapW = mapWidth;

        System.Random rand = new System.Random(seed.GetHashCode());

        float[,] noiseMap = RandomMap.genNoise(mapWidth, mapHeight, seed.GetHashCode(), noiseScale, octaves, persistence, lacunarity, offset)
            .carveRiver(generate1DPerlinNoise(seed.GetHashCode(), mapWidth, mapHeight, 7), deepSeaLevel)
            .carveWaterBody(deepSeaLevel, rand)
            .erodeNearWater(shallowSeaLevel, erosionAmount);

        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Color[] colorMap = new Color[mapWidth * mapHeight];

        for (int y =0; y<height;y++)
        {
            for(int x = 0; x<width; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < terrainTypes.Count; i++)
                {
                    if (currentHeight <= terrainTypes[i].height)
                    {
                        colorMap[y * mapWidth + x] = terrainTypes[i].color;
                        break;
                    }
                }
            }
        }

        if (drawMode == DrawMode.NoiseMap)
            currentTexture = TextureGenerator.textureFromHeightMap(noiseMap);
        else if (drawMode == DrawMode.ColorMap)
            currentTexture = TextureGenerator.texturePreviewFromMap(colorMap, mapWidth, mapHeight);
        else if (drawMode == DrawMode.Mesh)
            Debug.Log("not implemeneted");

        //List<WaterPoint> waterPoints =  TextureGenerator.perlinToTmap(noiseMap,terrainTypes,shallowSeaLevel,deepSeaLevel);

        TilemapPlace.placeTiles(noiseMap,terrainTypes.ToArray()); // for those wondering, this line cost me 7 days of work. because i forgot to put in the terraintypes of the current biome instead of the testing one in the unity editor.

        //TilemapPlace.Instance.placeTrees(generateTrees(), currentBiome.flora, rand, treeFab);
        generateWater();
        mapBounds.resizeBounds(mapWidth, mapHeight);
        
        new PathfindExtra();
    }

    private void generateWater()
    {
        foreach(Transform c in GameManager2D.Instance.groundTilemap.transform)
        {
            Destroy(c.gameObject); // kill kids (NOT LIKE THAT)
        }
        GameObject water = Instantiate(waterPrefab, GameManager2D.Instance.groundTilemap.transform);

        Mesh m = water.GetComponent<MeshFilter>().sharedMesh;
        Vector3 meshSize = m.bounds.size;
        
        float xScale = mapWidth / meshSize.x;
        float zScale = mapHeight / meshSize.z;

        water.transform.localScale = new Vector3(xScale, 1, zScale); // wtf this works for now
        water.transform.position = new Vector3(mapWidth/2, mapHeight/2, -11);
    }

    public static void generateTestRoom(int width, int height)
    {
        GameObject go = Instantiate(Loader.loader.testMat);
        go.transform.position = new Vector3(width/2,height/2,0);
        go.transform.localScale = new Vector3(width, height, 1);

        mapW = width;
        mapH = height;

        TilemapPlace.tilemap = new TerrainType[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                TilemapPlace.tilemap[x, y] = TerrainTypeManager.Get("Test Tile");
            }
        }

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
