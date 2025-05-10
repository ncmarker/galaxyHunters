using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    public Tilemap spawnArea;
    public float spawnTimer = 0.25f;
    private float spawnTime;
    private float levelCooldownTime = 5.0f;
    private bool levelCooldownStarted = false;

    public Transform minSpawn;
    public Transform maxSpawn; 
    private int currentLevel = 1;
    private int numEnemiesPerSpawn = 8;
    private bool isSpawning = false;
    private int enemiesSpawned = 0;
    private int totalEnemiesToSpawn = 0;
    private Dictionary<int, List<(EnemyTypeID, int)>> enemyWaves;
    private Queue<(EnemyTypeID, int)> spawnQueue = new Queue<(EnemyTypeID, int)>();
    private bool[] bossCheckPoints = { false, false, false };
    private int eggsDestroyed = 0;

    // for valid spawn points
    private List<Vector3> validSpawnPoints = new List<Vector3>();


    void Start()
    {
        spawnTime = spawnTimer;
        InitializeEnemyWaves();
        StartLevel(currentLevel);

        CacheValidSpawnPoints();
    }

    void Update()
    {        
        if (isSpawning && spawnQueue.Count > 0) 
        {
            spawnTime -= Time.deltaTime;
            if (spawnTime <= 0)
            {
                spawnTime = spawnTimer;

                for (int i=0; i<numEnemiesPerSpawn; i++) SpawnEnemy();
            }
        }
    }

    // creates dictionary of enemy waves for each level form of -> level : [(enemy type, count)]
    private void InitializeEnemyWaves()
    {
        enemyWaves = new Dictionary<int, List<(EnemyTypeID, int)>>
        {
            {1, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 10)}},
            {2, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 20)}},
            {3, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 20), (EnemyTypeID.SPEED_ENEMY, 5)}},
            {4, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 20), (EnemyTypeID.SPEED_ENEMY, 10)}},
            {5, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 30), (EnemyTypeID.SPEED_ENEMY, 10), (EnemyTypeID.STRONG_ENEMY, 5)}},
            {6, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 30), (EnemyTypeID.SPEED_ENEMY, 10), (EnemyTypeID.STRONG_ENEMY, 10)}},
            {7, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 40), (EnemyTypeID.TANK_ENEMY, 15), (EnemyTypeID.SPEED_ENEMY, 15)}},
            {8, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 70), (EnemyTypeID.TANK_ENEMY, 20), (EnemyTypeID.STRONG_ENEMY, 30), (EnemyTypeID.SPEED_ENEMY, 20)}},
            {9, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 100), (EnemyTypeID.TANK_ENEMY, 35), (EnemyTypeID.STRONG_ENEMY, 45), (EnemyTypeID.SPEED_ENEMY, 35)}},
            {10, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 125), (EnemyTypeID.TANK_ENEMY, 40), (EnemyTypeID.STRONG_ENEMY, 60), (EnemyTypeID.SPEED_ENEMY, 60)}},
            {11, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 160), (EnemyTypeID.TANK_ENEMY, 70), (EnemyTypeID.STRONG_ENEMY, 80), (EnemyTypeID.SPEED_ENEMY, 60)}},
            {12, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 180), (EnemyTypeID.TANK_ENEMY, 100), (EnemyTypeID.STRONG_ENEMY, 100), (EnemyTypeID.SPEED_ENEMY, 75)}},
            {13, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 200), (EnemyTypeID.TANK_ENEMY, 115), (EnemyTypeID.STRONG_ENEMY, 115), (EnemyTypeID.SPEED_ENEMY, 95)}},
            {14, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 225), (EnemyTypeID.TANK_ENEMY, 145), (EnemyTypeID.STRONG_ENEMY, 145), (EnemyTypeID.SPEED_ENEMY, 115)}},
            {15, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 255), (EnemyTypeID.TANK_ENEMY, 160), (EnemyTypeID.STRONG_ENEMY, 160), (EnemyTypeID.SPEED_ENEMY, 140)}},
            {16, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 275), (EnemyTypeID.TANK_ENEMY, 175), (EnemyTypeID.STRONG_ENEMY, 175), (EnemyTypeID.SPEED_ENEMY, 160)}},
            {17, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 290), (EnemyTypeID.TANK_ENEMY, 190), (EnemyTypeID.STRONG_ENEMY, 190), (EnemyTypeID.SPEED_ENEMY, 190)}},
            {18, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 315), (EnemyTypeID.TANK_ENEMY, 210), (EnemyTypeID.STRONG_ENEMY, 210), (EnemyTypeID.SPEED_ENEMY, 210)}},
            {19, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 330), (EnemyTypeID.TANK_ENEMY, 225), (EnemyTypeID.STRONG_ENEMY, 225), (EnemyTypeID.SPEED_ENEMY, 225)}},
            {20, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 350), (EnemyTypeID.TANK_ENEMY, 250), (EnemyTypeID.STRONG_ENEMY, 250), (EnemyTypeID.SPEED_ENEMY, 250)}},
            {21, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 360), (EnemyTypeID.TANK_ENEMY, 260), (EnemyTypeID.STRONG_ENEMY, 260), (EnemyTypeID.SPEED_ENEMY, 260)}},
            {22, new List<(EnemyTypeID, int)> { (EnemyTypeID.BASIC_ENEMY, 400), (EnemyTypeID.TANK_ENEMY, 400), (EnemyTypeID.STRONG_ENEMY, 400), (EnemyTypeID.SPEED_ENEMY, 400)}},
        };
    }


    // adds all bots for a given level to the spawn queue
    private void StartLevel(int level) 
    {
        // get corresponding level, fallback is last level
        List<(EnemyTypeID, int)> wave;
        if (enemyWaves.ContainsKey(level))
        {
            wave = enemyWaves[level];
        }
        else
        {
            int highestLevel = enemyWaves.Keys.Max();
            wave = enemyWaves[highestLevel];
        }
        // if (!enemyWaves.ContainsKey(level)) return;

        Debug.Log($"Starting level: {level}");
        spawnQueue.Clear();
        enemiesSpawned = 0;
        totalEnemiesToSpawn = 0;

        List<(EnemyTypeID, int)> enemiesToSpawn = new List<(EnemyTypeID, int)>();

        for (int j=0; j<wave.Count; j++) 
        {
            EnemyTypeID enemyType = wave[j].Item1;
            int count = wave[j].Item2;

            for (int i=0; i<count; i++) 
            {
                enemiesToSpawn.Add((enemyType, count));
            }
        }

        // randomize enemy spawn
        enemiesToSpawn = enemiesToSpawn.OrderBy(x => Random.Range(0f, 1f)).ToList();

        foreach (var enemy in enemiesToSpawn)
        {
            spawnQueue.Enqueue(enemy);
        }

        totalEnemiesToSpawn = enemiesToSpawn.Count;
        isSpawning = true;
    }


    // spawns enemy from bot queue with cooldown between levels
    private void SpawnEnemy()
    {
        if (spawnQueue.Count == 0)
        {
            if (!levelCooldownStarted)
            {
                Debug.Log("starting level cooldown for " + currentLevel);
                levelCooldownStarted = true;
                isSpawning = false; 
                StartCoroutine(LevelCooldown());
            }
            return;
        }

        var (enemyType, count) = spawnQueue.Dequeue();
        GameObject prefab = null;

        if (enemyType == EnemyTypeID.BASIC_ENEMY) {
            prefab = Resources.Load<GameObject>("Prefabs/BasicEnemy");
        }
        else if (enemyType == EnemyTypeID.SPEED_ENEMY) {
            prefab = Resources.Load<GameObject>("Prefabs/SpeedEnemy");
        }
        else if (enemyType == EnemyTypeID.TANK_ENEMY) {
            prefab = Resources.Load<GameObject>("Prefabs/TankEnemy");
        }
        else if (enemyType == EnemyTypeID.STRONG_ENEMY) {
            prefab = Resources.Load<GameObject>("Prefabs/StrongEnemy");
        }

        GameObject enemy = Instantiate(prefab, SelectSpawnPoint(), Quaternion.identity);
        enemy.GetComponent<EnemyController>().Initialize();

        if (currentLevel >= 8)
        {
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            enemyComponent.SetHealth(enemyComponent.GetHealth() + 10);
            Debug.Log("New Health is: " + enemyComponent.GetHealth());
        }

        enemiesSpawned++;

        if (enemiesSpawned >= totalEnemiesToSpawn)
        {
            isSpawning = false; 
            StartCoroutine(LevelCooldown());
        }
    }


    // forces cooldown between levels 
    private IEnumerator LevelCooldown()
    {
        // waits for all enemies from previous wave to be killed
        int numEnemiesLeft = GameObject.FindGameObjectsWithTag("Enemy").Length - (3 - GameManager.Instance.GetNumCheckpointsCompleted());
        while (numEnemiesLeft > 0)
        {
            Debug.Log(numEnemiesLeft + " ENEMIES LEFT");
            // yield return null; 
            yield return new WaitForSeconds(0.2f);
            numEnemiesLeft = GameObject.FindGameObjectsWithTag("Enemy").Length - (3 - GameManager.Instance.GetNumCheckpointsCompleted());
        }
        
        float countdown = levelCooldownTime;
        
        while (countdown > 0)
        {
            GameManager.Instance.uiMessages.color = Color.black;
            GameManager.Instance.uiMessages.text = "";
            GameManager.Instance.uiMessages.text = $"Next level starting in {Mathf.Ceil(countdown)}s";
            yield return new WaitForSeconds(1f); 
            countdown -= 1f;
        }
        
        GameManager.Instance.uiMessages.text = "";
        currentLevel++;
        numEnemiesPerSpawn += 1;
        levelCooldownStarted = false;
        StartLevel(currentLevel);
    }


    // selects a random spawn point to place the enemy bot around current screen view
    public Vector3 GetRandomSpawnPoint(){

        // get camera position and bounds relative to current camera view
        Vector3 cameraPos = Camera.main.transform.position;

        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float screenHeight = Camera.main.orthographicSize;

        Vector3 spawnPoint = Vector3.zero;
        bool isVerticalSpawn = Random.Range(0f, 1f) > 0.5f;

        if (isVerticalSpawn)
        {
            // spawn on the left or right
            spawnPoint.y = Random.Range(cameraPos.y - screenHeight, cameraPos.y + screenHeight);
            spawnPoint.x = (Random.Range(0f, 1f) > 0.5f) ? cameraPos.x + screenWidth + 1f : cameraPos.x - screenWidth - 1f;
        }
        else
        {
            // spawn on the top or bottom
            spawnPoint.x = Random.Range(cameraPos.x - screenWidth, cameraPos.x + screenWidth);
            spawnPoint.y = (Random.Range(0f, 1f) > 0.5f) ? cameraPos.y + screenHeight + 1f : cameraPos.y - screenHeight - 1f;
        }

        return spawnPoint;
    }


    // continues to select a random spawn point until a valid one is given
    private Vector2 SelectSpawnPoint()
    {
        // check 10x for a valid solid tile
        for (int attempts = 0; attempts < 10; attempts++)
        {
            Vector2 spawnPoint = GetRandomSpawnPoint();
            if (IsValidSpawnPoint(spawnPoint))
                return spawnPoint;
        }

        // fallback: If no valid spawn points found, return random valid one
        Debug.LogWarning("Could not find a valid spawn point after 10 attempts.");
        return validSpawnPoints[Random.Range(0, validSpawnPoints.Count)];
    }


    // ensures the selected spawn point is on a solid ground tile
    private bool IsValidSpawnPoint(Vector2 spawnPoint)
    {   
        Vector3Int cellPosition = spawnArea.WorldToCell(spawnPoint);
        TileBase tile = spawnArea.GetTile(cellPosition);

        return tile != null && tile.name == "terrain_109";
    }


    // helper function to spawn a boss enemy
    private void SpawnBoss(string prefabPath, string message, int idx, Vector3 pos)
    {
        GameObject bossPrefab = Resources.Load<GameObject>(prefabPath);
        if (bossPrefab != null)
        {
            GameObject enemy = Instantiate(bossPrefab, pos, Quaternion.identity);

            enemy.GetComponent<EnemyController>().Initialize();
            bossCheckPoints[idx] = true;
            GameManager.Instance.uiMessages.color = Color.black;
            GameManager.Instance.ShowTemporaryMessage(message, 1.5f);
        }
    }

    public void EggDestroyed(Vector3 pos)
    {
        eggsDestroyed++;

        if (eggsDestroyed == 1 && !bossCheckPoints[0])
        {
            SpawnBoss("Prefabs/miniboss_1", "Mini Boss 1 Incoming!", 0, pos);
        }
        else if (eggsDestroyed == 2 && !bossCheckPoints[1])
        {
            SpawnBoss("Prefabs/miniboss_2", "Mini Boss 2 Incoming!", 1, pos);
        }
        else if (eggsDestroyed == 3 && !bossCheckPoints[2])
        {
            SpawnBoss("Prefabs/finalBoss1", "Final Boss Incoming!", 2, pos);
        }
    }


    // gets all valid spawn positions at start
    private void CacheValidSpawnPoints()
    {
        BoundsInt bounds = spawnArea.cellBounds;
        validSpawnPoints.Clear(); 

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = spawnArea.GetTile(pos);
            if (tile != null)
            {
                if (tile.name == "terrain_109")
                {
                    validSpawnPoints.Add(spawnArea.GetCellCenterWorld(pos));
                }
            }
        }
    }
}
