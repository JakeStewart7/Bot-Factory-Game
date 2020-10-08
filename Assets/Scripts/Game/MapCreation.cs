using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapCreation : MonoBehaviour
{
    public int MapLength = 30;
    public int MapHeight = 15;
    Vector2 wallSize;

    public GameObject AllyBasePrefab;
    public GameObject AllySmallFactoryPrefab;
    public GameObject AllyRetrieverFactoryPrefab;

    public GameObject EnemyBasePrefab;
    public GameObject EnemyFactoryPrefab;
    public GameObject EnemySmallPrefab;

    public GameObject WallPrefab;
    public GameObject ScrapPrefab;
    public GameObject ScrapDispenserPrefab;

    public Image BackgroundBlueImage;
    public Image BackgroundRedImage;

    GameObject allyBase;
    GameObject enemyBase;

    public int AllySmallFactoryCount = 4;  // Must be even, symmetrical and based off of even count
    public int AllyRetrieverFactoryCount = 2;  // Must be even, symmetrical and based off of even count
    public int EnemyFactoryCount = 6;

    public int startingScrapCount = 10;
    public int startingEnemyCount = 5;

    // Start is called before the first frame update
    void Start()
    {
        wallSize = WallPrefab.GetComponent<SpriteRenderer>().bounds.size;
        createWalls();
        createBases();
        createFactories();
        createScrapDispensers();
        
        spawnPlayers();
        spawnEnemyUnits();
        spawnScrap();

        BackgroundBlueImage.GetComponent<RectTransform>().sizeDelta = new Vector2((MapLength - 1) / 2 * wallSize.x, MapHeight * wallSize.y);
        BackgroundBlueImage.transform.position = new Vector3(-MapLength / 4 * wallSize.x, 0, 0);

        BackgroundRedImage.GetComponent<RectTransform>().sizeDelta = new Vector2((MapLength - 1) / 2 * wallSize.x, MapHeight * wallSize.y);
        BackgroundRedImage.transform.position = new Vector3(MapLength / 4 * wallSize.x, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* ======================== WALLS ======================== */
    void createWalls()
    {
        Vector2 topLeftBlockPosition = new Vector2(-(MapLength - 1) * ((wallSize.x / 2)), (MapHeight - 1) * ((wallSize.y) / 2));
        Vector2 bottomLeftBlockPosition = new Vector2(-(MapLength - 1) * (wallSize.x / 2), -(MapHeight - 1) * ((wallSize.y) / 2));

        Vector2 topRightBlockPosition = new Vector2(-bottomLeftBlockPosition.x, -bottomLeftBlockPosition.y);
        
        createHorizontalWalls(topLeftBlockPosition);    // Create top walls
        createHorizontalWalls(bottomLeftBlockPosition); // Create bottom walls
        createVerticalWalls(topLeftBlockPosition - new Vector2(0, wallSize.y)); // Left wall; Prevent recreating top left block by starting 1 block down
        createVerticalWalls(topRightBlockPosition - new Vector2(0, wallSize.y)); // Right wall; Prevent recreating top right block by starting 1 block down
    }

    // Create walls horizontally (top and bottom walls) starting from the left side
    void createHorizontalWalls(Vector2 startPosition)
    {
        for (int i = 0; i < MapLength; i++)
        {
            GameObject wall = Instantiate(WallPrefab, startPosition + new Vector2(wallSize.x * i, 0), Quaternion.identity, GameObject.Find("Walls").transform);
            wall.name = "Wall";
        }
    }

    // Create walls vertically (left and right walls) starting from the top side
    void createVerticalWalls(Vector2 startPosition)
    {
        for (int i = 0; i < MapHeight - 2; i++)  // MapHeight - 2 because it starts 1 wall below top left/right block and finishes 1 above bottom left/right
        {
            GameObject wall = Instantiate(WallPrefab, startPosition - new Vector2(0, wallSize.y * i), Quaternion.identity, GameObject.Find("Walls").transform);
            wall.name = "Wall";
        }
    }
    /* ======================================================= */


    /* ======================== BUILDINGS ======================== */
    void createBases()
    {
        Vector2 midLeftBlockPosition = new Vector2(-(MapLength - 1) * ((wallSize.x / 2)), 0);
        Vector2 midRightBlockPosition = new Vector2((MapLength - 1) * ((wallSize.x / 2)), 0);

        allyBase = Instantiate(AllyBasePrefab, midLeftBlockPosition + new Vector2((wallSize.x * 5), 0),
                        Quaternion.identity, GameObject.Find("AllyBuildings").transform);
        allyBase.name = "Ally Base";

        enemyBase = Instantiate(EnemyBasePrefab, midRightBlockPosition - new Vector2((wallSize.x * 5), 0),
                        Quaternion.identity, GameObject.Find("EnemyBuildings").transform);
        enemyBase.name = "Enemy Base";
    }

    // ======================== Factories ======================== //
    void createFactories()
    {
        createAllyFactories();  // AllySmallFactories and AllyRetrieverFactories
        createEnemyFactories();
    }

    void createAllyFactories()
    {
        int spawnDirection = 1;

        for (int i = 0; i < AllySmallFactoryCount; i++)
        {
            if      (i % 2 == 0)  { spawnDirection = 1; }  // First, Third, etc. building spawns above base
            else if (i % 2 == 1)  { spawnDirection = -1; } // Second, Fourth, etc. building spawns below base
            float yDistance = (((i / 2) + 1) * AllySmallFactoryPrefab.GetComponent<SpriteRenderer>().bounds.size.y * 1.3f +
                                               allyBase.GetComponent<SpriteRenderer>().bounds.size.y / 3f) * spawnDirection;
            Vector3 factoryOffset = new Vector3(-0f + (i/2) * 2f, yDistance, 0);
            GameObject factory = Instantiate(AllySmallFactoryPrefab, allyBase.transform.position + factoryOffset, 
                                                Quaternion.identity, GameObject.Find("AllyBuildings").transform);
            factory.name = "Ally Small Factory";
        }

        for (int i = AllySmallFactoryCount; i < AllySmallFactoryCount + AllyRetrieverFactoryCount; i++)  // Continue from AllySmallFactories
        {
            if      (i % 2 == 0)  { spawnDirection = 1; }  // First, Third, etc. building spawns above base
            else if (i % 2 == 1)  { spawnDirection = -1; } // Second, Fourth, etc. building spawns below base
            float yDistance = (((i / 2) + 1) * AllySmallFactoryPrefab.GetComponent<SpriteRenderer>().bounds.size.y * 1.3f +
                                               allyBase.GetComponent<SpriteRenderer>().bounds.size.y / 3f) * spawnDirection;
            Vector3 factoryOffset = new Vector3(-0f + (i/2) * 2f, yDistance, 0); // Switch signs to change direction of factory positions
            GameObject factory = Instantiate(AllyRetrieverFactoryPrefab, allyBase.transform.position + factoryOffset, 
                                                Quaternion.identity, GameObject.Find("AllyBuildings").transform);
            factory.name = "Ally Retriever Factory";
        }
    }

    void createEnemyFactories()
    {
        int spawnDirection = 1;

        for (int i = 0; i < EnemyFactoryCount; i++)
        {
            if      (i % 2 == 0)  { spawnDirection = 1; }  // First, Third, etc. building spawns above base
            else if (i % 2 == 1)  { spawnDirection = -1; } // Second, Fourth, etc. building spawns below base
            float yDistance = (((i / 2) + 1) * EnemyFactoryPrefab.GetComponent<SpriteRenderer>().bounds.size.y * 1.1f +
                                               enemyBase.GetComponent<SpriteRenderer>().bounds.size.y / 3.5f) * spawnDirection;
            Vector3 factoryOffset = new Vector3(0f - (i/2) * 2f, yDistance, 0);  // Switched signs from ally
            GameObject factory = Instantiate(EnemyFactoryPrefab, enemyBase.transform.position + factoryOffset, Quaternion.identity, GameObject.Find("EnemyBuildings").transform);
            factory.name = "Enemy Factory";
        }
    }

    // ======================== ScrapDispenser ======================== //
    void createScrapDispensers()
    {
        float xDistance = MapLength * wallSize.x / 8;  // Divide each side into 4ths
        float yDistance = MapHeight * wallSize.y / 8;
        float xMin = 1f;
        float yMin = 1.5f;
        float xMax = 2f;
        float yMax = 2.6f;

        Vector3[] v = new Vector3[] { new Vector3 (1, 1, 0), new Vector3 (1, -1, 0), 
                                      new Vector3 (-1, 1, 0), new Vector3 (-1, -1, 0) };
        for (int i = 0; i < 4; i++)
        {
            float xRand = Random.Range(xMin, xMax);
            float yRand = Random.Range(yMin, yMax);

            Vector3 randPosition = new Vector3(xRand * xDistance, yRand * yDistance, 0);
            randPosition = Vector3.Scale(randPosition, v[i]);

            GameObject scrapDispenser = Instantiate(ScrapDispenserPrefab, randPosition, Quaternion.identity, GameObject.Find("NeutralBuildings").transform);
            scrapDispenser.name = "Scrap Dispenser";
        }  
    }

    /* ================================================================ */

    /* ========================= UNITS/SCRAP ========================== */
    void spawnPlayers()
    {
        Vector3 genericPosition = allyBase.transform.position + new Vector3 (allyBase.GetComponent<SpriteRenderer>().bounds.size.x / 2 + 2f, 0, 0);
        Vector3 position1 = genericPosition + new Vector3(0, 2f, 0);
        Vector3 position2 = genericPosition + new Vector3(0, -2f, 0);

        GetComponent<PlayerSpawner>().SpawnPlayer(position1, 1);  // Player 1
        GetComponent<PlayerSpawner>().SpawnPlayer(position2, 2);  // Player 2
    }

    void spawnEnemyUnits()
    {

        float xDistance = MapLength * wallSize.x / 8;  // Divide each side into 4ths
        float yDistance = MapHeight * wallSize.y / 8;
        float xMin = 0f;
        float yMin = 0f;
        float xMax = 0.7f;
        float yMax = 3.2f;

        Vector3[] v = new Vector3[] { new Vector3 (1, 1, 0), new Vector3 (1, -1, 0) }; // Stay on enemy side

        for (int i = 0; i < startingEnemyCount; i++)
        {
            float xRand = Random.Range(xMin, xMax);
            float yRand = Random.Range(yMin, yMax);
            int quadrant = Random.Range(0, 2);

            Vector3 randPosition = new Vector3(enemyBase.transform.position.x - (enemyBase.GetComponent<SpriteRenderer>().sprite.bounds.size.x / 4) - xRand * xDistance,
                                               yRand * yDistance, 0);
            randPosition = Vector3.Scale(randPosition, v[quadrant]);

            GameObject enemy = Instantiate(EnemySmallPrefab, randPosition, Quaternion.identity, GameObject.Find("EnemyUnits").transform);
            enemy.name = "Enemy Small";
        }
    }

    void spawnScrap()
    {
        float xDistance = MapLength * wallSize.x / 8;  // Divide each side into 4ths
        float yDistance = MapHeight * wallSize.y / 8;
        float xMin = 0f;
        float yMin = 0f;
        float xMax = 2.3f;
        float yMax = 3.6f;

        Vector3[] v = new Vector3[] { new Vector3 (1, 1, 0), new Vector3 (1, -1, 0), 
                                      new Vector3 (-1, 1, 0), new Vector3 (-1, -1, 0) };

        for (int i = 0; i < startingScrapCount; i++)
        {
            float xRand = Random.Range(xMin, xMax);
            float yRand = Random.Range(yMin, yMax);
            int quadrant = Random.Range(0, 4);

            Vector3 randPosition = new Vector3(xRand * xDistance, yRand * yDistance, 0);
            randPosition = Vector3.Scale(randPosition, v[quadrant]);

            GameObject scrap = Instantiate(ScrapPrefab, randPosition, Quaternion.identity);
            scrap.name = "Scrap";
        }
    }
    /* =============================================================== */
}
