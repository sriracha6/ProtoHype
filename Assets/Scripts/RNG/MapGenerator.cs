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
using Structures;
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

    public bool isTestMap = false;
    public Biome currentBiome;

    public static float[,] noiseMap;
    public static List<TerrainType> terrainTypes;

    public System.Random rand;

    public Structure structure;
    public Vector2Int structurePos;
    public Vector2Int structureSize;

    public GameObject water;

    public static bool isMountainless;

    public bool finishedLoading;
    [SerializeField] internal Tilemap groundTmap;
    [SerializeField] internal Tilemap solidTmap;
    [SerializeField] internal Tilemap roofTmap;
    [SerializeField] internal Tilemap specialTilemap;
    int why = 0;

    public void GenerateRandomMountainHeight()
    {
        int num = Random.Range(0,101);
        if (num >= 50) // no mountains
        {
            BiomeArea.mountainHeight = terrainTypes.OrderBy(x=>x.height).Last().height;
            isMountainless = true;
        }
    }

    protected void Awake()
    {
        if (!isTestMap && Menus.I != null && Menus.I.inBattle)
            treeHeight = currentBiome.plantDensity;

        if (I == null) // Main Menu
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //if (Menus.I == null) return;
            //else Debug.Log($"continuung;;");
            //I.why++;
            //if (I.why == 1) return;
            if (Menus.I.inBattle || Menus.I.inSC)
            {
                I.mapBounds = mapBounds;
                I.sun = sun;
                I.drawMode = DrawMode.Place;
                I.DrawMap();
                Destroy(gameObject);
                //if (UIManager.Reloads > 1)
                //else
                //DontDestroyOnLoad(gameObject);
            }
        };
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
        result <<= 32;
        result |= (long)rand.Next((int)min, (int)max);
        return result;
    }

    public void GenMap()
    {
        if (currentBiome == null) return;
        terrainTypes = new List<TerrainType>();
        terrainTypes.AddRange(currentBiome.terrainFrequencies.terrain);
        BiomeArea.waterHeight = currentBiome.waterComminality * BiomeArea.DEFAULT_WATER_HEIGHT;
        //GenerateRandomMountainHeight();
        terrainTypes.Add(new TerrainType("Water", "", BiomeArea.waterHeight, Color.blue, WCMngr.I.mountainTile, SpecialType.Water, false));
        terrainTypes.Add(new TerrainType("Mountain", "", BiomeArea.mountainHeight, new Color(256, 100, 100), WCMngr.I.mountainTile, SpecialType.Mountain, false));
        erosionAmount = Mathf.CeilToInt(I.mapWidth / 100);

        //long _seed = ParseSeed(seed);
        if (!int.TryParse(seed, out int _seed))
            _seed = seed.GetHashCode();

        rand = new System.Random(_seed);

        noiseMap = RandomMap.genNoise(I.mapWidth, I.mapHeight, _seed, noiseScale, octaves, persistence, lacunarity, offset, currentBiome)
            .carveRiver(rand, riverPerturbation, false)                
            .carveWaterBody(rand, false);                              
                                                                       
        for (int i = 0; i < riverCount; i++)                           
            noiseMap.carveRiver(rand, riverPerturbation, always:true); 
        for (int i = 0; i < seasidesCount; i++)                        
            noiseMap.carveWaterBody(rand, overrideRandom: true);       
                                                                       
        //noiseMap.erodeNearWater(shallowSeaLevel, erosionAmount);
        new PathfindExtra();
        DrawMap();
    }
    private void DrawMap()
    {
        if (I.drawMode == DrawMode.NoiseMap)
        {
            currentTexture = TextureGenerator.textureFromHeightMap(noiseMap);
            TilemapPlace.UpdateTilemap(noiseMap, terrainTypes.ToArray(), false); // for those wondering, this line cost me 7 days of work. because i forgot to put in the terraintypes of the current biome instead of the testing one in the unity editor.
        }
        else if (I.drawMode == DrawMode.ColorMap)
        {
            int width = noiseMap.GetLength(0);
            int height = noiseMap.GetLength(1);

            Color32[] colorMap = new Color32[I.mapWidth * I.mapHeight];

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
                            colorMap[y * I.mapWidth + x] = tts[i].color;
                            break;
                        }
                    }
                }
            }
            TilemapPlace.UpdateTilemap(noiseMap, terrainTypes.ToArray(), false); // for those wondering, this line cost me 7 days of work. because i forgot to put in the terraintypes of the current biome instead of the testing one in the unity editor.
            currentTexture = TextureGenerator.texturePreviewFromMap(colorMap, I.mapWidth, I.mapHeight);
        }
        else if (I.drawMode == DrawMode.Place)
        {
            Loading.I.Status = "Creating the world...";
            TilemapPlace.UpdateTilemap(noiseMap, terrainTypes.ToArray(), true); // for those wondering, this line cost me 7 days of work. because i forgot to put in the terraintypes of the current biome instead of the testing one in the unity editor.
            generateWater();
            I.mapBounds.resizeBounds(I.mapWidth, I.mapHeight);
            I.mapBounds.transform.position = new Vector3(I.mapWidth, I.mapHeight, -10); // position the camera in middle of scene. no /2 because cell size = 2
            WCMngr.I.solidTilemap.size = new Vector3Int(I.mapWidth, I.mapHeight, 1);
            WCMngr.I.solidTilemap.ResizeBounds();

            if(structure == null)
                TilemapPlace.UpdateBuildings();

            WCMngr.I.solidTilemap.RefreshAllTiles();
            WCMngr.I.groundTilemap.RefreshAllTiles();
            
            WCMngr.I.groundTilemap.RefreshAllTiles();

            //TilemapPlace.Instance.placeTrees(generateTrees(), currentBiome.flora, rand, treeFab);
        }
        I.finishedLoading = true;
    }

    private void generateWater()
    {
        foreach(Transform c in WCMngr.I.groundTilemap.transform)
            Destroy(c.gameObject); // kill kids (NOT LIKE THAT)
        
        GameObject water = Instantiate(waterPrefab, WCMngr.I.groundTilemap.transform);
        I.water = water;

        Mesh m = water.GetComponent<MeshFilter>().sharedMesh;
        Vector3 meshSize = m.bounds.size * 2;
        
        float xScale = (I.mapWidth * 2) / meshSize.x;
        float zScale = (I.mapHeight * 2) / meshSize.z;

        water.transform.localScale = new Vector3(xScale*2, 1, zScale*2);
        water.transform.position = new Vector3(I.mapWidth+1, I.mapHeight+1, 11);
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

    // todo: this is so ineffcient for some reasonn
    /// <summary>
    /// this shit fucking sucks. im not doing this. doesnt really fit the game anyway. maybe another update bc fuck
    /// </summary>
    /// <returns></returns>
    public List<Vector3Int> generateTrees() // i promies this is the most inefficient algorithm in the entire project
    {
        float[,] treeNoiseMap = RandomMap.genNoise(I.mapWidth, I.mapHeight, treesSeed.GetHashCode(), noiseScale, octaves, persistence, lacunarity, offset, currentBiome);
        treePoints = TreeSampling.generatePoints(radius, new Vector2(I.mapWidth,I.mapHeight), rejectionSamples);
        //Debug.Break();
        List<Vector3Int> points = new List<Vector3Int>();

        for (int x = 0; x < I.mapWidth; x++)
        {
            for(int y = 0; y < I.mapHeight;y++)
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

    public void ResizeMapToFit(Vector2Int size)
    {
        int ra = rand.Next(0, 25);
        I.mapWidth = size.x + 45 + ra;
        I.mapHeight = size.y + 45 + ra;
        if (I.mapWidth > I.mapHeight) I.mapHeight = I.mapWidth;
        else I.mapWidth = I.mapHeight;
        I.GenMap();
        TilemapPlace.UpdateBuildings();
    }    
}