using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;

public class Minimap : MonoBehaviour
{
    public Image minimapPanel;
    public GameObject player1Icon; 
    public GameObject player2Icon; 
    public Sprite checkpointSprite;

    private GameObject player1; 
    private GameObject player2; 
    private Vector2 worldMin; 
    private Vector2 worldMax; 
    private Vector2 minimapSize;
    private List<GameObject> checkpoints = new List<GameObject>();

    void Start()
    {
        player1 = GameObject.Find("Player1");

        player2 = GameObject.Find("Player2");

        // needs updating
        Tilemap tilemap = GameObject.Find("Background Grid/Tilemap")?.GetComponent<Tilemap>();

        if (tilemap != null)
        {
            BoundsInt cellBounds = tilemap.cellBounds;

            worldMin = tilemap.CellToWorld(cellBounds.min); 
            worldMax = tilemap.CellToWorld(cellBounds.max); 
        }

        minimapSize = minimapPanel.rectTransform.rect.size;
    }

    void OnEnable()
    {
        FindCheckpoints();
        UpdateCheckpointMarkers();
    }

    void OnDisable()
    {
        ClearOldMarkers();
    }

    private void Update()
    {
        UpdatePlayerIcon(player1, player1Icon);
        UpdatePlayerIcon(player2, player2Icon);
    }

    Vector2 ConvertWorldToMinimap(Vector2 worldPos)
    {
        // float xRatio = (worldPos.x - worldMin.x) / (worldMax.x - worldMin.x);
        // float yRatio = (worldPos.y - worldMin.y) / (worldMax.y - worldMin.y);

        // float minimapX = xRatio * minimapSize.x;
        // float minimapY = yRatio * minimapSize.y;

        // return new Vector2(minimapX, minimapY);

        // just hardcoded values for now
        return new Vector2(4.2f * worldPos.x - 210, 3.5f * worldPos.y - 30);
    }


    // updates the number of checkpoint markers to show as they get completed
    void UpdateCheckpointMarkers()
    {
        foreach (GameObject checkpoint in checkpoints)
        {
            Vector2 minimapPos = ConvertWorldToMinimap(checkpoint.transform.position);

            // create a new UI Image
            GameObject marker = new GameObject("CheckpointMarker");
            Image markerImage = marker.AddComponent<Image>(); 

            markerImage.sprite = checkpointSprite; 

            marker.transform.SetParent(transform, false);

            // position it on the minimap
            marker.GetComponent<RectTransform>().anchoredPosition = minimapPos;
            marker.GetComponent<RectTransform>().sizeDelta = new Vector2(15, 20);
        }
    }


    // removes old map markers when disabled
    void ClearOldMarkers()
    {
        foreach (Transform child in transform)
        {
            if (child.name == "CheckpointMarker")
            {
                Destroy(child.gameObject);
            }
        }
    }


    // finds updated checkpoints in map 
    void FindCheckpoints()
    {
        GameObject parent = GameObject.Find("Checkpoints"); 
        if (parent != null)
        {
            checkpoints.Clear(); 

            foreach (Transform child in parent.transform)
            {
                checkpoints.Add(child.gameObject);
            }
        }
    }

    // Helper function to update player icons as they move
    private void UpdatePlayerIcon(GameObject player, GameObject playerIcon)
    {
        if (player != null && player.GetComponent<PlayerHealthController>().GetIsAlive())
        {
            Vector2 minimapPos = ConvertWorldToMinimap(player.transform.position);
            playerIcon.GetComponent<RectTransform>().anchoredPosition = minimapPos;
            playerIcon.SetActive(true); 
        }
        else
        {
            playerIcon.SetActive(false); 
        }
    }

}
