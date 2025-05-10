using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ***************************************************************************
// NOT USED ANYMORE  -  infinite background object inactive in scene
// ***************************************************************************


public class InfiniteBackground : MonoBehaviour
{
    private GameObject backgroundPrefab;
    private Transform player1;
    private Transform player2;
    private int gridSize = 2;
    private float tileWidth;
    private float tileHeight;
    private Dictionary<Vector2, GameObject> spawnedTiles = new Dictionary<Vector2, GameObject>();


    void Start()
    {
        backgroundPrefab = Resources.Load<GameObject>("Prefabs/grey_BG");
        player1 = GameObject.Find("Player1").transform;
        player2 = GameObject.Find("Player2").transform;

        tileWidth = backgroundPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        tileHeight = backgroundPrefab.GetComponent<SpriteRenderer>().bounds.size.y;

        Vector2 camPos = GetCameraCenter();
        Vector2 currentTile = new Vector2(Mathf.Floor((camPos.x - tileWidth / 2) / tileWidth), Mathf.Floor((camPos.y - tileHeight / 2) / tileHeight));


        for (int x = -gridSize; x <= gridSize; x++)
        {
            for (int y = -gridSize; y <= gridSize; y++)
            {
                Vector2 tilePos = (currentTile + new Vector2(x, y)) * new Vector2(tileWidth, tileHeight);
                SpawnTile(tilePos);
            }
        }
    }

    void Update()
    {
        Vector2 camPos = GetCameraCenter();
        Vector2 currentTile = new Vector2(Mathf.Floor((camPos.x - tileWidth / 2) / tileWidth), Mathf.Floor((camPos.y - tileHeight / 2) / tileHeight));

        // Check for missing tiles and spawn them
        for (int x = -gridSize; x <= gridSize; x++)
        {
            for (int y = -gridSize; y <= gridSize; y++)
            {
                Vector2 newTilePos = new Vector2((currentTile.x + x) * tileWidth, (currentTile.y + y) * tileHeight);
                if (!spawnedTiles.ContainsKey(newTilePos))
                {
                    SpawnTile(newTilePos);
                }
            }
        }

        // Remove far away tiles -- needs work?
        List<Vector2> tilesToRemove = new List<Vector2>();
        foreach (var tile in spawnedTiles)
        {
            float distance = Vector2.Distance(tile.Key, camPos);
            float threshold = Mathf.Max(tileWidth, tileHeight) * (gridSize + 1);
            
            if (distance > threshold)
            {
                Destroy(tile.Value);
                tilesToRemove.Add(tile.Key);
            }
        }

        // Remove tiles from the list after destroying them
        foreach (Vector2 tilePos in tilesToRemove)
        {
            spawnedTiles.Remove(tilePos);
        }
    }

    void SpawnTile(Vector2 position)
    {
        Vector3 pos = new Vector3(position.x, position.y, 1);
        if (!spawnedTiles.ContainsKey(position))
        {
            GameObject newTile = Instantiate(backgroundPrefab, pos, Quaternion.identity);
            spawnedTiles[position] = newTile;
        }
    }

    // returns centerpoint between the 2 players
    Vector2 GetCameraCenter()
    {
        if (player1 != null && player2 != null) {
            return (player1.position + player2.position) / 2;
        }
        else if (player1 != null && player2 == null) {
            return player1.position;
        }
        else if (player1 == null && player2 != null) {
            return player2.position;
        }
        // 0 if all players dead
        return new Vector2(0,0);
    }
}
